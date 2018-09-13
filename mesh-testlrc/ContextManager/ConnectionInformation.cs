using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json;


namespace mesh_lrc
{
    public abstract class IConnectionManagerClient
    {
        public ConnectionMangerSettings settings;
        public bool? uniqueReplicas;
        protected IConnectionManagerClient(ConnectionMangerSettings settings)
        {
            this.settings.connectionType = settings.connectionType;
        }
        //init async resources
        public abstract Task<IConnectionManagerClient> InitializeAsync();
        public abstract bool UpdateApp(string pathToTemplate);

        public List<string> GetBackendReplicas()
        {
            return null;
        }
        public List<string> GetFrontendReplicas()
        {
            return null;
        }

    }
}
