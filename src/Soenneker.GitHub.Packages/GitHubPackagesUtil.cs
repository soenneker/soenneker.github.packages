using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.String;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.GitHub.ClientUtil.Abstract;
using Soenneker.GitHub.OpenApiClient;
using Soenneker.GitHub.OpenApiClient.Models;
using Soenneker.GitHub.Packages.Abstract;

namespace Soenneker.GitHub.Packages;

///<inheritdoc cref="IGitHubPackagesUtil"/>
public sealed class GitHubPackagesUtil : IGitHubPackagesUtil
{
    private readonly ILogger<GitHubPackagesUtil> _logger;
    private readonly IGitHubOpenApiClientUtil _gitHubClientUtil;
    private const int _maximumPerPage = 100;

    public GitHubPackagesUtil(ILogger<GitHubPackagesUtil> logger, IGitHubOpenApiClientUtil gitHubClientUtil)
    {
        _logger = logger;
        _gitHubClientUtil = gitHubClientUtil;
    }

    public async ValueTask<List<Package>> GetAllForUser(string owner, Package_package_type packageType, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all packages for owner ({owner})...", owner);

        GitHubOpenApiClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();

        var result = new List<Package>();
        var page = 1;

        while (true)
        {
            List<Package>? packages = await client.Users[owner]
                                                  .Packages.GetAsync(requestConfiguration =>
                                                  {
                                                      requestConfiguration.QueryParameters.PackageType = (Soenneker.GitHub.OpenApiClient.Users.Item.Packages.GetPackage_typeQueryParameterType)packageType;
                                                      requestConfiguration.QueryParameters.Page = page;
                                                      requestConfiguration.QueryParameters.PerPage = _maximumPerPage;
                                                  }, cancellationToken).NoSync();

            if (packages?.Count == 0)
                break;

            _logger.LogDebug("Found {Count} packages", packages?.Count ?? 0);

            if (packages != null)
            {
                result.AddRange(packages);
            }

            if (packages?.Count < _maximumPerPage)
                break;

            page++;
        }

        return result;
    }

    public async ValueTask DeleteAllVersions(string owner, string packageName, Package_package_type packageType, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting package ({packageName}) for owner ({owner})...", packageName, owner);

        try
        {
            GitHubOpenApiClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();
            
            // Get all versions of the package first
            var versions = await client.User.Packages[packageType.ToString().ToLowerInvariantFast()][packageName].Versions.GetAsync(cancellationToken: cancellationToken).NoSync();
            
            if (versions?.Count > 0)
            {
                _logger.LogDebug("Found {Count} versions to delete", versions.Count);
                
                // Delete each version
                foreach (var version in versions)
                {
                    if (version.Id.HasValue)
                    {
                        try
                        {
                            await client.User.Packages[packageType.ToString().ToLowerInvariantFast()][packageName].Versions[version.Id.Value].DeleteAsync(cancellationToken: cancellationToken).NoSync();
                            _logger.LogDebug("Deleted version {VersionId} of package {PackageName}", version.Id.Value, packageName);
                        }
                        catch (Exception versionEx)
                        {
                            _logger.LogWarning(versionEx, "Failed to delete version {VersionId} of package {PackageName}", version.Id.Value, packageName);
                        }
                    }
                }
                
                _logger.LogInformation("Successfully processed all versions of package {PackageName}", packageName);
            }
            else
            {
                _logger.LogInformation("No versions found for package {PackageName}", packageName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete package {PackageName}", packageName);
            throw;
        }
    }

    public async ValueTask Delete(string owner, string packageName, Package_package_type packageType, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting entire package ({packageName}) for owner ({owner})...", packageName, owner);

        try
        {
            GitHubOpenApiClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();
            
            // Delete the entire package (this deletes all versions automatically)
            await client.User.Packages[packageType.ToString().ToLowerInvariantFast()][packageName].DeleteAsync(cancellationToken: cancellationToken).NoSync();
            
            _logger.LogInformation("Successfully deleted entire package {PackageName}", packageName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete package {PackageName}", packageName);
            throw;
        }
    }
}