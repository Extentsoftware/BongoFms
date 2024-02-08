using System.Collections.ObjectModel;
using Bongo.Domain.Models;
using Bongo.Fms.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bongo.Fms.ViewModel
{
    public partial class SprintListViewModel : ObservableObject
    {
        IConnectivity connectivity;
        public SprintListViewModel(IConnectivity connectivity)
        {
            Items = new ObservableCollection<SprintCoreId>();
            this.connectivity = connectivity;
        }

        [ObservableProperty]
        ObservableCollection<SprintCoreId> items;


        [RelayCommand]
        void Delete(SprintCoreId s)
        {
            if (Items.Contains(s))
            {
                Items.Remove(s);
            }
        }

        [RelayCommand]
        async Task Tap(SprintCoreId s)
        {
            await Shell.Current.GoToAsync($"{nameof(SprintTasksPage)}?Id={s.Id}");
        }

    }
}
