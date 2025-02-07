namespace Smooth.Functions.Application.Contracts;

public interface IApiServiceFactory
{
    IApiService Create(string clientName);
}