using DesktopSearch.Core.DataModel;
using DesktopSearch.Core.ElasticSearch;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DesktopSearch.Core.DataModel.Documents;

namespace DesktopSearch.Core.DataModel.Code
{
    public class TypeDescriptor : IAPIElement
    {
        private IList<IDescriptor> _members;

        /// <summary>
        /// Name shall not be null, @namespace can return null.
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="name"></param>
        /// <param name="visibility"></param>
        /// <param name="namespace"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNr"></param>
        public TypeDescriptor(ElementType elementType, string name, Visibility visibility,
            string @namespace, string filePath, int lineNr) 
            : this(elementType, name, visibility, @namespace, filePath, lineNr, null)
        {
        }

        /// <summary>
        /// Name shall not be null, @namespace can return null.
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="name"></param>
        /// <param name="visibility"></param>
        /// <param name="namespace"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNR"></param>
        /// <param name="comment"></param>
        public TypeDescriptor(ElementType elementType, string name, Visibility visibility,
            string @namespace, string filePath, int lineNR, string comment)
        {
            if (string.IsNullOrEmpty(name))       throw new ArgumentException(nameof(name));
            //if (string.IsNullOrEmpty(@namespace)) throw new ArgumentNullException(nameof(@namespace));
            //if (comment == null)                  throw new ArgumentNullException(nameof(comment));
            if (lineNR <= 0)                      throw new ArgumentOutOfRangeException(nameof(lineNR));

            Name = name;
            Namespace = @namespace;
            ElementType = elementType;
            Visibility = visibility;

            FilePath = filePath;
            LineNr = lineNR;
            Comment = comment;
        }

        /// <summary>
        /// Name of type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Namespace of type. Can return null!
        /// </summary>
        public string Namespace { get; set; }
        public ElementType ElementType { get; private set; }
        public Visibility Visibility { get; set; }

        /// <summary>
        /// File in which the declaration was found
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Line number in file where declaration was found.
        /// </summary>
        public int LineNr { get; set; }

        public string Comment { get; set; }

        public API APIDefinition { get; set; }

        public MEF MEFDefinition { get; set; }

        [JsonConverter(typeof(CustomIDescriptorConverter))]
        public IList<IDescriptor> Members
        {
            get 
            {
                if (_members == null)
                {
                    _members = new List<IDescriptor>();
                }
                return this._members; 
            }
        }

        public string WCFContract { get; set; }
    }
}