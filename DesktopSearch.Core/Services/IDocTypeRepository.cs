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
        DocType GetDocTypeByName(string name);

        /// <summary>
        /// Tries to finds a registered <see cref="DocType"/> by matching the file location against against linked <see cref="Folder"/>s.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="DocTypeNotRegisteredException"/>
        DocType GetDocTypeForPath(FileInfo file);

        /// <summary>
        /// Adds a new <see cref="DocType"/> to the repository.
        /// </summary>
        /// <remarks>There must be at least one <see cref="Folder"/> linked</remarks>
        /// <param name="docType">unique name for the new <see cref="DocType"/> to be registered.</param>
        void AddDocType(DocType docType);
    }
}