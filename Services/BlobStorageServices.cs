using Azure.Storage.Blobs;
using Azure.Storage.Sas;

using Azure.Storage.Blobs;

using Azure.Storage.Blobs.Models;

using Azure.Storage.Sas;

using Microsoft.Extensions.Configuration;

using System;

using System.IO;

using System.Threading.Tasks;



namespace EventEaseCloud.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["ConnectionStrings:ContainerConnection"]);
            _containerName = configuration["ConnectionStrings:ContainerName"];
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            // Upload the file and overwrite if it already exists
            await blobClient.UploadAsync(fileStream, overwrite: true);

            // Return the public blob URL
            return blobClient.Uri.ToString();
        }
    }
}
