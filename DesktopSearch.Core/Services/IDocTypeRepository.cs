using DesktopSearch.Core.DataModel.Documents;
using System.IO;

namespace DesktopSearch.Core.Services
{
    internal interface IDocTypeRepository
    {
        /// <summary>
        /// Finds a registered 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="DocTypeNotRegisteredException"/>
        IDocType GetDocTypeByName(string name);

        /// <summary>
        /// Tries to finds a registered <see cref="IDocType"/> by matching the file location against against linked <see cref="IFolder"/>s.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="docType">found <see cref="IDocType"/></param>
        /// <returns>true, if found</returns>
        bool TryGetDocTypeForPath(FileInfo file, out IDocType docType);

        /// <summary>
        /// Adds a new <see cref="DocType"/> to the repository.
        /// </summary>
        /// <remarks>There must be at least one <see cref="IFolder"/> linked</remarks>
        /// <param name="docType">unique name for the new <see cref="IDocType"/> to be registered.</param>
        void AddDocType(IDocType docType);
    }
}