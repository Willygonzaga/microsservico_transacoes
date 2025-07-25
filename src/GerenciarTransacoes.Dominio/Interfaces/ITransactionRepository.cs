using System.Collections.Generic;
using System.Threading.Tasks; // Para usar Task (métodos assíncronos)

namespace GerenciarTransacoes.Dominio.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction); // Adicionar uma nova transação
        Task<IEnumerable<Transaction>> GetAllAsync(); // Obter todas as transações
        // Outros métodos como GetByIdAsync, UpdateAsync, DeleteAsync podem ser adicionados depois
    }
}