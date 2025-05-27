using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
                                                      requestConfiguration.QueryParameters.PackageType = packageType.ToString().ToLower();
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
}