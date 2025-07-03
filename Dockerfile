# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY Programaci-n/*.csproj ./Programaci-n/
RUN dotnet restore Programaci-n/APIPROYECT.csproj

COPY . .

WORKDIR /src/Programaci-n
RUN dotnet publish APIPROYECT.csproj -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "APIPROYECT.dll"]