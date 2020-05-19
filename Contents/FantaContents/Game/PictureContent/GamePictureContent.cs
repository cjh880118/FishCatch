using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using CellBig.Models;
using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;
using CellBig.Common;
using CellBig.Module;
using CellBig.Constants;

using DG.Tweening;
using Rect = UnityEngine.Rect;
using OpenCVForUnity;

namespace CellBig.Contents {
    public class GamePictureContent : IFantaBoxContent
    {
        ObjectPool mGameObjPool = null;
        Coroutine Cor_GameLogic = null;

        GameModel gm;

        float maxPlayTime;
        float currentPlayTime;

        GamePictureBubble Bubble;
        GamePictureBubble Bubble1;
        GamePicture Picture;

        protected override void OnLoadStart()
        {
            gm = Model.First<GameModel>();

            StartCoroutine(Cor_Load());

            maxPlayTime = gm.setting.Model.PlayTime;
            currentPlayTime = gm.setting.Model.PlayTime;
        }

        IEnumerator Cor_Load()
        {
            yield return null;
            // 슈팅게임 씬 로드
            ObjectList = new List<GameObject>();
            string scenename = "GamePicture";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            // 슈팅게임 관련 리소스 로딩
            string prefabname = scenename + "/Bubble";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mGameObjPool = OnPostLoadProcess(o, 40)));

            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            ObjectListOn();
            RequestContentEnter<GlobalContent>();
            RequestContentEnter<ReadyContent>();
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));
            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Picture);

            Bubble = transform.GetChild(0).Find("PictureMng").GetChild(0).Find("BubbleMng").GetComponent<GamePictureBubble>();
            Bubble1 = transform.GetChild(0).Find("PictureMng").GetChild(1).Find("BubbleMng").GetComponent<GamePictureBubble>();
            Picture = transform.GetChild(0).Find("PictureMng").GetComponent<GamePicture>();

            for (int i = 0; i < mGameObjPool.transform.childCount; i++)
            {
                Bubble.BubbleList.Add(mGameObjPool.transform.GetChild(i).gameObject);
                Bubble1.BubbleList.Add(mGameObjPool.transform.GetChild(i).gameObject);
            }

            mGameObjPool.transform.SetParent(this.transform);
            mGameObjPool.transform.localPosition = Vector3.zero;
            Picture.Enter();
        }

        protected override void OnExit()
        {
            Bubble.BubbleList1.Clear();
            Bubble1.BubbleList1.Clear();
            ObjectListOff();
        }

        protected override void OnPlay()
        {
            Cor_GameLogic = StartCoroutine(Cor_PlayContent());
        }

        protected override void OnAddMessage()
        {
            Message.AddListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnRemoveMessage()
        {
            Message.RemoveListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        IEnumerator Cor_PlayContent()
        {
            while (true)
            {
                yield return null;
            }
        }

        protected override void OnHit(GameObject obj)
        {
            GamePictureBubbleObj obj_sript = obj.transform.GetComponent<GamePictureBubbleObj>();

            if (obj_sript != null)
                obj_sript.Hit();

        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_PlayContent());
            Cor_GameLogic = null;
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Picture);
        }


        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            if (msg.TypeIndex == 0)
            {
                Bubble.BubbleList1.Remove(msg.myObject);
                if (Bubble.BubbleList1.Count <= 0)
                {
                    mGameObjPool.transform.parent = Bubble1.transform;
                    mGameObjPool.transform.localPosition = Vector3.zero;
                    Bubble.Next();
                }
            }
            else
            {
                Bubble1.BubbleList1.Remove(msg.myObject);
                if (Bubble1.BubbleList1.Count <= 0)
                {
                    mGameObjPool.transform.parent = Bubble.transform;
                    mGameObjPool.transform.localPosition = Vector3.zero;
                    Bubble1.Next();
                }
            }
            Picture.m_pPicture[msg.TypeIndex].IceOff();
        }
    }
}
