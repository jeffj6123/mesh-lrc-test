using CommandLine;
namespace mesh_lrc
{
    internal class Options
    {
        [Option('a', "appname",
            Required = false,
            HelpText = "Name of the application resource.")]
        public string AppName { get; set; }

        [Option('u', "apiurl",
            Required = true,
            HelpText = "Url for accessing REST APIs.")]
        public string APIUrl { get; set; }

        [Option('m', "clustercmanagementurl",
            Required = true,
            HelpText = "Url for communicating with the cluster.")]
        public string ClusterManagementnUrl { get; set; }

        [Option('l', "certificatepath",
            Required = true,
            HelpText = "Path to the certificate to connect to the cluster.")]
        public string CertLocation { get; set; }

        [Option('s', "servercertthumbprint",
            Required = true,
            HelpText = "Server certificate thumbprint.")]
        public string ServerCertThumbprint { get; set; }

        //@TODO: discuss how to handle multiple templates when upgrading
        [Option('p', "primarytemplatepath",
            Required = true,
            HelpText = "Path to the first template file.")]
        public string PrimaryTemplatePath { get; set; }

        [Option('s', "secondarytemplatepath",
            Required = true,
            HelpText = "Path to the second template file.")]
        public string SecondaryTemplatePath { get; set; }

        [Option('c', "cli",
            Required = false,
            HelpText = "Enter test REPL.")]
        public bool CLIenabled { get; set; }
    } 
}
