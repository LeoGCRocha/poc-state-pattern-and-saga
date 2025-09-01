using Refit;

namespace Application.Services;

public interface ISimpleApiService
{
    public Task<ApiResponse<object>> CallServiceEndpoint(string service);
}