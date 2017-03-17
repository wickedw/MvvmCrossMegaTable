using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Touch.Views;
using CoreGraphics;
using MvvmCrossMegaTable.Core.ViewModels;
using UIKit;

namespace MvvmCrossMegaTable.IOS.Views
{
    public class MegaTableView : MvxViewController
    {
        private UITableView _tableView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // This view always has the overlay so we can move the table down
            _tableView = new UITableView(new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height), UITableViewStyle.Plain);
            _tableView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;
            Add(_tableView);

            var source = new MegaTableSource(_tableView)
            {
                UseAnimations = true,
                AddAnimation = UITableViewRowAnimation.Top,
                RemoveAnimation = UITableViewRowAnimation.Left,
                ReplaceAnimation = UITableViewRowAnimation.Fade,
                SectionAddAnimation = UITableViewRowAnimation.Bottom,
                SectionRemoveAnimation = UITableViewRowAnimation.Right,
                SectionReplaceAnimation = UITableViewRowAnimation.Middle
            };
            _tableView.Source = source;

            var set = this.CreateBindingSet<MegaTableView, MegaTableViewModel>();
            set.Bind(source).To(vm => vm.SectionCollection);
            set.Apply();
        }
    }
}