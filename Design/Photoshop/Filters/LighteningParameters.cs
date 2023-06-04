using System;

namespace MyPhotoshop
{
    public class LighteningParameters : IParameters
    {
        public double Coefficient { get; set; }

        public ParameterInfo[] GetDescription()
        {
            return new []
			{
				new ParameterInfo
				{
					Name = "Коэффициент",
					MaxValue = 10,
					MinValue = 0,
					Increment = 0.1,
					DefaultValue = 1
				}
			};
        }

        public void Parse(double[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (parameters.Length < 1)
                throw new ArgumentException($"Invalid length: {nameof(parameters)}");

            Coefficient = parameters[0];
        }
    }
}
