using EasyReading.Application.Abstractions;
using EasyReading.Application.Configurations;
using EasyReading.Application.DTOs;
using EasyReading.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace EasyReading.Application.UseCases.User.Commands
{
    public class CreatePagesCommand : ICommand<int>
    {
        public int DocumentId { get; set; }
        public List<GetPageDTO>? Pages { get; set; }
    }

    public class CreatePagesCommandHandler : ICommandHandler<CreatePagesCommand, int>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly HttpClient _httpClient;
        private readonly OpenAIConfiguration _configuration;

        public CreatePagesCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IMediator mediator, IOptions<OpenAIConfiguration> configuration)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _httpClient = new HttpClient();
            _configuration = configuration.Value;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OpenAIKey"));
        }

        public async Task<int> Handle(CreatePagesCommand command, CancellationToken cancellationToken)
        {
            Document? document = await _dbContext.Documents.FirstOrDefaultAsync(x => x.Id == command.DocumentId && x.UserId == _currentUserService.UserId);
            int page = command.Pages?.Last()?.Page ?? -1;

            if (document is not null)
            {
                if (document.LastProcessedPage >= page || document.PagesCount < page)
                {
                    return page;
                }

                var prev = _dbContext.Pages.Where(x => x.PageNum == document.LastProcessedPage && x.DocumentId == command.DocumentId).
                    Select(x => new { x.Body }).ToList();

                string prevPage = "";

                if ((prev is not null) && (prev.Count == 1))
                {
                    prevPage = prev[0].Body!;
                }

                List<ChatMessage> messages = new List<ChatMessage>();
                string currentPage;

                foreach (var dto in command.Pages ?? new List<GetPageDTO>())
                {
                    if (dto.Page < document.LastProcessedPage)
                    {
                        prevPage = dto.Body ?? "";
                        continue;
                    }

                    currentPage = dto.Body ?? "";
                    string prompt = $"""
                      Text2 might be missing some space betwwen words.
                      Text1 = "{prevPage}".
                      Text2 = "{currentPage}".
                      Change the unclear pronouns (he/she/they/it/him/her/his/its/hers/them/their) of Text2 with the 
                      explicit subjects or 
                      objects (preferrably names of people) they reference to depending on Text1.
                      YOU DON'T ALWAYS HAVE TO CHANGE TEXT2! It might be already in
                      its perfect version. If Text1 is comprehensible and doesn't have 
                      any unclear pronouns then don't change it!
                      Also if the Text2's first sentence seems incomplete, 
                      then please full it using the context of Text1. 
                      NEVER CHANGE THE MEANING OF TEXT2! YOUR JOB IS TO MAKE TEXT2 
                      INDEPENDENT COMPREHENSIBLE TEXT! If you think that there is any type or some
                      irrelevant character or a grammatical mistake, please correct it.
                      I don't need
                      any explanations! Your response must be only and only modified version of Text2 .
                      Your answer should not contain TEXT1 or any NOTE. Just answer me with only Text2 content without any
                      'Text2' keyword! 
                    """;
                    messages = new List<ChatMessage>();
                    messages.Add(new ChatMessage
                    {
                        Role = "user",
                        Content = prompt
                    });

                    ChatRequest chatRequest = new ChatRequest
                    {
                        Messages = messages,
                        Temperature = 0.9,
                        Model = "gpt-3.5-turbo"
                    };

                    var json = JsonSerializer.Serialize(chatRequest);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(_configuration.GptUrl, content);
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();
                    ChatCompletionResponse? chat = JsonSerializer.Deserialize<ChatCompletionResponse>(responseString);
                    Console.WriteLine("Body -> " + chat?.Choices?[0].Message!.Content);

                    CreatePageCommand pageCommand = new CreatePageCommand
                    {
                        Document = document,
                        Body = chat?.Choices?[0].Message!.Content,
                        PageNum = dto.Page,
                    };

                    await _mediator.Send(pageCommand);
                    prevPage = pageCommand.Body ?? "";
                }
            }

            return document?.LastProcessedPage ?? -1;
        }
    }
}