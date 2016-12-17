using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.DataModel.Documents;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.ElasticSearch
{
    public class ManagementService
    {
        private readonly IElasticClient _client;
        private readonly string _codeSearchIndexName;
        private readonly string _documentSearchIndexName;

        //private readonly ILogger _logger;

        public ManagementService(IElasticClient client/*, ILogger logger*/)
            : this(client, false)
        {
            //_logger = logger;
        }

        internal ManagementService(IElasticClient client, bool useTestIndices)
        {
            _client = client;

            _documentSearchIndexName = Configuration.DocumentSearch.IndexName;
            _codeSearchIndexName = Configuration.CodeSearch.IndexName;

            if (useTestIndices)
            {
                _documentSearchIndexName += "_test";
                _codeSearchIndexName += "_test";
            }
        }

        public async Task EnsureIndicesCreated()
        {
            // --------------------------------------------------------------------
            // setup index
            // --------------------------------------------------------------------
            Task codeIndexTask = Task.CompletedTask;
            Task docIndexTask = Task.CompletedTask;

            if (!_client.IndexExists(_documentSearchIndexName).Exists)
            {
                //_logger.LogInformation("Creating Index '{0}'", Configuration.DocumentSearch.IndexName);

                var docIndex = new CreateIndexDescriptor(_documentSearchIndexName);

                docIndex.Mappings(mp => mp.Map<DocDescriptor>(m => m
                    //.SourceField(s => s.Excludes(new[] { "content" }))
                    //.Properties(ps => ps
                    //        .String(s => s
                    //            .Name(f => f.Path)
                    //            .Index(FieldIndexOption.Analyzed)
                    //            .Store(true))
                    //  .Attachment(atm => atm
                    //        .Name(f => f.Content)
                    //        .FileField(f => f
                    //            .Name(p => p.Content)
                    //            .Index(FieldIndexOption.Analyzed)
                    //            .Store(false)
                    //            .TermVector(TermVectorOption.WithPositionsOffsets))
                    //        .DateField(df => df
                    //            .Name(p => p.LastModified)
                    //            .Index(NonStringIndexOption.NotAnalyzed)
                    //            .Store(true))
                    //        .KeywordsField(d => d
                    //            .Name(n => n.Keywords)
                    //            .Store())
                    //  )
//-------------------------------------------------------------------
                        .Properties(p => p
                                .Attachment(a => a
                                    .Name(n => n.Attachment)
                                    .AuthorField(d => d
                                        .Name(n => n.Attachment.Author)
                                        .Store()
                                    )
                                    .FileField(d => d
                                        .Name(n => n.Attachment.Content)
                                        .Store(false)
                                    )
                                    .ContentLengthField((Func<NumberPropertyDescriptor<DocDescriptor>, INumberProperty>)(d => d
                                        .Name(n => n.Attachment.ContentLength)
                                        .Store())
                                    )
                                    .ContentTypeField(d => d
                                        .Name(n => n.Attachment.ContentType)
                                        .Store()
                                    )
                                    .DateField(d => d
                                        .Name(n => n.Attachment.Date)
                                        .Store()
                                    )
                                    .KeywordsField(d => d
                                        .Name(n => n.Attachment.Keywords)
                                        .Store()
                                    )
                                    .LanguageField((Func<TextPropertyDescriptor<DocDescriptor>, ITextProperty>)(d => d
                                        .Name(n => n.Attachment.Language)
                                        .Store())
                                    )
                                    .NameField(d => d
                                        .Name(n => n.Attachment.Name)
                                        .Store()
                                    )
                                    .TitleField(d => d
                                        .Name(n => n.Attachment.Title)
                                        .Store()
                                    )
                                )
                            )
                ));

                var task = _client.CreateIndexAsync(_documentSearchIndexName, i => docIndex);
                docIndexTask = task;

                var result = await task;
                if (!result.IsValid)
                {
                    Console.WriteLine(result.DebugInformation);
                }
            }

            if (!_client.IndexExists(_codeSearchIndexName).Exists)
            {
                //_logger.LogInformation("Creating Index '{0}'", Configuration.CodeSearch.IndexName);

                var indexDescriptor = new CreateIndexDescriptor(_codeSearchIndexName)
                    .Mappings(ms => ms
                        .Map<TypeDescriptor>(m => m.AutoMap())
                        .Map<MethodDescriptor>(m => m.AutoMap())
                        .Map<FieldDescriptor>(m => m.AutoMap()));

                var task = _client.CreateIndexAsync(_codeSearchIndexName, i => indexDescriptor);
                codeIndexTask = task;

                var result = await task;
                if (!result.IsValid)
                {
                    Console.WriteLine(result.DebugInformation);
                }
            }

            await Task.WhenAll(docIndexTask, codeIndexTask);
        }
    }
}
