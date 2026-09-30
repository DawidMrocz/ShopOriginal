#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UserApp/UserApp.csproj", "UserApp/"]
RUN dotnet restore "UserApp/UserApp.csproj"
COPY . .
WORKDIR "/src/UserApp"
RUN dotnet build "UserApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserApp.dll"]