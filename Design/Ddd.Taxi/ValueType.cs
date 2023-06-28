using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ddd.Infrastructure
{
    /// <summary>
    /// Базовый класс для всех Value типов.
    /// </summary>
    public class ValueType<T>
    {
        private static string _typeName;
        private static IEnumerable<PropertyInfo> _publicProperties;

        static ValueType()
        {
            Type type = typeof(T);
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            _publicProperties = type
                .GetProperties(bindingFlags)
                .OrderBy(prop => prop.Name);
            _typeName = type.Name;
        }

        protected bool Equals(ValueType<T> other)
        {
            return _publicProperties
                .All(prop =>
                {
                    var value1 = prop.GetValue(this);
                    var value2 = prop.GetValue(other);
                    return value1?.Equals(value2) ?? value1 == value2;
                });
        }

        public bool Equals(T other)
        {
            return (other as ValueType<T>)?.Equals(this) ?? false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValueType<T>)obj);
        }

        public override int GetHashCode()
        {
            int seed = 397;
            int hash = 0;
            
            foreach (var prop in _publicProperties)
                hash = unchecked((hash ^ prop.GetValue(this).GetHashCode()) * seed);

            return hash;
        }

        public override string ToString()
        {
            var props = string.Join("; ", _publicProperties
                .Select(prop => $"{prop.Name}: {prop.GetValue(this)}"));
            return $"{_typeName}({props})";
        }
    }
}