using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Dominio.Interfaces;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GerenciarTransacoes.Infraestrutura.Repositories
{
    // Implementação do repositório de transações para MongoDB.
    // Atua como um adaptador da camada de Infraestrutura para a interface ITransactionRepository.
    public class TransactionRepository : ITransactionRepository
    {
        // Coleção MongoDB que armazena os documentos de transação.
        private readonly IMongoCollection<Transaction> _transactions;

        // Construtor que inicializa a conexão com o MongoDB.
        // Recebe a string de conexão e o nome do banco de dados do arquivo de configuração.
        public TransactionRepository(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            // Obtém ou cria a coleção "Transactions" no banco de dados.
            _transactions = database.GetCollection<Transaction>("Transactions");
        }

        // Adiciona uma nova transação ao MongoDB de forma assíncrona.
        public async Task AddAsync(Transaction transaction)
        {
            await _transactions.InsertOneAsync(transaction);
        }

        // Obtém todas as transações do MongoDB de forma assíncrona.
        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            // O filtro "_ => true" significa obter todos os documentos da coleção.
            return await _transactions.Find(_ => true).ToListAsync();
        }
    }
}
