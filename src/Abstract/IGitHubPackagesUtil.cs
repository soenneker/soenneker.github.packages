using Octokit;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.GitHub.Packages.Abstract;

/// <summary>
/// A utility library for package related GitHub operations
/// </summary>
public interface IGitHubPackagesUtil
{
    [Pure]
    ValueTask<List<Package>> GetAllForUser(string owner, PackageType packageType, CancellationToken cancellationToken = default);
}
