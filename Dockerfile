FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
RUN curl -SLO https://deb.nodesource.com/nsolid_setup_deb.sh
RUN chmod 500 nsolid_setup_deb.sh
RUN ./nsolid_setup_deb.sh 21
RUN apt-get install nodejs -y
COPY ["SecureSend/SecureSend.csproj", "SecureSend/"]
COPY ["SecureSend.Application/SecureSend.Application.csproj", "SecureSend.Application/"]
COPY ["SecureSend.Infrastructure/SecureSend.Infrastructure.csproj", "SecureSend.Infrastructure/"]
COPY ["SecureSend.Domain/SecureSend.Domain.csproj", "SecureSend.Domain/"]
COPY ["SecureSend.SqlServerMigrations/SecureSend.SqlServerMigrations.csproj", "SecureSend.SqlServerMigrations/"]
COPY ["SecureSend.PostgresMigrations/SecureSend.PostgresMigrations.csproj", "SecureSend.PostgresMigrations/"]
RUN dotnet restore "SecureSend/SecureSend.csproj"
COPY . .
WORKDIR "/src/SecureSend"
RUN dotnet build "SecureSend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SecureSend.csproj" --no-restore -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80
ENV FileStorageOptions__Path=/app/files

RUN mkdir -p /app/files
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecureSend.dll"]
