namespace Microsoft.ServiceFabricMesh.End2EndTestFramework
{
    using System;
    using System.Threading.Tasks;

    public class TestOperation
    {
        public Context context;
        public string name;
        public string parentTestPhaseName;
        public string parentTestName;

        public TestOperation(Context context, string name)
        {
            this.context = context;
            this.name = name;
            parentTestName = "NA";
            parentTestPhaseName = "NA";
        }

        public virtual async Task<bool> ExecuteOperationAsync()
        {
            return await Task.FromResult(true);
        }

        public void Log(string str)
        {
            context.StatusWriter.WriteStatus($"{parentTestName}/ {parentTestPhaseName}/ {name} - msg: {str}");
        }
    }
}
