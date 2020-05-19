using UnityEngine;
using JHchoi.Constants;
using System.Collections.Generic;

namespace JHchoi.Contents.Event
{
    public class BlockDataMsg : Message
    {
        public List<string> Data = new List<string>();
        public List<Vector3> DataVec = new List<Vector3>();
        public List<int> DataX = new List<int>();
        public List<int> DataY = new List<int>();
        public BlockDataMsg(List<string> data, List<Vector3> dataVec, List<int> dataX, List<int> dataY)
        {
            Data = data;
            DataVec = dataVec;
            DataX = dataX;
            DataY = dataY;
        }
    }

    public class FreezeBlockMsg : Message
    {
        public int X, Y;
        public FreezeBlockMsg(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}