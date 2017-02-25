using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DesktopSearch.Core.Tests.Utils
{
    class LoggingInterceptor<T> : ILogger<T>
    {
        private readonly ICollection<LogEvent> _loggedEvents = new SynchronizedCollection<LogEvent>();

        public IEnumerable<LogEvent> LoggedEvents => _loggedEvents;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine(formatter(state, exception));
            _loggedEvents.Add(new LogEvent(logLevel, eventId.Id));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new State<TState>(state);
        }

        class State<TState> : IDisposable
        {
            private readonly TState _state;

            public State(TState state)
            {
                _state = state;
            }
            public void Dispose()
            {
                IDisposable d = _state as IDisposable;
                d?.Dispose();
            }
        }
    }

    internal class LogEvent
    {
        public LogLevel LogLevel { get; }
        public EventId EventId { get; }

        public LogEvent(LogLevel logLevel, EventId eventId)
        {
            LogLevel = logLevel;
            EventId = eventId;
        }
    }
}
