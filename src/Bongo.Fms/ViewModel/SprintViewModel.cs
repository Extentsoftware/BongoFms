using System.Collections.ObjectModel;
using Bongo.Domain.Models;
using Bongo.Fms.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bongo.Fms.ViewModel
{
    [QueryProperty("id", "id")]
    public partial class SprintViewModel : ObservableObject
    {
        [ObservableProperty]
        Guid id;

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
