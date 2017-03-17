using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using CoreGraphics;
using Foundation;
using MvvmCrossMegaTable.Core.ViewModels;
using UIKit;

namespace MvvmCrossMegaTable.IOS.Views
{   
    public class ACell : MvxTableViewCell
    {
        private readonly UILabel _headingLabel;
        private readonly UILabel _subheadingLabel;

        public static readonly NSString Key = new NSString("ACell");

        public ACell(IntPtr handle)
            : base(handle)
        {
            _headingLabel = new UILabel
            {
                BackgroundColor = UIColor.Clear
            };

            _subheadingLabel = new UILabel
            {
                BackgroundColor = UIColor.Clear
            };

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ACell, ASubViewModel>();
                set.Bind(_headingLabel).To(vm => vm.SomeProp);
                set.Apply();
            });

            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            ContentView.AddSubviews(_headingLabel, _subheadingLabel);
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            nfloat leftedge = 15.0f;
            nfloat imagewidth = 35.0f;
            nfloat horizPadding = 10f;
            nfloat vertPadding = 5f;
            nfloat textLeftEdge = leftedge + imagewidth + horizPadding;
            nfloat rhsTextWidth = 70f;

            _headingLabel.Frame = new CGRect(textLeftEdge, vertPadding, ContentView.Bounds.Width - horizPadding * 2 - textLeftEdge - rhsTextWidth, 20);
            _subheadingLabel.Frame = new CGRect(textLeftEdge, 22, ContentView.Bounds.Width - horizPadding * 2 - textLeftEdge - rhsTextWidth, 16);
        }
    }
}