using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSlime_SpriteAni : MonoBehaviour
{


    [SerializeField]
    private string nowPlaying;

    [SerializeField]
    private List<SpriteChunk> animationList;

    //  [SerializeField]
    //private Image targetImage;

    private string beforePlaying = "beforePlaying";
    private string tempPlaying = "tempPlaying";
    private SpriteChunk nowPlay;

    private float time = 0f;
    private float delayTime = 0f;
    private int frame = 0;
    private bool loop = false;

    [SerializeField]
    internal SpriteRenderer m_pSprite = null;


    bool m_bMaxFrameOnce = false;
    void Start()
    {
        //   if (targetImage == null)
        //       targetImage = GetComponent<Image>();

        if (m_pSprite == null)
        {

            m_pSprite = transform.GetComponent<SpriteRenderer>();
        }


        // m_pSprite.sprite.pivot = 
    }

    private void OnDestroy()
    {
        m_pSprite = null;
        if (animationList != null)
        {
            animationList.Clear();
            animationList = null;
        }
        if (nowPlay != null)
        {
            nowPlay.Destroy();
        }
        nowPlay = null;
    }

    public bool GetMaxFrameOnce()
    {
        if (nowPlay.GetAnimationFrame(frame) == null) return true;
        return false;
    }

    void Update()
    {
        if (tempPlaying.Equals(nowPlaying))
        {
            time += Time.deltaTime;

            if (time >= nowPlay.DelayTime)
            {
                Sprite sprite = nowPlay.GetAnimationFrame(frame);

                if (sprite == null)
                {
                    if (loop == false)
                    {
                        return;
                    }
                    else
                        frame = 0;
                }
                else
                {
                    frame++;
                    m_pSprite.sprite = sprite;
                    //  m_pSprite.
                    //  targetImage.sprite = sprite;
                    //  targetImage.SetNativeSize();
                }
                Reset();
            }
        }
        else
        {
            for (int i = 0; i < animationList.Count; i++)
            {
                if (animationList[i].AnimationName.Equals(nowPlaying))
                {
                    if (nowPlay != null) nowPlay.IsPlaying = false;
                    nowPlay = animationList[i];
                    nowPlay.IsPlaying = true;
                    Reset();
                    tempPlaying = nowPlaying;
                    break;
                }
            }
        }
    }

    public void Reset()
    {
        time = 0f;
        delayTime = nowPlay.DelayTime;
        loop = nowPlay.Loop;
    }

    /// <summary>
    /// 현재 플레이 리스트를 바꿉니다.
    /// </summary>
    /// <param name="name"></param>
    public void ChangePlaying(string name)
    {
        time = 10000.0f;
        beforePlaying = nowPlaying;
        nowPlaying = name;
        frame = 0;
    }

    public int GetCurrFrame()
    {
        return frame;
    }

    /// <summary>
    /// 전체 애니메이션을 바꿉니다.
    /// </summary>
    /// <param name="chunk"></param>
    public void ChangeAnimation(List<SpriteChunk> chunk)
    {
        animationList = chunk;
    }


    public int GetMaxAnimationList()
    {
        return animationList.Count;
    }

    public void SetFrame(int nFrame)
    {
        frame = nFrame;
        time = 30000.0f;
    }


    public int GetAnimation_ListNum()
    {
        return animationList.Count;
    }
}
