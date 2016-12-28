using DesktopSearch.Core.Tagging;
using DesktopSearch.PS.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace DesktopSearch.PS.UI
{
    public class KeywordViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private string _keywordToAdd;
        private Task _backgroundInitialization;
        private List<string> _suggestedKeywords = new List<string>();

        public KeywordViewModel(TagDescriptor descriptor, IKeywordSuggestions keywordSuggestions)
        {
            _keywordSuggestions = keywordSuggestions;

            this.Keywords = new ObservableCollection<Keyword>(descriptor.Keywords.Select(p => new Keyword(p)));

            this.SuggestedKeywordsList = CollectionViewSource.GetDefaultView(_suggestedKeywords);
            this.SuggestedKeywordsList.Filter = o =>
            {
                string entry = o as string;

                if ((entry != null) && (this.Keywords.Any(e => e.Text == entry)))
                    return false;

                return true;
            };

            _backgroundInitialization = InitializeKeywords();
        }
        #region Properties


        /// <summary>
        /// Selection of Combobox to be added
        /// </summary>
        public string KeywordToAdd
        {
            get { return _keywordToAdd; }

            set
            {
                Set(ref _keywordToAdd, value);
            }
        }

        public ObservableCollection<Keyword> Keywords { get; private set; }

        public ICollectionView SuggestedKeywordsList { get; private set; }
        #endregion

        #region Commands
        #region AddKeywordCommand Implementation
        ICommand _AddKeywordCommand;

        public ICommand AddKeywordCommand
        {
            get
            {
                if (_AddKeywordCommand == null)
                {
                    _AddKeywordCommand = new RelayCommand(OnAddKeyword, OnCanAddKeyword);
                }
                return _AddKeywordCommand;
            }
        }

        private void OnAddKeyword()
        {
            if (string.IsNullOrEmpty(this.KeywordToAdd))
                return;

            Keyword keyword = new Keyword(this.KeywordToAdd);
            if (!this.Keywords.Contains(keyword))
            {
                this.Keywords.Add(keyword);
                this.SuggestedKeywordsList.Refresh();
            }
            
            KeywordToAdd = null;
        }

        private bool OnCanAddKeyword()
        {
            return !string.IsNullOrEmpty(KeywordToAdd);
        }
        #endregion

        //DeleteSelectedKeywordsCommand
        #region DeleteSelectedKeywordsCommand Implementation
        ICommand _DeleteSelectedKeywordsCommand;
        private IKeywordSuggestions _keywordSuggestions;

        public ICommand DeleteSelectedKeywordsCommand
        {
            get
            {
                if (_DeleteSelectedKeywordsCommand == null)
                {
                    _DeleteSelectedKeywordsCommand = new RelayCommand<IList>(OnDeleteSelectedKeywords, OnCanDeleteSelectedKeywords);
                }
                return _DeleteSelectedKeywordsCommand;
            }
        }

        private void OnDeleteSelectedKeywords(IList keywords)
        {
            if (keywords == null || keywords.Count <= 0)
	        {
                return;
	        }

            foreach (Keyword keyword in keywords.Cast<Keyword>().ToArray())
            {
                this.Keywords.Remove(keyword);
                this.SuggestedKeywordsList.Refresh(); // refresh so that filter is reapplied
            }
        }

        private bool OnCanDeleteSelectedKeywords(IList keywords)
        {
            return (keywords != null) && (keywords.Count > 0);
        }
        #endregion
        #endregion

        private async Task InitializeKeywords()
        {
            // ask for Elastic for all keywords
            IEnumerable<string> kwds = await _keywordSuggestions.GetSuggestedKeywordsAsync();
            _suggestedKeywords.AddRange(kwds);

            this.SuggestedKeywordsList.Refresh();
        }
    }
}
