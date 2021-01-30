using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HaysR.FireAndForget.Exceptions;

namespace HaysR.FireAndForget
{
    public class RetryManager
    {
        private readonly List<StepModel> _steps = new();

        private readonly List<Func<Exception, Task>> _exceptionHandlers = new();

        private readonly HashSet<string> _processedStepsIds = new();

        private int _retryMaxAmount = 5;

        private async Task Exec()
        {
            foreach (var step in _steps)
            {
                if (_processedStepsIds.Contains(step.Id))
                    continue;

                var isProcessed = false;

                while (true)
                {
                    if (isProcessed)
                        break;

                    if (step.GetRetryCount() > _retryMaxAmount)
                        throw new MaxRetryHitException(
                            $"Cant process action with name: {step.Name}. Retry count: {step.GetRetryCount()}");

                    try
                    {
                        await step.StepAction();
                        _processedStepsIds.Add(step.Id);
                        isProcessed = true;
                    }
                    catch (Exception e)
                    {
                        step.IncrementRetryCounter();
                        await HandleException(e);
                    }
                }
            }
        }

        private async Task HandleException(Exception ex)
        {
            foreach (var handler in _exceptionHandlers)
            {
                await handler(ex);
            }
        }

        public RetryManager RegisterStep(string actionName, Func<Task> stepAction, TimeSpan delayBetweenRetry)
        {
            var step = new StepModel
                {Name = actionName, StepAction = stepAction, DelayBetweenRetry = delayBetweenRetry};
            _steps.Add(step);
            return this;
        }

        public RetryManager RegisterExceptionHandler(Func<Exception, Task> action)
        {
            _exceptionHandlers.Add(action);
            return this;
        }

        public void SetRetryMaxAmount(int retryAmount)
        {
            _retryMaxAmount = retryAmount;
        }

        public async Task ExecuteAsync()
        {
            await Exec();
        }

        public static RetryManager Create()
        {
            return new();
        }
    }
}