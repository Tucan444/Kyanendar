using Kyanendar.MVVM.ViewModels;

namespace Kyanendar.MVVM.Views;

public partial class MenuView : ContentPage
{
    public MenuView(MenuViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 