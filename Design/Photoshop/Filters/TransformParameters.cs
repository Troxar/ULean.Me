using System;

namespace MyPhotoshop
{
    public class TransformParameters : IParameters
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
