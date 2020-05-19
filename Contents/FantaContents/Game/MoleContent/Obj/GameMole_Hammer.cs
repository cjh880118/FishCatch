using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi.Contents.Event;

using DG.Tweening;

public class GameMole_Hammer : MonoBehaviour
{
    public IEnumerator Hit(Vector3 pos)
    {
        // Hummer Down		
        transform.position = new Vector3(pos.x, 0, pos.z);

        // Impact
        Message.Send<ShakeCameraMsg>(new ShakeCameraMsg(0.1f, 0.3f, false, true));

        transform.DOMoveY(6.0f, 0.2f);

        yield return null;
    }
}
