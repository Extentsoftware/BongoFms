using System.Collections.ObjectModel;
using Bongo.Domain.Models;
using Bongo.Fms.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bongo.Fms.ViewModel
{
    [QueryProperty("Text", "Text")]
    public partial class DetailViewModel : ObservableObject
    {
        [ObservableProperty]
        string text;

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }

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

        [ObservableProperty]
        string text;

        [RelayCommand]
        async Task Add()
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;

            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Uh Oh!", "No Internet", "OK");
                return;
            }

            // Items.Add(Text);
            // add our item
            Text = string.Empty;
        }

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
            await Shell.Current.GoToAsync($"{nameof(SprintTasksPage)}?Text={s}");
        }

    }
}
