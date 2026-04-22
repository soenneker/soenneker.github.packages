using Soenneker.GitHub.Packages.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.GitHub.Packages.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class GitHubPackagesUtilTests : HostedUnitTest
{
    private readonly IGitHubPackagesUtil _util;

    public GitHubPackagesUtilTests(Host host) : base(host)
    {
        _util = Resolve<IGitHubPackagesUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
