using CarsFinder.Services;

namespace CarsFinder.ViewModel;

public partial class CarsViewModel : BaseViewModel
{
    public ObservableCollection<Car> Cars { get; } = new();
    CarsService carService;
    IConnectivity connectivity;
    IGeolocation geolocation;
    public CarsViewModel(CarsService carService, IConnectivity connectivity, IGeolocation geolocation)
    {
        Title = "Car Finder";
        this.carService = carService;
        this.connectivity = connectivity;
        this.geolocation = geolocation;
    }
    
    [RelayCommand]
    async Task GoToDetails(Car car)
    {
        if (car == null)
        return;

        await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
        {
            {"Car", car }
        });
    }

    [ObservableProperty]
    bool isRefreshing;

    [RelayCommand]
    async Task GetCarsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;
            var cars = await carService.GetCars();

            if(Cars.Count != 0)
                Cars.Clear();

            foreach(var car in cars)
                Cars.Add(car);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get cars: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }

    }

    [RelayCommand]
    async Task GetClosestCars()
    {
        if (IsBusy || Cars.Count == 0)
            return;

        try
        {
            // Get cached location, else get real location.
            var location = await geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                location = await geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }

            // Find closest monkey to us
            var first = Cars.OrderBy(m => location.CalculateDistance(
                new Location(m.Latitude, m.Longitude), DistanceUnits.Miles))
                .FirstOrDefault();

            await Shell.Current.DisplayAlert("", first.Title + " " +
                first.Location, "OK");

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to query location: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
    }
}
