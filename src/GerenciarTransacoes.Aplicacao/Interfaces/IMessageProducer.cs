using System.Threading.Tasks;

namespace GerenciarTransacoes.Aplicacao.Interfaces
{
    public interface IMessageProducer
    {
        Task SendMessageAsync<T>(T message, string queueName);
    }
}
