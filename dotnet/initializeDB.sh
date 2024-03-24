#!/bin/sh

# Instal dotnet Entity Framework
dotnet tool install --global dotnet-ef --version 6.*
# Set path dotnet tools
PATH="$PATH:$HOME/.dotnet/tools/"
# Make Migrations
dotnet ef migrations add Initial
