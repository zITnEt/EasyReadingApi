using EasyReading.Application.Abstractions;
using EasyReading.Application.Configurations;
using EasyReading.Application.DTOs;
using EasyReading.Application.UseCases.User.Commands;
using EasyReading.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.InteropServices;
using EasyReading.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace EasyReading.Application.UseCases.User.Queries
{
    public class GetResponseQuery : IQuery<string>
    {
        public int DocumentId { get; set; }
        public string? Question { get; set; }
    }

    public class GetResponseQueryHandle : IQueryHandler<GetResponseQuery, string>
    {
        private readonly IMediator _mediator;
        private readonly OpenAIConfiguration _configuration;
        private readonly HttpClient _client;

        public GetResponseQueryHandle(IMediator mediator, IOptions<OpenAIConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _mediator = mediator;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OpenAIKey"));
        }

        public async Task<string> Handle(GetResponseQuery request, CancellationToken cancellationToken)
        {
            CreateEmbeddingCommand command = new CreateEmbeddingCommand
            {
                Body = request.Question
            };

            GetPagesQuery query = new GetPagesQuery
            {
                DocumentId = request.DocumentId,
                Embedding = await _mediator.Send(command)
            };

            List<PageDTO> dtos = await _mediator.Send(query);

            var context = "";

            foreach (var dto in dtos)
            {
                context += $"{dto.Body ?? ""}\n---\n";;
            }

            string prompt = $"""
              You are a helpful AI assistant.
              Given the following sections, answer any user questions by
              using only that information.
              If you are unsure and the answer is not explicitly written in
              the sections below, then try to answer it yourself. Answer in the 
              request's language!

              Context sections:
              {context}
            """;

            List<ChatMessage> messages = new List<ChatMessage>();

            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = prompt
            });

            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = request.Question
            });

            ChatRequest chatRequest = new ChatRequest
            {
                Messages = messages,
                Temperature = 0.7,
                Model = "gpt-3.5-turbo"
            };

            var json = JsonSerializer.Serialize(chatRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_configuration.GptUrl, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            ChatCompletionResponse? chat = JsonSerializer.Deserialize<ChatCompletionResponse>(responseString);

            if (chat is not null)
            {
                return chat?.Choices?[0].Message!.Content ?? "";
            }

            return "";
        }

    }
}

public class ChatMessage
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class ChatRequest
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }
    [JsonPropertyName("messages")]
    public List<ChatMessage>? Messages { get; set; }
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }
}

public class ChatCompletionResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }

    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }
}


public class Choice
{
    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    [JsonPropertyName("logprobs")]
    public object? Logprobs { get; set; } // Can be null, change type as needed

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }
}

public class Message
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
