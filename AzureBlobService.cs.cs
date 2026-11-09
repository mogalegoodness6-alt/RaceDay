
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDayMVC.Services;

public class AzureBlobService
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobService(IConfiguration configuration)
    {
        string connectionString =
            configuration["AzureStorage:ConnectionString"]
            ?? throw new InvalidOperationException(
                "Azure Storage connection string is missing.");

        var blobServiceClient =
            new BlobServiceClient(connectionString);

        _containerClient =
            blobServiceClient.GetBlobContainerClient("event-images");
    }

    public async Task<string> UploadImageAsync(
        Stream fileStream,
        string fileName,
        string contentType)
    {
        await _containerClient.CreateIfNotExistsAsync(
            PublicAccessType.None);

        string extension = Path.GetExtension(fileName);
        string uniqueFileName = $"{Guid.NewGuid()}{extension}";

        BlobClient blobClient =
            _containerClient.GetBlobClient(uniqueFileName);

        await blobClient.UploadAsync(
            fileStream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            });

        return blobClient.Uri.ToString();
    }
}
