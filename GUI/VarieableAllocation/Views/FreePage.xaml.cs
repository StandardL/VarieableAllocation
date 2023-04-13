using Microsoft.UI.Xaml.Controls;

using VarieableAllocation.ViewModels;

namespace VarieableAllocation.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class FreePage : Page
{
    public FreeViewModel ViewModel
    {
        get;
    }

    public FreePage()
    {
        ViewModel = App.GetService<FreeViewModel>();
        InitializeComponent();
    }
}
