using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace DeviceSimulator
{
    public class Producer
    {
        private const int INTERVAL_BETWEEN_MESSAGES = 1000;

        private ProducerConfig config;
        private Thermometer thermometer;
        private readonly string topic;
        private readonly string broker;

        public Producer(string broker, string connectionString, string topic, string certificateLocation)
        {
            thermometer = new Thermometer();
            config = BuildConfig(broker, connectionString, certificateLocation);
            this.topic = topic;
            this.broker = broker;
        }

        public async Task Run()
        {
            try
            {
                using (var producer = new ProducerBuilder<long, string>(config)
                    .SetKeySerializer(Serializers.Int64)
                    .SetValueSerializer(Serializers.Utf8)
                    .Build())
                {
                    int counter = 1;
                    Console.WriteLine("Rozpoczynam wysyłanie wiadomości, topic: {0}, broker: {1}", topic, broker);
                    while (true)
                    {
                        var measurement = thermometer.GenerateNextMeasurement();
                        var message = JsonConvert.SerializeObject(measurement);
                        Console.WriteLine("Id #{0} Value: '{1}'", counter, message);

                        await producer.ProduceAsync(
                            topic,
                            new Message<long, string>
                            {
                                Key = DateTime.UtcNow.Ticks,
                                Value = message
                            });

                        Thread.Sleep(INTERVAL_BETWEEN_MESSAGES);
                        counter++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Occurred - {0}", e.Message);
            }
        }

        private ProducerConfig BuildConfig(string broker, string connectionString, string certificateLocation) =>
            new ProducerConfig
            {
                BootstrapServers = broker,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connectionString,
                SslCaLocation = certificateLocation
            };
    }
}
