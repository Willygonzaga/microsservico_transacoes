using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Aplicacao.UseCases; // Adicione esta linha

namespace GerenciarTransacoes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        // Removemos a lista _transactions mockada daqui

        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            // Instancia o caso de uso e executa-o
            var useCase = new ListTransactionsUseCase();
            return useCase.Execute();
        }
    }
}