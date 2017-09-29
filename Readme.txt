PUBLISH NUGET PACKAGE
1. Update version and releaseNotes in "Nuget spec/Enferno.Public.Web.SalesTool.nuspec":

    <version>1.2.0.2</version>
    <releaseNotes>
      A description of each change that has been made.
    </releaseNotes>

2. Update "SalesTool/Properties/AssemblyInfo.cs:

	[assembly: AssemblyVersion("1.2.0.2")]
	[assembly: AssemblyFileVersion("1.2.0.2")]

3. Build project in Debug configuration.

4. Open PowerShell.

5. In PowerShell, use "cd"-command to change directory to "NuGet" folder in this project:

	d:
	cd \Dev\CodePlex\enfernopublic\Enferno.Public.Web.SalesTool\Release\1.2.0\NuGet

6. If this is the first time you build a package for NuGet, then you need to provide credentials to push a package. Use this PowerShell command:

	./nuget.exe setApiKey 089212ee-91af-4334-8fa6-8dd05ca07917

7. Start NuGet Build, replace path with path to your project folder:

	./New-NuGetPackage.ps1 -NuSpecFilePath "D:\Dev\CodePlex\enfernopublic\Enferno.Public.Web.SalesTool\Release\1.2.0\Enferno.Public.Web.SalesTool.nuspec" -DoNotUpdateNuSpecFile -PackOptions "-Build -Symbols -Properties Configuration=Debug"
	
	[ NuGet Package Version Number ] Dialog : [ OK ]
	[ Enter Release Notes For New Package ] Dialog : [ OK ]
	[ Do you want to push this package ] Dialog : [ Yes ]

8. Check in solution and use version and change descripion as comment.

9. Check in and merge changes in Release to Main. If Major release Bransh a new release folder.

10. Register release on Codeplex/GitHub