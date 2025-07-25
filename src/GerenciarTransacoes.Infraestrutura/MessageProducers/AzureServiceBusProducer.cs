using Azure.Messaging.ServiceBus; // Importa o SDK do Azure Service Bus
using GerenciarTransacoes.Aplicacao.Interfaces; // Para IMessageProducer
using Newtonsoft.Json; // Usaremos para serializar a mensagem em JSON (instale este pacote depois)
using System.Threading.Tasks;

namespace GerenciarTransacoes.Infraestrutura.MessageProducers
{
    public class AzureServiceBusProducer : IMessageProducer
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly string _queueName;

        // O construtor recebe a string de conexão e o nome da fila
        public AzureServiceBusProducer(string connectionString, string queueName)
        {
            _serviceBusClient = new ServiceBusClient(connectionString);
            _queueName = queueName;
        }

        public async Task SendMessageAsync<T>(T message, string queueNameOverride = null)
        {
            // Usa o nome da fila padrão ou um nome de fila sobrescrito
            string targetQueueName = queueNameOverride ?? _queueName;
            if (string.IsNullOrEmpty(targetQueueName))
            {
                throw new ArgumentNullException(nameof(targetQueueName), "O nome da fila não pode ser nulo ou vazio.");
            }

            ServiceBusSender sender = _serviceBusClient.CreateSender(targetQueueName);

            // Serializa o objeto da mensagem para JSON
            string messageBody = JsonConvert.SerializeObject(message);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(serviceBusMessage);

            // É uma boa prática fechar o sender quando ele não é mais necessário
            // ou gerenciá-lo com using para descarte automático.
            // Para simplificar, neste exemplo não faremos o dispose imediato aqui.
            // Em uma aplicação real, o ServiceBusClient deve ser um singleton.
            await sender.CloseAsync(); // Close the sender when done
        }
    }
}