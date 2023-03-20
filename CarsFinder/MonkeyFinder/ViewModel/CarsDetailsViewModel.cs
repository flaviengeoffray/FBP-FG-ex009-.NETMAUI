namespace CarsFinder.ViewModel;

[QueryProperty(nameof(Car), "Car")]
public partial class CarsDetailsViewModel : BaseViewModel
{
    IMap map;
    public CarsDetailsViewModel(IMap map)
    {
        this.map = map;
    }

    [ObservableProperty]
    Car car;

    [RelayCommand]
    async Task OpenMap()
    {
        try
        {
            await map.OpenAsync(Car.Latitude, Car.Longitude, new MapLaunchOptions
            {
                Name = Car.Title,
                NavigationMode = NavigationMode.None
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to launch maps: {ex.Message}");
            await Shell.Current.DisplayAlert("Error, no Maps app!", ex.Message, "OK");
        }
    }
}
