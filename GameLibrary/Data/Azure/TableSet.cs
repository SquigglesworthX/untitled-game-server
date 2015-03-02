using GameLibrary.Data.Azure.Model;
using GameLibrary.Data.Core;
using GameLibrary.Data.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure
{
    internal class TableSet<TEntity> where TEntity : BaseModel, new()
    {
        private string tableName;
        private AzureContext Context;

        internal CloudTableClient tableClient;
        internal CloudTable table;

        /// <summary>
        /// Creates a new TableSet, using the functions provided to assign row and partition keys. 
        /// </summary>
        /// <param name="context">An active Azure context.</param>
        /// <param name="tableName">The name of the table in Azure storage.</param>               
        public TableSet(AzureContext context, string tableName)
        {
            this.tableName = tableName;
            this.Context = context;

            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            tableClient = context.StorageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
        }

        public virtual List<TEntity> GetAll()
        {
            List<TEntity> mappedList = new List<TEntity>();
            var query = new TableQuery();
            TableContinuationToken token = null;

            var entities = new List<DynamicTableEntity>();
            do
            {
                var queryResult = table.ExecuteQuerySegmented(new TableQuery(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            foreach (var item in entities)
            {
                mappedList.Add(StripDTO(item));
            }
            return mappedList;
        }

        public virtual void Delete(TEntity entity)
        {
            dynamic mapped = CreateDTO(entity);
            TableOperation deleteOperation = TableOperation.Delete(mapped);
            table.Execute(deleteOperation);
        }

        public virtual void Insert(TEntity entity)
        {
            dynamic mapped = CreateDTO(entity);
            TableOperation insertOperation = TableOperation.Insert(mapped);
            table.Execute(insertOperation);
            entity.ETag = mapped.Etag;
        }

        public virtual void Update(TEntity entity)
        {
            dynamic mapped = CreateDTO(entity);
            TableOperation updateOpertaion = TableOperation.Replace(mapped);
            table.Execute(updateOpertaion);
            entity.ETag = mapped.Etag;
        }

        public virtual void BatchOperation(IEnumerable<AzureAction> actions)
        {
            TableBatchOperation batchOperation = PrepareBatch(actions);

            table.ExecuteBatch(batchOperation);
        }

        public virtual async Task BatchOperationAsync(IEnumerable<AzureAction> actions)
        {
            TableBatchOperation batchOperation = PrepareBatch(actions);

            await table.ExecuteBatchAsync(batchOperation);
        }

        public virtual TEntity GetById(object id)
        {
            var query = new TableQuery().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));
            var result = table.ExecuteQuery(query).First();

            dynamic mapped = StripDTO(result);
            return mapped;
        }

        private TableBatchOperation PrepareBatch(IEnumerable<AzureAction> actions)
        {
            TableBatchOperation batchOperation = new TableBatchOperation();

            foreach (AzureAction action in actions)
            {
                dynamic mapped = CreateDTO((TEntity)action.Model);

                switch (action.Action)
                {
                    case ActionType.Insert:
                        batchOperation.Insert(mapped);
                        break;
                    case ActionType.Delete:
                        batchOperation.Delete(mapped);
                        break;
                    case ActionType.Update:
                        batchOperation.Replace(mapped);
                        break;
                }

                action.IsProcessed = true;
            }
            return batchOperation;
        }

        #region object mapping
        dynamic CreateDTO(TEntity a)
        {
            TableEntityDTO dto = new TableEntityDTO();

            Type t1 = a.GetType();
            Type t2 = dto.GetType();

            foreach (System.Reflection.PropertyInfo p in t1.GetProperties())
            {
                bool isExcluded = false;
                RelationshipAttribute attr = null;
                foreach (object attribute in p.GetCustomAttributes(true))
                {
                    if (attribute is ExcludedAttribute)
                    {
                        isExcluded = true;
                    }
                    if (attribute is RelationshipAttribute)
                    {
                        attr = (RelationshipAttribute)attribute;
                    }
                }

                if (attr != null)
                {               
                    //First time saving any relationship, needs to be created.
                    if (a.Relationships == null)
                    {
                        a.Relationships = new List<RelationshipMapping>();
                    }

                    RelationshipMapping mapping = a.Relationships.FirstOrDefault(t => t.Name == attr.Name);
                    //First time saving this relationship, so it needs to be created.
                    if (mapping == null)
                    {
                        mapping = new RelationshipMapping();
                        mapping.Name = attr.Name;
                        a.Relationships.Add(mapping);
                    }

                    //If the relationship has been initialized (retrieved from the db), or hasn't been initialized but contains a value,
                    //compare to known values to see if an update is required.
                    if (mapping.Initialized || (!mapping.Initialized && p.GetValue(a) != null))
                    {
                        List<AzureAction> actions = mapping.GetUpdates(attr, (List<BaseModel>)p.GetValue(a));
                    }

                }
                else if (!isExcluded)
                {
                    if (IsPrimaryType(p.PropertyType))
                    {
                        dto.TrySetMember(p.Name, p.GetValue(a, null) ?? "");
                    }
                    else
                    {
                        string json = JsonConvert.SerializeObject(p.GetValue(a, null) ?? "");
                        dto.TrySetMember(p.Name, json);
                    }
                }
            }

            return dto;
        }

        TEntity StripDTO(Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity a)
        {
            TEntity result = new TEntity();

            Type t1 = result.GetType();
            var dictionary = (IDictionary<string, EntityProperty>)a.Properties;

            foreach (PropertyInfo p1 in t1.GetProperties())//for each property in the entity,
            {

                foreach (var value in dictionary)
                {
                    if (p1.Name == value.Key)
                    {
                        if (IsPrimaryType(p1.PropertyType))
                        {
                            p1.SetValue(result, GetValue(value.Value));
                        }
                        else
                        {
                            object val = JsonConvert.DeserializeObject(GetValue(value.Value).ToString(), p1.PropertyType);
                            p1.SetValue(result, val);
                        }
                    }
                }
                

            }
            result.ETag = a.ETag;
            result.PartitionKey = a.PartitionKey;
            result.RowKey = a.RowKey;

            return result;
        }

        private static bool IsPrimaryType(Type pt)
        {
            return (pt == typeof(byte[]) || pt == typeof(bool) || pt == typeof(DateTimeOffset) || pt == typeof(DateTime) || pt == typeof(double) || pt == typeof(Guid) || pt == typeof(int) || pt == typeof(long) || pt == typeof(string));
        }

        private object GetValue(EntityProperty source)
        {
            switch (source.PropertyType)
            {
                case EdmType.String:
                    return (object)source.StringValue;
                case EdmType.Binary:
                    return (object)source.BinaryValue;
                case EdmType.Boolean:
                    return (object)source.BooleanValue;
                case EdmType.DateTime:
                    return (object)source.DateTimeOffsetValue;
                case EdmType.Double:
                    return (object)source.DoubleValue;
                case EdmType.Guid:
                    return (object)source.GuidValue;
                case EdmType.Int32:
                    return (object)source.Int32Value;
                case EdmType.Int64:
                    return (object)source.Int64Value;
                default: throw new TypeLoadException(string.Format("not supported edmType:{0}", source.PropertyType));
            }
        }
        # endregion

    }
}
