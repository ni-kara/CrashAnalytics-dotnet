# Crash Logger

The Crash Logger is a dotnet REST API project with postgres.

## Dev Stack

- .NET 6
  - Entity Framework
- postgres

## Build

### Step 1

Clone the project

```
https://github.com/ni-kara/CrashLogger-dotnet.git
```

### Step 2

1. Set up the `.env` file that is located in the `root` directory. 
2. Set up the `.env` file that is located in the `CrashLogger` directory. `root > dotnet > CrashLogger`
### Step 3
```
cd dotnet/CrashLogger
```
Create migration
```
dotnet ef migrations add {migration-name}
```
#### In case of `dotnet ef` does not exist
```
export PATH="$PATH:/root/.dotnet/tools" # ONLY FOR LINUX
```
---
Install `dotnet ef`
```
dotnet tool install --global dotnet-ef --version 6.*
```

### Step 4

Run the command

```
docker-compose up --build
```

## Database

In the project's initialisation the tables are created in the databases

### Scheme

```SQL
Table projects
    id           |   Guid
    name         |   string
    created_at   |   DateTime
```

```SQL
Table crashes
    id          |   Guid
    name        |   string
    createdAt   |   DateTime
    message     |   string
    version     |   string      [max(15)]
    type        |   Enum        [Enum{Android | iOS}]
    projectId   |   Guid        [Fk]
```
