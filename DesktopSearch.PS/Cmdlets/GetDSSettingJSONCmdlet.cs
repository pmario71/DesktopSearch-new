using DesktopSearch.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS
{
    [Cmdlet(VerbsCommon.Get, "DSSettingJSON", DefaultParameterSetName = "All")]
    public class GetDSSettingJSONCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(ConfigAccess.GetJSONExample());
        }
    }
}
