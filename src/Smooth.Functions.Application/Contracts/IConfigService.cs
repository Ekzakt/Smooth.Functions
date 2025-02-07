using Smooth.Shared.Application.Configuration;

namespace Smooth.Functions.Application.Contracts;

public interface IConfigService
{
    Task<List<AllowedFileTypeOptions>> GetAllowedFileTypeOptionsList();
}
