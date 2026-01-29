@echo off
echo Building and running ShoppingCart sample...
echo Open https://localhost:52419 in your browser
dotnet run --project "%~dp0Silo\Orleans.ShoppingCart.Silo.csproj"
