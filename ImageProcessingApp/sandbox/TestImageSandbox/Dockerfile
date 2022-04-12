FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["sandbox/TestImageSandbox/TestImageSandbox.csproj", "TestImageSandbox/"]
RUN dotnet restore "sandbox/TestImageSandbox/TestImageSandbox.csproj"
COPY . .
WORKDIR "/src/TestImageSandbox"
RUN dotnet build "TestImageSandbox.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestImageSandbox.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestImageSandbox.dll"]
