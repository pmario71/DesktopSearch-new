using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopSearch.PS.UI
{
    /// <summary>
    /// Interaktionslogik für KeywordView.xaml
    /// </summary>
    public partial class KeywordView : UserControl
    {
        public KeywordView()
        {
            InitializeComponent();
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Right) && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (KeywordsGrid.Items.Count > 0 && KeywordsGrid.SelectedItem == null)
                {
                    KeywordsGrid.SelectedItem = KeywordsGrid.Items[0];
                }
                else
                    KeywordsGrid.Focus();
                e.Handled = true;
            }
        }
    }
}
