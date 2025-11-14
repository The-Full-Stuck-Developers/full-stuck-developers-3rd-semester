using dataccess;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace api.Etc;

/// <summary>
///     Custom Sieve processor with fluent API configuration for entities
/// </summary>
public class ApplicationSieveProcessor : SieveProcessor
{
    public ApplicationSieveProcessor(IOptions<SieveOptions> options) : base(options)
    {
    }

    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {

        return mapper;
    }
}