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

Set up the `.env` files that is located in the root directory.

### Step 3

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
