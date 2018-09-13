namespace Microsoft.ServiceFabricMesh.End2EndTestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TestPhase
    {
        public List<TestOperation> phaseOperations;
        string phaseName;
        public string parentTestName;

        public TestPhase(string phaseName)
        {
            phaseOperations = new List<TestOperation>();
            this.phaseName = phaseName;
            parentTestName = "NA";
        }

        public TestPhase(TestOperation operation) : this($"phase - {operation.name}", operation) { }

        public TestPhase(string phaseName, TestOperation operation) : this(phaseName)
        {
            AddOperation(operation);
        }

        public void AddOperation(TestOperation operation)
        {
            operation.parentTestName = parentTestName;
            operation.parentTestPhaseName = phaseName;
            phaseOperations.Add(operation);
        }

        public async Task<bool> ExecutePhaseAsync()
        {
            if (phaseOperations.Count == 0)
            {
                var timestamp = DateTime.Now.ToString();
                Console.WriteLine($"{timestamp}  -  {parentTestName}/ {phaseName} no operations");
                return await Task.FromResult(true);
            }
            else if (phaseOperations.Count == 1)
            {
                var result = await phaseOperations[0].ExecuteOperationAsync();
                var timestamp = DateTime.Now.ToString();
                Console.WriteLine($"{timestamp}  -  {parentTestName}/ {phaseName} result {result}");
                return result;
            }
            else
            {
                List<Task<bool>> executeTasks = new List<Task<bool>>(phaseOperations.Count);
                foreach (var operation in phaseOperations)
                {
                    executeTasks.Add(Task.Run(operation.ExecuteOperationAsync));
                }

                bool[] result = await Task.WhenAll(executeTasks);
                var timestamp = DateTime.Now.ToString();
                Console.WriteLine($"{timestamp}  -  {parentTestName}/ {phaseName} results {string.Join(", ", result)}");
                return result.All(b => b);
            }
        }

        public void UpdateTestName()
        {
            foreach (var op in phaseOperations)
            {
                op.parentTestName = parentTestName;
            }
        }
    }
}
