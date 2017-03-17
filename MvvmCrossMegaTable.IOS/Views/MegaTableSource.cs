using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Cirrious.CrossCore;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using MvvmCrossMegaTable.Core.ViewModels;
using UIKit;

namespace MvvmCrossMegaTable.IOS.Views
{
    // FUTURE make MegaTableSource a base class for reuse
    public class MegaTableSource : MvxTableViewSource
    {
        private IDisposable _subscriptionSectionNotify;
        private List<IDisposable> _subscriptionRowNotify = new List<IDisposable>();

        public UITableViewRowAnimation SectionAddAnimation { get; set; }
        public UITableViewRowAnimation SectionRemoveAnimation { get; set; }
        public UITableViewRowAnimation SectionReplaceAnimation { get; set; }

        public MegaTableSource(UITableView tableView)
            : base(tableView)
        {
            tableView.RegisterClassForCellReuse(typeof(ACell), ACell.Key);
            tableView.RegisterClassForCellReuse(typeof(ACell), AnotherCell.Key);

            // Some defaults
            SectionAddAnimation = UITableViewRowAnimation.Top;
            SectionRemoveAnimation = UITableViewRowAnimation.Left;
            SectionReplaceAnimation = UITableViewRowAnimation.Fade;
            AddAnimation = UITableViewRowAnimation.Top;
            RemoveAnimation = UITableViewRowAnimation.Left;
            ReplaceAnimation = UITableViewRowAnimation.Fade;
        }

        private ObservableCollection<TableSection> _sectionData = new ObservableCollection<TableSection>();
        public new ObservableCollection<TableSection> ItemsSource
        {
            get
            {
                return _sectionData;
            }
            set
            {
                if (ReferenceEquals(_sectionData, value) && !ReloadOnAllItemsSourceSets)
                    return;

                if (_subscriptionSectionNotify != null)
                {
                    _subscriptionSectionNotify.Dispose();
                    _subscriptionSectionNotify = null;
                }

                _sectionData = value;

                INotifyCollectionChanged collectionChanged = _sectionData;
                if (collectionChanged != null)
                    _subscriptionSectionNotify = collectionChanged.WeakSubscribe(SectionOnCollectionChanged);

                foreach (TableSection tableSection in _sectionData)
                {
                    AttachCollectionChangedWeakHandler(tableSection);
                }

                ReloadTableData();
            }
        }

        private void SectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Mvx.Trace("[Section] Action on ItemsSource is " + args.Action);

            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                AttachCollectionChangedWeakHandler(_sectionData[args.NewStartingIndex]);
            }
            else if (args.Action == NotifyCollectionChangedAction.Replace)
            {
                // Attach handler to new section on replacement
                AttachCollectionChangedWeakHandler(args.NewItems[0] as TableSection);
            }
            else if (args.Action == NotifyCollectionChangedAction.Move)
            {
                // TODO need to ensure SectionIndex is correct on move
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                // TODO Would it help to remove handler subscription here even if weak
            }

            if (!UseAnimations)
            {
                ReloadTableData();
            }
            else
            {
                if (TryDoSectionAnimatedChange(args))
                    return;
                ReloadTableData();
            }
        }

        // This is key to handler row level changes within our TableSection
        private void AttachCollectionChangedWeakHandler(TableSection section)
        {
            INotifyCollectionChanged collectionChanged = section as INotifyCollectionChanged;
            if (collectionChanged != null)
                _subscriptionRowNotify.Add(collectionChanged.WeakSubscribe(CollectionChangedOnCollectionChanged));
        }

        protected override void CollectionChangedOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Mvx.Trace("[Row] Action on ItemsSource is " + args.Action);

            // Any change requires a table reload
            if (!UseAnimations)
            {
                ReloadTableData();
            }
            else
            {
                if (TryDoRowsAnimatedChange(sender, args))
                    return;
                ReloadTableData();
            }
        }

        private bool TryDoSectionAnimatedChange(NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    TableView.InsertSections(new NSIndexSet((nuint)args.NewStartingIndex), AddAnimation);
                    return true;
                case NotifyCollectionChangedAction.Remove:
                    TableView.DeleteSections(new NSIndexSet((nuint)args.OldStartingIndex), RemoveAnimation);
                    return true;
                case NotifyCollectionChangedAction.Replace:
                    if (args.NewItems.Count != args.OldItems.Count)
                        return false;
                    TableView.ReloadSections(new NSIndexSet((nuint)args.NewStartingIndex), ReplaceAnimation);
                    return true;
                case NotifyCollectionChangedAction.Move:
                    if (args.NewItems.Count != 1 && args.OldItems.Count != 1)
                        return false;
                    TableView.MoveSection(args.OldStartingIndex, args.NewStartingIndex);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates an NSIndexPath array which consists of one NSIndexPath for each row affected
        /// </summary>
        /// <param name="startingPosition">RowIndex where you are starting the Action</param>
        /// <param name="count">No of rows affected</param>
        /// <param name="sectionIndex">Section you are applying the change to</param>       
        protected static NSIndexPath[] CreateNsIndexPathArray(int startingPosition, int count, int sectionIndex)
        {
            NSIndexPath[] nsIndexPathArray = new NSIndexPath[count];
            for (int index = 0; index < count; ++index)
            {
                nsIndexPathArray[index] = NSIndexPath.FromRowSection(index + startingPosition, sectionIndex);
            }
            return nsIndexPathArray;
        }

        private bool TryDoRowsAnimatedChange(object sender, NotifyCollectionChangedEventArgs args)
        {
            TableSection tableSection = sender as TableSection;

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    TableView.InsertRows(CreateNsIndexPathArray(args.NewStartingIndex, args.NewItems.Count, tableSection.SectionIndex), AddAnimation);
                    return true;
                case NotifyCollectionChangedAction.Remove:
                    TableView.DeleteRows(CreateNsIndexPathArray(args.OldStartingIndex, args.OldItems.Count, tableSection.SectionIndex), RemoveAnimation);
                    return true;
                case NotifyCollectionChangedAction.Replace:
                    if (args.NewItems.Count != args.OldItems.Count)
                        return false;
                    TableView.ReloadRows(new[]
                    {
                        NSIndexPath.FromRowSection(args.NewStartingIndex, tableSection.SectionIndex)
                    }, UITableViewRowAnimation.Fade);
                    return true;
                case NotifyCollectionChangedAction.Move:
                    if (args.NewItems.Count != 1 && args.OldItems.Count != 1)
                        return false;
                    TableView.MoveRow(NSIndexPath.FromRowSection(args.OldStartingIndex, tableSection.SectionIndex),
                        NSIndexPath.FromRowSection(args.NewStartingIndex, tableSection.SectionIndex));
                    return true;
                default:
                    return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _subscriptionSectionNotify != null)
            {
                _subscriptionSectionNotify.Dispose();
                _subscriptionSectionNotify = null;

                foreach (var disposable in _subscriptionRowNotify)
                {
                    disposable.Dispose();
                }
                _subscriptionRowNotify.Clear();
            }
            base.Dispose(disposing);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 44;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            return new HeaderView
            {
                DataContext = _sectionData[(int)section].HeaderViewModel
            };
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            if (_sectionData == null) return 0;

            return _sectionData.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (_sectionData == null) return 0;

            return _sectionData[(int)section].Rows.Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            if (_sectionData == null) return "";
            return _sectionData[(int)section].HeaderViewModel.HeaderTitle;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            try
            {
                return _sectionData[indexPath.Section].Rows[indexPath.Row];
            }
            catch (Exception)
            {
                Mvx.TaggedError("GetItemAt", string.Format("Section {0}, Row {1}", indexPath.Section, indexPath.Row));
                return null;
            }
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            NSString cellIdentifier;

            item = GetItemAt(indexPath);

            if (item is ASubViewModel)
            {
                cellIdentifier = ACell.Key;
            }
            else if (item is AnotherSubViewModel)
            {
                cellIdentifier = AnotherCell.Key;
            }
            else
            {
                throw new ArgumentException("Unknown item type " + item.GetType().Name);
            }

            var cell = TableView.DequeueReusableCell(cellIdentifier, indexPath);

            return cell;
        }
    }
}