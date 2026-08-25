# BaroCsSchema

Barotrauma C# Mod Template — template project for Barotrauma mods.

## Structure

```python
├── Assets/                     # Mod content package
│   ├── Assets.csproj
│   ├── filelist.xml            # Project entry point and definition of assets and files
│   ├── metadata.xml            # Author information or anything else (not essential)
│   ├── ModConfig.xml           # Paths to binaries
│   ├── RunConfig.xml           # Mod requirements
│   ├── Config/                 # Shared settings
│   ├── Content/                # Content files (textures, etc.)
│   └── Localization/           # Translation files
│
├── ClientProject/              # Client code
│   ├── ClientSource/Plugin.cs  # Source files (*.cs)
│   ├── WindowsClient.csproj
│   ├── LinuxClient.csproj
│   └── OSXClient.csproj
│
├── ServerProject/              # Server code
│   ├── ServerSource/Plugin.cs  # Source files (*.cs)
│   ├── WindowsServer.csproj
│   ├── LinuxServer.csproj
│   └── OSXServer.csproj
│
├── SharedProject/              # Shared client/server code
│   ├── SharedSource/Plugin.cs
│   ├── SharedProject.projitems
│   └── SharedSource.shproj
│
├── Refs/                       # Game dependencies
├── Build.props                 # Global project properties
├── Directory.Build.props       # Shared MSBuild properties
├── UserBuildData.props         # Local developer configuration
├── CheckLocalization.targets   # Localization verification task
└── BaroCsSchema.sln            # Visual Studio solution
```

## How to use

1. Clone this project and rename it with your mod’s name, or paste its content into your git project.  
2. Edit `Build.props` — change `ProjectName` and `AssemblyName`.  
3. Edit `filelist.xml` — change `name`.  
4. Edit `metadata.xml` — change `author` and `id`.  
5. Copy and rename `UserBuildData.props.example` to `UserBuildData.props` and adjust the paths.  
6. Run `get-refs.ps1` and wait for it to finish.  
7. Run freely, either via `build.ps1` or any other method.  
