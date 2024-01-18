using Bongo.Client;
using Bongo.Domain.Models;
using System.Collections.ObjectModel;

namespace Bongo.Fms.Views;

public partial class SprintListPage : ContentPage
{
    private IBongoApiService _bongoApiService;

    public ObservableCollection<Sprint> Items { get; set; } = [];

    public SprintListPage(IBongoApiService bongoApiService)
	{
		InitializeComponent();

        _bongoApiService = bongoApiService;
        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
    }

    private void Current_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SemanticScreenReader.Announce("selection changed");
    }

    private void OnItemAdded(object sender, EventArgs e)
    {
        SemanticScreenReader.Announce("Add new sprint");
    }
}