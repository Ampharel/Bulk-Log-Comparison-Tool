using Bulk_Log_Comparison_Tool_Tests.Substitutes;
using Bulk_Log_Comparison_Tool.DataClasses;

namespace Bulk_Log_Comparison_Tool_Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void LogHasProperName()
        {
            TestParser parser = new TestParser();
            IParsedEvtcLog log = parser.ParseLog("TestLogStart");
            Assert.That(log.GetFileName(), Is.EqualTo("TestLogStart"));
        }

        [Test]
        public void LogHasStartTime()
        {
            TestParser parser = new TestParser();
            IParsedEvtcLog log = parser.ParseLog("TestLogStart","LogStartTime");
            Assert.That(log.GetLogStart(), Is.EqualTo("LogStartTime"));
        }

        public void PlayerHasCorrectDpsForPhaseTest()
        {
            TestParser parser = new TestParser();
            IParsedEvtcLog log = parser.ParseLog("TestLogStart");
        }

        [Test]
        public void PlayerHasFailedStealthTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PlayerHasSucceededStealthTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PlayerHasFailedWithSpecificSkillStealthTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PlayerHasFailedWithGgStealthTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PlayerHasFailedWithDeathStealthTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PhaseNameTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PhasePlayerDpsTest()
        {
            Assert.Fail();
        }
    }
}