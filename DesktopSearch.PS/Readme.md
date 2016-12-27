# Readme

1) [Running all necessary services](../Documents/Operations.md)
2) [Requirements Specification](../Documents/RequirementsSpec.md)
3) [Design Specification](../DesktopSearch.Core/Designspec.md)

## Help Generation

This should normally do the trick in that it combines Update and adding new Cmdlets.\
`Update-MarkdownHelpModule -Path .\docs`

### Updating Help
Update help files (*.md) after code changes.
```
Import-Module platyPS
import-module .\bin\Debug\DesktopSearch.PS.dll
Update-MarkdownHelp .\Docs
```

### Create Documentation for new Cmdlet
```
Import-Module platyPS
import-module .\bin\Debug\DesktopSearch.PS.dll
New-MarkdownHelp -Command <cmdlet name> -OutputFolder .\Docs
```

### Render Markdown to Helpfile

```
New-ExternalHelp .\docs -OutputPath en-US\ -Force
```

### References

**Docs**: https://github.com/PowerShell/platyPS

## UI Activation

* see DialogFactory for details
* first control is not automatically in focus