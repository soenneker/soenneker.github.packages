using Soenneker.GitHub.Packages.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Octokit;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.GitHub.Client.Abstract;

namespace Soenneker.GitHub.Packages;

/// <inheritdoc cref="IGitHubPackagesUtil"/>
public class GitHubPackagesUtil: IGitHubPackagesUtil
{
    private readonly IGitHubClientUtil _gitHubClientUtil;
    private readonly ILogger<GitHubPackagesUtil> _logger;

    private const int _pageSize = 100; // GitHub's API maximum page size

    public GitHubPackagesUtil(IGitHubClientUtil gitHubClientUtil, ILogger<GitHubPackagesUtil> logger)
    {
        _gitHubClientUtil = gitHubClientUtil;
        _logger = logger;
    }

    public async ValueTask<List<Package>> GetAllForUser(string owner, PackageType packageType, CancellationToken cancellationToken = default)
    {
        GitHubClient client = await _gitHubClientUtil.Get(cancellationToken).NoSync();

        _logger.LogInformation("Getting all packages for owner ({owner})...", owner);

        var packages = new List<Package>();
        var page = 1;

        IReadOnlyList<Package> currentPage;
        do
        {
            var apiOptions = new ApiOptions
            {
                PageSize = _pageSize,
                PageCount = 1,
                StartPage = page
            };

            currentPage = await client.Packages.GetAllForUser(owner, packageType, apiOptions).NoSync();
            packages.AddRange(currentPage);
            page++;
        }
        while (currentPage.Count == _pageSize && !cancellationToken.IsCancellationRequested);

        return packages;
    }
}
