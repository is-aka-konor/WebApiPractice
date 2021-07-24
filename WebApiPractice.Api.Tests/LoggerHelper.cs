using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace WebApiPractice.Api.Tests
{
    public static class LoggerHelper
    {
        public static Mock<ILogger<T>> GetLogger<T>()
        {
            var logger = new Mock<ILogger<T>>();

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var logLevel = (LogLevel)invocation.Arguments[0]; 
                    var eventId = (EventId)invocation.Arguments[1];  
                    var state = invocation.Arguments[2];
                    var exception = (Exception)invocation.Arguments[3];
                    var formatter = invocation.Arguments[4];

                    var invokeMethod = formatter.GetType().GetMethod("Invoke");
                    var logMessage = invokeMethod?.Invoke(formatter, new[] { state, exception }) as string;
                }));

            return logger;
        }
    }
}
