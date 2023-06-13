using System;
using System.Collections.Generic;
using System.Text;

namespace Delegates.Observers
{
    public class StackOperationsLogger
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public void SubscribeOn<T>(ObservableStack<T> stack)
        {
            stack.Notify += HandleEvent;
        }

        public void HandleEvent(object eventData)
        {
            _sb.Append(eventData);
        }

        public string GetLog()
        {
            return _sb.ToString();
        }
    }

    public class ObservableStack<T>
    {
        private readonly List<T> data = new List<T>();
        public event Action<StackEventData<T>> Notify;

        public void Push(T obj)
        {
            data.Add(obj);
            Notify?.Invoke(new StackEventData<T> { IsPushed = true, Value = obj });
        }

        public T Pop()
        {
            if (data.Count == 0)
                throw new InvalidOperationException();
            var result = data[data.Count - 1];
            Notify?.Invoke(new StackEventData<T> { IsPushed = false, Value = result });
            return result;
        }
    }
}