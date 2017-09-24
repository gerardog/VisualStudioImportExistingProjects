# Create a Visual Studio Solution from Existing Projects

This console application traverse a folder tree finding every existing Visual Studio Project and adding them to a new solution. It creates 'solution folders' so the projects are grouped in the same hierarchy as in the file system.

Usage:
```
  -f, --folder     Required. Folder to traverse and where the solution will be
                   generated.

  -o, --output     (Default: AllProjects.sln) FileName of the solution to
                   create.

  -e, --exclude    (Default: .git,bin,obj,packages,node_modules) Comma
                   separated list of folders to exclude.

  -v, --verbose    Verbose
```

Example:
```
   SolutionGenerator.exe --folder C:\git\SomeSolutionRoot --output MySolutionFile.sln
```

## Supported Project Types:
Right now it only supports *.csproj file types. It can be extended to other project types by correctly handling the each Project Type GUIDs.