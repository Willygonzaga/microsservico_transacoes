using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Dominio.Interfaces;
using MongoDB.Driver; // Importa o driver do MongoDB
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GerenciarTransacoes.Infraestrutura.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IMongoCollection<Transaction> _transactions;

        // O construtor recebe a string de conexão e o nome do banco de dados
        public TransactionRepository(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _transactions = database.GetCollection<Transaction>("Transactions"); // "Transactions" será o nome da sua coleção no MongoDB
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _transactions.InsertOneAsync(transaction);
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _transactions.Find(_ => true).ToListAsync();
        }
    }
}