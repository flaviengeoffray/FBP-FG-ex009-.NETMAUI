using System.Net.Http.Json;

namespace CarsFinder.Services;

public class CarsService
{
    HttpClient httpClient;
    public CarsService()
    {
        this.httpClient = new HttpClient();
    }

    List<Car> carList;
    public async Task<List<Car>> GetCars()
    {
        if (carList?.Count > 0)
            return carList;
        
        // Online
        var response = await httpClient.GetAsync("https://www.montemagno.com/monkeys.json");
        if (response.IsSuccessStatusCode)
        {
            carList = await response.Content.ReadFromJsonAsync<List<Car>>();
        }
        // Offline
        
        using var stream = await FileSystem.OpenAppPackageFileAsync("carsdata.json");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();
        carList = JsonSerializer.Deserialize<List<Car>>(contents);
        
        return carList;
    }
}
