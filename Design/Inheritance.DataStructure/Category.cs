using System;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly MessageType _type;
        public MessageType Type
        {
            get { return _type; }
        }

        private readonly MessageTopic _topic;
        public MessageTopic Topic
        {
            get { return _topic; }
        }

        public Category(string name, MessageType type, MessageTopic topic)
        {
            _name = name;
            _type = type;
            _topic = topic;
        }

        public override string ToString()
        {
            return $"{_name}.{_type}.{_topic}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 1430287;
                result = (7302013 * result) ^ _name?.GetHashCode() ?? base.GetHashCode();
                result = (7302013 * result) ^ _type.GetHashCode();
                result = (7302013 * result) ^ _topic.GetHashCode();
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Category);
        }

        public bool Equals(Category other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetHashCode() != other.GetHashCode()) return false;
            return Name == other.Name
                && Type == other.Type
                && Topic == other.Topic;
        }

        public int CompareTo(object obj)
        {
            var other = obj as Category;
            if (other is null) return 1;

            int nameComparison = string.Compare(_name, other._name);
            if (nameComparison != 0) return nameComparison;

            int typeComparison = _type.CompareTo(other._type);
            if (typeComparison != 0) return typeComparison;

            return _topic.CompareTo(other._topic);
        }

        public static bool operator <(Category left, Category right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Category left, Category right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Category left, Category right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Category left, Category right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}