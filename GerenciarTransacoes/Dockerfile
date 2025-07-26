# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Copia os arquivos de projeto primeiro para que o Docker possa usar o cache
COPY ["GerenciarTransacoes/GerenciarTransacoes.csproj", "GerenciarTransacoes/"]
COPY ["src/GerenciarTransacoes.Dominio/GerenciarTransacoes.Dominio.csproj", "src/GerenciarTransacoes.Dominio/"]
COPY ["src/GerenciarTransacoes.Aplicacao/GerenciarTransacoes.Aplicacao.csproj", "src/GerenciarTransacoes.Aplicacao/"]
COPY ["src/GerenciarTransacoes.Infraestrutura/GerenciarTransacoes.Infraestrutura.csproj", "src/GerenciarTransacoes.Infraestrutura/"]
# Restaura as dependências dos pacotes NuGet
RUN dotnet restore "GerenciarTransacoes/GerenciarTransacoes.csproj"

# Copia o restante do código-fonte
COPY . .
# Navega para o diretório do projeto principal
WORKDIR "/src/GerenciarTransacoes"
# Compila a aplicação no modo "Release"
RUN dotnet build "GerenciarTransacoes.csproj" -c Release -o /app/build

# Estágio de Publicação (Publica apenas o necessário para rodar)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS publish
WORKDIR /src
# Copia apenas os arquivos de projeto novamente, para garantir que as referências estejam corretas para o publish
COPY --from=build /src/GerenciarTransacoes/GerenciarTransacoes.csproj GerenciarTransacoes/
COPY --from=build /src/GerenciarTransacoes.Dominio/GerenciarTransacoes.Dominio.csproj src/GerenciarTransacoes.Dominio/
COPY --from=build /src/GerenciarTransacoes.Aplicacao/GerenciarTransacoes.Aplicacao.csproj src/GerenciarTransacoes.Aplicacao/
COPY --from=build /src/GerenciarTransacoes.Infraestrutura/GerenciarTransacoes.Infraestrutura.csproj src/GerenciarTransacoes.Infraestrutura/
# Restaura novamente (garante todas as dependências)
RUN dotnet restore "GerenciarTransacoes/GerenciarTransacoes.csproj"
COPY . .
WORKDIR "/src/GerenciarTransacoes"
# Publica a aplicação para um diretório final
RUN dotnet publish "GerenciarTransacoes.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final (A imagem final que será executada)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
# Copia os arquivos publicados para o diretório de execução
COPY --from=publish /app/publish .

# Informa ao Docker que a aplicação escuta na porta 80 (HTTP)
EXPOSE 80

# Define o comando que será executado quando o contêiner iniciar
ENTRYPOINT ["dotnet", "GerenciarTransacoes.dll"]