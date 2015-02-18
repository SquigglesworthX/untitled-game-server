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

        internal CloudTableClient tableClient;
        internal CloudTable table;

        /// <summary>
        /// Creates a new TableSet, using the functions provided to assign row and partition keys. 
        /// </summary>
        /// <param name="connectionString">The Azure connection string.</param>
        /// <param name="tableName">The name of the table in Azure storage.</param>               
        public TableSet(CloudStorageAccount storageAccount, string tableName)
        {
            this.tableName = tableName;

            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            tableClient = storageAccount.CreateCloudTableClient();
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

        public virtual void Insert(TEntity entity)
        {
            dynamic mapped = CreateDTO(entity);
            TableOperation insertOperation = TableOperation.Insert(mapped);
            table.Execute(insertOperation);
        }

        public virtual TEntity GetById(object id)
        {
            var query = new TableQuery().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));
            var result = table.ExecuteQuery(query).First();

            dynamic mapped = StripDTO(result);
            return mapped;
        }

        #region object mapping
        dynamic CreateDTO(TEntity a)
        {
            TableEntityDTO dto = new TableEntityDTO();

            Type t1 = a.GetType();
            Type t2 = dto.GetType();

            //not returning inherited properties, need to fix this for rowkey
            foreach (System.Reflection.PropertyInfo p in t1.GetProperties())
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

            //Should be set through the basemodel property
            dto.RowKey = a.RowKey;
            dto.PartitionKey = a.PartitionKey;

            return dto;
        }

        TEntity StripDTO(Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity a)
        {
            TEntity result = new TEntity();

            Type t1 = result.GetType();
            var dictionary = (IDictionary<string, EntityProperty>)a.Properties;

            foreach (PropertyInfo p1 in t1.GetProperties())//for each property in the entity,
            {
                bool isExcluded = false;
                foreach (object attribute in p1.GetCustomAttributes(true))
                {
                    if (attribute is ExcludedAttribute)
                    {
                        isExcluded = true;
                    }
                }

                if (!isExcluded)
                {
                    foreach (var value in dictionary)//see if we have a correspinding property in the DTO
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

            }
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
