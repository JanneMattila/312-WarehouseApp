# Database deployment

For local deployment use `SQLPackage` tool:

```batch
SqlPackage /Action:Publish /SourceFile:WarehouseDatabase.dacpac /TargetDatabaseName:WarehouseDatabase /TargetServerName:"localhost"
```
