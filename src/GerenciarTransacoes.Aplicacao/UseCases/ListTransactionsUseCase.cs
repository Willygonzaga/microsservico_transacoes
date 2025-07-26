using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Dominio.Interfaces;

namespace GerenciarTransacoes.Aplicacao.UseCases
{
    public class ListTransactionsUseCase
    {
        private readonly ITransactionRepository _transactionRepository;

        public ListTransactionsUseCase(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        // Obtém todas as transações do repositório de forma assíncrona.
        public async Task<IEnumerable<Transaction>> Execute()
        {
            return await _transactionRepository.GetAllAsync();
        }
    }
}
