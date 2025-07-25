using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // Adicione esta linha
using System; // Adicione esta linha

namespace GerenciarTransacoes.Controllers
{
    [ApiController]
    [Route("[controller]")] // A rota será /transactions
    public class TransactionsController : ControllerBase
    {
        // Crie uma classe interna para representar a transação mockada
        public class Transaction
        {
            public string Id { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public string Type { get; set; } // Ex: "Credit", "Debit"
        }

        private readonly List<Transaction> _transactions = new List<Transaction> {
            new Transaction { Id = Guid.NewGuid().ToString(), Amount = 150.75m, Description = "Compra no Supermercado", Date = DateTime.Now.AddDays(-2), Type = "Debit" },
            new Transaction { Id = Guid.NewGuid().ToString(), Amount = 500.00m, Description = "Salário Mensal", Date = DateTime.Now.AddDays(-1), Type = "Credit" },
            new Transaction { Id = Guid.NewGuid().ToString(), Amount = 25.50m, Description = "Cafia", Date = DateTime.Now, Type = "Debit" }
        };

        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            return _transactions;
        }
    }
}