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
            _KeywordViewModel = new KeywordViewModel(_descriptor);
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

            _KeywordViewModel = new KeywordViewModel(_descriptor);
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
            }
        }

        public TagDescriptor TagDescriptor { get; internal set; }

        public KeywordViewModel KeywordViewModel
        {
            get => _KeywordViewModel;
        }

        #region Commands


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
        #endregion

    }
}
