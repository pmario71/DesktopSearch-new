using DesktopSearch.Core.Contracts;
using DesktopSearch.Core.DataModel.Documents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Services
{

    public interface IIndexingService
    {
        Task IndexRepositoryAsync(IDocumentCollection documentCollection, IProgress<int> progress = null);

        Task IndexRepositoryAsync(IFolder folder, IProgress<int> progress = null);

        Task IndexDocumentAsync(string documentPath);

        Task IndexDocumentAsync(DocDescriptor documentDescriptor);
    }
}
