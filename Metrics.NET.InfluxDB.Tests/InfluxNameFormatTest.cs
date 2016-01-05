using NUnit.Framework;

namespace Metrics.NET.InfluxDB.Tests
{
    [TestFixture]
    public class InfluxNameFormatTest
    {
        [Test]
        public void ContextIsADottedPrefix ()
        {
            Assert.That (ConfigOptions.StatsdConverter("[context] stat - count"), Is.EqualTo ("context.stat.count"));
        }
    }
}

