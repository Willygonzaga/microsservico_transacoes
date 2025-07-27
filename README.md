# Microsserviço de Gerenciamento de Transações Financeiras

Microsserviço feito para **criar e listar transações financeiras**.

## O que foi feito:

* **Criação e Listagem de Transações:** Registro de novas entradas ou saídas de dinheiro e consultas de todas as transações.
* **Armazenamento em Nuvem (MongoDB Atlas):** Todas as transações estão sendo guardadas no MongoDB Atlas.
* **Mensagens Assíncronas (Azure Service Bus):** Ao criar uma transação, uma notificação é enviada para o Azure.
* **Documentação Interativa (Swagger/OpenAPI):** Implementei o Swagger/OpenAPI para testar a API no navegador.
* **Docker:** Configurei o projeto para rodar em um contêiner Docker.

## Como Preparar e Rodar o Projeto na Sua Máquina (Passo a Passo)

Instruções para o microsserviço funcionar no seu computador.

### 1. Ferramentas que Você Precisa

Antes de começar, certifique-se de ter estas ferramentas instaladas no seu computador:

* **[Git](https://git-scm.com/downloads):** Essencial para baixar o código do projeto.
* **[.NET SDK 9.0 ou mais recente](https://dotnet.microsoft.com/download):** Necessário para construir e executar a aplicação (especialmente se você optar por não usar Docker).
* **[Docker Desktop](https://www.docker.com/products/docker-desktop/):** **Recomendado** Para rodar o projeto em um contêiner (simplifica a execução).
* **Seu Editor de Código:** Eu utilizei o **[Visual Studio Code](https://code.visualstudio.com/)** durante o desenvolvimento. Se você for usar o VS Code, recomendo instalar as seguintes extensões para C#:
    * **C#** (da Microsoft)
    * **C# Dev Kit** (da Microsoft)
    * **NuGet Package Manager**
* **[Thunder Client (Extensão do VS Code)](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client):** Esta é a extensão que eu usei no VS Code para testar a API.

### 2. Baixe e Prepare o Projeto

1.  **Baixe o código (Clone o Repositório):**
    * Primeiro, escolha uma pasta no seu computador onde você quer guardar o projeto (por exemplo, na sua Área de Trabalho, ou em Documentos).
    * Abra o terminal nessa pasta. Você pode fazer isso de algumas formas:
        * **Diretamente no Windows:** Abra o `Prompt de Comando` ou `PowerShell` e navegue até sua pasta com o comando `cd C:\Caminho\Da\Sua\Pasta`.
        * **Pelo VS Code:** Abra o VS Code, vá em `Terminal` > `Novo Terminal` (`Ctrl + '`) e navegue até sua pasta desejada.
    * Uma vez no terminal, dentro da pasta que você escolheu, digite o comando para baixar o projeto:
        ```bash
        git clone https://github.com/Willygonzaga/microsservico_transacoes.git
        ```
    * Após o download, entre na pasta do projeto:
        ```bash
        cd microsservico_transacoes
        ```
    * **Importante:** Agora, abra a pasta `microsservico_transacoes` no VS Code (foi o editor de código que utilizei). Vá em `Arquivo` > `Abrir Pasta...` e selecione `microsservico_transacoes`. Isso garantirá que o VS Code reconheça todo o projeto.

2.  **Construa a imagem Docker:**
    Se quiser rodar com esse método. Ainda no terminal, na pasta `microsservico_transacoes` (a raiz do projeto):
    ```bash
    docker build -t gerenciartransacoes-api .
    ```
    Este comando vai criar uma "imagem" da sua aplicação para o Docker. Pode demorar um pouco.

### 3. Configurações Essenciais: Suas Chaves de Conexão

Para que o microsserviço se conecte ao banco de dados (MongoDB Atlas) e ao serviço de fila (Azure Service Bus), você precisará usar suas próprias chaves de conexão. **Minhas chaves não estão no repositório por segurança.**

Siga estes passos para configurar as suas:

1.  **Obtenha suas Chaves e Nomes de Serviço:**

    * **MongoDB Atlas:**
        * Acesse [cloud.mongodb.com](https://cloud.mongodb.com/) e crie uma conta.
        * Crie um "Cluster" (o plano **M0 Shared** é gratuito).
        * Configure o "Network Access" para permitir o IP da sua máquina.
        * Crie um "Database User" (anote o usuário e a senha que você criar).
        * Pegue a **Connection String** do seu cluster (ex: `mongodb+srv://SEU_USUARIO:SUA_SENHA@SEU_CLUSTER.mongodb.net/...`).

    * **Azure Service Bus:**
        * Acesse [portal.azure.com](https://portal.azure.com/) e crie uma conta (Usei de estudante).
        * Crie um "Namespace" do Service Bus (o plano "Standard" é necessário). Anote o nome (ex: `sb-meunamespace`).
        * Crie uma "Fila" dentro desse Namespace (ex: `minha-fila-de-transacoes`). Anote o nome.
        * No seu Namespace, vá em `Políticas de acesso compartilhadas` > `RootManageSharedAccessKey` e pegue a **Primary Connection String (Cadeia de conexão primária)**.

2.  **Crie e configure o arquivo de chaves:**

    * No seu computador, **dentro da pasta do projeto `GerenciarTransacoes`** (o caminho é `microsservico_transacoes/GerenciarTransacoes`), crie um novo arquivo chamado **`appsettings.Development.json`**. Este arquivo será ignorado pelo Git, mantendo suas chaves seguras.

    * Cole o conteúdo abaixo nesse novo arquivo `appsettings.Development.json`:

        ```json
        {
          "Logging": {
            "LogLevel": {
              "Default": "Information",
              "Microsoft.AspNetCore": "Warning"
            }
          },
          "MongoDbSettings": {
            "ConnectionString": "COLE_AQUI_SUA_CONNECTION_STRING_DO_MONGO",
            "DatabaseName": "GerenciarTransacoesDB" // Pode usar este nome padrão ou outro de sua preferência
          },
          "ServiceBusSettings": {
            "ConnectionString": "COLE_AQUI_SUA_CONNECTION_STRING_DO_AZURE_SERVICE_BUS",
            "QueueName": "COLE_AQUI_O_NOME_DA_SUA_FILA_DO_AZURE"
          }
        }
        ```
    * **Importante:** No JSON acima, substitua:
        * `COLE_AQUI_SUA_CONNECTION_STRING_DO_MONGO` pela Connection String que você obteve do seu MongoDB Atlas.
        * `COLE_AQUI_SUA_CONNECTION_STRING_DO_AZURE_SERVICE_BUS` pela Primary Connection String (Cadeia de conexão primária) do seu Azure Service Bus.
        * `COLE_AQUI_O_NOME_DA_SUA_FILA_DO_AZURE` pelo nome da fila que você criou.

## Como Rodar o Microsserviço

Seu microsserviço pode ser executado de duas formas principais: usando **Docker** ou diretamente com o **.NET SDK**.

### 1. Rodar com Docker (Recomendado)

1.  **Inicie o contêiner Docker:**
    No terminal (já na raiz do projeto `microsservico_transacoes`), rode este comando. Ele vai iniciar sua aplicação dentro do Docker e a deixar acessível na porta `5010` do seu computador.

    **Atenção para usuários de PowerShell (Windows):** O comando abaixo usa `\` para quebra de linha. No PowerShell, você deve usar a crase ` ` `` ` ` ` ` no lugar da barra invertida, ou simplesmente copiar todo o comando e colá-lo em uma única linha, sem quebras.

    ```bash
    docker run -d -p 5010:8080 --name gerenciartransacoes-app \
        -e "MongoDbSettings__ConnectionString=COLE_AQUI_SUA_CONNECTION_STRING_DO_MONGO" \
        -e "MongoDbSettings__DatabaseName=GerenciarTransacoesDB" \
        -e "ServiceBusSettings__ConnectionString=COLE_AQUI_SUA_CONNECTION_STRING_DO_AZURE_SERVICE_BUS" \
        -e "ServiceBusSettings__QueueName=COLE_AQUI_O_NOME_DA_SUA_FILA_DO_AZURE" \
        gerenciartransacoes-api
    ```
    **Importante:** No comando acima, substitua os `COLE_AQUI_SUA_...` pelos **MESMOS VALORES** que você colocou no `appsettings.Development.json`.

2.  **Verifique se está rodando:**
    Para ter certeza que o contêiner iniciou:
    ```bash
    docker ps
    ```
    Você deve ver `gerenciartransacoes-app` listado com status "Up".

### 2. Rodar Diretamente com .NET SDK (Alternativa)

Se você preferir não usar Docker ou precisar de uma execução mais rápida para desenvolvimento:

1.  **Instale as dependências:**
    No terminal (na raiz do projeto `microsservico_transacoes`):
    ```bash
    dotnet restore
    ```
2.  **Inicie a aplicação:**
    No terminal, entre na pasta do projeto principal:
    ```bash
    cd GerenciarTransacoes
    ```
3.  **Executar a aplicação:**    
    E depois rode o projeto:
    ```bash
    dotnet run
    ```
    Aguarde a mensagem "Application started" (aplicação iniciada). O serviço estará disponível em `http://localhost:5010` (ou uma porta parecida que aparecerá no terminal).

## Como Testar a API

Com o microsserviço rodando (seja com Docker ou .NET SDK), você pode testar seus "endereços" (endpoints) usando o Thunder Client no VS Code, ou no próprio navegador com o Swagger.

### 1. Documentação Interativa (Swagger/OpenAPI)

Com o projeto em execução, acesse no seu navegador: `http://localhost:5010/swagger`
Você verá uma página onde pode visualizar e testar todos os endpoints da API.

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
    Vai aparecer uma mensagem no final indicando se todos os testes passaram (ex: "bem-sucedido: 8").

---
## Desenvolvedor
Willy Gonzaga Balieiro
