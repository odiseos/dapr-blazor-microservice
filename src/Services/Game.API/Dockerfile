#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ms-game-history/ms-game-history.csproj", "src/ms-game-history/"]
RUN dotnet restore "src/ms-game-history/ms-game-history.csproj"
COPY . .
WORKDIR "/src/src/ms-game-history"
RUN dotnet build "ms-game-history.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ms-game-history.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ms-game-history.dll"]