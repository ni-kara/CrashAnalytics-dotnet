# Crash Analytics

The CrashAnalytics is a dotnet REST Api project

## Database

Database: Postgres

### Schema

```SQL
Table project
    Id          |   Guid
    Name        |   string
    CreatedAt   |   DateTime
```

```SQL
Table crash
    Id          |   Guid
    Name        |   string
    CreatedAt   |   DateTime
    ProjectId   |   Guid        [Fk]
    Message     |   string
    Version     |   string      [max(15)]
    Type        |   Enum        [Enum{Android | iOS}]
```

### Migration

Create Migration

```
dotnet ef migrations add {migration-name}
```

Update database with migration scheme

```
dotnet ef database update
```

Database drop

```
dotnet ef database update
```

### Know Issues

Cannot add via migration in the IDs of the table the `uuid_generate_v4()`, it should be add it manualy.
