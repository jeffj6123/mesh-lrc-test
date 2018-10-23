FROM microsoft/dotnet:2.1-runtime-nanoserver-1709 AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY AppClient/AppClient.csproj AppClient/
RUN dotnet restore AppClient/AppClient.csproj
COPY . .
WORKDIR /src/AppClient
RUN dotnet build AppClient.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AppClient.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AppClient.dll"]
