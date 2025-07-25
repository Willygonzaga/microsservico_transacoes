using System.Collections.Generic;
using System.Threading.Tasks; // Adicione para suportar Task
using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Dominio.Interfaces; // Para ITransactionRepository

namespace GerenciarTransacoes.Aplicacao.UseCases
{
    public class ListTransactionsUseCase
    {
        private readonly ITransactionRepository _transactionRepository;

        // O construtor agora recebe a interface do repositório
        public ListTransactionsUseCase(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<Transaction>> Execute() // O método agora é assíncrono
        {
            // Usa o repositório para obter as transações do banco de dados
            return await _transactionRepository.GetAllAsync();
        }
    }
}