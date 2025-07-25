using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Aplicacao.UseCases;

using System.Threading.Tasks; // <--- ADICIONE ESTA LINHA

namespace GerenciarTransacoes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ListTransactionsUseCase _listTransactionsUseCase; // Adicione esta linha

        // Construtor que recebe o caso de uso por Injeção de Dependência
        public TransactionsController(ListTransactionsUseCase listTransactionsUseCase) // Adicione este construtor
        {
            _listTransactionsUseCase = listTransactionsUseCase;
        }

        [HttpGet]
        public async Task<IEnumerable<Transaction>> Get() // O método agora é assíncrono
        {
            // Chama o caso de uso injetado
            return await _listTransactionsUseCase.Execute();
        }
    }
}