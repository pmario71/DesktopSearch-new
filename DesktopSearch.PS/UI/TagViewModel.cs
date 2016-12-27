using DesktopSearch.Core.Contracts;
using DesktopSearch.Core.Tagging;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DesktopSearch.PS.UI
{

    public class TagViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private readonly TagDescriptor _descriptor;
        private KeywordViewModel _KeywordViewModel;

        public TagViewModel(TagDescriptor descriptor)
        {
            _descriptor = descriptor;
            _KeywordViewModel = new KeywordViewModel(_descriptor, new Services.KeywordSuggestionService());
        }

        public TagViewModel()
        {
            if (!base.IsInDesignMode)
            {
                throw new InvalidOperationException("Default ctor only for design time!");
            }

            var tags = new Dictionary<string, string>()
            {
                { MetaTypes.Title, "The Title" },
                { MetaTypes.Author, "The Author" },
                { MetaTypes.Keywords, "SQL;Test" }
            };
            var td = new TagDescriptor(tags);
            _descriptor = td;

            _KeywordViewModel = new KeywordViewModel(_descriptor, new Services.KeywordSuggestionService());
        }
        

        public string Title
        {
            get { return _descriptor.Title; }

            set
            {
                if (_descriptor.Title == value)
                {
                    return;
                }
                _descriptor.Title = value;

                RaisePropertyChanged();
                _CloseCommand.RaiseCanExecuteChanged();
            }
        }

        public string Author
        {
            get { return _descriptor.Author; }

            set
            {
                if (_descriptor.Author == value)
                {
                    return;
                }
                _descriptor.Author = value;

                RaisePropertyChanged();
                _CloseCommand.RaiseCanExecuteChanged();
            }
        }

        public TagDescriptor TagDescriptor { get; internal set; }

        private string _closeTrigger;
        /// <summary>
        /// Observed by View. Set to "OK" or "Cancel" if view shall be closed.
        /// </summary>
        public string CloseTrigger
        {
            get { return _closeTrigger; }
            set
            {
                base.Set(ref _closeTrigger, value);
            }
        }

        public KeywordViewModel KeywordViewModel
        {
            get => _KeywordViewModel;
        }

        #region Commands

        #region CloseCommand Implementation
        RelayCommand<string> _CloseCommand;

        public ICommand CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                {
                    _CloseCommand = new RelayCommand<string>(OnClose, OnCanClose);
                }
                return _CloseCommand;
            }
        }

        private void OnClose(string action)
        {
            action = action.ToUpper();
            if ((action == "OK") && HasValidContent())
            {
                this.ApplyChangesToSourceDescriptor();
            }
            this.CloseTrigger = action;
        }

        private bool OnCanClose(string action)
        {
            if (action == "Cancel")
            {
                return true;
            }
            
            return HasValidContent();
        }
        #endregion
        #endregion

        #region other
        public void ApplyChangesToSourceDescriptor()
        {
            _descriptor.Keywords.Clear();

            foreach (var item in _KeywordViewModel.Keywords)
            {
                _descriptor.Keywords.Add(item.Text);
            }
        }

        private bool HasValidContent()
        {
            return !string.IsNullOrEmpty(this.Title) && !string.IsNullOrEmpty(Author);
        }
        #endregion

    }
}
