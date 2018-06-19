using System;
using System.Diagnostics;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using Xunit;

namespace Log4Net.Appenders.NetCore.Tests
{
    /// <summary>
    ///     Used for internal unit testing the <see cref="EventLogAppender" /> class.
    /// </summary>
    public class EventLogAppenderTests
    {
        //
        // Helper functions to dig into the appender
        //
        private static EventLogEntryType GetEntryType(EventLogAppender appender, Level level)
        {
            return (EventLogEntryType) InvokeMethod(appender, "GetEntryType", level);
        }

        public static object InvokeMethod(object target, string name, params object[] args)
        {
            return target.GetType().GetTypeInfo().GetDeclaredMethod(name).Invoke(target, args);
        }

        [Fact(Skip =
            "Need to first create an event source via powershell before running...  New-EventLog -LogName Application -Source xUnit")]
        public void Integration()
        {
            var rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            var eventAppender = new EventLogAppender
            {
                ApplicationName = "xUnit",
                Layout = new SimpleLayout()
            };
            eventAppender.ActivateOptions();

            var stringAppender = new OutputDebugStringAppender
            {
                Layout = new SimpleLayout()
            };
            stringAppender.ActivateOptions();
            BasicConfigurator.Configure(rep, eventAppender, stringAppender);

            var logger = LogManager.GetLogger(rep.Name, "LoggerThread");

            logger.Error(new Exception("exception test"));
        }

        [Fact]
        public void TestGetEntryTypeForExistingApplicationName()
        {
            var eventAppender = new EventLogAppender {ApplicationName = "Winlogon"};
            eventAppender.ActivateOptions();

            Assert.Equal(EventLogEntryType.Information,
                GetEntryType(eventAppender, Level.All));

            Assert.Equal(
                EventLogEntryType.Information,
                GetEntryType(eventAppender, Level.Debug));

            Assert.Equal(
                EventLogEntryType.Information,
                GetEntryType(eventAppender, Level.Info));

            Assert.Equal(
                EventLogEntryType.Warning,
                GetEntryType(eventAppender, Level.Warn));

            Assert.Equal(
                EventLogEntryType.Error,
                GetEntryType(eventAppender, Level.Error));

            Assert.Equal(
                EventLogEntryType.Error,
                GetEntryType(eventAppender, Level.Fatal));

            Assert.Equal(
                EventLogEntryType.Error,
                GetEntryType(eventAppender, Level.Off));
        }
    }
}