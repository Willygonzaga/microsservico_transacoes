using System;
using System.Threading.Tasks;
using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Dominio.Interfaces; // Para ITransactionRepository
using GerenciarTransacoes.Aplicacao.Interfaces; // Para IMessageProducer <--- NOVO USING

namespace GerenciarTransacoes.Aplicacao.UseCases
{
    // Classe de comando para receber os dados de entrada para criação
    public class CreateTransactionCommand
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "Credit" ou "Debit"
    }

    public class CreateTransactionUseCase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMessageProducer _messageProducer; // <--- ADICIONE ESTA LINHA

        public CreateTransactionUseCase(ITransactionRepository transactionRepository, // <--- MODIFIQUE ESTE CONSTRUTOR
                                        IMessageProducer messageProducer) // <--- ADICIONE ESTE PARÂMETRO
        {
            _transactionRepository = transactionRepository;
            _messageProducer = messageProducer; // <--- ADICIONE ESTA LINHA
        }

        public async Task<Transaction> Execute(CreateTransactionCommand command)
        {
            // ... (código de validação existente)
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
                Id = Guid.NewGuid().ToString(),
                Amount = command.Amount,
                Description = command.Description,
                Date = DateTime.UtcNow,
                Type = command.Type
            };

            // Adiciona a transação ao banco de dados via repositório
            await _transactionRepository.AddAsync(transaction);

            // Envia uma mensagem para o Service Bus sobre a nova transação
            // Podemos enviar a própria transação ou um DTO mais leve
            // Por simplicidade, vamos enviar a transação completa por enquanto
            await _messageProducer.SendMessageAsync(transaction, "transactions-queue"); // <--- ADICIONE ESTA LINHA

            return transaction; // Retorna a transação criada
        }
    }
}