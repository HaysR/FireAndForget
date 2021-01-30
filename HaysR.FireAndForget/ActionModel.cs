using System;
using System.Threading.Tasks;

namespace HaysR.FireAndForget
{
    internal class StepModel
    {
        protected internal readonly string Id = Guid.NewGuid().ToString();
        protected internal string Name { get; set; }
        private int RetryCounter { get; set; }
        protected internal Func<Task> StepAction { get; set; }
        protected internal TimeSpan DelayBetweenRetry { get; set; }
        
        public void IncrementRetryCounter()
        {
            RetryCounter++;
        }

        public int GetRetryCount()
        {
            return RetryCounter;
        }
    }
}