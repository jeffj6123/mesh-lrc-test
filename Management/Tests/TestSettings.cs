// ------------------------------------------------------------------
// Copyright (c) Microsoft.  All Rights Reserved.
// ------------------------------------------------------------------

namespace Microsoft.ServiceFabricMesh.End2EndTestFramework
{
    /// <summary>
    /// Settings for the test.
    /// </summary>
    public class TestSettings
    {
        /// <summary>
        /// Create an instance of TestSettings class.
        /// </summary>
        public TestSettings(string subscription, string location, string resourceGroup, string logName, string pool, bool doNotDeleteFailed, bool loginWithCert, bool useOnebox, bool enableChaos, bool measurePerf)
        {
            this.SubscriptionId = subscription;
            this.Location = location;
            this.ResourceGroup = resourceGroup;
            this.TestLogFileName = logName;
            this.DoNotDeleteFailedResources = doNotDeleteFailed;
            this.LoginWithCert = loginWithCert;
            this.Pool = pool;
            this.UseOnebox = useOnebox;
            this.EnableChaos = enableChaos;
            this.MeasurePerf = measurePerf;
        }

        /// <summary>
        /// Gets or sets the subscription id.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets location for resources.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the resource group.
        /// </summary>
        public string ResourceGroup { get; set; }

        /// <summary>
        /// Gets or sets the name of the test log file.
        /// </summary>
        public string TestLogFileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the pool to target.
        /// </summary>
        public string Pool { get; set; }

        /// <summary>
        /// Gets or sets if on failures to delete resources or not.
        /// </summary>
        public bool DoNotDeleteFailedResources { get; set; }

        /// <summary>
        /// Gets or sets whether to use the test cert for login, otherwise prompt user.
        /// </summary>
        public bool LoginWithCert { get; set; }

        /// <summary>
        /// Gets or sets whether to use the local onebox cluster.
        /// </summary>
        public bool UseOnebox { get; set; }

        /// <summary>
        /// Gets or sets whether to enable chaos on the cluster.
        /// </summary>
        public bool EnableChaos { get; set; }

        /// <summary>
        /// Gets or sets whether to measure performance for operations.
        /// </summary>
        public bool MeasurePerf { get; set; }
    }
}