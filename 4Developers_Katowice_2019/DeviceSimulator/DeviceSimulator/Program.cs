using System;
using System.Configuration;

namespace DeviceSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string broker = ConfigurationManager.AppSettings["EVENT_HUB_NAMESPACE"];
            string connectionString = ConfigurationManager.AppSettings["EVENT_HUB_CONNECTION_STRING"];
            string topic = ConfigurationManager.AppSettings["EVENT_HUB_NAME"];
            string certificateLocation = ConfigurationManager.AppSettings["CA_CERTIFICATION_LOCATION"];

            // Start producing messages
            new Producer(broker, connectionString, topic, certificateLocation)
                .Run()
                .Wait();

            Console.ReadKey();
        }
    }
}
