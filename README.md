[![](https://img.shields.io/nuget/v/soenneker.github.packages.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.github.packages/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.github.packages/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.github.packages/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.github.packages.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.github.packages/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.github.packages/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.github.packages/actions/workflows/codeql.yml)

# Soenneker.GitHub.Packages

Lists packages for a GitHub user and deletes a package or all of its versions.

## Installation

```bash
dotnet add package Soenneker.GitHub.Packages
```

## Configure and register

```json
{
  "GH": {
    "Token": "your-github-token"
  }
}
```

```csharp
using Soenneker.GitHub.Packages.Registrars;

services.AddGitHubPackagesUtilAsSingleton();
```

The token needs package read permission for listing and package deletion permission for destructive operations.

## List user packages

```csharp
List<Package> packages = await packageUtil.GetAllForUser(
    owner: "example-user",
    packageType: PackagePackageType.Container,
    cancellationToken);
```

`GetAllForUser()` retrieves every page for the selected package type. These methods use GitHub's user-package endpoints; they do not target organization package endpoints.

## Delete package data

```csharp
await packageUtil.DeleteAllVersions(
    owner: "example-user",
    packageName: "example-package",
    packageType: PackagePackageType.Container,
    cancellationToken);
```

`DeleteAllVersions()` first retrieves all pages, then attempts every version deletion. If any deletion fails, it continues with the remaining versions and throws an `AggregateException` afterward so partial completion is visible.

`Delete()` removes the entire named package for the supplied user. Both deletion methods are permanent and may remove versions consumed by deployments or package references; verify the owner, package type, and package name before calling them.
