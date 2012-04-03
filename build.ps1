Get-ChildItem .\Tests -Recurse -Include *Tests.dll | % {
    .\tools\xunit\xunit.console.clr4.exe $_.FullName
}