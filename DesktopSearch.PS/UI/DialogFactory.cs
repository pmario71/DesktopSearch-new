using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.UI
{
    public class DialogFactory
    {
        public static void ShowDialog<TItem>(TItem dlg)
            where TItem : System.Windows.Window
        {
            if (dlg == null)
                throw new ArgumentNullException("dlg");

            var result = dlg.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                throw new Exception("Tagging aborted by user!");
            }
        }

    }
}
