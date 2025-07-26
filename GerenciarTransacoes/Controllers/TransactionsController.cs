using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Aplicacao.UseCases;

namespace GerenciarTransacoes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ListTransactionsUseCase _listTransactionsUseCase;
        private readonly CreateTransactionUseCase _createTransactionUseCase;

        public TransactionsController(ListTransactionsUseCase listTransactionsUseCase,
                                      CreateTransactionUseCase createTransactionUseCase)
        {
            _listTransactionsUseCase = listTransactionsUseCase;
            _createTransactionUseCase = createTransactionUseCase;
        }

        [HttpGet]
        public async Task<IEnumerable<Transaction>> Get()
        {
            return await _listTransactionsUseCase.Execute();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTransactionCommand command)
        {
            try
            {
                var newTransaction = await _createTransactionUseCase.Execute(command);
                return CreatedAtAction(nameof(Get), new { id = newTransaction.Id }, newTransaction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno ao criar a transação.", error = ex.Message });
            }
        }
    }
}