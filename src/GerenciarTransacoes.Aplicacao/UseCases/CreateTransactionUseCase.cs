using System;
using System.Threading.Tasks;
using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Dominio.Interfaces;
using GerenciarTransacoes.Aplicacao.Interfaces;

namespace GerenciarTransacoes.Aplicacao.UseCases
{
    // Representa os dados de entrada para a criação de uma nova transação.
    public class CreateTransactionCommand
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // Pode ser "Credit" para entrada ou "Debit" para saída
    }

    public class CreateTransactionUseCase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMessageProducer _messageProducer;

        public CreateTransactionUseCase(ITransactionRepository transactionRepository,
                                        IMessageProducer messageProducer)
        {
            _transactionRepository = transactionRepository;
            _messageProducer = messageProducer;
        }

        public async Task<Transaction> Execute(CreateTransactionCommand command)
        {
            // Validações de negócio para os dados de entrada
            if (command.Amount <= 0)
            {
                throw new ArgumentException("O valor da transação deve ser positivo.");
            }
            if (string.IsNullOrWhiteSpace(command.Description))
            {
                throw new ArgumentException("A descrição da transação não pode ser vazia.");
            }
            if (string.IsNullOrWhiteSpace(command.Type) || (command.Type != "Credit" && command.Type != "Debit"))
            {
                throw new ArgumentException("O tipo da transação deve ser 'Credit' ou 'Debit'.");
            }

            // Cria uma nova instância da entidade de domínio Transaction
            var transaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(), // Gera um ID único para a transação
                Amount = command.Amount,
                Description = command.Description,
                Date = DateTime.UtcNow, // Usa a data e hora UTC para consistência global
                Type = command.Type
            };

            // Adiciona a transação ao banco de dados via repositório
            await _transactionRepository.AddAsync(transaction);

            // Envia uma mensagem para o Service Bus sobre a nova transação
            await _messageProducer.SendMessageAsync(transaction, "transactions-queue");

            return transaction; // Retorna a transação criada
        }
    }
}
