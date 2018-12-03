# Create a Visual Studio Solution from Existing Projects

This console application traverse a folder tree finding every existing Visual Studio Project and adding them to a new solution. It groups the projects inside 'visual studio solution folders' in the same tree hierarchy as they are in the file system.

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

## Deterministic
This console does not use any kind of random numbers (i.e. it uses sequentially generated Guid's), so the outcome can be easily stored in source-control and the differences between different executions are minimized and should only be related to which projects were added or removed.

## Supported Project Types:
Right now it only supports *.csproj file types. It can be extended to other project types by correctly handling the each Project Type GUIDs.
