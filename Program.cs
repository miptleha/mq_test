using IBM.WMQ;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TestQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            var channelParams = ConfigurationManager.AppSettings.Get("ChannelInfo").Split(new[] { '/' });
            string manager = ConfigurationManager.AppSettings.Get("QueueManagerName");
            string[] queueNames = ConfigurationManager.AppSettings.Get("QueueName").Split(';');
            string channel = channelParams[0];
            string conn = channelParams[2];
            string _userID = null;
            string _pwd = null;

            Console.WriteLine(string.Format("Connect to mq: manager={0}, channel={1}, conn={2}", manager, channel, conn));
            MQEnvironment.UserId = _userID;
            MQEnvironment.Password = _pwd;
            MQQueueManager queueManager = null;
            try
            {
                queueManager = new MQQueueManager(manager, channel, conn);
                Console.WriteLine("Connected");
            }
            catch (MQException ex)
            {
                Console.WriteLine(string.Format("!!!Error connect to MQ: {0}, reason: {1}", ex.Message, ex.Reason));
                return;
            }
            Console.WriteLine();

            foreach (var queueName in queueNames) 
            {
                try
                {
                    using (var queue = queueManager.AccessQueue(queueName, MQC.MQOO_INPUT_SHARED + MQC.MQOO_FAIL_IF_QUIESCING + MQC.MQOO_INQUIRE))
                    {
                        Console.WriteLine(queueName + ": " + queue.CurrentDepth);
                    }
                }
                catch (MQException e)
                {
                    Console.WriteLine(string.Format("!!!Error get currentDepth from queue {0}, error message: {1}, reason: {2}",
                        queueName, e.Message, e.Reason));
                }
            }

            queueManager.Disconnect();
        }
    }
}
