// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.CommandLine;
using Microsoft.DotNet.Cli;
using Microsoft.DotNet.Cli.Commands;
using Microsoft.DotNet.Cli.Commands.Tool.Install;
using Microsoft.DotNet.Cli.ToolManifest;
using Microsoft.DotNet.Cli.ToolPackage;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.Cli.Utils.Extensions;
using Microsoft.DotNet.Tools.Tests.ComponentMocks;
using Microsoft.Extensions.DependencyModel.Tests;
using Microsoft.Extensions.EnvironmentAbstractions;
using NuGet.Frameworks;
using NuGet.Versioning;
using Parser = Microsoft.DotNet.Cli.Parser;

namespace Microsoft.DotNet.Tests.Commands.Tool
{
    public class ToolInstallLocalCommandTests : SdkTest
    {
        private readonly IFileSystem _fileSystem;
        private readonly IToolPackageStore _toolPackageStore;
        private readonly ToolPackageDownloaderMock _toolPackageDownloaderMock;
        private readonly ParseResult _parseResult;
        private readonly BufferedReporter _reporter;
        private readonly string _temporaryDirectory;
        private readonly string _pathToPlacePackages;
        private readonly ILocalToolsResolverCache _localToolsResolverCache;
        private readonly string _manifestFilePath;
        private readonly PackageId _packageIdA = new("local.tool.console.a");
        private readonly NuGetVersion _packageVersionA;
        private readonly NuGetVersion _packageNewVersionA;
        private readonly ToolCommandName _toolCommandNameA = new("a");
        private readonly ToolManifestFinder _toolManifestFinder;
        private readonly ToolManifestEditor _toolManifestEditor;

        public ToolInstallLocalCommandTests(ITestOutputHelper log) : base(log)
        {
            _packageVersionA = NuGetVersion.Parse("1.0.4");
            _packageNewVersionA = NuGetVersion.Parse("2.0.0");

            _reporter = new BufferedReporter();
            _fileSystem = new FileSystemMockBuilder().UseCurrentSystemTemporaryDirectory().Build();
            _temporaryDirectory = _fileSystem.Directory.CreateTemporaryDirectory().DirectoryPath;
            _pathToPlacePackages = Path.Combine(_temporaryDirectory, "pathToPlacePackage");
            ToolPackageStoreMock toolPackageStoreMock =
                new(new DirectoryPath(_pathToPlacePackages), _fileSystem);
            _toolPackageStore = toolPackageStoreMock;

            _toolPackageDownloaderMock = new ToolPackageDownloaderMock(
                store: _toolPackageStore,
                fileSystem: _fileSystem,
                reporter: _reporter,
                new List<MockFeed>
                {
                    new MockFeed
                    {
                        Type = MockFeedType.ImplicitAdditionalFeed,
                        Packages = new List<MockFeedPackage>
                        {
                            new MockFeedPackage
                            {
                                PackageId = _packageIdA.ToString(),
                                Version = _packageVersionA.ToNormalizedString(),
                                ToolCommandName = _toolCommandNameA.ToString()
                            }
                        }
                    }
                });

            _localToolsResolverCache
                = new LocalToolsResolverCache(
                    _fileSystem,
                    new DirectoryPath(Path.Combine(_temporaryDirectory, "cache")),
                    1);

            _manifestFilePath = Path.Combine(_temporaryDirectory, "dotnet-tools.json");
            _fileSystem.File.WriteAllText(Path.Combine(_temporaryDirectory, _manifestFilePath), _jsonContent);
            _toolManifestFinder = new ToolManifestFinder(new DirectoryPath(_temporaryDirectory), _fileSystem, new FakeDangerousFileDetector());
            _toolManifestEditor = new ToolManifestEditor(_fileSystem);

            _parseResult = Parser.Parse($"dotnet tool install {_packageIdA.ToString()}");

            _localToolsResolverCache
                = new LocalToolsResolverCache(
                    _fileSystem,
                    new DirectoryPath(Path.Combine(_temporaryDirectory, "cache")),
                    1);
        }
        [Fact]
        public void WhenPassingRestoreActionConfigOptions()
        {
            var parseResult = Parser.Parse($"dotnet tool install {_packageIdA.ToString()} --ignore-failed-sources");
            var toolInstallCommand = new ToolInstallLocalCommand(parseResult);
            toolInstallCommand.restoreActionConfig.IgnoreFailedSources.Should().BeTrue();
        }

        [Fact]
        public void WhenPassingIgnoreFailedSourcesItShouldNotThrow()
        {
            _fileSystem.File.WriteAllText(Path.Combine(_temporaryDirectory, "nuget.config"), _nugetConfigWithInvalidSources);
            var parseResult = Parser.Parse($"dotnet tool install {_packageIdA.ToString()} --ignore-failed-sources");
            var toolInstallCommand = new ToolInstallLocalCommand(parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            toolInstallCommand.Execute().Should().Be(0);

            _fileSystem.File.Delete(Path.Combine(_temporaryDirectory, "nuget.config"));
        }

        [Fact]
        public void WhenRunWithPackageIdItShouldSaveToCacheAndAddToManifestFile()
        {
            var toolInstallLocalCommand = GetDefaultTestToolInstallLocalCommand();

            toolInstallLocalCommand.Execute().Should().Be(0);

            AssertDefaultInstallSuccess();
        }

        [Fact]
        public void GivenCreateManifestIfNeededWithoutArgumentTheDefaultIsTrueForLegacyBehavior()
        {
            _fileSystem.File.Delete(_manifestFilePath);
            ParseResult parseResult =
            Parser.Parse(
               $"dotnet tool install {_packageIdA.ToString()} --create-manifest-if-needed");

            var toolInstallLocalCommand = new ToolInstallLocalCommand(
                parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            toolInstallLocalCommand.Execute().Should().Be(0);
        }

        [Fact]
        public void GivenNoManifestFileItShouldThrowAndContainNoManifestGuide()
        {
            _fileSystem.File.Delete(_manifestFilePath);
            ParseResult parseResult =
            Parser.Parse(
               $"dotnet tool install {_packageIdA.ToString()} --create-manifest-if-needed false");

            var toolInstallLocalCommand = new ToolInstallLocalCommand(
                parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            Action a = () => toolInstallLocalCommand.Execute();

            a.Should().Throw<GracefulException>()
                .And.Message.Should()
                .Contain(string.Format(CliStrings.CannotFindAManifestFile, ""));
        }

        [Fact]
        public void WhenRunWithExplicitManifestFileItShouldAddEntryToExplicitManifestFile()
        {
            var explicitManifestFilePath = Path.Combine(_temporaryDirectory, "subdirectory", "dotnet-tools.json");
            _fileSystem.File.Delete(_manifestFilePath);
            _fileSystem.Directory.CreateDirectory(Path.Combine(_temporaryDirectory, "subdirectory"));
            _fileSystem.File.WriteAllText(explicitManifestFilePath, _jsonContent);

            ParseResult parseResult =
                Parser.Parse(
                    $"dotnet tool install {_packageIdA.ToString()} --tool-manifest {explicitManifestFilePath}");

            var installLocalCommand = new ToolInstallLocalCommand(
                parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            _toolManifestFinder.Find(new FilePath(explicitManifestFilePath)).Should().HaveCount(1);
        }

        [Fact]
        public void WhenRunWithRollForwardItShouldRollForwardToTrueInManifestFile()
        {
            ParseResult parseResult =
                Parser.Parse(
                    $"dotnet tool install {_packageIdA.ToString()} --allow-roll-forward");

            var installLocalCommand = new ToolInstallLocalCommand(
                parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            _fileSystem.File.ReadAllText(_manifestFilePath).Should()
                .Contain("\"rollForward\": true");
        }

        [Fact]
        public void WhenRunWithoutRollForwardItShouldDefaultRollForwardToFalseInManifestFile()
        {
            ParseResult parseResult =
                Parser.Parse(
                    $"dotnet tool install {_packageIdA.ToString()}");

            var installLocalCommand = new ToolInstallLocalCommand(
                parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            _fileSystem.File.ReadAllText(_manifestFilePath).Should()
                .Contain("\"rollForward\": false");
        }

        [Fact]
        public void WhenRunFromToolInstallRedirectCommandWithPackageIdItShouldSaveToCacheAndAddToManifestFile()
        {
            var toolInstallLocalCommand = GetDefaultTestToolInstallLocalCommand();

            var toolInstallCommand = new ToolInstallCommand(
                _parseResult,
                toolInstallLocalCommand: toolInstallLocalCommand);

            toolInstallCommand.Execute().Should().Be(0);
            AssertDefaultInstallSuccess();
        }

        [Fact]
        public void WhenRunWithPackageIdItShouldShowSuccessMessage()
        {
            var toolInstallLocalCommand = GetDefaultTestToolInstallLocalCommand();

            toolInstallLocalCommand.Execute().Should().Be(0);

            _reporter.Lines[0].Should()
                .Contain(
                    string.Format(CliCommandStrings.LocalToolInstallationSucceeded,
                        _toolCommandNameA.ToString(),
                        _packageIdA,
                        _packageVersionA.ToNormalizedString(),
                        _manifestFilePath).Green());
        }

        [Fact]
        public void GivenFailedPackageInstallWhenRunWithPackageIdItShouldNotChangeManifestFile()
        {
            ParseResult result = Parser.Parse($"dotnet tool install non-exist");

            var installLocalCommand = new ToolInstallLocalCommand(
                result,
                new PackageId("non-exist"),
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            Action a = () => installLocalCommand.Execute();
            a.Should().Throw<GracefulException>()
                .And.Message.Should()
                .Contain(CliCommandStrings.ToolInstallationRestoreFailed);

            _fileSystem.File.ReadAllText(_manifestFilePath).Should()
                .Be(_jsonContent, "Manifest file should not be changed");
        }

        [Fact]
        public void GivenManifestFileConflictItShouldNotAddToCache()
        {
            _toolManifestEditor.Add(
                new FilePath(_manifestFilePath),
                _packageIdA,
                new NuGetVersion(1, 1, 1),
                new[] { _toolCommandNameA });

            var toolInstallLocalCommand = GetDefaultTestToolInstallLocalCommand();

            Action a = () => toolInstallLocalCommand.Execute();
            a.Should().Throw<GracefulException>();

            _localToolsResolverCache.TryLoad(new RestoredCommandIdentifier(
                    _packageIdA,
                    _packageVersionA,
                    NuGetFramework.Parse(BundledTargetFramework.GetTargetFrameworkMoniker()),
                    Constants.AnyRid,
                    _toolCommandNameA),
                out ToolCommand restoredCommand
            ).Should().BeFalse("it should not add to cache if add to manifest failed. " +
                               "But restore do not need to 'revert' since it just set in nuget global directory");
        }

        private ToolInstallLocalCommand GetDefaultTestToolInstallLocalCommand()
        {
            return new ToolInstallLocalCommand(
                _parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);
        }

        [Fact]
        public void WhenRunWithExactVersionItShouldSucceed()
        {
            ParseResult result = Parser.Parse(
                $"dotnet tool install {_packageIdA.ToString()} --version {_packageVersionA.ToNormalizedString()}");

            var installLocalCommand = new ToolInstallLocalCommand(
                result,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            AssertDefaultInstallSuccess();
        }

        [Fact]
        public void WhenRunWithValidVersionRangeItShouldSucceed()
        {
            ParseResult result = Parser.Parse(
                $"dotnet tool install {_packageIdA.ToString()} --version 1.*");

            var installLocalCommand = new ToolInstallLocalCommand(
                result,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            AssertDefaultInstallSuccess();
        }

        [Fact]
        public void WhenRunWithPrereleaseAndPackageVersionItShouldSucceed()
        {
            ParseResult result =
                Parser.Parse($"dotnet tool install {_packageIdA.ToString()} --prerelease");

            var installLocalCommand = new ToolInstallLocalCommand(
                result,
                _packageIdA,
                GetToolToolPackageInstallerWithPreviewInFeed(),
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            var manifestPackages = _toolManifestFinder.Find();
            manifestPackages.Should().HaveCount(1);
            var addedPackage = manifestPackages.Single();
            _localToolsResolverCache.TryLoad(new RestoredCommandIdentifier(
                    addedPackage.PackageId,
                    new NuGetVersion("2.0.1-preview1"),
                    NuGetFramework.Parse(BundledTargetFramework.GetTargetFrameworkMoniker()),
                    Constants.AnyRid,
                    addedPackage.CommandNames.Single()),
                out ToolCommand restoredCommand
            ).Should().BeTrue();

            _fileSystem.File.Exists(restoredCommand.Executable.Value);
        }

        [Fact]
        public void GivenNoManifestFileAndCreateManifestIfNeededFlagItShouldCreateManifestInGit()
        {
            _fileSystem.Directory.CreateDirectory(Path.Combine(_temporaryDirectory, ".git"));
            _fileSystem.File.Delete(_manifestFilePath);
            var currentFolder = Path.Combine(_temporaryDirectory, "subdirectory1", "subdirectory2");
            _fileSystem.Directory.CreateDirectory(currentFolder);

            ParseResult parseResult =
                Parser.Parse(
                    $"dotnet tool install {_packageIdA.ToString()} --create-manifest-if-needed");

            var installLocalCommand = new ToolInstallLocalCommand(
                parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            _fileSystem.File.Exists(Path.Combine(_temporaryDirectory, ".config", "dotnet-tools.json")).Should().BeTrue();
        }

        [Fact]
        public void GivenNoManifestFileItUsesCreateManifestIfNeededByDefault()
        {
            _fileSystem.File.Delete(_manifestFilePath);

            ParseResult parseResult =
                Parser.Parse(
                    $"dotnet tool install {_packageIdA.ToString()}");

            var installLocalCommand = new ToolInstallLocalCommand(
                parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            _fileSystem.File.Exists(Path.Combine(_temporaryDirectory, ".config", "dotnet-tools.json")).Should().BeTrue();
        }

        [Fact]
        public void GivenNoManifestFileAndCreateManifestIfNeededFlagItShouldCreateManifestInSln()
        {
            _fileSystem.Directory.CreateDirectory(Path.Combine(_temporaryDirectory, "test1.sln"));
            _fileSystem.File.Delete(_manifestFilePath);
            var currentFolder = Path.Combine(_temporaryDirectory, "subdirectory1", "subdirectory2");
            _fileSystem.Directory.CreateDirectory(currentFolder);

            ParseResult parseResult =
                Parser.Parse(
                    $"dotnet tool install {_packageIdA.ToString()} --create-manifest-if-needed");

            var installLocalCommand = new ToolInstallLocalCommand(
                parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            _fileSystem.File.Exists(Path.Combine(_temporaryDirectory, ".config", "dotnet-tools.json")).Should().BeTrue();
        }

        [Fact]
        public void GivenNoManifestFileAndCreateManifestIfNeededFlagItShouldCreateManifestInCurrentFolder()
        {
            _fileSystem.File.Delete(_manifestFilePath);

            ParseResult parseResult =
                Parser.Parse(
                    $"dotnet tool install {_packageIdA.ToString()} --create-manifest-if-needed");

            var installLocalCommand = new ToolInstallLocalCommand(
                parseResult,
                _packageIdA,
                _toolPackageDownloaderMock,
                _toolManifestFinder,
                _toolManifestEditor,
                _localToolsResolverCache,
                _reporter);

            installLocalCommand.Execute().Should().Be(0);
            _fileSystem.File.Exists(Path.Combine(_temporaryDirectory, ".config", "dotnet-tools.json")).Should().BeTrue();
        }

        private IToolPackageDownloader GetToolToolPackageInstallerWithPreviewInFeed()
        {
            List<MockFeed> feeds = new()
            {
                new MockFeed
                {
                    Type = MockFeedType.ImplicitAdditionalFeed,
                    Packages = new List<MockFeedPackage>
                    {
                        new MockFeedPackage
                        {
                            PackageId = _packageIdA.ToString(),
                            Version = "1.0.4",
                            ToolCommandName = "SimulatorCommand"
                        },
                        new MockFeedPackage
                        {
                            PackageId = _packageIdA.ToString(),
                            Version = "2.0.1-preview1",
                            ToolCommandName = "SimulatorCommand"
                        }
                    }
                }
            };

            var toolToolPackageDownloader = (IToolPackageDownloader)new ToolPackageDownloaderMock(
                fileSystem: _fileSystem,
                store: _toolPackageStore,
                reporter: _reporter,
                feeds: feeds,
                downloadCallback: null);
            return toolToolPackageDownloader;
        }

        private void AssertDefaultInstallSuccess()
        {
            var manifestPackages = _toolManifestFinder.Find();
            manifestPackages.Should().HaveCount(1);
            var addedPackage = manifestPackages.Single();
            _localToolsResolverCache.TryLoad(new RestoredCommandIdentifier(
                    addedPackage.PackageId,
                    addedPackage.Version,
                    NuGetFramework.Parse(BundledTargetFramework.GetTargetFrameworkMoniker()),
                    Constants.AnyRid,
                    addedPackage.CommandNames.Single()),
                out ToolCommand restoredCommand
            ).Should().BeTrue();

            _fileSystem.File.Exists(restoredCommand.Executable.Value);
        }

        private string _jsonContent =
            @"{
   ""version"":1,
   ""isRoot"":true,
   ""tools"":{
   }
}";

        private string _nugetConfigWithInvalidSources = @"{
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <add key=""nuget"" value=""https://api.nuget.org/v3/index.json"" />
    <add key=""invalid_source"" value=""https://api.nuget.org/v3/invalid.json"" />
  </packageSources>
</configuration>
}";
    }
}
