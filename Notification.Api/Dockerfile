#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notification.Api/Notification.Api.csproj", "Notification.Api/"]
COPY ["Notification.Consumer/Notification.Consumer.csproj", "Notification.Consumer/"]
COPY ["Notification.Common/Notification.Common.csproj", "Notification.Common/"]
COPY ["Notification.Data/Notification.Data.csproj", "Notification.Data/"]
COPY ["Notification.RabbitMQ/Notification.RabbitMQ.csproj", "Notification.RabbitMQ/"]
RUN dotnet restore "./Notification.Api/Notification.Api.csproj"
COPY . .
WORKDIR "/src/Notification.Api"
RUN dotnet build "./Notification.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notification.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.Api.dll"]