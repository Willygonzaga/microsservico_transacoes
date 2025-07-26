# Estágio de Build: Utiliza a imagem SDK do .NET para compilar a aplicação.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia todo o conteúdo do diretório atual (raiz do repositório) para o diretório de trabalho no contêiner.
# Isso inclui todos os arquivos de projeto (.csproj), o arquivo de solução (.sln) e o código-fonte.
COPY . . 

# Define o diretório de trabalho para o projeto principal da API para as próximas operações.
WORKDIR /src/GerenciarTransacoes

# Restaura as dependências dos pacotes NuGet definidas no arquivo .csproj do projeto principal.
RUN dotnet restore "GerenciarTransacoes.csproj"

# Compila e publica a aplicação em modo "Release" para otimização de desempenho e tamanho.
# O '-o /app/publish' define o diretório de saída dentro do contêiner.
# '/p:UseAppHost=false' é importante para garantir que a aplicação seja executável diretamente pelo 'dotnet'.
RUN dotnet publish "GerenciarTransacoes.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final (Runtime): Utiliza uma imagem ASP.NET menor e otimizada para execução em produção.
# Esta imagem contém apenas o runtime necessário, resultando em um contêiner mais leve e seguro.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia os arquivos da aplicação já publicados (do estágio 'build') para o diretório de execução da imagem final.
COPY --from=build /app/publish .

# Expõe a porta 80 do contêiner, indicando que a aplicação escuta requisições HTTP nesta porta.
EXPOSE 80

# Define o comando que será executado quando o contêiner for iniciado.
# Inicia a aplicação .NET principal.
ENTRYPOINT ["dotnet", "GerenciarTransacoes.dll"]
