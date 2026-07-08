FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os manifestos primeiro para otimizar cache de restore
COPY CapagWebAPI.sln .
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY WebAPI/WebAPI.csproj WebAPI/

RUN dotnet restore WebAPI/WebAPI.csproj

# Copia o restante do código e publica
COPY . .
RUN dotnet publish WebAPI/WebAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Servir em HTTP dentro do container; HTTPS pode ser tratado por proxy reverso externo
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]