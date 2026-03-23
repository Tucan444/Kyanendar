using Kyanendar.MVVM.Models;
using Kyanendar.MVVM.ViewModels;

namespace Kyanendar.MVVM.Views;

public partial class KyanendarView : ContentPage
{
	public KyanendarView(Models.Kyanendar kyanendar, KyanendarService kyanendarService)
	{
		InitializeComponent();
		BindingContext = new KyanendarViewModel(kyanendar, kyanendarService);
	}

	protected override void OnAppearing() {
		base.OnAppearing();

		if (BindingContext is KyanendarViewModel vm) {
			vm.Save();
		}
	}
}