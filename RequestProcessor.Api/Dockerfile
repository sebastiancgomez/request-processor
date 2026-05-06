
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["RequestProcessor.Api/RequestProcessor.Api.csproj", "RequestProcessor.Api/"]
COPY ["RequestProcessor.Application/RequestProcessor.Application.csproj", "RequestProcessor.Application/"]
COPY ["RequestProcessor.Infrastructure/RequestProcessor.Infrastructure.csproj", "RequestProcessor.Infrastructure/"]
COPY ["RequestProcessor.Domain/RequestProcessor.Domain.csproj", "RequestProcessor.Domain/"]


RUN dotnet restore "RequestProcessor.Api/RequestProcessor.Api.csproj"


COPY . .
WORKDIR "/src/RequestProcessor.Api"
RUN dotnet publish "RequestProcessor.Api.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080


COPY --from=build /app/publish .


ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RequestProcessor.Api.dll"]