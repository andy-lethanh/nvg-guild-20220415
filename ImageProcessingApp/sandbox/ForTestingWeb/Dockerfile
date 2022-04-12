FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["sandbox/ForTestingWeb/*", "ForTestingWeb/"]
RUN dotnet restore "ForTestingWeb/ForTestingWeb.csproj"
WORKDIR "/src/ForTestingWeb"
RUN dotnet build "ForTestingWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ForTestingWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForTestingWeb.dll"]
