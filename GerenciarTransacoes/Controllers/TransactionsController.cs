using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks; // Certifique-se que este using está presente
using GerenciarTransacoes.Dominio;
using GerenciarTransacoes.Aplicacao.UseCases;

namespace GerenciarTransacoes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ListTransactionsUseCase _listTransactionsUseCase;
        private readonly CreateTransactionUseCase _createTransactionUseCase; // <--- ADICIONE ESTA LINHA

        // Construtor que recebe ambos os casos de uso por Injeção de Dependência
        public TransactionsController(ListTransactionsUseCase listTransactionsUseCase, // <--- MODIFIQUE ESTE CONSTRUTOR
                                      CreateTransactionUseCase createTransactionUseCase) // <--- ADICIONE ESTE PARÂMETRO
        {
            _listTransactionsUseCase = listTransactionsUseCase;
            _createTransactionUseCase = createTransactionUseCase; // <--- ADICIONE ESTA LINHA
        }

        [HttpGet]
        public async Task<IEnumerable<Transaction>> Get()
        {
            return await _listTransactionsUseCase.Execute();
        }

        [HttpPost] // <--- ADICIONE ESTE NOVO MÉTODO
        public async Task<IActionResult> Post([FromBody] CreateTransactionCommand command)
        {
            try
            {
                var newTransaction = await _createTransactionUseCase.Execute(command);
                // Retorna 201 Created com a transação criada e o local do recurso
                return CreatedAtAction(nameof(Get), new { id = newTransaction.Id }, newTransaction);
            }
            catch (ArgumentException ex)
            {
                // Retorna 400 Bad Request para erros de validação
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Retorna 500 Internal Server Error para outros erros
                return StatusCode(500, new { message = "Ocorreu um erro interno ao criar a transação.", error = ex.Message });
            }
        }
    }
}