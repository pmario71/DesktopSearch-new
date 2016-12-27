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

        public override int GetHashCode()
        {
            return _keyword.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Keyword kwd = obj as Keyword;
            if (kwd != null)
	        {
                return this.GetHashCode() == kwd.GetHashCode();
	        }

            return false;
        }
    }
}
