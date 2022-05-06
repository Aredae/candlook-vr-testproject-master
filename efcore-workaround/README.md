# Workaround for EFCore Tools and Unity

Read this to learn how to use the migration tools from EntityFrameworkCore.

## Background

Unity does not really integrate well with NuGet package management and the ways
of modern VS project layout. To get these automated tools working, we have to
rely on a fairly ugly hack (trust me, I've tried everything...): This directory
contains a new Visual Studio solution separate from the one that Unity is
managing, thus allowing us to make use of the modern NuGet features etc. and
setting up the project in a way that works with the EFCore tools. However,
since we really want to be working on the same DB.cs file containing the same
models as the rest of the project, we need to create a hardlink to the `DB.cs`
file in `Assets/Util/DB.cs`.

A hardlink means that the same file (same contents etc) appears at two
different places in the filesystem. Since the underlying file is the same,
edits affect both "versions" of the file. The fact that a file appears in
multiple places of the filesystem is mostly tansparent to programs, so it is
very important to keep DB.cs in `.gitignore` so that the files do not become
accidentally separated again on `git pull`.

As a final touch to the workaround, the `Stubs.cs` file in this directory
declares signatures from the Unity project that are used in DB.cs (no
implementation needed) so that this workaround-solution can compile
successfully. This is required by the EFCore tools to that they can inspect the
models in `DB.cs` to generate migrations.

## What to do

1. (You only have to do this step once, after initially cloning the repository
   from git): Create a hardlink from `./DB.cs` to `../Assets/Util/DB.cs` by
   opening a terminal (on Windows, press Win+R, type cmd, press enter) and
   running the following command:
   - Windows: `mklink /H DB.cs ..\Assets\Util\DB.cs`
   - Linux:   `ln DB.cs ../Assets/Util/DB.cs`
   - Max:     Probably same as Linux?
2. Now you can open the efcore-workaround solution in Visual Studio
3. Use the tools: See https://docs.microsoft.com/en-us/ef/core/cli/powershell
   for complete documentation.
   - To create a new migration, run
       Add-Migration <Name>
   - To revert the last migration, run
       Remove-Migration [ -Force ]
     Use -Force to revert a migration that was already applied to the DB
   - To apply all pending migrations to the DB, run
       Update-Database
4. There can be issues with EFCore.Tools package. If you cant run Add-Migrations or similar commands, try running this command in the package       manager console: C:\Users\YourUserAccount\.nuget\packages\Microsoft.EntityFrameworkCore.Tools\1.1.0-preview4-final\tools\init.ps1. Also remember to restart Visual Studio before you try to run EFCore.Tools commands again.
5. If EFCore.Design does not work, try uninstalling both EFCoreTools and EFCore.Design and reinstalling them both. Make sure the version is 3.1.18.
