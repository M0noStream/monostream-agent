#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MonoStreamAgent.csproj", "."]
RUN dotnet restore "./MonoStreamAgent.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MonoStreamAgent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonoStreamAgent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonoStreamAgent.dll"]