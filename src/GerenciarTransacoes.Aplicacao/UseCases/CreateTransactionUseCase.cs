using System;
using System.Threading.Tasks;
using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Dominio.Interfaces; // Para ITransactionRepository

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

        public CreateTransactionUseCase(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Transaction> Execute(CreateTransactionCommand command)
        {
            // Aqui podemos adicionar validações de negócio, se necessário
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
                Date = DateTime.UtcNow, // Usa UTC para consistência global
                Type = command.Type
            };

            // Adiciona a transação ao banco de dados via repositório
            await _transactionRepository.AddAsync(transaction);

            return transaction; // Retorna a transação criada
        }
    }
}