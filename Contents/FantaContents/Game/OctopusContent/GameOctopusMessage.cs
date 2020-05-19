using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Contents.Event
{
    public class OctopusCreateMsg : Message
    {
        public Vector3 Position = Vector3.zero;
        public OctopusCreateMsg(Vector3 pos)
        {
            Position = pos;
        }
    }

    public class OctopusShitCreateMsg : Message
    {
        public Vector3 Position = Vector3.zero;

        public OctopusShitCreateMsg(Vector3 pos)
        {
            Position = pos;
        }
    }
}
