using UnityEngine;
using CellBig.Constants;
using System.Collections.Generic;

namespace CellBig.Contents.Event
{
    public class StainFootMsg : Message
    {
        public bool IsStain;
        public DirtRoom_SpriteFootColor SpriteFootColor;

        public StainFootMsg(bool isStain, DirtRoom_SpriteFootColor spriteFootColor)
        {
            IsStain = isStain;
            SpriteFootColor = spriteFootColor;
        }
    }
}