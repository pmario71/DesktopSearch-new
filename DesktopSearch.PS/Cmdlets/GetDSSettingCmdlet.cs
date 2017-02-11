﻿using DesktopSearch.Core.Configuration;
using DesktopSearch.PS.Composition;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS
{
    /// <summary>
    /// Returns configuration settings.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DSSetting")]
    public class GetDSSettingCmdlet : PSCmdlet
    {
        #region Dependencies
        [Import]
        internal IConfigAccess<LuceneConfig> ConfigAccess { set; get; }
        #endregion

        protected override void ProcessRecord()
        {
            this.Compose();

            WriteObject(this.ConfigAccess);
        }
    }
}
