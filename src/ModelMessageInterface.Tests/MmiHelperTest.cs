﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpTestsEx;
using NetMQ;
using NetMQ.Sockets;

namespace ModelMessageInterface.Tests
{
    [TestFixture]
    public class MmiHelperTest
    {
        [Test]
        public void ArrayToBytes()
        {
            var values = new double[] {1, 2, 3};
            var expectedBytes = values.SelectMany(BitConverter.GetBytes).ToArray();

            var bytes = MmiHelper.ArrayToBytes(values);

            bytes.Should().Have.SameSequenceAs(expectedBytes);
        }

        [Test]
        public void BytesToArray()
        {
            var valuesExpected = new double[] {1, 2, 3};
            var bytes = valuesExpected.SelectMany(BitConverter.GetBytes).ToArray();

            var values = (double[]) MmiHelper.BytesToArray(bytes, "float64", new[] {3});

            values.Should().Have.SameSequenceAs(valuesExpected);
        }

        /// <summary>
        /// Sends and receives message using a very low level sockets.
        /// </summary>
        [Test]
        public void SendAndReceive()
        {
            const string host = "127.0.0.1";
            const uint port = 5558;

            using (var context = NetMQContext.Create())
            using (var server = context.CreateResponseSocket()) // server
            using (var client = context.CreateRequestSocket()) // client
            {
                server.Bind("tcp://" + host + ":" + port);
                client.Connect("tcp://" + host + ":" + port);


                var array = new double[] { 1, 2, 3 };

                MmiHelper.SendMessageAndData(client, new MmiMessage { Values = array });
                var message = MmiHelper.ReceiveMessageAndData(server);

                Assert.AreEqual(array, message.Values);
            }
        }
    }
}
