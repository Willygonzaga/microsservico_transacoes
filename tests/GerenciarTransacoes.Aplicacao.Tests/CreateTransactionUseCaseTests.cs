using Xunit; // Para atributos de teste como [Fact]
using Moq; // Para criar mocks
using System.Threading.Tasks; // Para Task
using GerenciarTransacoes.Aplicacao.UseCases; // Para CreateTransactionUseCase e CreateTransactionCommand
using GerenciarTransacoes.Dominio.Interfaces; // Para ITransactionRepository e IMessageProducer
using GerenciarTransacoes.Aplicacao.Interfaces; // Para IMessageProducer <--- ADICIONE/CONFIRME ESTA LINHA
using GerenciarTransacoes.Dominio; // Para a entidade Transaction
using System; // Para ArgumentException, Guid

namespace GerenciarTransacoes.Aplicacao.Tests
{
    public class CreateTransactionUseCaseTests
    {
        [Fact] // Este atributo marca o método como um teste
        public async Task Execute_ShouldCreateTransaction_WhenCommandIsValid()
        {
            // Arrange (Preparar)
            // Criar mocks para as dependências
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockMessageProducer = new Mock<IMessageProducer>();

            // Configurar o mock do repositório para não fazer nada quando AddAsync for chamado
            mockTransactionRepository.Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                                     .Returns(Task.CompletedTask);

            // Configurar o mock do produtor de mensagens para não fazer nada quando SendMessageAsync for chamado
            mockMessageProducer.Setup(producer => producer.SendMessageAsync(It.IsAny<Transaction>(), It.IsAny<string>()))
                               .Returns(Task.CompletedTask);

            // Instanciar o caso de uso com as dependências mockadas
            var useCase = new CreateTransactionUseCase(mockTransactionRepository.Object, mockMessageProducer.Object);

            // Criar um comando de transação válido
            var command = new CreateTransactionCommand
            {
                Amount = 100.00m,
                Description = "Teste de compra",
                Type = "Debit"
            };

            // Act (Agir)
            // Executar o caso de uso
            var result = await useCase.Execute(command);

            // Assert (Verificar)
            // Verificar se a transação foi criada corretamente
            Assert.NotNull(result);
            Assert.NotEmpty(result.Id);
            Assert.Equal(command.Amount, result.Amount);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.Type, result.Type);

            // Verificar se o método AddAsync do repositório foi chamado exatamente uma vez
            mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Once);
            // Verificar se o método SendMessageAsync do produtor de mensagens foi chamado exatamente uma vez
            mockMessageProducer.Verify(producer => producer.SendMessageAsync(It.IsAny<Transaction>(), "transactions-queue"), Times.Once);
        }

        [Theory] // Usamos [Theory] para testar com diferentes dados
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

            // Act & Assert (Agir e Verificar)
            // Verificar se o método lança uma ArgumentException com a mensagem esperada
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => useCase.Execute(command));
            Assert.Equal(expectedErrorMessage, exception.Message);

            // Verificar que nenhum método de persistência ou envio de mensagem foi chamado
            mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Never);
            mockMessageProducer.Verify(producer => producer.SendMessageAsync(It.IsAny<Transaction>(), It.IsAny<string>()), Times.Never);
        }
    }
}