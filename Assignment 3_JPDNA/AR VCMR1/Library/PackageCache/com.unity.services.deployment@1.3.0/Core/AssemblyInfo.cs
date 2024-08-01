using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Services.Deployment")]
[assembly: InternalsVisibleTo("Unity.Services.Cli.Authoring")]
[assembly: InternalsVisibleTo("Unity.Services.Cli.Authoring.UnitTest")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

// section for tests
#if UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("Unity.Services.Deployment.Tests.Editor")]
#endif
