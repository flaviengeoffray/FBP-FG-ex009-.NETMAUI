namespace CarsFinder.View;

public partial class MainPage : ContentPage
{
	public MainPage(CarsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

