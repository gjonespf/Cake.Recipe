///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.Codecov&version=0.7.0
#addin nuget:?package=Cake.Coveralls&version=0.10.0
#addin nuget:?package=Cake.Figlet&version=1.3.0
#addin nuget:?package=Cake.Git&version=0.21.0
#addin nuget:?package=Cake.Gitter&version=0.11.0
// Needed for RazorLight & Cake.Graph
// //&loaddependencies=true - which deps?
#addin nuget:?package=System.Text.Encodings.Web&version=4.4.0
#addin nuget:?package=Microsoft.AspNetCore.Hosting.Abstractions&version=2.0.0
#addin nuget:?package=Microsoft.AspNetCore.Html.Abstractions&version=2.0.0
#addin nuget:?package=Microsoft.AspNetCore.Mvc.Razor.Extensions&version=2.0.0
#addin nuget:?package=Microsoft.AspNetCore.Razor&version=2.0.0
#addin nuget:?package=Microsoft.AspNetCore.Razor.Language&version=2.0.0
#addin nuget:?package=Microsoft.AspNetCore.Razor.Runtime&version=2.0.0
//#addin nuget:?package=Microsoft.CodeAnalysis.Razor&version=2.0.0
#addin nuget:?package=RazorLight&version=2.0.0-beta1&prerelease=true
// Needed for Cake.Graph
//#addin nuget:?package=RazorEngine&version=3.10.0&loaddependencies=true
#addin nuget:?package=System.Collections.Immutable&version=1.3.1
#addin nuget:?package=System.Reflection.Metadata&version=1.4.2
#addin nuget:?package=Microsoft.CodeAnalysis.Common&version=2.3.1

#addin nuget:?package=Cake.Graph&version=0.8.0

#addin nuget:?package=Cake.Incubator&version=5.0.1
#addin nuget:?package=Cake.Kudu&version=0.10.0
#addin nuget:?package=Cake.MicrosoftTeams&version=0.9.0
#addin nuget:?package=Cake.ReSharperReports&version=0.11.0
#addin nuget:?package=Cake.Slack&version=0.13.0
#addin nuget:?package=Cake.Transifex&version=0.8.0
#addin nuget:?package=Cake.Twitter&version=0.10.0
#addin nuget:?package=Cake.Wyam&version=2.2.5
#addin nuget:?package=Cake.Issues&version=0.7.0
#addin nuget:?package=Cake.Issues.MsBuild&version=0.7.0
#addin nuget:?package=Cake.Issues.InspectCode&version=0.7.1
#addin nuget:?package=Cake.Issues.Reporting&version=0.7.0
// FIXME: Doesn't work with dotnet-tool
//#addin nuget:?package=Cake.Issues.Reporting.Generic&version=0.7.2



Action<string, IDictionary<string, string>> RequireAddin = (code, envVars) => {
    var script = MakeAbsolute(File(string.Format("./{0}.cake", Guid.NewGuid())));
    try
    {
        System.IO.File.WriteAllText(script.FullPath, code);
        var arguments = new Dictionary<string, string>();

        if(BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient") != null) {
            arguments.Add("nuget_useinprocessclient", BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient"));
        }

        if(BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification") != null) {
            arguments.Add("settings_skipverification", BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification"));
        }

        CakeExecuteScript(script,
            new CakeSettings
            {
                EnvironmentVariables = envVars,
                Arguments = arguments
            });
    }
    finally
    {
        if (FileExists(script))
        {
            DeleteFile(script);
        }
    }
};
