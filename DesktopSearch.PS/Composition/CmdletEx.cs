using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Composition
{
    /// <summary>
    /// Extension to <see cref="PSCmdlet"/> that enables depdendency injection using MEF.
    /// </summary>
    /// <example><code><![CDATA[
    /// [Cmdlet(VerbsData.Update, "Account")]
    /// public class UpdateAccountCmdlet : PSCmdlet
    /// {
    ///     private IHBCIClient _hbciClient;
    ///  
    ///     [Import]
    ///     internal IHBCIClient HBCIClient { set { _hbciClient = value; } }
    /// 
    ///     protected override void BeginProcessing()
    ///     {
    ///         this.Compose();
    ///     }
    /// 
    ///     protected override void ProcessRecord()
    ///     {
    ///             WriteObject(statements);
    ///     }
    /// }
    /// ]]></code></example>
    public static class CmdletEx
    {
        const string CachedHost = "CachedHost";

        /// <summary>
        /// Factory needs to be set before call to <see cref="Compose"/>.
        /// </summary>
        public static Func<IHost> Factory { get; set; }

        /// <summary>
        /// Call Compose() in <see cref="Cmdlet.BeginProcessing"/> to have all dependencies injected into <see cref="PSCmdlet"/>.
        /// </summary>
        /// <remarks>In case <see cref="Factory"/> is not set explicitly, all dlls in the application directory are searched for
        /// an implementation of <see cref="IHost"/></remarks>
        /// <param name="cmdlet"></param>
        public static void Compose(this PSCmdlet cmdlet)
        {
            //Contract.Requires<PSArgumentNullException>(cmdlet != null);
            if (cmdlet == null)
                throw new PSArgumentNullException();

            if (Factory == null)
            {
                var factory = new DefaultFactory();
                Factory = factory.Create();
            }

            var host = cmdlet.GetVariableValue(CachedHost) as IHost;
            if (host == null)
            {
                host = Factory();
                var psVariable = new PSVariable(CachedHost, host);

                cmdlet.SessionState.PSVariable.Set(psVariable);

                Runspace.DefaultRunspace.StateChanged += (sender, args) =>
                {
                    if (args.RunspaceStateInfo.State == RunspaceState.Closing ||
                        args.RunspaceStateInfo.State == RunspaceState.Disconnected)
                    {
                        cmdlet.WriteVerbose("Disposing resources");
                        host.Dispose();
                        host = null;
                    }
                };
            }

            host.Container.Inject(cmdlet);
        }
    }
}