using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mesh_lrc
{
    public static class ConnectionManagerClientFactory
    {
        public static Task<IConnectionManagerClient> CreateAsync(ConnectionMangerSettings settings)
        {
            if (settings.AreInvalid())
            {
                //@TODO log invalid settings
                return null;
            }

            IConnectionManagerClient connectionManagerClient;
            switch (settings.connectionType)
            {
                case ConnectionType.ONEBOX:
                case ConnectionType.AZURE:
                    connectionManagerClient = new LRCCLient(settings);
                    break;
                case ConnectionType.LRC:
                    connectionManagerClient = new ACClient(settings);
                    break;
                default:
                    //@TODO add log error
                    connectionManagerClient = null;
                    break;
            }
            return connectionManagerClient.InitializeAsync();
        }

    }
}
