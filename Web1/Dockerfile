## Build Stage ##
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY Web1/Web1.csproj ./
RUN dotnet restore "./Web1.csproj"
COPY Web1/ ./
RUN dotnet publish "./Web1.csproj" -c Release -o /app/publish /p:UseAppHost=false


## Run Stage ##
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /run
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Web1.dll"]