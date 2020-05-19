using UnityEngine;
using JHchoi.Constants;
using System.Collections.Generic;

namespace JHchoi.Contents.Event
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