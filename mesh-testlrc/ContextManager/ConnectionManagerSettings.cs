using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Client;

namespace mesh_lrc
{
    public enum ConnectionType
    {
        AZURE, ONEBOX, LRC
    };

    public class ConnectionMangerSettings
    {

        public ConnectionType? connectionType;
        public string applicationName { get; private set; }
        public string applicationFileLocation { get; private set; }
        public string applicationResourceFile { get; private set; }

        //LRC
        public string serverCertThumbprint { get; private set; } //= "561D35769884F6A5F91293D8F44B0FAACE7772AE";
        public string certLocation { get; private set; } //= @"C:\Users\jejarry\Downloads\jejarry-lrc-jejarry-lrc-20180820.pfx";

        public string clusterUrl { get; private set; } //= @"http://jejarry-testlrc.centralus.cloudapp.azure.com:3030";
        public string clusterConnectionUrl { get; private set; } //= @"https://jejarry-testlrc.centralus.cloudapp.azure.com:19080";

        //ONEBOX
        public  string powershellClientID = "1950a258-227b-4e31-a9cf-717495945fc2";
        public Uri powershellRedirectURI = new Uri("urn:ietf:wg:oauth:2.0:oob");


        public bool AreInvalid()
        {
            if (this.connectionType == null)
            {
                //@TODO log no connection type provided error
                return true;
            }
            valaidateSHared

            switch (this.connectionType)
            {
                case ConnectionType.ONEBOX:
                case ConnectionType.AZURE:

                    return false;
                case ConnectionType.LRC:

                    return false;
                default:
                    //@TODO add log error
                    return true;
            }
        }
    }
}
