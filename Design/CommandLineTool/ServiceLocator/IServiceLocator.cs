using System.Collections.Generic;

namespace CommandLineTool
{
    public interface IServiceLocator
    {
        TService Get<TService>();
        IEnumerable<TService> GetAll<TService>();
    }
}