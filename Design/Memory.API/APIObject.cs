using System;

namespace Memory.API
{
    public class APIObject : IDisposable
    {
        private readonly int _id;
        private bool _disposed;

        public APIObject(int id)
        {
            _id = id;
            MagicAPI.Allocate(id);
        }

        ~APIObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                MagicAPI.Free(_id);
                _disposed = true;
            }
        }
    }
}
