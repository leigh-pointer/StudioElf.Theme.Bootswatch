del "*.nupkg"
"..\..\oqtane.framework\oqtane.package\nuget.exe" pack StudioElf.Theme.Bootswatch.nuspec 
XCOPY "*.nupkg" "..\..\oqtane.framework\Oqtane.Server\Packages\" /Y
