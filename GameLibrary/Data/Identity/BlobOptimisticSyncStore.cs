using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Identity
{
    /// <summary>
    /// Stores a single string value in Blob storage and provides an easy way to update 
    /// the value using Optimistic Concurrency.
    /// </summary>
    public class BlobOptimisticSyncStore : IOptimisticSyncStore
    {
        private readonly CloudBlockBlob BlobReference;

        public BlobOptimisticSyncStore(CloudStorageAccount account, string container, string address)
        {
            var blobClient = account.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(container.ToLower());
            blobContainer.CreateIfNotExists();
            BlobReference = blobContainer.GetBlockBlobReference(address);            
        }

        public string GetData()
        {
            string data = "0";
            try
            {
                data = BlobReference.DownloadText();
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                    TryOptimisticWrite(data);
                else
                    throw ex;
            }
            return data;
        }

        public bool TryOptimisticWrite(string data)
        {
            try
            {
                BlobReference.UploadText(
                    data,
                    Encoding.Default,
                    new AccessCondition()
                    {
                        IfMatchETag = BlobReference.Properties.ETag
                    }
                    );
            }
            catch (StorageException exc)
            {
                if (exc.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }
    }
}
