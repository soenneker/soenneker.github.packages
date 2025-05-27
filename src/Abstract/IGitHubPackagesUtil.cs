using Soenneker.GitHub.OpenApiClient.Models;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.GitHub.Packages.Abstract;

/// <summary>
/// A utility library for package related GitHub operations
/// </summary>
public interface IGitHubPackagesUtil
{
    [Pure]
    ValueTask<List<Package>> GetAllForUser(string owner, Package_package_type packageType, CancellationToken cancellationToken = default);
}