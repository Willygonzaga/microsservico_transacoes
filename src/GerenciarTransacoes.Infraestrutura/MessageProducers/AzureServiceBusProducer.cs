using Azure.Messaging.ServiceBus;
using GerenciarTransacoes.Aplicacao.Interfaces;
using Newtonsoft.Json;
using System; // Necessário para ArgumentNullException
using System.Threading.Tasks;

namespace GerenciarTransacoes.Infraestrutura.MessageProducers
{
    // Implementação do produtor de mensagens usando o Azure Service Bus.
    // Atua como um adaptador da camada de Infraestrutura para a interface IMessageProducer.
    public class AzureServiceBusProducer : IMessageProducer
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly string _queueName;

        // Construtor que inicializa o cliente do Service Bus com a string de conexão e o nome da fila.
        public AzureServiceBusProducer(string connectionString, string queueName)
        {
            _serviceBusClient = new ServiceBusClient(connectionString);
            _queueName = queueName;
        }

        // Envia uma mensagem para a fila do Azure Service Bus.
        // A mensagem é serializada para JSON antes do envio.
        public async Task SendMessageAsync<T>(T message, string queueNameOverride = null)
        {
            // Determina o nome da fila alvo, permitindo sobrescrita.
            string targetQueueName = queueNameOverride ?? _queueName;
            if (string.IsNullOrEmpty(targetQueueName))
            {
                throw new ArgumentNullException(nameof(targetQueueName), "O nome da fila não pode ser nulo ou vazio.");
            }

            // Cria um remetente para a fila específica.
            ServiceBusSender sender = _serviceBusClient.CreateSender(targetQueueName);

            // Serializa o objeto T para uma string JSON, que será o corpo da mensagem.
            string messageBody = JsonConvert.SerializeObject(message);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(messageBody);

            // Envia a mensagem assincronamente.
            await sender.SendMessageAsync(serviceBusMessage);

            await sender.CloseAsync();
        }
    }
}
