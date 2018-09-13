namespace Microsoft.ServiceFabricMesh.End2EndTestFramework
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class TestDefinition
    {
        public List<TestPhase> testPhases;
        public TestPhase failHandlingPhase;
        public string testName;
        bool ignoreFailedPhase;

        public TestDefinition(string testName, bool ignoreFailedPhase = false)
        {
            this.testPhases = new List<TestPhase>();
            this.testName = testName;
            this.ignoreFailedPhase = ignoreFailedPhase;
            this.failHandlingPhase = null;
        }

        public void AddPhase(TestPhase testPhase)
        {
            testPhase.parentTestName = testName;
            testPhase.UpdateTestName();
            this.testPhases.Add(testPhase);
        }

        // custom failure handling phase
        public void SetFailHandlingPhase(TestPhase failHandlingPhase)
        {
            failHandlingPhase.parentTestName = testName;
            failHandlingPhase.UpdateTestName();
            this.failHandlingPhase = failHandlingPhase;
        }

        // sets it to the last phase in the list, usually a cleanup operation
        public void SetFailHandlingPhaseToLastPhase()
        {
            this.failHandlingPhase = testPhases[testPhases.Count - 1];
        }

        public async Task<bool> ExecuteTestAsync()
        {
            bool success = true;
            foreach (var phase in testPhases)
            {
                if (!(await phase.ExecutePhaseAsync()))
                {
                    if (ignoreFailedPhase)
                    {
                        success = false;
                    }
                    else
                    {
                        if (failHandlingPhase != null)
                        {
                            await failHandlingPhase.ExecutePhaseAsync();
                        }
                        return false;
                    }
                }
            }

            return success;
        }
    }
}
