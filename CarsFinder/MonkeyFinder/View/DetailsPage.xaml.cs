namespace CarsFinder;

public partial class DetailsPage : ContentPage
{
	public DetailsPage(CarsDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}