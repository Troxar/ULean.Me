using System;
using System.Collections.Generic;
using System.Text;

namespace Delegates.Observers
{
    public class StackOperationsLogger
	{
        private readonly IObserver observer = new Observer();
        
        public void SubscribeOn<T>(ObservableStack<T> stack)
		{
			stack.Notify += observer.HandleEvent;
		}

        public string GetLog()
		{
			return observer.Log.ToString();
		}
	}

	public interface IObserver
	{
		string Log { get; }
        void HandleEvent(object eventData);
	}

	public class Observer : IObserver
	{
		private readonly StringBuilder _sb = new StringBuilder();
        public string Log => _sb.ToString();

		public void HandleEvent(object eventData)
		{
			_sb.Append(eventData);
		}
	}

    public class ObservableStack<T>
    {
		public event Action<StackEventData<T>> Notify;

		private readonly List<T> data = new List<T>();

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