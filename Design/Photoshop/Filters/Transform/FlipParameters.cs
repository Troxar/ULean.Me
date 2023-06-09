using System;

namespace MyPhotoshop
{
    public class FlipParameters : IParameters
    {
        public ParameterInfo[] GetDescription()
        {
            return Array.Empty<ParameterInfo>();
        }

        public void Parse(double[] parameters)
        {
            
        }
    }
}
