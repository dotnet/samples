#!/bin/bash
echo "Building and running ShoppingCart sample..."
echo "Open https://localhost:52419 in your browser"
dotnet run --project "$(dirname "$0")/Silo/Orleans.ShoppingCart.Silo.csproj"
