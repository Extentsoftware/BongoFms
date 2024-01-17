using Bongo.Domain.Models;
using System.Collections.ObjectModel;

namespace Bongo.Fms.Views;

public partial class SprintListPage : ContentPage
{
    public ObservableCollection<Sprint> Items { get; set; } = [];

    public SprintListPage()
	{
		InitializeComponent();
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