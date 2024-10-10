FROM mcr.microsoft.com/dotnet/sdk:8.0  AS build
    WORKDIR /src
    # Target-args er satt av builder, må nevnes her for å få innhold. PAT må settes utenfra
    ARG PAT
    ENV HNDEV_PAT=$PAT
    # Publish
    COPY . .
    RUN dotnet publish "tlstest.csproj" -c Release -o /app/publish

### 2 Runnable image ###
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-extra AS base
    WORKDIR /app
    COPY --from=build /app/publish .
    ARG VERSION
    ENV HNCORE_ConfigTag=$VERSION
    ENTRYPOINT ["dotnet", "tlstest.dll"]