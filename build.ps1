$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$a = "HiveSuite\HiveSuite.sln /target:Clean /target:Build"
$nugetrestore = "nuget restore .\HiveSuite\HiveSuite.sln"
Invoke-Expression "$nugetrestore"
Invoke-Expression "$msbuild $a"
Write-Host "If you are having issues building the project from this scrpit, please run prereqs.ps1." -ForegroundColor Red
Write-Host "If this project can be built on Linux than there will be a build.sh script." -ForegroundColor Green