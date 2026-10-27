
using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SchoolFunctionsDemo;

public class BubbleSortBlobFunction
{
    private readonly ILogger<BubbleSortBlobFunction> _logger;
    private readonly BlobServiceClient _blobServiceClient;

    public BubbleSortBlobFunction(
        ILogger<BubbleSortBlobFunction> logger,
        BlobServiceClient blobServiceClient)
    {
        _logger = logger;
        _blobServiceClient = blobServiceClient;
    }

    [Function("BubbleSortBlob")]
    public async Task Run(
        [BlobTrigger(
            "integer-arrays/{name}",
            Connection = "BlobStorageConnection")]
        string content,
        string name)
    {
        int[] numbers = JsonSerializer.Deserialize<int[]>(content)
            ?? throw new InvalidOperationException("Invalid integer array.");

        // Bubble Sort
        for (int i = 0; i < numbers.Length - 1; i++)
        {
            bool swapped = false;

            for (int j = 0; j < numbers.Length - 1 - i; j++)
            {
                if (numbers[j] > numbers[j + 1])
                {
                    (numbers[j], numbers[j + 1]) =
                        (numbers[j + 1], numbers[j]);

                    swapped = true;
                }
            }

            if (!swapped)
                break;
        }

        // Write the result to a separate container.
        var outputContainer =
            _blobServiceClient.GetBlobContainerClient("sorted-results");

        await outputContainer.CreateIfNotExistsAsync();

        var outputBlob = outputContainer.GetBlobClient(name);

        await outputBlob.UploadAsync(
            BinaryData.FromObjectAsJson(numbers),
            overwrite: true);

        _logger.LogInformation(
            "Sorting completed for blob {BlobName}", name);
    }
}
