---
external help file: DesktopSearch.PS.dll-Help.xml
online version: 
schema: 2.0.0
---

# Get-TikaExtractRaw

## SYNOPSIS
Runs Tika extraction for a given file and returns the raw results (json format) as output.

## SYNTAX

```
Get-TikaExtractRaw [-File] <String[]> [<CommonParameters>]
```

## DESCRIPTION
Streams the content of the file to a running Tika server. The extracted meta data and file content is returned as string which contains native json output.

## EXAMPLES

### Example 1
```
PS C:\> Get-TikaExtractRaw -File 'd:\some path\filename.pdf'
```

Runs extraction over the given pdf file.

## PARAMETERS

### -File
Files to extract index for.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: True
Position: Benannt
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Keine

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

