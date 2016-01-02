using NUnit.Framework;

namespace Metrics.NET.InfluxDB.Tests
{
    [TestFixture]
    public class InfluxValueFormatTests
    {
        [Test]
        public void StringsAreQuoted()
        {
            Assert.That (InfluxDbRecord.StringifyValue ("test-string"), Is.EqualTo ("\"test-string\""));
        }

        [Test]
        public void DoubleQuotesAreEscaped()
        {
            Assert.That (InfluxDbRecord.StringifyValue ("test \"string\" here"), Is.EqualTo ("\"test \\\"string\\\" here\""));
            Assert.That (InfluxDbRecord.StringifyValue ("test \"string\""), Is.EqualTo ("\"test \\\"string\\\"\""));
        }

        [Test]
        public void BoolStringsAreNotQuoted()
        {
            Assert.That (InfluxDbRecord.StringifyValue (true), Is.EqualTo ("true"));
            Assert.That (InfluxDbRecord.StringifyValue (false), Is.EqualTo ("false"));
            Assert.That (InfluxDbRecord.StringifyValue ("true"), Is.EqualTo ("true"));
            Assert.That (InfluxDbRecord.StringifyValue ("false"), Is.EqualTo ("false"));
            Assert.That (InfluxDbRecord.StringifyValue ("TRUE"), Is.EqualTo ("true"));
            Assert.That (InfluxDbRecord.StringifyValue ("FALSE"), Is.EqualTo ("false"));
        }

        [Test]
        public void NumbersUsEnUsFormatting()
        {
            Assert.That (InfluxDbRecord.StringifyValue (123456.78), Is.EqualTo ("123456.78"));
        }

        [Test]
        public void NoGroupingSeparator()
        {
            Assert.That (InfluxDbRecord.StringifyValue (12345), Is.Not.ContainsSubstring (","));
            Assert.That (InfluxDbRecord.StringifyValue (123456789.1011), Is.Not.ContainsSubstring (","));
            Assert.That (InfluxDbRecord.StringifyValue (1234567891011121314.2), Is.Not.ContainsSubstring (","));
        }

        [Test]
        public void DontLosePrecision()
        {
            Assert.That (InfluxDbRecord.StringifyValue (12345.678), Is.StringEnding (".678"));
        }

        [Test]
        public void IntegralsEndInLetterI()
        {
            Assert.That (InfluxDbRecord.StringifyValue (1), Is.StringEnding ("i"));
        }

        [Test]
        public void FloatsDoNotEndInLetterI()
        {
            Assert.That (InfluxDbRecord.StringifyValue (1.1), Is.Not.StringContaining ("i"));
        }
    }
}

