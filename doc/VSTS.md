# VSTS Instructions

## Build

Create build definition which connects to newly created Git repository: 

```yaml
queue:
  name: Hosted VS2017
  demands: 
  - msbuild
  - visualstudio

steps:
- task: NuGetCommand@2

- task: VSBuild@1
  inputs:
    vsVersion: 15.0
    msbuildArgs: '/p:DeployOnBuild=true /p:PublishProfile=Publish.pubxml'
    configuration: Release
    clean: true

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: deploy
    ArtifactName: deploy
    ArtifactType: Container

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: 'src/WarehouseApp/Publish'
    ArtifactName: app
    ArtifactType: Container

- task: CopyFiles@2
  inputs:
    SourceFolder: 'src/WarehouseDatabase/bin/Release'
    Contents: '*.dacpac'
    TargetFolder: '$(build.artifactstagingdirectory)/dacpac'

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: '$(build.artifactstagingdirectory)/dacpac'
    ArtifactName: dacpac
    ArtifactType: Container
```

## Release

Create release definition which connects to build and uses build artifacts 
for deployment:

```yaml
queue:
  name: Hosted VS2017
  demands: 
  - msbuild
  - visualstudio

steps:
- task: AzurePowerShell@2
  inputs:
    ScriptPath: '$(System.DefaultWorkingDirectory)/WarehouseApp/deploy/deploy.ps1'
    ScriptArguments: '-ResourceGroupName "warehouseapp-dev-rg" -Location "westeurope" -DatabaseAdminUser $(SqlServerUser) -DatabaseAdminPassword (ConvertTo-SecureString -String "$(SqlServerPassword)" -Force -AsPlainText)'
    configuration: Release

- task: SqlAzureDacpacDeployment@1
  inputs:
    ServerName: '$(Custom.SQLServerName).database.windows.net'
    DatabaseName: '$(Custom.DatabaseName)'
    SqlUsername: '$(SqlServerUser)'
    SqlPassword: '$(SqlServerPassword)'
    DacpacFile: '$(System.DefaultWorkingDirectory)/WarehouseApp/dacpac/WarehouseDatabase.dacpac'

- task: AzureRMWebAppDeployment@3
  inputs:
    WebAppName: '$(Custom.WebAppName)'
    Package: '$(System.DefaultWorkingDirectory)/WarehouseApp/app/WarehouseApp.zip'
```

*Note:* You need to have two variables per environment `SqlServerUser` and `SqlServerPassword` (encrypted).
