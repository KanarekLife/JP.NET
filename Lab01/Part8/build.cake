//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./OtwarteDane.Cli/bin") + Directory(configuration);
var testDir = Directory("./OtwarteDane.Tests/bin") + Directory(configuration);
var publishDir = Directory("./publish");
var solutionFile = "./OtwarteDane.sln";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(testDir);
    if (DirectoryExists(publishDir))
    {
        CleanDirectory(publishDir);
    }
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetRestore(solutionFile);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetBuild(solutionFile, new DotNetBuildSettings
    {
        Configuration = configuration,
        NoRestore = true
    });
});

Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetBuild(solutionFile, new DotNetBuildSettings
    {
        Configuration = configuration,
        NoRestore = true
    });
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetTest("./OtwarteDane.Tests/OtwarteDane.Tests.csproj", new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
        NoRestore = true,
        Loggers = new[] { "trx", "console;verbosity=normal" },
        ResultsDirectory = "./TestResults"
    });
});

Task("Clean-Publish-Folder")
    .Does(() =>
{
    if (DirectoryExists(publishDir))
    {
        DeleteDirectory(publishDir, new DeleteDirectorySettings 
        {
            Recursive = true,
            Force = true
        });
    }
    CreateDirectory(publishDir);
});

Task("Publish")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Clean-Publish-Folder")
    .Does(() =>
{
    DotNetPublish("./OtwarteDane.Cli/OtwarteDane.Cli.csproj", new DotNetPublishSettings
    {
        Configuration = configuration,
        OutputDirectory = publishDir,
        SelfContained = true,
        Runtime = "linux-x64"
    });
    
    Information("Application published to: " + publishDir);
});

Task("Publish-Portable")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Clean-Publish-Folder")
    .Does(() =>
{
    DotNetPublish("./OtwarteDane.Cli/OtwarteDane.Cli.csproj", new DotNetPublishSettings
    {
        Configuration = configuration,
        OutputDirectory = publishDir + Directory("portable"),
        SelfContained = false
    });
    
    Information("Portable application published to: " + publishDir + Directory("portable"));
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Rebuild")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Clean-Publish-Folder")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);