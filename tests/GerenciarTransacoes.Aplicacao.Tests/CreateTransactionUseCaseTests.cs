using Xunit;
using Moq;
using System.Threading.Tasks;
using GerenciarTransacoes.Aplicacao.UseCases;
using GerenciarTransacoes.Dominio.Interfaces;
using GerenciarTransacoes.Aplicacao.Interfaces;
using GerenciarTransacoes.Dominio;
using System;
using System.Collections.Generic; // Adicionado para IMessageProducer.Setup - It.IsAny<string>() precisa de ToString() para ser ambíguo, mas sem ele não precisa. Adicionado para garantir.

namespace GerenciarTransacoes.Aplicacao.Tests
{
    public class CreateTransactionUseCaseTests
    {
        [Fact]
        public async Task Execute_ShouldCreateTransaction_WhenCommandIsValid()
        {
            // Arrange
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockMessageProducer = new Mock<IMessageProducer>();

            mockTransactionRepository.Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                                     .Returns(Task.CompletedTask);

            mockMessageProducer.Setup(producer => producer.SendMessageAsync(It.IsAny<Transaction>(), It.IsAny<string>()))
                               .Returns(Task.CompletedTask);

            var useCase = new CreateTransactionUseCase(mockTransactionRepository.Object, mockMessageProducer.Object);

            var command = new CreateTransactionCommand
            {
                Amount = 100.00m,
                Description = "Teste de compra",
                Type = "Debit"
            };

            // Act
            var result = await useCase.Execute(command);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Id);
            Assert.Equal(command.Amount, result.Amount);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.Type, result.Type);

            mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Once);
            mockMessageProducer.Verify(producer => producer.SendMessageAsync(It.IsAny<Transaction>(), "transactions-queue"), Times.Once);
        }

        [Theory]
        [InlineData(0, "Desc", "Debit", "O valor da transação deve ser positivo.")]
        [InlineData(-50, "Desc", "Credit", "O valor da transação deve ser positivo.")]
        [InlineData(100, "", "Debit", "A descrição da transação não pode ser vazia.")]
        [InlineData(100, "Desc", "", "O tipo da transação deve ser 'Credit' ou 'Debit'.")]
        [InlineData(100, "Desc", "InvalidType", "O tipo da transação deve ser 'Credit' ou 'Debit'.")]
        public async Task Execute_ShouldThrowArgumentException_WhenCommandIsInvalid(
            decimal amount, string description, string type, string expectedErrorMessage)
        {
            // Arrange
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockMessageProducer = new Mock<IMessageProducer>();
            var useCase = new CreateTransactionUseCase(mockTransactionRepository.Object, mockMessageProducer.Object);

            var command = new CreateTransactionCommand
            {
                Amount = amount,
                Description = description,
                Type = type
            };

            // Act & Assert
            // Verifica se o método lança uma ArgumentException com a mensagem esperada.
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => useCase.Execute(command));
            Assert.Equal(expectedErrorMessage, exception.Message);

            // Verifica que nenhum método de persistência ou envio de mensagem foi chamado.
            mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Never);
            mockMessageProducer.Verify(producer => producer.SendMessageAsync(It.IsAny<Transaction>(), It.IsAny<string>()), Times.Never);
        }
    }
}
