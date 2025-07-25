using System.Collections.Generic;
using GerenciarTransacoes.Dominio; // Para acessar a entidade Transaction

namespace GerenciarTransacoes.Aplicacao.UseCases
{
    public class ListTransactionsUseCase
    {
        // No futuro, teremos um repositório real aqui
        // Por enquanto, vamos manter a lista mockada para facilitar a migração
        private readonly List<Transaction> _transactions = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid().ToString(), Amount = 150.75m, Description = "Compra no Supermercado", Date = DateTime.Now.AddDays(-2), Type = "Debit" },
            new Transaction { Id = Guid.NewGuid().ToString(), Amount = 500.00m, Description = "Salário Mensal", Date = DateTime.Now.AddDays(-1), Type = "Credit" },
            new Transaction { Id = Guid.NewGuid().ToString(), Amount = 25.50m, Description = "Cafia", Date = DateTime.Now, Type = "Debit" }
        };

        public IEnumerable<Transaction> Execute()
        {
            // Aqui a lógica para listar as transações (por enquanto, retorna o mock)
            return _transactions;
        }
    }
}