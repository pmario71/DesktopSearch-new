using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;

namespace DesktopSearch.Core.Tests.Services
{

    class NullMockStore : IDocTypePersistence
    {
        IDocType[] _docTypes;

        public IDocType[] DocTypes
        {
            get
            {
                if (_docTypes == null)
                {
                    _docTypes = new IDocType[] { };
                }
                return _docTypes;
            }
            set => _docTypes = value;
        }


        public Task<IEnumerable<IDocType>> LoadAsync()
        {
            return Task.FromResult<IEnumerable<IDocType>>(this.DocTypes);
        }

        public Task StoreOrUpdateAsync(IDocType docType)
        {
            return Task.CompletedTask;
        }
    }
}
