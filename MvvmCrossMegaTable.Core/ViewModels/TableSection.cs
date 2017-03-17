using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;

namespace MvvmCrossMegaTable.Core.ViewModels
{
    public class TableSection: INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int SectionIndex { get; set; }
        public HeaderViewModel HeaderViewModel { get; set; }
        public ObservableCollection<MvxViewModel> Rows { get; private set; }

        public TableSection(
            int sectionIndex, 
            HeaderViewModel headerViewModel, 
            ObservableCollection<MvxViewModel> rows)
        {
            SectionIndex = sectionIndex;
            Rows = rows;
            HeaderViewModel = headerViewModel;

            Rows.CollectionChanged += RowItemsOnCollectionChanged;
        }

        private void RowItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Mvx.Trace(string.Format("Changed [{0}] '{1}', {2} <{3}>, <{4}>", 
            //    SectionIndex, 
            //    HeaderViewModel.HeaderTitle, 
            //    e.Action,
            //    e.NewItems != null ? e.NewItems.Count.ToString() : "No New",
            //    e.OldItems != null ? e.OldItems.Count.ToString() : "NoOld"));

            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }
    }
}