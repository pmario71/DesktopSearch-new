---
external help file: DesktopSearch.PS.dll-Help.xml
online version: 
schema: 2.0.0
---

# Get-TikaExtract

## SYNOPSIS
Runs Tika extraction for a given file and returns a `DocDescriptor` as output.

## SYNTAX

```
Get-TikaExtract [-File] <String[]> [<CommonParameters>]
```

## DESCRIPTION
Streams the content of the file to a running Tika server. The extracted meta data and file content is mapped to
`DocDescriptor` DTO, which can be passed to ElasticSearch for indexing.

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

