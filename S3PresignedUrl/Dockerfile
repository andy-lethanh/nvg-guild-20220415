FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["s3/S3PresignedUrl/S3PresignedUrl.csproj", "S3PresignedUrl/"]
RUN dotnet restore "s3/S3PresignedUrl/S3PresignedUrl.csproj"
COPY . .
WORKDIR "/src/S3PresignedUrl"
RUN dotnet build "S3PresignedUrl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "S3PresignedUrl.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "S3PresignedUrl.dll"]
