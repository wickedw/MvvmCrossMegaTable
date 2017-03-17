using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;

namespace MvvmCrossMegaTable.Core.ViewModels
{

    public class MegaTableViewModel : BaseViewModel
    {
        private ObservableCollection<TableSection> _sectionCollection = new ObservableCollection<TableSection>();
        public ObservableCollection<TableSection> SectionCollection
        {
            get { return _sectionCollection; }
            set { SetProperty(ref _sectionCollection, value, () => SectionCollection); }
        }

        public async Task Init()
        {
            var tempSections = new ObservableCollection<TableSection>();
            var tempCellCollection = new ObservableCollection<MvxViewModel>();

            // Test where you set-up your model in one go as this is the common
            // scenario and the notify hook ups must all work 
            CreateSomeAModels(tempCellCollection);
            tempSections.Add(
                new TableSection(0, new HeaderViewModel("Header 1"),
                    tempCellCollection));

            tempCellCollection = new ObservableCollection<MvxViewModel>();
            CreateSomeAnotherModels(tempCellCollection);
            tempSections.Add(
                new TableSection(1, new HeaderViewModel("Header 2"),
                    tempCellCollection));

            SectionCollection = tempSections;

            InvokeOnMainThread(async () =>
            {
                await TestReplaceSection();

                await TestDeleteLastSection();

                await TestAddSection("Header 3");
                await TestAddSection("Header 4");

                await TestDeleteLastSection();

                await TestHeaderTextChanged(0, "Changed Header 1");
                await TestHeaderTextChanged(1, "Changed Header 2");

                await TestDeleteLastSection();
                await TestDeleteLastSection();
                await TestDeleteLastSection();

                // We will ensure all our handlers are hooked up before we start 
                // trying to invoke row changes
                await TestRowAnimations();
            });

			await Task.Delay(2000);

			await TestHeaderTextChanged(0, "Changed Header 1 Again");

            Mvx.Trace("Done");
        }

        private async Task TestRowAnimations()
        {
            var tempCellCollection = new ObservableCollection<MvxViewModel>();
            CreateSomeAModels(tempCellCollection);
            SectionCollection.Add(
                new TableSection(0, new HeaderViewModel("Header 1 (again)"),
                    tempCellCollection));

            await Task.Delay(2000);
            // Add 3 at a time
            SectionCollection[0].Rows.Add(new ASubViewModel("Row 1.1"));
            SectionCollection[0].Rows.Add(new ASubViewModel("Row 1.2"));
            SectionCollection[0].Rows.Add(new ASubViewModel("Row 1.3"));
            // Add 1 at a time
            await Task.Delay(2000);
            SectionCollection[0].Rows.Add(new ASubViewModel("Row 1.4"));

            await TestAddSection("Header 2 (again)");

            await Task.Delay(2000);
            SectionCollection[1].Rows.Add(new ASubViewModel("Row 2.1"));
            SectionCollection[1].Rows.Add(new ASubViewModel("Row 2.2"));
            SectionCollection[1].Rows.Add(new ASubViewModel("Row 2.3"));
            SectionCollection[1].Rows.Add(new ASubViewModel("Row 2.4"));

            // Delete 2 at a time (top)
            await Task.Delay(2000);
            SectionCollection[0].Rows.RemoveAt(0);
            SectionCollection[0].Rows.RemoveAt(0);

            // Delete 2 at a time (in middle somewhere)
            SectionCollection[1].Rows.Add(new ASubViewModel("Row A"));
            SectionCollection[1].Rows.Add(new ASubViewModel("Row B"));
            await Task.Delay(2000);
            SectionCollection[1].Rows.RemoveAt(1);
            SectionCollection[1].Rows.RemoveAt(2);

            // Replace
            SectionCollection[0].Rows[1] = new ASubViewModel("Replaced");
            SectionCollection[1].Rows[0] = new ASubViewModel("Replaced Also");

            await Task.Delay(2000);
            SectionCollection[0].Rows.Add(new ASubViewModel("Row X"));
            SectionCollection[0].Rows.Add(new ASubViewModel("Row Y"));
            SectionCollection[0].Rows.Add(new ASubViewModel("Row Z"));

            // Movement                
            await Task.Delay(2000);
            ((ASubViewModel)SectionCollection[0].Rows[0]).SomeProp = "Look at me!";
            await Task.Delay(1000);
            SectionCollection[0].Rows.Move(0, 1);
            await Task.Delay(1000);
            SectionCollection[0].Rows.Move(1, 2);
            await Task.Delay(1000);
            SectionCollection[0].Rows.Move(2, 3);
            await Task.Delay(1000);
            SectionCollection[0].Rows.Move(3, 4);
            await Task.Delay(1000);
            SectionCollection[0].Rows.Move(4, 3);
            await Task.Delay(1000);
            SectionCollection[0].Rows.Move(3, 2);
        }

        private async Task TestHeaderTextChanged(int section, string header)
        {
            await Task.Delay(2000);
            SectionCollection[section].HeaderViewModel.HeaderTitle = header;
        }

        private async Task TestReplaceSection()
        {
            await Task.Delay(2000);
            var tempCellCollection = new ObservableCollection<MvxViewModel>();
            CreateSomeAnotherModels(tempCellCollection);
            SectionCollection[0] = new TableSection(
                0, new HeaderViewModel("Header 1 Replaced"), tempCellCollection);
        }

        private async Task TestDeleteLastSection()
        {
            await Task.Delay(2000);

            if (SectionCollection.Any())
                SectionCollection.RemoveAt(SectionCollection.Count - 1);
        }

        private async Task TestAddSection(string header)
        {
            await Task.Delay(2000);
            var tempCellCollection = new ObservableCollection<MvxViewModel>();
            CreateSomeAModels(tempCellCollection);
            SectionCollection.Add(
                new TableSection(
                    SectionCollection.Count,
                    new HeaderViewModel(header),
                    tempCellCollection));
        }

        private void CreateSomeAModels(ObservableCollection<MvxViewModel> tempCellCollection)
        {
            Random r = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < r.Next(2, 5); i++)
            {
                tempCellCollection.Add(new ASubViewModel(r.Next(0, 1000).ToString()));
            }
        }

        private void CreateSomeAnotherModels(ObservableCollection<MvxViewModel> tempCellCollection)
        {
            Random r = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < r.Next(2, 5); i++)
            {
                tempCellCollection.Add(new AnotherSubViewModel(r.Next(0, 1000).ToString()));
            }
        }
    }
}

