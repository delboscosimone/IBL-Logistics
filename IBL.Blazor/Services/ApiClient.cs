using System.Net.Http.Json;
using System.Web;
using IBL.Blazor.Models;

namespace IBL.Blazor.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public Task<ContainerInfoDto?> GetContainerByIdAsync(int id) =>
        GetAsync<ContainerInfoDto>($"Container/GetContainerById?containerId={id}");

    public Task<BookingSummaryDto?> GetBookingByIdAsync(int id) =>
        GetAsync<BookingSummaryDto>($"Booking/GetBookingById?bookingId={id}");

    public async Task<List<CategoryInfoDto>> GetCategoriesAsync() =>
        await GetAsync<List<CategoryInfoDto>>("Container/GetAllMscCodesDetail") ?? new List<CategoryInfoDto>();

    public async Task<List<PortDto>> GetPortsAsync() =>
        await GetAsync<List<PortDto>>("Port/GetAllPorts") ?? new List<PortDto>();

    public async Task<List<ContainerInfoDto>> GetAvailableContainersAsync(int portId, DateTime shippingDate) =>
        await GetAsync<List<ContainerInfoDto>>($"Container/GetAvailableContainersForPort?portId={portId}&shippingDate={HttpUtility.UrlEncode(shippingDate.ToString("o"))}") ?? new List<ContainerInfoDto>();

    public Task<float?> GetAverageTemperatureAsync(int containerId, string agentCode) =>
        GetAsync<float?>($"Container/GetAverageTemperatureForContainer?containerId={containerId}&AgentCode={HttpUtility.UrlEncode(agentCode)}");

    public async Task<List<string>> GetTelemetriesOutOfRangeAsync(int containerId) =>
        await GetAsync<List<string>>($"Container/GetTelemetriesOutOfRange?containerId={containerId}") ?? new List<string>();

    public Task<int> CreateContainerAsync(CreateContainerRequest request)
    {
        var url = $"Container/InsertEmptyContainer?containerNumber={HttpUtility.UrlEncode(request.ContainerNumber)}&AgencyCode={HttpUtility.UrlEncode(request.AgencyCode)}&agentCode={HttpUtility.UrlEncode(request.AgentCode)}&categoryCode={request.CategoryCode}&currentPortId={request.CurrentPortId}";
        return PostQueryAsync<int>(url);
    }

    public Task<int> CreateBookingAsync(CreateBookingRequest request)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["BkNumber"] = request.BkNumber;
        query["CustomerName"] = request.CustomerName;
        query["CustomerSurname"] = request.CustomerSurname;
        query["CustomerTaxCode"] = request.CustomerTaxCode;
        query["CustomerCompany"] = request.CustomerCompany;
        query["ConsigneeName"] = request.ConsigneeName;
        query["ConsigneeSurname"] = request.ConsigneeSurname;
        query["ConsigneeTaxCode"] = request.ConsigneeTaxCode;
        query["ConsigneeCompany"] = request.ConsigneeCompany;
        query["OriginNation"] = request.OriginNation;
        query["OriginCity"] = request.OriginCity;
        query["OriginAddress"] = request.OriginAddress;
        query["OriginPostalCode"] = request.OriginPostalCode;
        query["DestinationNation"] = request.DestinationNation;
        query["DestinationCity"] = request.DestinationCity;
        query["DestinationAddress"] = request.DestinationAddress;
        query["DestinationPostalCode"] = request.DestinationPostalCode;
        query["agencyCode"] = request.AgencyCode;
        query["ShippingDate"] = request.ShippingDate.ToString("o");
        query["EstimatedArrivalDate"] = request.EstimatedArrivalDate.ToString("o");
        query["originPortId"] = request.OriginPortId?.ToString();
        query["destinationPortId"] = request.DestinationPortId?.ToString();
        return PostQueryAsync<int>($"Booking/CreateNewBooking?{query}");
    }

    public async Task<string> AssociateContainerAsync(int containerId, int bookingId, string agencyCode)
    {
        var url = $"Booking/AssociateContainers?containerId={containerId}&bookingId={bookingId}&agentCode={HttpUtility.UrlEncode(agencyCode)}";
        return await GetTextAsync(url);
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        using var response = await _http.GetAsync(url);
        return await ReadResponse<T>(response);
    }

    private async Task<string> GetTextAsync(string url)
    {
        using var response = await _http.GetAsync(url);
        return await ReadTextResponse(response);
    }

    private async Task<T> PostQueryAsync<T>(string url)
    {
        using var response = await _http.PostAsync(url, null);
        var result = await ReadResponse<T>(response);
        return result!;
    }

    private static async Task<T?> ReadResponse<T>(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(text)
                ? $"Errore API: {(int)response.StatusCode} {response.ReasonPhrase}"
                : text);
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)text;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return default;
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<T>(new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            if (typeof(T) == typeof(float?) && float.TryParse(text.Replace('"', ' ').Trim(), out var f))
            {
                return (T)(object)(float?)f;
            }
            if (typeof(T) == typeof(int) && int.TryParse(text.Replace('"', ' ').Trim(), out var i))
            {
                return (T)(object)i;
            }
            throw;
        }
    }

    private static async Task<string> ReadTextResponse(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(text)
                ? $"Errore API: {(int)response.StatusCode} {response.ReasonPhrase}"
                : text);
        }

        return text;
    }
}
