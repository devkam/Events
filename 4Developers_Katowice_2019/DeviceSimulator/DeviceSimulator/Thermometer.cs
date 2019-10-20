using System;
using DeviceSimulator.Model;

namespace DeviceSimulator
{
    public class Thermometer
    {
        private const int DEFAULT_PRECISION = 2;

        private readonly double minTemperature;
        private readonly double maxTemperature;
        private readonly double minDelta;
        private readonly double maxDelta;
        private readonly Random random;
        private double temperature;

        public Thermometer()
        {
            temperature = 20.0;
            minTemperature = 5.0;
            maxTemperature = 40.0;
            minDelta = 1.0;
            maxDelta = 2.0;
            random = new Random();
        }

        public MeasurementModel GenerateNextMeasurement()
        {
            var currentDate = DateTime.Now.Ticks;
            var measurement = new MeasurementModel
            {
                Temperature = temperature,
                Timestamp = currentDate
            };

            double delta = random.NextDouble() * maxDelta - minDelta;
            temperature += delta;
            temperature = Math.Round(temperature, DEFAULT_PRECISION);
            temperature = Math.Min(temperature, maxTemperature);
            temperature = Math.Max(temperature, minTemperature);
            return measurement;
        }
    }
}
