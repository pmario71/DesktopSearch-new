using DesktopSearch.Core.Contracts;
using DesktopSearch.Core.Tagging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DesktopSearch.PS.UI
{

    public class TagViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private readonly TagDescriptor _descriptor;

        public TagViewModel(TagDescriptor descriptor)
        {
            _descriptor = descriptor;
            this.Keywords = new ObservableCollection<Keyword>(descriptor.Keywords.Select(p => new Keyword(p)));
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
        }

        public void ApplyChangesToSourceDescriptor()
        {
            _descriptor.Keywords.Clear();
            foreach (var item in this.Keywords)
            {
                _descriptor.Keywords.Add(item.Text);
            }
        }

        public ObservableCollection<Keyword> Keywords { get; private set; }

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
    }
}
