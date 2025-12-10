using dataccess;
using dataccess.Entities;
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
        // Configure User entity
        mapper.Property<User>(u => u.Id)
            .CanFilter()
            .CanSort();

        mapper.Property<User>(u => u.Name)
            .CanFilter()
            .CanSort();

        mapper.Property<User>(u => u.Email)
            .CanFilter()
            .CanSort();

        mapper.Property<User>(u => u.PhoneNumber)
            .CanFilter()
            .CanSort();

        return mapper;
    }
}