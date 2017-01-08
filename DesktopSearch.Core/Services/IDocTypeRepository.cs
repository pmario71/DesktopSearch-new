using DesktopSearch.Core.DataModel.Documents;
using System.Collections.Generic;
using System.IO;

namespace DesktopSearch.Core.Services
{
    public interface IDocumentCollectionRepository
    {
        /// <summary>
        /// Finds a registered <see cref="IDocumentCollection"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true, if a matching <see cref="IDocumentCollection"/> was found!</returns>
        bool TryGetDocumentCollectionByName(string name, out IDocumentCollection documentCollection);

        /// <summary>
        /// Tries to finds a registered <see cref="IDocumentCollection"/> by matching the file location against against linked <see cref="IFolder"/>s.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="documentCollection">found <see cref="IDocumentCollection"/></param>
        /// <returns>true, if found</returns>
        bool TryGetDocumentCollectionForPath(FileInfo file, out IDocumentCollection documentCollection);

        /// <summary>
        /// Tries to find if a local <see cref="IFolder"/> is defined for the provided <see cref="IDocumentCollection"/> instance.
        /// </summary>
        /// <param name="documentCollection"></param>
        /// <param name="localFolder">reference to local folder</param>
        /// <returns>true, if a local folder was found</returns>
        bool TryGetConfiguredLocalFolder(IDocumentCollection documentCollection, out IFolder localFolder);

        /// <summary>
        /// Returns all configured IndexedCollections.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDocumentCollection> GetIndexedCollections();

        /// <summary>
        /// Adds a new <see cref="DocumentCollection"/> to the repository.
        /// </summary>
        /// <remarks>There must be at least one <see cref="IFolder"/> linked</remarks>
        /// <param name="documentCollection">unique name for the new <see cref="IDocumentCollection"/> to be registered.</param>
        void AddDocumentCollection(IDocumentCollection documentCollection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentCollection"></param>
        /// <param name="path"></param>
        IFolder AddFolderToDocumentCollection(IDocumentCollection documentCollection, string path);
    }
}