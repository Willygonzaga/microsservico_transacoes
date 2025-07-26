# Microsserviço de Gerenciamento de Transações Financeiras

## 🚀 Sobre Este Projeto

Este microsserviço foi desenvolvido em **C# com ASP.NET Core** para gerenciar transações financeiras. Ele permite **criar e listar transações**.

* Os dados são armazenados no **MongoDB Atlas** (um banco de dados na nuvem).
* Ao criar uma transação, uma mensagem é enviada para uma fila no **Azure Service Bus**, permitindo processamentos futuros de forma assíncrona.
* O projeto segue princípios de **Arquitetura Limpa** e inclui **Testes Unitários** para a lógica de negócio.

## 🛠️ O que Você Precisa para Rodar

Para executar este projeto na sua máquina, você precisará ter instalado:

* **[.NET SDK 9.0 ou mais recente](https://dotnet.microsoft.com/download)**
* **[Visual Studio Code](https://code.visualstudio.com/)** (com as extensões de C#, C# Dev Kit, NuGet Package Manager)
* **[Git](https://git-scm.com/downloads)**
* **Acesso a uma conta no [MongoDB Atlas](https://cloud.mongodb.com/)** (use o plano gratuito M0)
* **Acesso a uma conta no [Azure](https://portal.azure.com/)** (use uma conta gratuita ou de estudante)
* **[Thunder Client](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client)** (extensão do VS Code) ou Postman/Insomnia, para testar a API.

## ⚙️ Como Configurar para Rodar

Para que o projeto funcione, precisa configurar as chaves de conexão. Elas ficarão salvas em um arquivo que o Git ignora, por segurança.

### 1. Configure suas Conexões

Abra o arquivo `GerenciarTransacoes/appsettings.Development.json` e adicione/atualize as seguintes informações com as **suas próprias chaves e nomes** que você configurou no MongoDB Atlas e no Azure:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb+srv://admin:admin2000-07-07@gerenciartransacoesclus.udlzfm8.mongodb.net/?retryWrites=true&w=majority&appName=GerenciarTransacoesCluster",
    "DatabaseName": "GerenciarTransacoesDB"
  },
  "ServiceBusSettings": {
    "ConnectionString": "Endpoint=sb://sb-gerenciartransacoes-willy.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SUA_CHAVE_DE_ACESSO_COMPARTILHADA_AQUI",
    "QueueName": "transactions-queue"
  }
}

### 2. Prepare o Projeto

1.  **Baixe o código:**
    Abra seu terminal e baixe o projeto:
    ```bash
    git clone [https://github.com/Willygonzaga/microsservico_transacoes.git](https://github.com/Willygonzaga/microsservico_transacoes.git)
    cd microsservico_transacoes
    ```
2.  **Instale as dependências:**
    Ainda no terminal (na raiz do projeto `microsservico_transacoes`):
    ```bash
    dotnet restore
    ```
    Isso vai baixar todas as bibliotecas que o projeto precisa.

## ▶️ Como Rodar o Microsserviço

1.  **Inicie a aplicação:**
    No terminal, entre na pasta do projeto principal e execute:
    ```bash
    cd GerenciarTransacoes
    dotnet run
    ```
    Aguarde a mensagem "Application started" (aplicação iniciada). O serviço estará disponível em `http://localhost:5010` (ou uma porta parecida que aparecerá no terminal).

## 🚀 Como Testar os Endpoints da API

Com o microsserviço rodando, você pode usar o **Thunder Client** no VS Code (foi o que eu usei) para enviar requisições.

Os endereços (endpoints) sempre começam com `/transactions`.

### 1. Listar Transações (GET)

* **Endereço (URL):** `http://localhost:5010/transactions`
* **Método:** `GET`
* **Resultado esperado:** Uma lista de transações em formato JSON. Se o banco estiver vazio, ele retornará `[]`.

### 2. Criar Transação (POST)

* **Endereço (URL):** `http://localhost:5010/transactions`
* **Método:** `POST`
* **Corpo do Pedido (JSON que você envia):**
    ```json
    {
      "amount": 150.75,
      "description": "Exemplo de compra de café",
      "type": "Debit" // Ou você pode usar "Credit"
    }
    ```
* **Resultado esperado:**
    * `201 Created` (sucesso) e os detalhes da transação criada.
    * Um erro `400 Bad Request` se os dados enviados estiverem incorretos.

## ✅ Como Rodar os Testes Unitários

Para garantir que a lógica do projeto está funcionando corretamente:

1.  Abra seu terminal e navegue até a **raiz do repositório**.
2.  Execute o comando:
    ```bash
    dotnet test tests/GerenciarTransacoes.Aplicacao.Tests/GerenciarTransacoes.Aplicacao.Tests.csproj
    ```
    Você verá uma mensagem no final indicando que todos os testes passaram (ex: "bem-sucedido: 8").

---
Desenvolvido por: Willy Gonzaga Balieiro