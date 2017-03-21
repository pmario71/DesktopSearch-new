using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Miscellaneous;
using Microsoft.Extensions.Logging;

namespace DesktopSearch.Core.Utils
{
    public static class Logging
    {
        private static readonly LoggerFactory _loggerFactory = new LoggerFactory();

        public static LoggerFactory Factory => _loggerFactory;

        public static ILogger<T> GetLogger<T>()
        {
            
            return _loggerFactory.CreateLogger<T>();
        }
    }
}
