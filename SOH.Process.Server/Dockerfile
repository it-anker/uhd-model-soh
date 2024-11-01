FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SOH.Process.Server/SOH.Process.Server.csproj", "SOH.Process.Server/"]
RUN dotnet restore "SOH.Process.Server/SOH.Process.Server.csproj"
COPY . .
WORKDIR "/src/SOH.Process.Server"
RUN dotnet build "SOH.Process.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SOH.Process.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SOH.Process.Server.dll"]
