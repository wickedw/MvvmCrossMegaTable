using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using CoreGraphics;
using MvvmCrossMegaTable.Core.ViewModels;
using UIKit;

namespace MvvmCrossMegaTable.IOS.Views
{
    public class HeaderView : MvxView
    {
        private UILabel _title;

        public HeaderView()
        {
            Initialise();
        }

        private void Initialise()
        {
            BackgroundColor = UIColor.Blue;

            Frame = new CGRect(0, 0, 320, 34);
            _title = new UILabel(new CGRect(10, 4, 320 - 20, 20));
            _title.TextColor = UIColor.White;
            _title.Text = "Header";

            Add(_title);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<HeaderView, HeaderViewModel>();
                set.Bind(_title).To(vm => vm.HeaderTitle);
                set.Bind(this).For(v => v.Hidden).To(vm => vm.Hidden);
                set.Apply();
            });
        }
    }
}