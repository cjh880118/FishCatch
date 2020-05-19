using System.Collections.Generic;

namespace JHchoi.T3
{
    public class T3SensorCatchMsg : Message
    {
        public bool T3Catch = false;
        public T3SensorCatchMsg(bool t3Catch)
        {
            T3Catch = t3Catch;
        }
    }

    public class T3SensorReRequestMsg : Message
    {

    }

    public class T3SensorStartMsg : Message
    {
    }

    public class T3ResultMsg : Message
    {
        public List<T3SensorData> Datas = new List<T3SensorData>();
    }
}
