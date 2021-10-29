#addin "nuget:?package=Cake.FileHelpers&version=3.2.1"
#addin "nuget:?package=Cake.Powershell&version=0.4.8"
#tool "nuget:?package=GitVersion.CommandLine&version=5.0.1"

using System;
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var baseDir = MakeAbsolute(Directory("../")).ToString();
var buildDir = baseDir + "/build";
var toolsDir = buildDir + "/tools";

var Solution = baseDir + "/ColorCode.sln";
var nupkgDir = buildDir + "/nupkg";

var gitVersioningVersion = "2.1.23";
var versionClient = toolsDir + "/nerdbank.gitversioning/tools/Get-Version.ps1";
string Version = null;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    Information("\nCleaning Package Directory");
    CleanDirectory(nupkgDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(Solution);
});

Task("Build")
    .Does(() =>
{
    Information("\nBuilding Solution");
    var buildSettings = new MSBuildSettings
    {
        MaxCpuCount = 0,
		MSBuildPlatform = MSBuildPlatform.x86
    }
    .SetConfiguration("Release")
    .WithTarget("Restore;Build")
    .WithProperty("IncludeSymbols", "true")
    .WithProperty("GenerateLibraryLayout", "true")
    .WithProperty("PackageOutputPath", nupkgDir)
    .WithProperty("PackageVersion", System.Environment.GetEnvironmentVariable("GITVERSION_FullSemVer"))
    .WithProperty("GeneratePackageOnBuild", "true");

    EnsureDirectoryExists(nupkgDir);
    MSBuild(Solution, buildSettings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
