namespace DesktopSearch.PS.UI
{

    public class Keyword : GalaSoft.MvvmLight.ObservableObject
    {
        private string _keyword;

        public Keyword()
        {
            _keyword = string.Empty;
        }

        public Keyword(string keyword)
        {
            _keyword = keyword;
        }

        public string Text
        {
            get { return _keyword; }

            set
            {
                if (_keyword == value)
                {
                    return;
                }
                _keyword = value;

                RaisePropertyChanged();
            }
        }
    }
}
