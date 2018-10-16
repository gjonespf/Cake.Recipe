public class BuildVersion
{
    public string Version { get; private set; }
    public string SemVersion { get; private set; }
    public string Milestone { get; private set; }
    public string CakeVersion { get; private set; }
    public string InformationalVersion { get; private set; }
    public string FullSemVersion { get; private set; }

    public static BuildVersion CalculatingSemanticVersion(
        ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        string version = null;
        string semVersion = null;
        string milestone = null;
        string informationalVersion = null;
        string fullSemVersion = null;
        GitVersion assertedVersions = null;

        if (BuildParameters.ShouldRunGitVersion)
        {
            var noFetch = !BuildParameters.ShouldAllowFetch;
            // TODO better approach
            context.Information("Testing GitVersion...");
                context.GitVersion(new GitVersionSettings{
                    OutputType = GitVersionOutput.BuildServer,
                    NoFetch = noFetch
                });

            context.Information("Calculating Semantic Version...");
            assertedVersions = context.GitVersion(new GitVersionSettings{
                    OutputType = GitVersionOutput.Json,
                    NoFetch = noFetch
            });

            version = assertedVersions.MajorMinorPatch;
            semVersion = assertedVersions.LegacySemVerPadded;
            informationalVersion = assertedVersions.InformationalVersion;
            milestone = string.Concat(version);
            fullSemVersion = assertedVersions.FullSemVer;

            context.Information("Calculated Semantic Version: {0}", semVersion);

            if(BuildParameters.ShouldUpdateAssemblyVersion && BuildParameters.Paths.Files.SolutionInfoFilePath != null)
            {
                context.Information("Updating Assemblies Version...");

                context.GitVersion(new GitVersionSettings{
                    UpdateAssemblyInfoFilePath = BuildParameters.Paths.Files.SolutionInfoFilePath,
                    UpdateAssemblyInfo = true,
                    OutputType = GitVersionOutput.BuildServer,
                    NoFetch = noFetch
                });
            }
        }

        if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(semVersion))
        {
            context.Information("Fetching version from SolutionInfo...");
            var assemblyInfo = context.ParseAssemblyInfo(BuildParameters.Paths.Files.SolutionInfoFilePath);
            version = assemblyInfo.AssemblyVersion;
            semVersion = assemblyInfo.AssemblyInformationalVersion;
            informationalVersion = assemblyInfo.AssemblyInformationalVersion;
            milestone = string.Concat(version);
        }

        var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

        return new BuildVersion
        {
            Version = version,
            SemVersion = semVersion,
            Milestone = milestone,
            CakeVersion = cakeVersion,
            InformationalVersion = informationalVersion,
            FullSemVersion = fullSemVersion
        };
    }
}
