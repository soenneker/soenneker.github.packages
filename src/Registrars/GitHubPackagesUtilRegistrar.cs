using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.GitHub.ClientUtil.Registrars;
using Soenneker.GitHub.Packages.Abstract;

namespace Soenneker.GitHub.Packages.Registrars;

/// <summary>
/// A utility library for package related GitHub operations
/// </summary>
public static class GitHubPackagesUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="IGitHubPackagesUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddGitHubPackagesUtilAsSingleton(this IServiceCollection services)
    {
        services.AddGitHubOpenApiClientUtilAsSingleton()
                .TryAddSingleton<IGitHubPackagesUtil, GitHubPackagesUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IGitHubPackagesUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddGitHubPackagesUtilAsScoped(this IServiceCollection services)
    {
        services.AddGitHubOpenApiClientUtilAsSingleton()
                .TryAddScoped<IGitHubPackagesUtil, GitHubPackagesUtil>();

        return services;
    }
}