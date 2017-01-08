﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.Processors;
using DesktopSearch.Core.DataModel.Documents;

namespace DesktopSearch.Core
{
    public interface IFolderProcessor
    {
        Task ProcessAsync(string file, string documentCollectionName);

        Task ProcessAsync(IFolder folder);

        Task ProcessAsync(IFolder folder, IProgress<int> progress);
    }

    public interface IContainer
    {
        object GetService(Type type);

        TInstance GetService<TInstance>();

        IFace GetService<IFace, TInstance>();
    }
}
