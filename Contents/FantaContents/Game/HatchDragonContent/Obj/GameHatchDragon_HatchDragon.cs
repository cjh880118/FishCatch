using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

public class GameHatchDragon_HatchDragon : MonoBehaviour
{
    public HatchDragonColorType dragonColor;

    public GameObject normalEgg;
    public GameObject crackedEgg;
    public GameObject bornDragon;

    public SkinnedMeshRenderer normalSkinRen;
    public Material crackedMat;

    Animator normalEggAnim;
    Animator crackedEggAnim;
    Animator bornDragonAnim;

    Transform directionTarget;

    public GameObject hitStarParticle;
    public GameObject hitHeartParticle;

    public ScoreTextControl scoreTextControl;

    Collider collider = null;

    public int spawnIndex;

    int maxHP = 3;
    int currentHP = 0;
    int score = 100;

    Vector3 distPos = new Vector3(10,0,0);
    Vector3 moveDirection;

    float moveDist;
    float moveSpeed = 0.025f;
    float rotMinX = 0.0f;
    float rotMaxX = -60.0f;
    float rotMinY = -60.0f;
    float rotMaxY = 60.0f;

    Coroutine hitEventCor;
    Coroutine moveCor;
    
    private void Awake()
    {
        directionTarget = transform;
        
        normalEggAnim = normalEgg.GetComponent<Animator>();
        crackedEggAnim = crackedEgg.GetComponent<Animator>();
        bornDragonAnim = bornDragon.GetComponent<Animator>();

        collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        Active();
        AddMessage();
    }

    private void OnDisable()
    {
        RemoveMessage();
    }

    void AddMessage()
    {
        Message.AddListener<PoolObjectMsg>(OnDeActiveHatchDragonMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<PoolObjectMsg>(OnDeActiveHatchDragonMsg);
    }

    void Active()
    {
        currentHP = maxHP;
        moveDist = 0.0f;
        moveDirection = Vector3.zero;

        collider.enabled = true;
        normalEgg.SetActive(true);

        StartCoroutine(LoopAnimation());
    }

    void OnDeActiveHatchDragonMsg(PoolObjectMsg msg)
    {
        DeActive();
    }

    void DeActive()
    {
        if(moveCor != null)
            StopCoroutine(moveCor);

        bornDragon.transform.localPosition = Vector3.zero;
        bornDragon.transform.localEulerAngles = Vector3.zero;

        crackedEgg.SetActive(false);
        bornDragon.SetActive(false);

        hitStarParticle.SetActive(false);
        hitHeartParticle.SetActive(false);

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)dragonColor, spawnIndex, gameObject));
    }

    public void SetSpawn(int index, Transform spawnPoint)
    {
        spawnIndex = index;
        transform.position = spawnPoint.transform.position;
    }

    IEnumerator LoopAnimation()
    {
        // 알 형태일때 까지.
        while(currentHP > 1)
        {
            yield return new WaitForSeconds(normalEggAnim.GetCurrentAnimatorStateInfo(0).length * 2);

            // 애니메이션 파라미터 이름 : IDLE1, IDLE2 라서 랜덤 범위를 1~2 설정하고 그 값을 string으로 변환해서 이름에 대입.
            int randomAnimTrigger = Random.Range(1, 3);
            normalEggAnim.SetTrigger("IDLE" + randomAnimTrigger);
        }
    }

    public void Hit()
    {
        if (hitEventCor != null)
            return;

        currentHP--;

        hitEventCor = StartCoroutine(HitEvent(currentHP));
    }

    IEnumerator HitEvent(int index)
    {
        // 모델, 애니메이션, 이동
        switch (index)
        {
            case 2:
                normalSkinRen.material = crackedMat;
                normalEggAnim.SetTrigger("HIT");

                hitStarParticle.SetActive(true);

                SoundManager.Instance.PlaySound((int)SoundType_GameFX.HatchDragon_EggCrack);
                break;
            case 1:
                normalEgg.SetActive(false);
                crackedEgg.SetActive(true);
                crackedEggAnim.SetTrigger("HIT");
                bornDragon.SetActive(true);

                hitStarParticle.SetActive(true);

                SoundManager.Instance.PlaySound((int)SoundType_GameFX.HatchDragon_EggCrack);
                break;

            case 0:
                moveCor = StartCoroutine(Move());
                bornDragonAnim.SetTrigger("HIT");

                collider.enabled = false;
                hitHeartParticle.SetActive(true);

                int randomSoundIndex = Random.Range(0, 2);

                if(randomSoundIndex == 0)
                    SoundManager.Instance.PlaySound((int)SoundType_GameFX.HatchDragon_DragonCry1);
                else
                    SoundManager.Instance.PlaySound((int)SoundType_GameFX.HatchDragon_DragonCry2);

                break;
        }

        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);

        // 다음 HIT까지 대기시간.
        yield return new WaitForSeconds(0.5f);

        if(hitStarParticle.activeSelf)
            hitStarParticle.SetActive(false);

        if (hitHeartParticle.activeSelf)
            hitHeartParticle.SetActive(false);

        hitEventCor = null;
    }

    IEnumerator Move()
    {
        SetDirection();

        while (true)
        {
            moveDist += moveSpeed;

            Vector3 currentTrans = bornDragon.transform.position;
            bornDragon.transform.position = currentTrans + moveDirection * moveSpeed;

            yield return null;

            if (distPos.magnitude < moveDist)
                break;
        }

        DeActive();
    }

    void SetDirection()
    {
        float rotRandomX = Random.Range(rotMinX, rotMaxX);
        float rotRandomY = Random.Range(rotMinY, rotMaxY);

        bornDragon.transform.localEulerAngles = new Vector3(rotRandomX, rotRandomY, 0);
        moveDirection = bornDragon.transform.forward;
    }
}
