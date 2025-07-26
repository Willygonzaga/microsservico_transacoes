using GerenciarTransacoes.Dominio.Interfaces; // Para ITransactionRepository
using GerenciarTransacoes.Infraestrutura.Repositories; // Para TransactionRepository
using GerenciarTransacoes.Aplicacao.UseCases; // Para ListTransactionsUseCase <--- NOVO USING
using Microsoft.Extensions.Configuration; // Para acessar as configurações do appsettings.json
using GerenciarTransacoes.Aplicacao.Interfaces; // Para IMessageProducer
using GerenciarTransacoes.Infraestrutura.MessageProducers; // Para AzureServiceBusProducer

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen(); // <--- SUBSTITUA AddOpenApi() POR ESTA LINHA

// Adicione suporte para controladores MVC (API)
builder.Services.AddControllers();

// Adiciona o repositório MongoDB como um serviço para Injeção de Dependência
builder.Services.AddSingleton<ITransactionRepository>(sp =>
{
    // Pega as configurações do appsettings.json
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetSection("MongoDbSettings:ConnectionString").Value;
    var databaseName = configuration.GetSection("MongoDbSettings:DatabaseName").Value;

    // Valida se as strings de conexão e nome do banco de dados foram encontradas
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("MongoDB ConnectionString não configurada em appsettings.json.");
    }
    if (string.IsNullOrEmpty(databaseName))
    {
        throw new InvalidOperationException("MongoDB DatabaseName não configurado em appsettings.json.");
    }

    return new TransactionRepository(connectionString, databaseName);
});

// Registra o caso de uso de listagem de transações para Injeção de Dependência <--- NOVA SEÇÃO
builder.Services.AddScoped<ListTransactionsUseCase>();

// Registra o caso de uso de listagem de transações
builder.Services.AddScoped<ListTransactionsUseCase>();
builder.Services.AddScoped<CreateTransactionUseCase>(); // <--- ADICIONE ESTA LINHA


// Registra o produtor de mensagens do Azure Service Bus
builder.Services.AddSingleton<IMessageProducer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetSection("ServiceBusSettings:ConnectionString").Value;
    var queueName = configuration.GetSection("ServiceBusSettings:QueueName").Value;

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Service Bus ConnectionString não configurada em appsettings.json.");
    }
    if (string.IsNullOrEmpty(queueName))
    {
        throw new InvalidOperationException("Service Bus QueueName não configurado em appsettings.json.");
    }

    return new AzureServiceBusProducer(connectionString, queueName);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // <--- ADICIONE ESTA LINHA
    app.UseSwaggerUI(); // <--- ADICIONE ESTA LINHA
}

// app.UseHttpsRedirection(); // Continua comentada

// Mantendo o endpoint de exemplo WeatherForecast da Minimal API
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Mapeia os controladores para que a aplicação consiga encontrá-los
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}