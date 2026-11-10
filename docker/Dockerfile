FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/FaceAuth.Domain/FaceAuth.Domain.csproj src/FaceAuth.Domain/
COPY src/FaceAuth.Contracts/FaceAuth.Contracts.csproj src/FaceAuth.Contracts/
COPY src/FaceAuth.Application/FaceAuth.Application.csproj src/FaceAuth.Application/
COPY src/FaceAuth.Infrastructure/FaceAuth.Infrastructure.csproj src/FaceAuth.Infrastructure/
COPY src/FaceAuth.Api/FaceAuth.Api.csproj src/FaceAuth.Api/
RUN dotnet restore src/FaceAuth.Api/FaceAuth.Api.csproj

COPY src/ src/
RUN dotnet publish src/FaceAuth.Api/FaceAuth.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FaceAuth.Api.dll"]
