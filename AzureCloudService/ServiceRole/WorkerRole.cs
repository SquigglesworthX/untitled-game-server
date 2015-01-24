using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using GameLibrary.Data.Model;
using GameLibrary.Data.Core;
using Newtonsoft.Json;

namespace ServiceRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("ServiceRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("ServiceRole has been started");

            FetchTest(InsertTest());

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ServiceRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("ServiceRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

        //This is a test function to demonstrate the azure table storage
        private string InsertTest()
        {
            Idea idea = new Idea()
            {
                Body = "Test body goes in here.",
                Name = "Test name",
                IdeaId = Guid.NewGuid().ToString()
            };
            Idea idea2 = new Idea()
            {
                Body = "Test body goes in here.",
                Name = "Test name",
                IdeaId = Guid.NewGuid().ToString()
            };
            idea.RelatedIdeas = new List<Idea>();
            idea.RelatedIdeas.Add(idea2);
            var context = ContextFactory.GetContext();
            context.IdeaRepository.Add(idea);

            return idea.IdeaId.ToString();
        }

        private Idea FetchTest(string id)
        {
            var context = ContextFactory.GetContext();
            return context.IdeaRepository.GetById(id);
        }

    }
}
