using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Broadcaster.Tests
{
    [TestClass]
    public class BroadcasterTests
    {
        [TestMethod]
        public async Task TestBroadcaster()
        {
            var b = new TestViewModel();
            int oneFired = 0;
            var token = b.Listener.Listen(TestViewModel.Simple.One, async () => oneFired++);
            await b.Broadcast(TestViewModel.Simple.One);
            Assert.AreEqual(1, oneFired);
            await b.Broadcast(TestViewModel.Simple.One);
            Assert.AreEqual(2, oneFired);

            await b.Broadcast(TestViewModel.Simple.Two);
            Assert.AreEqual(2, oneFired);
            token.Dispose();
        }

        [TestMethod]
        public async Task TestBroadcaster_Unsubscribe()
        {
            var b = new TestViewModel();
            int oneFired = 0;
            var token = b.Listener.Listen(TestViewModel.Simple.One, async () => oneFired++);
            await b.Broadcast(TestViewModel.Simple.One);
            Assert.AreEqual(1, oneFired);
            await b.Broadcast(TestViewModel.Simple.One);
            Assert.AreEqual(2, oneFired);

            await b.Broadcast(TestViewModel.Simple.One);
            Assert.AreEqual(3, oneFired);

            await b.Broadcast(TestViewModel.Simple.Two);
            Assert.AreEqual(3, oneFired);

            token.Dispose();
            await b.Broadcast(TestViewModel.Simple.One);
            Assert.AreEqual(3, oneFired);

            await b.Broadcast(TestViewModel.Simple.Two);
            Assert.AreEqual(3, oneFired);
        }

        [TestMethod]
        public async Task TestBroadcasterWithPayload_Enum()
        {
            var b = new TestViewModel();
            int oneFired = 0;
            object payload = null;
            var token = b.ListenerWithPayload.Listen(TestViewModel.Simple.Two, async arg =>
            {
                oneFired++;
                payload = arg;
            });
            var toSend = new object();
            await b.Broadcast(TestViewModel.Simple.Two, toSend);
            Assert.AreEqual(1, oneFired);
            Assert.AreSame(toSend, payload);
            payload = null;
            await b.Broadcast(TestViewModel.Simple.Two, toSend);
            Assert.AreEqual(2, oneFired);
            Assert.AreSame(toSend, payload);

            payload = null;
            await b.Broadcast(TestViewModel.Simple.One, toSend);
            Assert.AreEqual(2, oneFired);
            Assert.IsNull(payload);

            token.Dispose();
        }

        [TestMethod]
        public async Task TestBroadcasterWithPayload_Enum_Unsubscribe()
        {
            var b = new TestViewModel();
            int oneFired = 0;
            object payload = null;
            var token = b.ListenerWithPayload.Listen(TestViewModel.Simple.Two, async arg =>
            {
                oneFired++;
                payload = arg;
            });
            var toSend = new object();
            await b.Broadcast(TestViewModel.Simple.Two, toSend);
            Assert.AreEqual(1, oneFired);
            Assert.AreSame(toSend, payload);
            payload = null;
            await b.Broadcast(TestViewModel.Simple.Two, toSend);
            Assert.AreEqual(2, oneFired);
            Assert.AreSame(toSend, payload);
            token.Dispose();

            await b.Broadcast(TestViewModel.Simple.Two, toSend);
            Assert.AreEqual(2, oneFired);
            Assert.AreSame(toSend, payload);
        }

        [TestMethod]
        public async Task TestBroadcasterWithPayload_Object()
        {
            var b = new TestViewModel();
            int oneFired = 0;
            TestPayload payload = null;
            var token = b.BroadcasterPayloadObject.Get<TestEvent>().Listen(async arg =>
            {
                oneFired++;
                payload = arg;
            });
            var toSend = new TestPayload();
            await b.Broadcast(toSend);
            Assert.AreEqual(1, oneFired);
            Assert.AreSame(toSend, payload);
            payload = null;
            await b.Broadcast(toSend);
            Assert.AreEqual(2, oneFired);
            Assert.AreSame(toSend, payload);

            payload = null;
            await b.Broadcast(toSend);
            Assert.AreEqual(3, oneFired);
            Assert.AreSame(toSend, payload);

            token.Dispose();
        }

        [TestMethod]
        public async Task TestBroadcasterWithPayload_Object_Unsubscribe()
        {
            var b = new TestViewModel();
            int oneFired = 0;
            TestPayload payload = null;
            var token = b.BroadcasterPayloadObject.Get<TestEvent>().Listen(async arg =>
            {
                oneFired++;
                payload = arg;
            });
            var toSend = new TestPayload();
            await b.Broadcast(toSend);
            Assert.AreEqual(1, oneFired);
            Assert.AreSame(toSend, payload);
            payload = null;
            await b.Broadcast(toSend);
            Assert.AreEqual(2, oneFired);
            Assert.AreSame(toSend, payload);
            token.Dispose();

            await b.Broadcast(toSend);
            Assert.AreEqual(2, oneFired);
            Assert.AreSame(toSend, payload);
        }

        internal class TestViewModel
        {
            public TestViewModel()
            {
                Listener = new Listener<Simple>(Broadcaster);
                ListenerWithPayload = new ListenerWithPayload<Simple>(BroadcasterPayload);
            }

            public enum Simple

            {
                One,
                Two,
                Three
            }
            Broadcaster<Simple> Broadcaster { get; } = new Broadcaster<Simple>();

            public Listener<Simple> Listener { get; }

            BroadcasterWithPayload<Simple> BroadcasterPayload { get; } = new BroadcasterWithPayload<Simple>();

            public ListenerWithPayload<Simple> ListenerWithPayload { get; }

            public BroadcasterWithPayload BroadcasterPayloadObject { get; } = new BroadcasterWithPayload();

            /// <summary>
            /// Usually this would be published due to an internal state or data change
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public async Task Broadcast(Simple message)
            {
                await Broadcaster.Broadcast(message);
            }

            public async Task Broadcast(Simple message, object payload)
            {
                await BroadcasterPayload.Broadcast(message, payload);
            }

            public async Task Broadcast(TestPayload payload)
            {
                await BroadcasterPayloadObject.Get<TestEvent>().Broadcast(payload);
            }

        }

        internal class TestEvent : MessageWithPayload<TestPayload> { }

        internal class TestPayload
        {
            public string Value { get; set; }
        }
    }
}
