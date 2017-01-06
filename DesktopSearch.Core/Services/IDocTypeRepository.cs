using DesktopSearch.Core.DataModel.Documents;
using System.Collections.Generic;
using System.IO;

namespace DesktopSearch.Core.Services
{
    public interface IDocTypeRepository
    {
        /// <summary>
        /// Finds a registered <see cref="IDocType"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true, if a matching <see cref="IDocType"/> was found!</returns>
        bool TryGetDocTypeByName(string name, out IDocType docType);

        /// <summary>
        /// Tries to finds a registered <see cref="IDocType"/> by matching the file location against against linked <see cref="IFolder"/>s.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="docType">found <see cref="IDocType"/></param>
        /// <returns>true, if found</returns>
        bool TryGetDocTypeForPath(FileInfo file, out IDocType docType);

        /// <summary>
        /// Tries to find if a local <see cref="IFolder"/> is defined for the provided <see cref="IDocType"/> instance.
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="localFolder">reference to local folder</param>
        /// <returns>true, if a local folder was found</returns>
        bool TryGetConfiguredLocalFolder(IDocType docType, out IFolder localFolder);

        /// <summary>
        /// Returns all configured IndexedCollections.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDocType> GetIndexedCollections();

        /// <summary>
        /// Adds a new <see cref="DocType"/> to the repository.
        /// </summary>
        /// <remarks>There must be at least one <see cref="IFolder"/> linked</remarks>
        /// <param name="docType">unique name for the new <see cref="IDocType"/> to be registered.</param>
        void AddDocType(IDocType docType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="path"></param>
        void AddFolderToDocType(IDocType docType, string path);
    }
}