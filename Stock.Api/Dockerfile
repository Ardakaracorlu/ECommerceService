#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Stock.Api/Stock.Api.csproj", "Stock.Api/"]
COPY ["Stock.Consumer/Stock.Consumer.csproj", "Stock.Consumer/"]
COPY ["Stock.Common/Stock.Common.csproj", "Stock.Common/"]
COPY ["Stock.Data/Stock.Data.csproj", "Stock.Data/"]
COPY ["Stock.RabbitMQ/Stock.RabbitMQ.csproj", "Stock.RabbitMQ/"]
RUN dotnet restore "./Stock.Api/Stock.Api.csproj"
COPY . .
WORKDIR "/src/Stock.Api"
RUN dotnet build "./Stock.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Stock.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stock.Api.dll"]