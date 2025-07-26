using Xunit;
using Moq;
using System.Threading.Tasks;
using GerenciarTransacoes.Aplicacao.UseCases;
using GerenciarTransacoes.Dominio.Interfaces;
using GerenciarTransacoes.Dominio;
using System.Collections.Generic; // Para IEnumerable
using System.Linq; // Para .ToList()

namespace GerenciarTransacoes.Aplicacao.Tests
{
    public class ListTransactionsUseCaseTests
    {
        [Fact]
        public async Task Execute_ShouldReturnAllTransactions()
        {
            // Arrange
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            // Configurar o mock para retornar uma lista de transações de exemplo
            var expectedTransactions = new List<Transaction>
            {
                new Transaction { Id = "1", Amount = 10m, Description = "Test1", Type = "Debit" },
                new Transaction { Id = "2", Amount = 20m, Description = "Test2", Type = "Credit" }
            };
            mockTransactionRepository.Setup(repo => repo.GetAllAsync())
                                     .ReturnsAsync(expectedTransactions); // Retorna a lista mockada assincronamente

            var useCase = new ListTransactionsUseCase(mockTransactionRepository.Object);

            // Act
            var result = await useCase.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.Count, result.Count()); // Verifica se o número de itens é o mesmo
            Assert.Contains(result, t => t.Id == "1"); // Verifica se contém itens específicos
            Assert.Contains(result, t => t.Id == "2");

            // Verificar se o método GetAllAsync do repositório foi chamado exatamente uma vez
            mockTransactionRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldReturnEmptyList_WhenNoTransactionsExist()
        {
            // Arrange
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            // Configurar o mock para retornar uma lista vazia
            mockTransactionRepository.Setup(repo => repo.GetAllAsync())
                                     .ReturnsAsync(new List<Transaction>());

            var useCase = new ListTransactionsUseCase(mockTransactionRepository.Object);

            // Act
            var result = await useCase.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Verifica se a lista está vazia
            mockTransactionRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}