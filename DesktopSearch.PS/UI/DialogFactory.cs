using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopSearch.PS.UI
{
    public class DialogFactory
    {
        public static void ShowDialog<TItem>(TItem dlg)
            where TItem : System.Windows.Window
        {
            if (dlg == null)
                throw new ArgumentNullException("dlg");
            dlg.Loaded += DlgLoaded;

            var result = dlg.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                throw new Exception("Tagging aborted by user!");
            }
        }

        private static void DlgLoaded(object sender, RoutedEventArgs e)
        {
            var wnd = ((Window)sender);
            Delegate dlg = (Action)(() =>
            {
                Console.WriteLine("Activating ...");
                wnd.Activate();
                wnd.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
            });
            wnd.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, dlg);
        }
    }
}
