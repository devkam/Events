using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using DeviceSimulator.Model;
using Newtonsoft.Json;

namespace DeviceSimulator
{
    public class Producer
    {
        private const int INTERVAL_BETWEEN_MESSAGES = 1000;

        private ProducerConfig config;
        private readonly string topic;
        private readonly string broker;
        private readonly string dataFileLocation;

        public Producer(
            string broker,
            string connectionString,
            string topic,
            string certificateLocation,
            string dataFileLocation)
        {
            config = BuildConfig(broker, connectionString, certificateLocation);
            this.topic = topic;
            this.broker = broker;
            this.dataFileLocation = dataFileLocation;
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

                    using (var reader = new StreamReader(@dataFileLocation))
                    {
                        // skip header
                        string headerLine = reader.ReadLine();
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',');
                            var measurement = new MeasurementModel()
                            {
                                VendorId = int.Parse(values[0]),
                                PickupDateTime = DateTime.Parse(values[1]),
                                DropOffDateTime = DateTime.Parse(values[2]),
                                PassengerCount = int.Parse(values[3]),
                                TripDistance = double.Parse(values[4]),
                                RateCodeId = int.Parse(values[5]),
                                StoreAndFwdFlag = string.Equals("Y", values[6]),
                                PULocationID = int.Parse(values[7]),
                                DOLocationID = int.Parse(values[8]),
                                PaymentType = int.Parse(values[9]),
                                FareAmount = double.Parse(values[10]),
                                Extra = double.Parse(values[11]),
                                MtaTax = double.Parse(values[12]),
                                TipAmount = double.Parse(values[13]),
                                TollsAmount = double.Parse(values[14]),
                                ImprovementSurcharge = double.Parse(values[15]),
                                TotalAmount = double.Parse(values[16])
                            };

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
