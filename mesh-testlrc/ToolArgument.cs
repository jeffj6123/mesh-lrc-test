using Microsoft.ServiceFabric.CommandLineParser;

namespace mesh_lrc
{
    internal class ToolArguments
    {
    /// <summary>
    /// Optional name of the application resource in the cluster
    /// </summary>
    [CommandLineArgument(
        CommandLineArgumentType.AtMostOnce,
        Description = "Name of the application resource to be used.",
        LongName ="appname",
        ShortName = "a")]
    public string appName = "myapp";

    /// <summary>
    /// URL with port for the cluster to be accessible for API calls.
    /// </summary>
    [CommandLineArgument(
        CommandLineArgumentType.Required | CommandLineArgumentType.AtMostOnce,
        Description = "Url for the cluster to be connected to.",
        LongName = "clusterurl",
        ShortName = "u")]
    public string clusterUrl = null;

    /// <summary>
    /// Client connection url for the cluster, used for applying upgrades.
    /// </summary>
    [CommandLineArgument(
        CommandLineArgumentType.Required | CommandLineArgumentType.AtMostOnce,
        Description = "Url for the cluster to be connected to.",
        LongName = "clusterconnectionurl",
        ShortName = "cc")]
    public string clusterConnectionUrl = null;

        /// <summary>
        /// Location of the certificate.
        /// </summary>
        [CommandLineArgument(
        CommandLineArgumentType.Required | CommandLineArgumentType.AtMostOnce,
        Description = "Location of the Certificate to connect to the cluster.",
        LongName = "certlocation",
        ShortName = "c")]
    public string certLocation = null;

    /// <summary>
    /// Server certificate thumbprint to connect to cluster.
    /// </summary>
    [CommandLineArgument(
        CommandLineArgumentType.Required | CommandLineArgumentType.AtMostOnce,
        Description = "Server certificate thumbprint",
        LongName = "servercertthumbprint",
        ShortName = "s")]
    public string serverCertThumbprint = null;

    /// <summary>
    /// Location of the first application resource file, requires the env variable version to have 1.
    /// </summary>
    [CommandLineArgument(
        CommandLineArgumentType.Required | CommandLineArgumentType.AtMostOnce,
        Description = "Location of the first app resource file",
        LongName = "primaryappresourcefile",
        ShortName = "pa")]
    public string applicationFileLocation = null;

    /// <summary>
    /// Location of the second application resource file, requires the env variable version to have anything but 1.
    /// </summary>
    [CommandLineArgument(
        CommandLineArgumentType.Required | CommandLineArgumentType.AtMostOnce,
        Description = "Location of the second app resource file",
        LongName = "secondaryappresourcefile",
        ShortName = "sa")]
    public string applicationFileLocation2 = null;

    }
}
