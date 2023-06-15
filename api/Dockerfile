# Build application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY . /source
WORKDIR /source
RUN dotnet publish -c release -o /api

# Run application
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /api
COPY --from=build /api ./
ENTRYPOINT ["dotnet", "TheMostAmazingLunchAPI.dll"]