# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia o arquivo da solução (.sln) e todas as pastas dos projetos.
# O ponto "." no final significa copiar tudo do contexto de build (a raiz do repositório) para /src no contêiner.
COPY . . 

# Define o diretório de trabalho para o projeto principal (API) para restaurar e construir
WORKDIR /src/GerenciarTransacoes

# Restaura as dependências dos pacotes NuGet para o projeto principal
RUN dotnet restore "GerenciarTransacoes.csproj"

# Compila e publica a aplicação em modo "Release" para um diretório final /app/publish
# O /p:UseAppHost=false é importante para o Dockerfile
RUN dotnet publish "GerenciarTransacoes.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final (Runtime) - A imagem final que será executada
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia os arquivos publicados do estágio 'build' para o diretório de execução da imagem final
COPY --from=build /app/publish .

# Informa ao Docker que a aplicação escuta na porta 80 (HTTP)
EXPOSE 80

# Define o comando que será executado quando o contêiner iniciar
ENTRYPOINT ["dotnet", "GerenciarTransacoes.dll"]