FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DevsuCustomer.Api/DevsuCustomer.Api.csproj", "src/DevsuCustomer.Api/"]
COPY ["src/Devsu.Shared/Devsu.Shared.csproj", "src/Devsu.Shared/"]
RUN dotnet restore "src/DevsuCustomer.Api/DevsuCustomer.Api.csproj"
COPY . .
WORKDIR "/src/src/DevsuCustomer.Api"
RUN dotnet build "DevsuCustomer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DevsuCustomer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevsuCustomer.Api.dll"]
