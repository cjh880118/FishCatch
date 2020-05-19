using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirtRoom_ObjectControl : MonoBehaviour
{
    public Color[] spriteFootColors;

    Vector3 currentSpriteFootPos = Vector3.zero;
    Vector3 tempSpriteFootPos = Vector3.zero;

    public void SetSpriteFoot(GameDirtRoom_SpriteFoot spriteFoot, int footSpriteIndex, DirtRoom_SpriteFootColor spriteFootColor)
    {
        currentSpriteFootPos = spriteFoot.transform.position;

        spriteFoot.SetSpriteFoot(footSpriteIndex, spriteFootColors[(int)spriteFootColor]);
        spriteFoot.SetSpriteFootRot(Quaternion.LookRotation(currentSpriteFootPos - tempSpriteFootPos));

        tempSpriteFootPos = currentSpriteFootPos;
    }
}
