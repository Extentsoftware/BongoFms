using Bongo.Domain.Models;
using Bongo.Fms.Services;
using System.Collections.ObjectModel;

namespace Bongo.Fms.Views;

public partial class SprintListPage : ContentPage
{
    private ICachedDataService _cachedDataService;

    public ObservableCollection<SprintCoreId> Items { get; set; } = new();

    public SprintListPage(ICachedDataService cachedDataService)
	{
        InitializeComponent();
        BindingContext = this;
        _cachedDataService = cachedDataService;
        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
    } 
    

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var sprints = await _cachedDataService.GetSprintsAsync(default);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Items.Clear();
            foreach (var sprint in sprints)
                Items.Add(sprint);
        });
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