#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Bongo.Functions/Bongo.Functions.csproj", "Bongo.Functions/"]
COPY ["Bongo.Application/Bongo.Application.csproj", "Bongo.Application/"]
COPY ["Bongo.Domain/Bongo.Domain.csproj", "Bongo.Domain/"]
RUN dotnet restore "./Bongo.Functions/./Bongo.Functions.csproj"
COPY . .
WORKDIR "/src/Bongo.Functions"
RUN dotnet build "./Bongo.Functions.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bongo.Functions.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

RUN mkdir /etc/secrets/
ENV FUNCTIONS_SECRETS_PATH=/etc/secrets
ENV AzureWebJobsSecretStorageType=Files
ENV apiFunctionKey somekey
ADD Bongo.Functions/host_secrets.json /etc/secrets/host.json
ADD Bongo.Functions/startup.sh startup.sh
RUN sed -i -e 's/\r$//' startup.sh
RUN chmod 700 startup.sh

ENTRYPOINT ["bash", "/home/site/wwwroot/startup.sh"]