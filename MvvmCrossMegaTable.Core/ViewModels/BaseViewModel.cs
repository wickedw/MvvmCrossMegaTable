using System;
using System.Linq.Expressions;
using Cirrious.MvvmCross.ViewModels;

namespace MvvmCrossMegaTable.Core.ViewModels
{
    public abstract class BaseViewModel : MvxViewModel
    {            
        protected void SetProperty<T>(
            ref T backingStore,
            T value,
            Expression<Func<T>> property)
        {
            if (Equals(backingStore, value))
            {
                return;
            }

            backingStore = value;

            RaisePropertyChanged(property);
        }        
    }
}
