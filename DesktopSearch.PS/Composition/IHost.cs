using DesktopSearch.Core;
using System;

namespace DesktopSearch.PS.Composition
{
    /// <summary>
    /// Interface that needs to be implemented by the host that is used to compose <see cref="Cmdlet"/> s.
    /// </summary>
    public interface IHost : IDisposable
    {
        /// <summary>
        /// Returns a fully setup <see cref="CompositionContainer"/> instance.
        /// </summary>
        IContainer Container { get; }
    }
}