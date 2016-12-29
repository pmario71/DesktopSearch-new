# Design DesktopSearch.Core

## Domain Entities

* Processors
  * coordinate iterating over folders or multiple files
* Extractors
  * are responsible for extracting index information from a single file

![Extractors](../Documents/images/Extractors.png)

### DocTypes & Folders

`DocType` refers to a logical group of files that are stored under a single root directory (Buecher, Unterlagen, Rechnungen).\
The assignment of a file to a given `DocType` is inferred from its location when it is indexed.

* `DocType` --> `Folder`
* "file path" starts with `Folder`'s path  =>  file belongs to according `DocType`
* a `Folder`can have multiple storage locations on different machines
* `DocDescriptor.Path` holds a uri with the following encoding: `DesktopSearch://Buecher/someDirectory/filename.pdf`
  * `Uri.Scheme` : `DesktopSearch` to potentially be able to define a url handler for it.
  * `Uri.Host`   : `Buecher` specifies the `DocType`
  * `LocalPath`  : specifies the relative url underneath the root folder
  * full path can only be resolved when `DocType` & `machinename` is known

  ![DocDescriptor - DocType - Folder Relationship](../Documents/images/DocType.png)

### SearchService

![SearchService Dependencies](../Documents/images/SearchServiceStatic.png)

* SearchService combines search as well as indexing functionality (should be changed!)

### FolderProcessor

Responsible for walking a folder (recursively) and indexing all its files using the configured strategy (code, documents)

### CodeFolderProcessor

Takes the contents of e.g. a files and extracts relevant index information using Roslyn

### DocumentFolderProcessor

Takes the contents of e.g. a files and sends it to ElasticSearch directly so that it is processed by Tika

## Configuration

Configuration is read from json file located in installation folder. To get acces use:

```cs

[Import]
internal ConfigAccess ConfigAccess { set; get; }
...
Settings settings = ConfigAccess.Get();

```

### Adding Configuration

## Usage of Tika

* service is run as docker container using the official image
  * <https://hub.docker.com/r/logicalspark/docker-tikaserver/>
* default port 9998 is used
* TikaServerExtractor implements the interfacing with Tika and DocDescriptor mapping

## Performance

Measuring over standalone Tika container and version running together with ElasticSearch shows that memory is critical

|   | Doc1  | Doc2| Doc1  | Doc2  |
|--:|------:|----:|------:|------:|
| 0 | 1.121 | 991 | 5.365 | 1.914 |
| 1 | 632   | 939 | 813   | 1.056 |
| 2 | 612   | 724 | 688   | 780   |
| 3 | 706   | 717 | 649   | 713   |
| 4 | 692   | 680 | 663   | 705   |
| 5 | 626   | 702 | 620   | 670   |
| 6 | 584   | 663 | 551   | 699   |
| 7 | 624   | 572 | 627   | 627   |
| 8 | 597   | 600 | 655   | 668   |
| 9 | 670   | 783 | 662   | 809   |
