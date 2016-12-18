# Design DesktopSearch.Core

## Domain Entities

* Processors
  * coordinate iterating over folders or multiple files
* Extractors
  * are responsible for extracting index information from a single file

![Extractors](../Documents/images/Extractors.png)

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
  * https://hub.docker.com/r/logicalspark/docker-tikaserver/
* default port 9998 is used
* TikaServerExtractor implements the interfacing with Tika and DocDescriptor mapping


