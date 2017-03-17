namespace MvvmCrossMegaTable.Core.ViewModels
{
    public class ASubViewModel : BaseViewModel
    {
        private string _someProp;
        public string SomeProp
        {
            get { return _someProp; }
            set { SetProperty(ref _someProp, value, () => SomeProp); }
        }

        public ASubViewModel(string someString)
        {
            SomeProp = someString;
        }       
    }
}