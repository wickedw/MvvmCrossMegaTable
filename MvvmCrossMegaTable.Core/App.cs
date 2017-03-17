using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossMegaTable.Core.ViewModels;

namespace MvvmCrossMegaTable.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
				
            RegisterAppStart<MegaTableViewModel>();
        }
    }
}