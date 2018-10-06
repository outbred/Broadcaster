using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Broadcaster.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        internal class TestViewModelA
        {
            public enum Simple

            {
                One,
                Two,
                Three
            }
            public Broadcaster<Simple> Broadcaster { get; } = new Broadcaster<Simple>();

            public BroadcasterWithPayload<Simple> BroadcasterPayload { get; } = new BroadcasterWithPayload<Simple>();
            public BroadcasterWithPayload BroadcasterPayloadObject { get; } = new BroadcasterWithPayload();


        }

        internal class TestEvent : MessageWithPayload<TestPayload> { }

        internal class TestPayload
        {
            public string Value { get; set; }
        }
    }
}
