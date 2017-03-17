namespace MvvmCrossMegaTable.Core.ViewModels
{
    public class HeaderViewModel : BaseViewModel
    {
        public HeaderViewModel()
        {
            HeaderTitle = "";
            RhsText = "";
        }

        public HeaderViewModel(string title = "", string rhsText = "", bool hidden = false)
        {
            HeaderTitle = title;
            RhsText = rhsText;
            Hidden = hidden;
        }

        private string _headerTitle;
        public string HeaderTitle
        {
            get { return _headerTitle; }
            set { SetProperty(ref _headerTitle, value, () => HeaderTitle); }
        }

        private string _rhsText;
        public string RhsText
        {
            get { return _rhsText; }
            set { SetProperty(ref _rhsText, value, () => RhsText); }
        }

        private bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { SetProperty(ref _hidden, value, () => Hidden); }
        }
    }
}