namespace MvvmCrossMegaTable.Core.ViewModels
{
    public class AnotherSubViewModel : BaseViewModel
    {
        private string _someProp;
        public string SomeProp
        {
            get { return _someProp; }
            set { SetProperty(ref _someProp, value, () => SomeProp); }
        }

        public AnotherSubViewModel(string someString)
        {
            SomeProp = "another " + someString;
        }
    }
}