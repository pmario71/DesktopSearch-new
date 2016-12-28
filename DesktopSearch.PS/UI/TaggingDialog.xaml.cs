using DesktopSearch.Core.Tagging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using DesktopSearch.PS.Services;

namespace DesktopSearch.PS.UI
{
    /// <summary>
    /// Interaktionslogik für TaggingDialog.xaml
    /// </summary>
    public partial class TaggingDialog
    {
        private IKeywordSuggestions _keywordSuggestionService;
        private TagViewModel _viewmodel;

        public TaggingDialog()
        {
            InitializeComponent();
        }

        public TaggingDialog(Services.IKeywordSuggestions keywordSuggestionService) : this()
        {
            _keywordSuggestionService = keywordSuggestionService;
        }

        public TagDescriptor TagDescriptor
        {
            get
            {
                if (_viewmodel == null)
                    return null;

                _viewmodel.ApplyChangesToSourceDescriptor();
                return _viewmodel.TagDescriptor;
            }
            set
            {
                _viewmodel = new TagViewModel(value, _keywordSuggestionService);
                this.DataContext = _viewmodel;
            }
        }

        //private void OnClosing(object sender, CancelEventArgs e)
        //{
        //    if (e.Cancel)
        //        return;

        //    _viewmodel.ApplyChangesToSourceDescriptor();
        //    this.DialogResult = true;
        //}

        //private void OnCloseEx(object sender, ExecutedRoutedEventArgs e)
        //{
        //    _viewmodel.ApplyChangesToSourceDescriptor();
        //    this.DialogResult = true;
        //}

        //private void OnActivated(object sender, EventArgs e)
        //{
        //    TitleText.Focus();
        //}
    }
}
