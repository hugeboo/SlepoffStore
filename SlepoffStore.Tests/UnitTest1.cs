using NUnit.Framework;
using SlepoffStore.Core;

namespace SlepoffStore.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            using var repository = new RemoteRepository("http://localhost:5000", "root", "1", "LAPTOP-SSV");
            var res = repository.GetSections();
            Assert.Pass();
        }
    }
}