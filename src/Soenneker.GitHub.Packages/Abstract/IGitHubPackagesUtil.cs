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
        /// <summary>
        /// Gets all for user.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="packageType">The package type.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task containing the result of the operation.</returns>
        [Pure]
        ValueTask<List<Package>> GetAllForUser(string owner, Package_package_type packageType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes all versions.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="packageName">The package name.</param>
        /// <param name="packageType">The package type.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask DeleteAllVersions(string owner, string packageName, Package_package_type packageType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the delete operation.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="packageName">The package name.</param>
        /// <param name="packageType">The package type.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask Delete(string owner, string packageName, Package_package_type packageType, CancellationToken cancellationToken = default);
    }