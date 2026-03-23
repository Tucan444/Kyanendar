using Kyanendar.MVVM.ViewModels;

namespace Kyanendar.MVVM.Views;

public partial class WeekView : ContentPage
{
	private readonly KyanendarViewModel _kyanendarViewModel;

	public WeekView(Models.Week week, KyanendarViewModel kyanendarViewModel) {
		_kyanendarViewModel = kyanendarViewModel;

		InitializeComponent();
		BindingContext = new WeekViewModel(week);
	}

	public void SaveState() {
		if (BindingContext is WeekViewModel) {
			_kyanendarViewModel.Save();
		}
	}
}