using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Contents.Event
{
    public class PictureBubbleCreateMsg : Message
    {
        public Vector3 Position = Vector3.zero;
        public int XPoint = 0;
        public int YPoint = 0;
        public bool Boom = false;
        public PictureBubbleCreateMsg(Vector3 pos, int nX, int nY, bool boom)
        {
            Position = pos;
            XPoint = nX;
            YPoint = nY;
            Boom = boom;
        }
    }

    public class PictureBubbleAllDieMsg : Message{}

    public class PictureBubbleTagIn : Message { }
    public class PictureBubbleTagOut : Message { }
    public class PictureBubbleCreate : Message { }

    public class CurrentPictureNumMsg : Message
    {
        public int Num;
        public CurrentPictureNumMsg(int num)
        {
            Num = num;
        }
    }
}
