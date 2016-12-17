# Requirements Specification

## Document Processing Pipeline

![Flow of operations](./Images/Flow_of_operations.png)

1. Take file from Scan folder
   * take latest file from predefined folder
   * support explicit file selection

1. run OCR software
   * use temp folder to store intermediate results
   * create pdf/A

1. correct existing / add additional tags to pdf document
   * use isharp.dll / 

1. derive storage location proposal
   * Unterlagen vs. Rechnungen, Reports  => storage folder
   * Herausgeber (SDK, comdirect, Siemens, ...)
   * Zusatztext (Jahresbescheinigung)
   * Jahr

1. index file
   * use tags created

1. move file to storage location

## Index Attributes for Document

* Title
* Author
* LastModified
* Keywords
* Attachment
* Path