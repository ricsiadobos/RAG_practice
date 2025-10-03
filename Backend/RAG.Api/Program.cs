using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using RAG.Core;
using RAG.Infrastructure;
using RAG.Core.Interfaces;
using RAG.Infrastructure.Rag;
using RAG.Infrastructure.Search;
using RAG.Infrastructure.Data;
using RAG.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RAG.Api", Version = "v1" });
});

// OpenAI beállítások (Options pattern)
builder.Services.Configure<AiSettings>(builder.Configuration.GetSection("OpenAI"));

// HttpClient gyár
builder.Services.AddHttpClient();

// RAG komponensek
builder.Services.AddSingleton<IEmbeddingStore, InMemoryEmbeddingStore>();
builder.Services.AddSingleton<IRetriever, SimpleRetriever>();
builder.Services.AddSingleton<IRagService, RagService>();

// IAiProvider → OpenAiProvider
builder.Services.AddSingleton<IAiProvider>(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<AiSettings>>().Value;
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
    return new OpenAiProvider(http, cfg);
});

//EF - DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Debug

// Hozzáférés a konfigurációhoz
var configuration = builder.Configuration;

// Kiolvasás az appsettings.json-ból
string chatModel = configuration["OpenAI:ChatModel"] ?? "nincs beállítva";
string embeddingModel = configuration["OpenAI:EmbeddingModel"] ?? "nincs beállítva";

Console.WriteLine($"⚡ OpenAI.ChatModel: {chatModel}");
Console.WriteLine($"⚡ OpenAI.EmbeddingModel: {embeddingModel}");

//

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RAG.Api v1"));

app.MapControllers();


// debug miatt logolás
app.MapGet("/", () => "API fut!");
app.Run();
