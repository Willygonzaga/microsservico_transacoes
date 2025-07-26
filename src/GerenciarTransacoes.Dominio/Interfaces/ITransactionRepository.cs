using System.Collections.Generic;
using System.Threading.Tasks;

namespace GerenciarTransacoes.Dominio.Interfaces
{
    // Define o contrato (interface) para operações de persistência de transações.
    // Esta interface é a "porta" na Clean Architecture para o domínio interagir com dados.
    public interface ITransactionRepository
    {
        // Adiciona uma nova transação ao repositório de forma assíncrona.
        Task AddAsync(Transaction transaction); 
        
        // Obtém todas as transações do repositório de forma assíncrona.
        Task<IEnumerable<Transaction>> GetAllAsync(); 
        
        // Outros métodos como GetByIdAsync, UpdateAsync, DeleteAsync podem ser adicionados depois.
    }
}
