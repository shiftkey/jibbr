@echo Off
set config=%1
if "%config%" == "" (
   set config=debug
)

:: compile the code
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Jabbot.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

:: remove all obj folder contents
for /D %%f in (".\Tests\*") do @(
del /S /Q "%%f\obj\*"
)

powershell -File build.ps1