using System.IO;
using Jabbot.Core;
using Moq;
using NUnit.Framework;
using Subject = WeatherSprocket.WeatherSprocket;

namespace ExtensionTests
{
    public class WeatherSprocketTests
    {
        Mock<IProxy> _proxy;
        Mock<IBot> _bot;
        Subject _subject;

        [SetUp]
        public void SetUp()
        {
            _proxy = new Mock<IProxy>();
            _subject = new Subject(_proxy.Object);
            _bot = new Mock<IBot>();
        }

        [Test]
        public void Pattern_WithEmptyString_ReturnsFalse()
        {
            Assert.That(_subject.Pattern.IsMatch(""), Is.False);
        }

        [Test]
        public void Pattern_WithPostCodeDefined_ReturnsTrue()
        {
            Assert.That(_subject.Pattern.IsMatch("weather 90001"), Is.True);
        }

        [Test]
        public void Pattern_WithPostCodeDefinedAndOtherText_ReturnsTrue()
        {
            Assert.That(_subject.Pattern.IsMatch("weather 90001 this is some other words"), Is.True);
        }

        [Test]
        public void Pattern_WithPostCodeLessThanFourNumbers_ReturnsFalse()
        {
            Assert.That(_subject.Pattern.IsMatch("weather 900"), Is.False);
        }

        [Test]
        public void ProcessMatch_WithPostCodeLessThanFourNumbers_ReturnsFalse()
        {
            // arrange
            _proxy.Setup(c => c.Get(It.IsAny<string>())).Returns(File.ReadAllText("90001.txt"));
            var match = _subject.Pattern.Match("weather 90001");
            var message = new ChatMessage("ABC", "user", "room");
            
            _subject.ProcessMatch(match, message, _bot.Object);
            
            _bot.Verify(b => b.Send(It.IsAny<string>(), "room"));
        }
    }
}
