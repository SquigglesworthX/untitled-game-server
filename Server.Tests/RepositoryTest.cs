using GameLibrary.Data.Core;
using GameLibrary.Data.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Tests
{
    [TestFixture]
    public class RepositoryTest
    {
        [Test]
        public void MultipleBatchTest()
        {
            IContext context = ContextFactory.GetContext();

            List<Idea> ideas = new List<Idea>();

            //201 gives us 3 batches. 
            for (int i = 0; i < 201; i++)
            {
                Idea idea = new Idea()
                {
                    Body = "Test body goes in here.",
                    Name = "Test name"
                };
                ideas.Add(idea);
                context.IdeaRepository.Add(idea);
            }

            context.Commit();

            //Cleanup the ideas
            foreach (Idea idea in ideas)
            {
                context.IdeaRepository.Remove(idea);
            }

            context.Commit();

        }
    }
}
