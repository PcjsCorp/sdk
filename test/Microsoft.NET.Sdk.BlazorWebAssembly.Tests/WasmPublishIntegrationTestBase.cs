// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Text.Json;
using Microsoft.NET.Sdk.WebAssembly;
using ResourceHashesByNameDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace Microsoft.NET.Sdk.BlazorWebAssembly.Tests
{
    public abstract class WasmPublishIntegrationTestBase(ITestOutputHelper log) : AspNetSdkTest(log)
    {
        protected static void VerifyBootManifestHashes(TestAsset testAsset, string blazorPublishDirectory)
        {
            var bootManifestResolvedPath = Path.Combine(blazorPublishDirectory, "_framework", WasmBootConfigFileName);
            var bootManifest = BootJsonDataLoader.ParseBootData(bootManifestResolvedPath);

            VerifyBootManifestHashes(testAsset, blazorPublishDirectory, bootManifest.resources.coreAssembly);
            VerifyBootManifestHashes(testAsset, blazorPublishDirectory, bootManifest.resources.assembly);
            if (bootManifest.resources.corePdb != null)
            {
                VerifyBootManifestHashes(testAsset, blazorPublishDirectory, bootManifest.resources.corePdb);
            }
            if (bootManifest.resources.pdb != null)
            {
                VerifyBootManifestHashes(testAsset, blazorPublishDirectory, bootManifest.resources.pdb);
            }
            if (bootManifest.resources.runtime != null)
            {
                VerifyBootManifestHashes(testAsset, blazorPublishDirectory, bootManifest.resources.runtime);
            }
            if (bootManifest.resources.icu != null)
            {
                VerifyBootManifestHashes(testAsset, blazorPublishDirectory, bootManifest.resources.icu);
            }
            if (bootManifest.resources.wasmNative != null)
            {
                VerifyBootManifestHashes(testAsset, blazorPublishDirectory, bootManifest.resources.wasmNative);
            }

            if (bootManifest.resources.satelliteResources != null)
            {
                foreach (var resourcesForCulture in bootManifest.resources.satelliteResources)
                {
                    VerifyBootManifestHashes(testAsset, blazorPublishDirectory, resourcesForCulture.Value, resourcesForCulture.Key);
                }
            }

            static void VerifyBootManifestHashes(TestAsset testAsset, string blazorPublishDirectory, ResourceHashesByNameDictionary resources, string prefix = null)
            {
                foreach (var (name, hash) in resources)
                {
                    var relativePath = prefix != null
                        ? Path.Combine(blazorPublishDirectory, "_framework", prefix, name)
                        : Path.Combine(blazorPublishDirectory, "_framework", name);
                    new FileInfo(Path.Combine(testAsset.TestRoot, relativePath)).Should().HashEquals(ParseWebFormattedHash(hash));
                }
            }

            static string ParseWebFormattedHash(string webFormattedHash)
            {
                Assert.StartsWith("sha256-", webFormattedHash);
                return webFormattedHash.Substring(7);
            }
        }
    }
}
