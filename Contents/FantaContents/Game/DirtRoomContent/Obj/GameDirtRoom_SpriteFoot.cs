using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig;
using CellBig.Contents.Event;

using DG.Tweening;

public class GameDirtRoom_SpriteFoot : MonoBehaviour
{
    public SpriteRenderer spriteRen;
    public Sprite[] sprites;

    float liveTime = 3.0f;

    Coroutine fadeCor;

    private void OnEnable()
    {
        AddMessage();
        Active();
    }

    private void OnDisable()
    {
        RemoveMessage();
    }

    void AddMessage()
    {
        Message.AddListener<PoolObjectMsg>(OnPoolObjectMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<PoolObjectMsg>(OnPoolObjectMsg);
    }

    void Active()
    {
        spriteRen.DOFade(1.0f, 0.0f);
    }

    void OnPoolObjectMsg(PoolObjectMsg msg)
    {
        DeActive();
    }

    public void SetSpriteFoot(int footSpriteIndex, Color color)
    {
        spriteRen.sprite = sprites[footSpriteIndex];
        spriteRen.color = color;

        // 발자국 소리 첫번째 + 스프라이트 번호(0 ~ 9)
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.DirtRoom_Shoes + footSpriteIndex);

        fadeCor = StartCoroutine(SpriteFade());
    }

    public void SetSpriteFootRot(Quaternion quaternion)
    {
        transform.localRotation = quaternion;
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    IEnumerator SpriteFade()
    {
        yield return new WaitForSeconds(liveTime);

        spriteRen.DOFade(0.0f, 1.0f);

        yield return new WaitForSeconds(1.0f);

        DeActive();
    }

    void DeActive()
    {
        if (fadeCor != null)
            StopCoroutine(fadeCor);

        fadeCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)DirtRoom_FootType.SpriteFoot, gameObject));
    }
}
