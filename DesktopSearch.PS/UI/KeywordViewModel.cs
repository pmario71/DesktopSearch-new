using DesktopSearch.Core.Tagging;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopSearch.PS.UI
{
    public class KeywordViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private string _keywordToAdd;

        public KeywordViewModel(TagDescriptor descriptor)
        {
            this.Keywords = new ObservableCollection<Keyword>(descriptor.Keywords.Select(p => new Keyword(p)));

            this.KeywordsList = new ObservableCollection<string>(new[] { "SQL", "Test" });
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

        public ObservableCollection<string> KeywordsList { get; private set; }
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
            }
        }

        private bool OnCanDeleteSelectedKeywords(IList keywords)
        {
            return (keywords != null) && (keywords.Count > 0);
        }
        #endregion
        #endregion
    }
}
