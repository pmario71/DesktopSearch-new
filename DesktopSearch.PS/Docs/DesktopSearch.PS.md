---
Module Name: DesktopSearch.PS
Module Guid: 00000000-0000-0000-0000-000000000000
Download Help Link: {{Please enter FwLink manually}}
Help Version: {{Please enter version of help manually (X.X.X.X) format}}
Locale: en-US
---

# DesktopSearch.PS Module
## Description

### Example: Decoupling Extraction and Indexing

```powershell
$files = [System.IO.Directory]::GetFiles('Z:\Buecher\Programming\Database', '*.pdf')
$results = Get-TikaExtract -File $files

# save extracted information to disk
$results | Export-Clixml -Path C:\todo\Books.xml

Index-DSDocument -Doc $results
```

## DesktopSearch.PS Cmdlets
### [Add-DSFolderToIndex](Add-DSFolderToIndex.md)
{{Manually Enter Add-DSFolderToIndex Description Here}}

### [Get-DSSetting](Get-DSSetting.md)
{{Manually Enter Get-DSSetting Description Here}}

### [Get-DSSettingJSON](Get-DSSettingJSON.md)
{{Manually Enter Get-DSSettingJSON Description Here}}

### [Get-TikaExtract](Get-TikaExtract.md)
{{Manually Enter Get-TikaExtract Description Here}}

### [Get-TikaExtractRaw](Get-TikaExtractRaw.md)
{{Manually Enter Get-TikaExtractRaw Description Here}}

### [Initialize-DSIndex](Initialize-DSIndex.md)
{{Manually Enter Initialize-DSIndex Description Here}}

### [Save-DSSetting](Save-DSSetting.md)
{{Manually Enter Save-DSSetting Description Here}}

### [Sync-DSIndex](Sync-DSIndex.md)
{{Manually Enter Sync-DSIndex Description Here}}

