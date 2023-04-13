using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using VarieableAllocation.Classes;
using VarieableAllocation.Contracts.ViewModels;
using VarieableAllocation.Core.Contracts.Services;
using VarieableAllocation.Core.Models;

namespace VarieableAllocation.ViewModels;

public class FreeViewModel : ObservableRecipient, INavigationAware
{
    private readonly Memory _memory = new();
    public ObservableCollection<Assignment> Source { get; } = new ObservableCollection<Assignment>();


    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = _memory.Freespace;

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
