# Use the official Microsoft ASP.NET Core SDK image
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

# Set the working directory in the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY ./CrashLogger/*.csproj ./
RUN dotnet restore

# Copy the remaining source code
COPY ./CrashLogger .

# Build the application
RUN dotnet publish -c Release -o out

COPY initializeDB.sh ./
RUN chmod +x initializeDB.sh

ENTRYPOINT ["./initializeDB.sh"]


# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS runtime

WORKDIR /app

COPY --from=build /app/out ./
COPY --from=build /app/Migrations ./

COPY ./CrashLogger/*.env ./

COPY general.sh ./
RUN chmod +x general.sh

ENTRYPOINT ["./general.sh"]