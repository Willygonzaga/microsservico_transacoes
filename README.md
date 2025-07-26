# Microsserviço de Gerenciamento de Transações Financeiras

Microsserviço feito para **criar e listar transações financeiras**.

## O que foi feito:

* **Criação e Listagem de Transações:** Registro de novas entradas ou saídas de dinheiro e consultas de todas as transações.
* **Armazenamento em Nuvem (MongoDB Atlas):** Todas as transações estão sendo guardadas no MongoDB Atlas.
* **Mensagens Assíncronas (Azure Service Bus):** Ao criar uma transação, uma notificação é enviada para o Azure.
* **Documentação Interativa (Swagger/OpenAPI):** Implementei o Swagger/OpenAPI para testar a API no navegador.
* **Pronto para Docker:** Configurei o projeto para rodar em um contêiner Docker.

## Como Preparar e Rodar o Projeto

Instruções para o projeto rodar:

### 1. Ferramentas Necessárias

Precisa dessas ferramentas:

* **[Docker Desktop](https://www.docker.com/products/docker-desktop/):** Para rodar o projeto em um contêiner.
* **[.NET SDK 9.0 ou mais recente](https://dotnet.microsoft.com/download):** Para construir e gerenciar o projeto.
* **[Git](https://git-scm.com/downloads):** Para baixar o código.
* **[Thunder Client (VS Code Extension)](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client):** Uma extensão do VS Code que ajuda a testar a API (Utilizei essa).

### 2. Configuração das Chaves de Conexão

Você irá precisar de "chaves" para se conectar ao MongoDB e ao Azure Service Bus. Por segurança (e o GitHuba não deixar realizar o "push"), essas chaves não ficam no código principal, mas em um arquivo separado que precisa ser criado no seu computador.

1.  **Crie sua conta e serviços na nuvem:**
    * **MongoDB Atlas:** Acesse [cloud.mongodb.com](https://cloud.mongodb.com/), crie uma conta. Crie um usuário de banco de dados (usei `admin` com a senha `admin2000-07-07`). Pegue a **Connection String** do seu cluster.
    * **Azure Service Bus:** Acesse [portal.azure.com](https://portal.azure.com/), crie uma conta (usei de estudante). Crie um "Namespace" (usei `sb-gerenciartransacoes-willy`) e uma "Fila" dentro dele (usei `transactions-queue`). Pegue a **Primary Connection String** da política `RootManageSharedAccessKey` do seu Namespace.

2.  **Crie e configure o arquivo de chaves:**
    Abra o arquivo `GerenciarTransacoes/appsettings.Development.json` e cole o seguinte conteúdo. **Substitua `SUA_CHAVE_DE_ACESSO_COMPARTILHADA_AQUI` pela sua chave real do Azure Service Bus.**

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
    ```

### 3. Baixar e Preparar o Projeto

1.  **Baixar o código:**
    Abra seu terminal e digite:
    ```bash
    git clone [https://github.com/Willygonzaga/microsservico_transacoes.git](https://github.com/Willygonzaga/microsservico_transacoes.git)

    cd microsservico_transacoes
    ```
2.  **Construa a imagem Docker:**
    Ainda no terminal, na pasta `microsservico_transacoes` (a raiz do projeto):
    ```bash
    docker build -t gerenciartransacoes-api .
    ```
    Este comando vai criar uma "imagem" da aplicação para o Docker.



## Como Rodar o Microsserviço

O microsserviço pode ser executado de duas formas principais: usando **Docker** ou diretamente com o **.NET SDK**.

### 1. Rodar com Docker

Depois que a imagem Docker for construída, você pode iniciar o serviço.

1.  **Inicie o contêiner Docker:**
    No terminal, rode este comando. Ele vai iniciar sua aplicação dentro do Docker e a deixar acessível na porta `5010` do computador.

    ```bash
    docker run -d -p 5010:8080 --name gerenciartransacoes-app -e "MongoDbSettings__ConnectionString=mongodb+srv://admin:admin2000-07-07@gerenciartransacoesclus.udlzfm8.mongodb.net/?retryWrites=true&w=majority&appName=GerenciarTransacoesCluster" -e "MongoDbSettings__DatabaseName=GerenciarTransacoesDB" -e "ServiceBusSettings__ConnectionString=Endpoint=sb://sb-gerenciartransacoes-willy.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SUA_CHAVE_DE_ACESSO_COMPARTILHADA_AQUI" -e "ServiceBusSettings__QueueName=transactions-queue" gerenciartransacoes-api
    ```
    (Precisa substituir `SUA_CHAVE_DE_ACESSO_COMPARTILHADA_AQUI` pela sua chave real).

2.  **Verifique se está rodando:**
    Para ter certeza que o contêiner iniciou:
    ```bash
    docker ps
    ```

### 2. Rodar Diretamente com .NET SDK

Se não usar Docker:

1.  **Inicie a aplicação:**
    No terminal, entre na pasta do projeto principal:
    ```bash
    cd GerenciarTransacoes

    dotnet run
    ```
    O serviço vai estar em `http://localhost:5010` (ou uma porta parecida que aparecerá no terminal).

---


## Como Testar a API

Com o microsserviço rodando, você pode testar os endpoints usando o Thunder Client (no VS Code foi o que usei) ou no navegador.

### 1. Documentação Interativa (Swagger/OpenAPI)

Acesse no navegador: `http://localhost:5010/swagger`
Vai aparecer uma página onde dá para visualizar e testar todos os endpoints da API.

### 2. Listar Transações (GET)

* **URL:** `http://localhost:5010/transactions`
* **Método:** `GET`
* **Resultado:** Uma lista de transações em formato JSON.

### 3. Criar Transação (POST)

* **URL:** `http://localhost:5010/transactions`
* **Método:** `POST`
* **Corpo do Pedido (JSON que você envia):**
    ```json
    {
      "amount": 150.75,
      "description": "Exemplo de compra via Docker",
      "type": "Debit"
    }
    ```
* **Resultado:** `201 Created` mensagem de sucesso e os detalhes da transação criada.

## Como Rodar os Testes Unitários

1.  Abra o terminal e navegue até a **raiz do repositório** (`microsservico_transacoes`).
2.  Execute:
    ```bash
    dotnet test tests/GerenciarTransacoes.Aplicacao.Tests/GerenciarTransacoes.Aplicacao.Tests.csproj
    ```
    Vai aparecer uma mensagem no final indicando que todos os testes passaram (ex: "bem-sucedido: 8").

---
Desenvolvido por: Willy Gonzaga
