using System;
using System.Collections.Generic;
using System.Linq;

namespace Incapsulation.Failures
{
    public class ReportMaker
    {
        /// <summary>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="failureTypes">
        /// 0 for unexpected shutdown, 
        /// 1 for short non-responding, 
        /// 2 for hardware failures, 
        /// 3 for connection problems
        /// </param>
        /// <param name="deviceId"></param>
        /// <param name="times"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes, 
            int[] deviceId, 
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            return FindDevicesFailedBeforeDate(
                new DateTime(year, month, day),
                ToFailures(deviceId, ToDevices(devices), failureTypes, times));
        }
        
        private static List<string> FindDevicesFailedBeforeDate(DateTime date, Failure[] failures)
        {
            return failures
                .Where(x => x.Date < date && x.FailureType.IsSerious())
                .Select(x => x.Device.Name)
                .ToList();
        }

        private static Failure[] ToFailures(int[] deviceIds, Dictionary<int, Device> devices, int[] failureTypes, object[][] times)
        {
            if (deviceIds is null) 
                throw new ArgumentNullException(nameof(deviceIds));
            if (failureTypes is null)
                throw new ArgumentNullException(nameof(failureTypes));
            if (times is null) 
                throw new ArgumentNullException(nameof(times));
            
            if (failureTypes.Length != deviceIds.Length) 
                throw new FormatException($"Invalid length: {nameof(failureTypes)}");
            if (times.Length != deviceIds.Length)
                throw new FormatException($"Invalid length: {nameof(times)}");
            

            var failures = new Failure[deviceIds.Length];
            for (int i = 0; i < failures.Length; i++)
            {
                if (!devices.ContainsKey(deviceIds[i]))
                    throw new ArgumentOutOfRangeException($"{nameof(deviceIds)}[{i}]");
                if (!Enum.IsDefined(typeof(FailureType), failureTypes[i]))
                    throw new ArgumentOutOfRangeException($"{nameof(failureTypes)}[{i}]");
                failures[i] = new Failure(devices[deviceIds[i]], (FailureType)failureTypes[i], ToDate(times[i]));
            }
            
            return failures;
        }

        private static DateTime ToDate(object[] time)
        {
            if (time is null)
                throw new NullReferenceException(nameof(time));
            if (time.Length < 3)
                throw new FormatException($"Invalid length: {nameof(time)}");


            if (!(time[0] is int day))
                throw new InvalidCastException($"{nameof(time)}[0] to int");
            if (!(time[1] is int month))
                throw new InvalidCastException($"{nameof(time)}[1] to int");
            if (!(time[2] is int year))
                throw new InvalidCastException($"{nameof(time)}[2] to int");

            return new DateTime(year, month, day);
        }

        private static Dictionary<int, Device> ToDevices(List<Dictionary<string, object>> list)
        {
            return list.Select(x => ToDevice(x))
                .ToDictionary(x => x.Id, x => x);
        }

        private static Device ToDevice(Dictionary<string, object> dict)
        {
            if (!dict.Keys.Contains("DeviceId"))
                throw new FormatException($"{nameof(dict)} does not contain key <DeviceId>");
            if (!dict.Keys.Contains("Name"))
                throw new FormatException($"{nameof(dict)} does not contain key <Name>");

            if (!(dict["DeviceId"] is int id))
                throw new InvalidCastException($"{nameof(dict)}[<DeviceId>] to int");
            if (!(dict["Name"] is string name))
                throw new InvalidCastException($"{nameof(dict)}[<Name>] to string");

            return new Device(id, name);
        }
    }

    internal enum FailureType
    {
        UnexpectedShutdown = 0,
        ShortNonResponding = 1,
        HardwareFailure = 2,
        ConnectionsProblems = 3
    }

    internal static class FailureTypeExtensions
    {
        public static bool IsSerious(this FailureType failureType)
        {
            return (int)failureType % 2 == 0;
        }
    }

    internal class Failure
    {
        public readonly Device Device;
        public readonly FailureType FailureType;
        public readonly DateTime Date;
        
        public Failure(Device device, FailureType failureType, DateTime date)
        {
            Device = device;
            FailureType = failureType;
            Date = date;
        }
    }

    internal class Device
    {
        public readonly int Id;
        public readonly string Name;
        
        public Device(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Device;
            if (item is null) return false;
            return Id.Equals(item.Id);
        }
    }
}