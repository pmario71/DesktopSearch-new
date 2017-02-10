using DesktopSearch.Core.Utils.Async;
using Nito.AsyncEx;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace DesktopSearch.Core.Tests.Utils.Async
{
    [TestFixture]
    public class AggregatingProgressReporterTests
    {
        [Test]
        public async Task Report_single_progress_value()
        {
            int reportedValue = -1;
            var l = new AsyncLock();
            var release = l.Lock();

            Action<int> callback = t =>
            {
                reportedValue = t;
                release.Dispose();
            };

            var sut = new AggregatingProgressReporter(callback);

            var c1 = sut.CreateClient();

            c1.Report(30);

            await l.LockAsync();

            Assert.AreEqual(30, reportedValue);
        }

        [Test]
        public async Task Reports_from_multiple_clients_are_averaged()
        {
            int reportedValue = -1;
            var l = new AsyncLock();
            var release = l.Lock();

            Action<int> callback = t => 
            {
                reportedValue = t;
                release.Dispose();
            };

            var sut = new AggregatingProgressReporter(callback);

            var c1 = sut.CreateClient();
            var c2 = sut.CreateClient();
            var c3 = sut.CreateClient();

            c1.Report(30);

            await l.LockAsync();

            Assert.AreEqual(10, reportedValue);
        }

        [Test]
        public async Task Reports_from_different_clients_are_summed()
        {
            int reportedValue = -1;
            var l = new AsyncLock();
            var release = l.Lock();

            int timesToCall = 2;
            Action<int> callback = t =>
            {
                reportedValue = t;

                if ((--timesToCall) <= 0)
                    release.Dispose();
            };

            var sut = new AggregatingProgressReporter(callback);

            var c1 = sut.CreateClient();
            var c2 = sut.CreateClient();
            var c3 = sut.CreateClient();

            c1.Report(30);
            c2.Report(60);

            await l.LockAsync();

            Assert.AreEqual((30+60)/3, reportedValue);
        }

        [Test]
        public async Task Reports_from_same_client()
        {
            int reportedValue = -1;
            var l = new AsyncLock();
            var release = l.Lock();

            int timesToCall = 2;
            Action<int> callback = t =>
            {
                reportedValue = t;

                if ((Interlocked.Decrement(ref timesToCall)) <= 0)
                    release.Dispose();
            };

            var sut = new AggregatingProgressReporter(callback);

            var c1 = sut.CreateClient();
            var c2 = sut.CreateClient();
            var c3 = sut.CreateClient();

            c1.Report(30);
            c1.Report(60);

            await l.LockAsync();

            Assert.AreEqual(60 / 3, reportedValue);
        }
    }
}
