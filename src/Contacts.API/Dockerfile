FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Contacts.API/Contacts.API.csproj", "src/Contacts.API/"]
COPY ["src/Contacts.Application/Contacts.Application.csproj", "src/Contacts.Application/"]
COPY ["src/Contacts.Domain/Contacts.Domain.csproj", "src/Contacts.Domain/"]
COPY ["src/Contacts.Infrastructure/Contacts.Infrastructure.csproj", "src/Contacts.Infrastructure/"]

RUN dotnet restore "src/Contacts.API/Contacts.API.csproj"

COPY . .
RUN dotnet publish "src/Contacts.API/Contacts.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Contacts.API.dll"]
