using System;

namespace CellBig.Module.Detection
{
    public abstract class OutputMessageBase<T> : Message
    {
        public T Value;
    }
    
    public class SaveSettings : Message
    {

    }

    public class DetectionEvent : Message
    {
        public enum EventType
        {
            FileNotFound,
            FileDataMismatch
        }

        public DetectionEvent(EventType type, object data, string message)
        {
            Type = type;
            Data = data;
            Message = message;
        }

        public EventType Type { get; }
        public object Data { get; }
        public string Message { get; }
    }
}