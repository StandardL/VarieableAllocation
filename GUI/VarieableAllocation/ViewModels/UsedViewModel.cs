using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using VarieableAllocation.Classes;
using VarieableAllocation.Contracts.ViewModels;
using VarieableAllocation.Core.Contracts.Services;
using VarieableAllocation.Core.Models;


namespace VarieableAllocation.ViewModels;

public class UsedViewModel : ObservableRecipient, INavigationAware
{
    private readonly Memory _memory = new();
    public ObservableCollection<Assignment> Source { get; } = new ObservableCollection<Assignment>();

    public void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var mydata = _memory.Assignments;

        foreach (var item in mydata )
        {
            Source.Add(item);
        }

    }

    public void OnNavigatedFrom()
    {
    }
}
