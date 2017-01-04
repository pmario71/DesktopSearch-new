using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.Processors;
using DesktopSearch.Core.DataModel.Documents;

namespace DesktopSearch.Core
{
    public class FolderProcessorFactory
    {
        private readonly IContainer _container;

        private Dictionary<string, Type> _map = new Dictionary<string, Type>()
        {
            { "Code"      , typeof(CodeFolderProcessor) },
            { "Documents" , typeof(DocumentFolderProcessor) },
        };

        public FolderProcessorFactory(IContainer container)
        {
            _container = container;
        }

        public IFolderProcessor GetProcessorByFolder(IFolder folder)
        {
            if (folder == null)
                throw new ArgumentNullException("folder");

            Type folderProcessorType;
            if (!_map.TryGetValue(folder.IndexingType, out folderProcessorType))
            {
                throw new ArgumentOutOfRangeException("indexingTypeName", $"'{folder.IndexingType}' is unknown!");
            }

            return (IFolderProcessor)_container.GetService(folderProcessorType);
        }

    }

    public interface IFolderProcessor
    {
        Task Process(string file, string indexingTypeName);

        Task Process(IFolder folder);

        Task Process(IFolder folder, IProgress<int> progress);
    }

    public interface IContainer
    {
        object GetService(Type type);

        TInstance GetService<TInstance>();

        IFace GetService<IFace, TInstance>();
    }
}
