using Refit;

namespace Application.Services;

public class SimpleApiService(IApiClient apiClient) : ISimpleApiService
{
    public async Task<ApiResponse<object>> CallServiceEndpoint(string service)
    {
        return service switch
        {
            "serviceA" => await apiClient.CallServiceA(),
            "serviceB" => await apiClient.CallServiceB(),
            "serviceC" => await apiClient.CallServiceC(),
            "serviceD" => await apiClient.CallServiceD(),
            "serviceE" => await apiClient.CallServiceE(),
            "serviceF" => await apiClient.CallServiceF(),
            _ => throw new ArgumentException("Invalid service called")
        };
    }
}