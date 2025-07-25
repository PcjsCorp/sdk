// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.DotNet.Cli;
using Microsoft.DotNet.Cli.Commands.Tool;
using Microsoft.DotNet.Cli.Commands.Tool.Install;
using Microsoft.DotNet.Cli.Commands.Tool.Update;
using Parser = Microsoft.DotNet.Cli.Parser;

namespace Microsoft.DotNet.Tests.ParserTests
{
    public class UpdateInstallToolParserTests
    {
        private readonly ITestOutputHelper _output;

        public UpdateInstallToolParserTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("console.test.app --version 1.0.0", "1.0.0")]
        [InlineData("console.test.app --version 1.*", "1.*")]
        [InlineData("console.test.app@1.0.0", "1.0.0")]
        [InlineData("console.test.app@1.*", "1.*")]
        public void UpdateGlobalToolParserCanGetPackageIdentityWithVersion(string arguments, string expectedVersion)
        {
            var result = Parser.Parse($"dotnet tool update -g {arguments}");
            var packageIdentity = result.GetValue(ToolUpdateCommandParser.PackageIdentityArgument);
            var packageId = packageIdentity?.Id;
            var packageVersion = packageIdentity?.VersionRange?.OriginalString ?? result.GetValue(ToolInstallCommandParser.VersionOption);
            packageId.Should().Be("console.test.app");
            packageVersion.Should().Be(expectedVersion);
        }


        [Fact]
        public void UpdateGlobaltoolParserCanGetPackageId()
        {
            var result = Parser.Parse("dotnet tool update -g console.test.app");

            var packageId = result.GetValue(ToolUpdateCommandParser.PackageIdentityArgument)?.Id;

            packageId.Should().Be("console.test.app");
        }

        [Fact]
        public void UpdateToolParserCanGetGlobalOption()
        {
            var result = Parser.Parse("dotnet tool update -g console.test.app");

            result.GetValue(ToolInstallCommandParser.GlobalOption).Should().Be(true);
        }

        [Fact]
        public void UpdateToolParserCanGetFollowingArguments()
        {
            var result =
                Parser.Parse(
                    $@"dotnet tool update -g console.test.app --version 1.0.1 --framework {ToolsetInfo.CurrentTargetFramework} --configfile C:\TestAssetLocalNugetFeed");

            result.GetValue(ToolInstallCommandParser.ConfigOption).Should().Be(@"C:\TestAssetLocalNugetFeed");
            result.GetValue(ToolInstallCommandParser.FrameworkOption).Should().Be(ToolsetInfo.CurrentTargetFramework);
        }

        [Fact]
        public void UpdateToolParserCanParseSourceOption()
        {
            const string expectedSourceValue = "TestSourceValue";

            var result =
                Parser.Parse($"dotnet tool update -g --add-source {expectedSourceValue} console.test.app");

            result.GetRequiredValue(ToolInstallCommandParser.AddSourceOption).First().Should().Be(expectedSourceValue);
        }

        [Fact]
        public void UpdateToolParserCanParseMultipleSourceOption()
        {
            const string expectedSourceValue1 = "TestSourceValue1";
            const string expectedSourceValue2 = "TestSourceValue2";

            var result =
                Parser.Parse(
                    $"dotnet tool update -g " +
                    $"--add-source {expectedSourceValue1} " +
                    $"--add-source {expectedSourceValue2} console.test.app");

            result.GetValue(ToolInstallCommandParser.AddSourceOption).Should().BeEquivalentTo([expectedSourceValue1, expectedSourceValue2]);
        }

        [Fact]
        public void UpdateToolParserCanParseVerbosityOption()
        {
            const string expectedVerbosityLevel = "diag";

            var result =
                Parser.Parse($"dotnet tool update -g --verbosity:{expectedVerbosityLevel} console.test.app");

            Enum.GetName(result.GetValue(ToolInstallCommandParser.VerbosityOption)).Should().Be(expectedVerbosityLevel);
        }

        [Fact]
        public void UpdateToolParserCanParseToolPathOption()
        {
            var result =
                Parser.Parse(@"dotnet tool update --tool-path C:\TestAssetLocalNugetFeed console.test.app");

            result.GetValue(ToolInstallCommandParser.ToolPathOption).Should().Be(@"C:\TestAssetLocalNugetFeed");
        }

        [Fact]
        public void UpdateToolParserCanParseNoCacheOption()
        {
            var result =
                Parser.Parse(@"dotnet tool update -g console.test.app --no-cache");

            result.GetValue(ToolCommandRestorePassThroughOptions.NoCacheOption).Should().Be(true);
        }

        [Fact]
        public void UpdateToolParserCanParseNoHttpCacheOption()
        {
            var result =
                Parser.Parse(@"dotnet tool update -g console.test.app --no-http-cache");

            result.GetValue(ToolCommandRestorePassThroughOptions.NoHttpCacheOption).Should().Be(true);
        }

        [Fact]
        public void UpdateToolParserCanParseIgnoreFailedSourcesOption()
        {
            var result =
                Parser.Parse(@"dotnet tool update -g console.test.app --ignore-failed-sources");

            result.GetValue(ToolCommandRestorePassThroughOptions.IgnoreFailedSourcesOption).Should().Be(true);
        }

        [Fact]
        public void UpdateToolParserCanParseDisableParallelOption()
        {
            var result =
                Parser.Parse(@"dotnet tool update -g console.test.app --disable-parallel");

            result.GetValue(ToolCommandRestorePassThroughOptions.DisableParallelOption).Should().Be(true);
        }

        [Fact]
        public void UpdateToolParserCanParseInteractiveRestoreOption()
        {
            var result =
                Parser.Parse(@"dotnet tool update -g console.test.app --interactive");
            result.GetValue(ToolCommandRestorePassThroughOptions.InteractiveRestoreOption).Should().Be(true);
        }

        [Fact]
        public void UpdateToolParserCanParseVersionOption()
        {
            var result =
                Parser.Parse(@"dotnet tool update -g console.test.app --version 1.2");

            result.GetValue(ToolInstallCommandParser.VersionOption).Should().Be("1.2");
        }

        [Fact]
        public void UpdateToolParserCanParseLocalOption()
        {
            var result =
                Parser.Parse(@"dotnet tool update --local console.test.app");

            result.GetValue(ToolInstallCommandParser.LocalOption).Should().Be(true);
        }

        [Fact]
        public void UpdateToolParserCanParseToolManifestOption()
        {
            var result =
                Parser.Parse(@"dotnet tool update --tool-manifest folder/my-manifest.format console.test.app");

            result.GetValue(ToolInstallCommandParser.ToolManifestOption).Should().Be(@"folder/my-manifest.format");
        }
    }
}
