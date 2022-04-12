FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY "nuget.config" .
COPY ["Me/SimpleApp.Me.Domain/*", "Me/SimpleApp.Me.Domain/"]
COPY ["Me/SimpleApp.Me.Infra/*", "Me/SimpleApp.Me.Infra/"]
COPY ["Me/SimpleApp.Me.Application/*", "Me/SimpleApp.Me.Application/"]
COPY ["Me/SimpleApp.Me.WebApi/*", "Me/SimpleApp.Me.WebApi/"]
COPY ["Shared/SimpleApp.Avatar.Shared/*", "Shared/SimpleApp.Avatar.Shared/"]
RUN dotnet restore "Me/SimpleApp.Me.WebApi/SimpleApp.Me.WebApi.csproj"
WORKDIR "/src/Me/SimpleApp.Me.WebApi"
RUN dotnet build "SimpleApp.Me.WebApi.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "SimpleApp.Me.WebApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleApp.Me.WebApi.dll"]
