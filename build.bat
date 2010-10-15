SET FRAMEWORKDIR=c:\Windows\Microsoft.NET\Framework64\v4.0.30319

%FRAMEWORKDIR%\MSBuild.exe Membase.Management.sln /T:Rebuild /p:Configuration=Release
mkdir build
tools\ILMerge.exe /t:library /internalize /targetplatform:v4,%FRAMEWORKDIR% /out:build/Membase.Management.dll Membase.Management/bin/Release/Membase.Management.dll lib/Newtonsoft.Json.dll lib/log4net.dll
xcopy "Membase.Management\bin\Release\Membase.Management.xml" build /Y