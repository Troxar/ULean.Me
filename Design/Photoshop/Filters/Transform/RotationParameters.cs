using System;

namespace MyPhotoshop
{
    public class RotationParameters : IParameters
    {
        public double Angle { get; set; }

        public ParameterInfo[] GetDescription()
        {
            return new[]
            {
                new ParameterInfo
                {
                    Name = "Угол",
                    MaxValue = 360,
                    MinValue = 0,
                    Increment = 5,
                    DefaultValue = 0
                }
            };
        }

        public void Parse(double[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (parameters.Length < 1)
                throw new ArgumentException($"Invalid length: {nameof(parameters)}");

            Angle = parameters[0];
        }
    }
}