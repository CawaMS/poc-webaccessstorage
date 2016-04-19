using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using CawaConfigurationManager;

namespace WebAppStrg
{
    public static class Helpers
    {
        public static string GetThemePath(this System.Web.Mvc.HtmlHelper helper)
        {
            return GetBlobSasUri("strgConnStr", "mycontainer", "minnie.png");
        }


        private static string GetBlobSasUri(string settingKey, string containerName, string blobName)
        {
            string strgConnStr = GetConnectionString.GetValueFomKeyVault(settingKey);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(strgConnStr);

            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            //Get a reference to a blob within the container.
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            //Set the expiry time and permissions for the blob.
            //In this case the start time is specified as a few minutes in the past, to mitigate clock skew.
            //The shared access signature will be valid immediately.
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }
    }
}
