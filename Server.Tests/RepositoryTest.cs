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
            //Not looking for any specific results.. just don't want to see it fail. 
            //We could probably add confirmation logic after the insert to test to see if it's working, 
            //but the delete will likely fail if it's not.
        }
    }
}
