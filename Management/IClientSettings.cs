namespace mesh_lrc
{
    public interface IClientSettings
    {
        string applicationFileLocation { get; }
        string applicationName { get; }
        string certLocation { get; }
        string clusterConnectionUrl { get; }
        string clusterUrl { get; }
        string serverCertThumbprint { get; }

        bool AreInvalid();
    }
}