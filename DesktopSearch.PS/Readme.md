# Readme


## Help Generation

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
New-ExternalHelp .\docs -OutputPath en-US\
```

### References

**Docs**: https://github.com/PowerShell/platyPS