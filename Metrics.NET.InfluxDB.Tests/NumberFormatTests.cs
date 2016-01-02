using NUnit.Framework;

namespace Metrics.NET.InfluxDB.Tests
{
    [TestFixture]
    public class NumberFormatTests
    {
        [Test]
        public void EnUsFormatting()
        {
            Assert.That (InfluxDbRecord.Stringify (123456.78), Is.StringMatching ("123456.78"));
        }

        [Test]
        public void NoGroupingSeparator()
        {
            Assert.That (InfluxDbRecord.Stringify (12345), Is.Not.ContainsSubstring (","));
            Assert.That (InfluxDbRecord.Stringify (123456789.1011), Is.Not.ContainsSubstring (","));
            Assert.That (InfluxDbRecord.Stringify (1234567891011121314.2), Is.Not.ContainsSubstring (","));
        }

        [Test]
        public void DontLosePrecision()
        {
            Assert.That (InfluxDbRecord.Stringify (12345.678), Is.StringEnding (".678"));
        }

        [Test]
        public void IntegralsEndInLetterI()
        {
            Assert.That (InfluxDbRecord.Stringify (1), Is.StringEnding ("i"));
        }

        [Test]
        public void FloatsDoNotEndInLetterI()
        {
            Assert.That (InfluxDbRecord.Stringify (1.1), Is.Not.StringContaining ("i"));
        }
    }
}

