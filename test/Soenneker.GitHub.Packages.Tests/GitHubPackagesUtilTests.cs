using Soenneker.GitHub.Packages.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.GitHub.Packages.Tests;

[Collection("Collection")]
public class GitHubPackagesUtilTests : FixturedUnitTest
{
    private readonly IGitHubPackagesUtil _util;

    public GitHubPackagesUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IGitHubPackagesUtil>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
