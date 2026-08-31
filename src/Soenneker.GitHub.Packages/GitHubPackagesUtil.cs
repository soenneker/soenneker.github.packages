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

    public async ValueTask<List<Package>> GetAllForUser(string owner, PackagePackageType packageType, CancellationToken cancellationToken = default)
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
                                                      requestConfiguration.QueryParameters.PackageType = (PackagesListPackagesForUserPackageTypeParameter) packageType;
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

    public async ValueTask DeleteAllVersions(string owner, string packageName, PackagePackageType packageType, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting package ({packageName}) for owner ({owner})...", packageName, owner);

        try
        {
            GitHubOpenApiClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();
            string packageTypePath = packageType.ToString().ToLowerInvariantFast();
            var versions = new List<PackageVersion>();
            var page = 1;

            while (true)
            {
                string versionsUrl = $"https://api.github.com/users/{Uri.EscapeDataString(owner)}/packages/{packageTypePath}/" +
                                     $"{Uri.EscapeDataString(packageName)}/versions?page={page}&per_page={_maximumPerPage}";

                List<PackageVersion>? pageVersions = await client.Users[owner].Packages[packageTypePath][packageName].Versions
                    .WithUrl(versionsUrl)
                    .GetAsync(cancellationToken: cancellationToken)
                    .NoSync();

                if (pageVersions == null || pageVersions.Count == 0)
                    break;

                versions.AddRange(pageVersions);

                if (pageVersions.Count < _maximumPerPage)
                    break;

                page++;
            }

            if (versions.Count > 0)
            {
                _logger.LogDebug("Found {Count} versions to delete", versions.Count);
                var failures = new List<Exception>();

                foreach (PackageVersion version in versions)
                {
                    if (version.Id.HasValue)
                    {
                        try
                        {
                            await client.Users[owner].Packages[packageTypePath][packageName].Versions[version.Id.Value.ToString()]
                                .DeleteAsync(cancellationToken: cancellationToken)
                                .NoSync();
                            _logger.LogDebug("Deleted version {VersionId} of package {PackageName}", version.Id.Value, packageName);
                        }
                        catch (Exception versionEx)
                        {
                            _logger.LogWarning(versionEx, "Failed to delete version {VersionId} of package {PackageName}", version.Id.Value, packageName);
                            failures.Add(versionEx);
                        }
                    }
                }

                if (failures.Count > 0)
                    throw new AggregateException($"Failed to delete {failures.Count} version(s) of package {packageName}.", failures);

                _logger.LogInformation("Deleted all versions of package {PackageName}", packageName);
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

    public async ValueTask Delete(string owner, string packageName, PackagePackageType packageType, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting entire package ({packageName}) for owner ({owner})...", packageName, owner);

        try
        {
            GitHubOpenApiClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();
            string packageTypePath = packageType.ToString().ToLowerInvariantFast();
            await client.Users[owner].Packages[packageTypePath][packageName].DeleteAsync(cancellationToken: cancellationToken).NoSync();
            
            _logger.LogInformation("Successfully deleted entire package {PackageName}", packageName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete package {PackageName}", packageName);
            throw;
        }
    }
}
