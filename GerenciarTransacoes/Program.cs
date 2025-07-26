using GerenciarTransacoes.Dominio.Interfaces;
using GerenciarTransacoes.Infraestrutura.Repositories;
using GerenciarTransacoes.Aplicacao.UseCases;
using Microsoft.Extensions.Configuration;
using GerenciarTransacoes.Aplicacao.Interfaces;
using GerenciarTransacoes.Infraestrutura.MessageProducers;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner.
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Adiciona o repositório MongoDB como um serviço para Injeção de Dependência
builder.Services.AddSingleton<ITransactionRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetSection("MongoDbSettings:ConnectionString").Value;
    var databaseName = configuration.GetSection("MongoDbSettings:DatabaseName").Value;

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("MongoDB ConnectionString não configurada em appsettings.Development.json.");
    }
    if (string.IsNullOrEmpty(databaseName))
    {
        throw new InvalidOperationException("MongoDB DatabaseName não configurado em appsettings.Development.json.");
    }

    return new TransactionRepository(connectionString, databaseName);
});

// Registra os casos de uso para Injeção de Dependência
builder.Services.AddScoped<ListTransactionsUseCase>();
builder.Services.AddScoped<CreateTransactionUseCase>();


// Registra o produtor de mensagens do Azure Service Bus
builder.Services.AddSingleton<IMessageProducer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetSection("ServiceBusSettings:ConnectionString").Value;
    var queueName = configuration.GetSection("ServiceBusSettings:QueueName").Value;

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Service Bus ConnectionString não configurada em appsettings.Development.json.");
    }
    if (string.IsNullOrEmpty(queueName))
    {
        throw new InvalidOperationException("Service Bus QueueName não configurado em appsettings.Development.json.");
    }

    return new AzureServiceBusProducer(connectionString, queueName);
});


var app = builder.Build();

// Configura o pipeline de solicitação HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Mapeia os controladores para que a aplicação consiga encontrá-los
app.MapControllers();

app.Run();
