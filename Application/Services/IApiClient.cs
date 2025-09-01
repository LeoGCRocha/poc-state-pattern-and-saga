using Refit;

namespace Application.Services;

public interface IApiClient
{
    [Post("/serviceA")]
    Task<ApiResponse<object>> CallServiceA();

    [Post("/serviceB")]
    Task<ApiResponse<object>> CallServiceB();
    
    [Post("/serviceC")]
    Task<ApiResponse<object>> CallServiceC();

    [Post("/serviceD")]
    Task<ApiResponse<object>> CallServiceD();
    
    [Post("/serviceE")]
    Task<ApiResponse<object>> CallServiceE();

    [Post("/serviceB")]
    Task<ApiResponse<object>> CallServiceF();
}