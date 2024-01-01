using EasyReading.Application.Abstractions;
using EasyReading.Application.Configurations;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyReading.Application.UseCases.User.Commands
{
    public class CreateEmbeddingCommand : ICommand<double[]?>
    {
        public string? Body { get; set; }
    }

    public class CreateEmbeddingCommandHandler : ICommandHandler<CreateEmbeddingCommand, double[]?>
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAIConfiguration _configuration;

        public CreateEmbeddingCommandHandler(IOptions<OpenAIConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OpenAIKey"));
        }

        public async Task<double[]?> Handle(CreateEmbeddingCommand request, CancellationToken cancellationToken)
        {
            var requestBody = new { input = request.Body, model = "text-embedding-ada-002" };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_configuration.EmbeddingUrl, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<EmbeddingResponse>(responseString);

            return responseObject?.Data?[0]?.Embedding;
        }
    }
}

public class EmbeddingResponse
{
    [JsonPropertyName("data")]
    public EmbeddingData[]? Data { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }
}

public class EmbeddingData
{
    [JsonPropertyName("embedding")]
    public double[]? Embedding { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }
}

public class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
