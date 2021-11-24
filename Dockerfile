#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NodeWebApi/NodeWebApi.csproj", "NodeWebApi/"]
COPY ["NodeNetworking/NodeNetworking.csproj", "NodeNetworking/"]
RUN dotnet restore "Node.sln"
COPY . .
WORKDIR "/src"
RUN dotnet build "Node.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Node.sln" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NodeWebApi.dll"]