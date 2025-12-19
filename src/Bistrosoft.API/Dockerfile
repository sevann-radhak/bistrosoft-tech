FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Bistrosoft.API/Bistrosoft.API.csproj", "Bistrosoft.API/"]
COPY ["Bistrosoft.Application/Bistrosoft.Application.csproj", "Bistrosoft.Application/"]
COPY ["Bistrosoft.Domain/Bistrosoft.Domain.csproj", "Bistrosoft.Domain/"]
COPY ["Bistrosoft.Infrastructure/Bistrosoft.Infrastructure.csproj", "Bistrosoft.Infrastructure/"]

RUN dotnet restore "Bistrosoft.API/Bistrosoft.API.csproj"

COPY . .
WORKDIR "/src/Bistrosoft.API"
RUN dotnet build "Bistrosoft.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Bistrosoft.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bistrosoft.API.dll"]
