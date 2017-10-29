public static bool TransifexUserSettingsExists(ICakeContext context)
{
    var path = GetTransifexUserSettingsPath();
    return context.FileExists(path);
}

public static string GetTransifexUserSettingsPath()
{
    var path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/.transifexrc");
    return path;
}

public static bool TransifexIsConfiguredForRepository(ICakeContext context)
{
    return context.FileExists("./.tx/config");
}

// Before we do anything with transifex, we must make sure that it have been properly
// Initialized, this is mostly related to running on appveyor or other CI.
// Because we expect the repository to already be configured to use
// transifex, we cannot run tx init, or it would replace the repository configuration file.
BuildParameters.Tasks.TransifexSetupTask = Task("Transifex-Setup")
    .WithCriteria(() => BuildParameters.TransifexEnabled)
    .WithCriteria(() => !TransifexUserSettingsExists(Context))
    .WithCriteria(() => BuildParameters.Transifex.HasCredentials)
    .Does(() =>
    {
        var path = GetTransifexUserSettingsPath();
        var encoding = new System.Text.UTF8Encoding(false);
        var text = "[https://www.transifex.com]\r\nhostname = https://www.transifex.com\r\npassword = " + BuildParameters.Transifex.ApiToken + "\r\nusername = api";
        System.IO.File.WriteAllText(path, text, encoding);
    });

BuildParameters.Tasks.TransifexPushSourceResource = Task("Transifex-Push-SourceFiles")
    .WithCriteria(() => BuildParameters.TransifexEnabled)
    .WithCriteria(() => BuildParameters.IsRunningOnAppVeyor || string.Equals(BuildParameters.Target, "Transifex-Push-SourceFiles", StringComparison.OrdinalIgnoreCase))
    .IsDependentOn("Transifex-Setup")
    .Does(() =>
    {
        TransifexPush(new TransifexPushSettings {
            UploadSourceFiles = true,
            Force = string.Equals(BuildParameters.Target, "Transifex-Push-SourceFiles", StringComparison.OrdinalIgnoreCase)
        });
    });

BuildParameters.Tasks.TransifexPullTranslations = Task("Transifex-Pull-Translations")
    .WithCriteria(() => BuildParameters.TransifexEnabled)
    .IsDependentOn("Transifex-Push-SourceFiles")
    .Does(() =>
    {
        TransifexPull(new TransifexPullSettings {
            All = true,
            Mode = BuildParameters.TransifexPullMode,
            MinimumPercentage = BuildParameters.TransifexPullPercentage
        });
    });

BuildParameters.Tasks.TransifexPushTranslations = Task("Transifex-Push-Translations")
    .Does(() =>
    {
        TransifexPush(new TransifexPushSettings {
            UploadTranslations = true
        });
    });
