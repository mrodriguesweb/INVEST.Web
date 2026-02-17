using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

public class CompanyFunction
{
    private readonly ILogger _logger;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public CompanyFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CompanyFunction>();

        // Lê os valores dos App Settings
        string blobServiceUri = Environment.GetEnvironmentVariable("LOGOS_BLOB_SERVICE_URI");
        _containerName = Environment.GetEnvironmentVariable("LOGOS_CONTAINER_NAME");

        // Local Testes
        if (Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development")
        {
            // Usa connection string local
            _blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        }
        // Cria o cliente do Blob Storage usando Managed Identity (DefaultAzureCredential)
        else
        {
            // Usa Managed Identity em produção
            _blobServiceClient = new BlobServiceClient(new Uri(blobServiceUri), new DefaultAzureCredential());
        }

    }

    [Function("GetCompanyLogo")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        _logger.LogInformation("HTTP trigger function processando requisição...");

        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        string empresa = query["empresa"];

        if (string.IsNullOrEmpty(empresa))
        {
            var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Informe o parâmetro 'empresa'.");
            return badResponse;
        }

        // Monta o nome do arquivo (ex.: BRADESCO.png)
        empresa = empresa.ToUpper();
        string fileName = $"{empresa}.png";

        // Obtém o container e o blob
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        // Verifica se existe
        if (!await blobClient.ExistsAsync())
        {
            var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"Logo {fileName} não encontrada.");
            return notFoundResponse;
        }

        // Faz streaming do blob para o response
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "image/png");
        response.Headers.Add("Cache-Control", "public, max-age=86400"); // cache de 1 dia

        using var stream = await blobClient.OpenReadAsync();
        await stream.CopyToAsync(response.Body);

        return response;
    }
}