#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Silo/Orleans.ShoppingCart.Silo.csproj", "Silo/"]
COPY ["Abstractions/Orleans.ShoppingCart.Abstractions.csproj", "Abstractions/"]
COPY ["Grains/Orleans.ShoppingCart.Grains.csproj", "Grains/"]
RUN dotnet restore "Silo/Orleans.ShoppingCart.Silo.csproj"
COPY . .
WORKDIR "/src/Silo"
RUN dotnet build "Orleans.ShoppingCart.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Orleans.ShoppingCart.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orleans.ShoppingCart.Silo.dll"]