﻿using Confluent.Kafka;
using Newtonsoft.Json;
using StatusCommon;

namespace AppTwo
{
    internal class Program
    {
        private const string BootstrapServers = "localhost:9092";
        private const string TopicName = "ping";

        private static void Main(string[] args)
        {
            // Create producer configuration
            var config = new ProducerConfig
            {
                BootstrapServers = BootstrapServers
            };

            // Create a new producer
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                Console.WriteLine("Type your message (or type 'exit' to quit):");

                // Task.Factory.StartNew(()=>{)

                string input;
                do
                {
                    //input = Console.ReadLine();
                    producer.Produce(TopicName, new Message<Null, string> { Value = JsonConvert.SerializeObject(new Model { AppName = "AppTwo", Date = DateTime.Now }) },
                        (deliveryReport) =>
                        {
                            if (deliveryReport.Error.IsError)
                            {
                                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                            }
                            else
                            {
                                Console.WriteLine($"Message delivered to {deliveryReport.TopicPartitionOffset}");
                            }
                        });
                    Thread.Sleep(1000);
                } while (true);
            }
        }
    }
}