using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HaysR.FireAndForget.Test
{
    public class StepsProcessTest
    {
        [Test]
        public void TestSteps()
        {
            var count = 0;

            var incrementCounter = new Func<Task>(() =>
            {
                count++;
                return Task.CompletedTask;
            });

            RetryManager.Create()
                .RegisterStep("inc 1", incrementCounter, TimeSpan.FromSeconds(1))
                .RegisterStep("inc 2", incrementCounter, TimeSpan.FromSeconds(1))
                .RegisterStep("inc 3", incrementCounter, TimeSpan.FromSeconds(1))
                .RegisterStep("inc 4", incrementCounter, TimeSpan.FromSeconds(1))
                .RegisterStep("inc 5", incrementCounter, TimeSpan.FromSeconds(1))
                .ExecuteAsync()
                .Wait();

            Assert.AreEqual(5, count);
        }

        [Test]
        public void TestRetryAndExHandle()
        {
            var count = 0;
            var exCounter = 0;
            var exCounterHandle = 0;

            var incrementCounter = new Func<Task>(() =>
            {
                count++;
                return Task.CompletedTask;
            });

            RetryManager.Create()
                .RegisterStep("inc 1", incrementCounter, TimeSpan.FromSeconds(1))
                .RegisterStep("inc 2", incrementCounter, TimeSpan.FromSeconds(1))
                .RegisterStep("inc 3", incrementCounter, TimeSpan.FromSeconds(1))
                .RegisterStep("inc 4", incrementCounter, TimeSpan.FromSeconds(1))
                .RegisterStep("inc 5", () =>
                {
                    if (exCounter < 4)
                    {
                        exCounter++;
                        throw new Exception("test");
                    }

                    count++;
                    return Task.CompletedTask;
                }, TimeSpan.FromSeconds(1))
                .RegisterExceptionHandler(exception =>
                {
                    exCounterHandle++;
                    return Task.CompletedTask;
                })
                .ExecuteAsync()
                .Wait();


            Assert.AreEqual(5, count);
            Assert.AreEqual(4, exCounter);
            Assert.AreEqual(4, exCounterHandle);
        }
    }
}