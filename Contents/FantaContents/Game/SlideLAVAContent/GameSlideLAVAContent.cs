using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using JHchoi.Models;
using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;
using JHchoi.Common;
using JHchoi.Module;
using JHchoi.Constants;

using DG.Tweening;
using Rect = UnityEngine.Rect;
using OpenCVForUnity;

namespace JHchoi.Contents
{
    public class GameSlideLAVAContent : IFantaBoxContent
    {
        GameSlideLAVA_ObjectControl gameSlideLAVA_ObjectControl;

        ObjectPool footPool;
        GameSlideLAVA_Foot tempFoot = null;

        GameModel gm;

        Coroutine delayCheckCor;

        bool isDelayCheck = true;

        protected override void OnLoadStart()
        {
            gm = Model.First<GameModel>();
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;

            ObjectList = new List<GameObject>();
            string scenename = "GameSlideLAVA";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameSlideLAVA_ObjectControl = OnPostLoadProcess(o).GetComponent<GameSlideLAVA_ObjectControl>()));

            string prefabname = scenename + "/Object/Foot";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => footPool = OnPostLoadProcess(o, 10)));

            SetLoadComplete();
        }
        protected override void OnEnter()
        {
            // 게임 시작시 Ready 연출을 시작해 줍니다.
            ObjectListOn();

            RequestContentEnter<GlobalContent>();
            RequestContentEnter<ReadyContent>();

            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));

            Message.Send<PoolObjectMsg>(new PoolObjectMsg());

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.SlideLAVA);
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.SlideLAVA_Sizzle);
        }

        protected override void OnExit()
        {
            SoundManager.Instance.StopSound((int)SoundType_GameFX.SlideLAVA_TapleRe);
            Message.Send<PoolObjectMsg>(new PoolObjectMsg());

            ObjectListOff();
            ReloadObject();
        }

        void ReloadObject()
        {
            ObjectList.Remove(gameSlideLAVA_ObjectControl.gameObject);
            Destroy(gameSlideLAVA_ObjectControl.gameObject);

            string scenename = "GameSlideLAVA";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameSlideLAVA_ObjectControl = OnPostLoadProcess(o).GetComponent<GameSlideLAVA_ObjectControl>()));
        }

        protected override void OnAddMessage()
        {
            Message.AddListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnRemoveMessage()
        {
            Message.RemoveListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnPlay()
        {
            Message.Send<MultiTouchMsg>(new MultiTouchMsg());
            gameSlideLAVA_ObjectControl.GameStart();
        }

        protected override void OnHit(GameObject obj)
        {
            if (!gameSlideLAVA_ObjectControl.isReady && obj.GetComponent<FracturedChunk>() != null)
                return;

            if (tempFoot != null)
            {
                if (Vector3.Distance(tempFoot.transform.position, obj.transform.position) < 1)
                    return;
            }

            if (isDelayCheck)
            {
                contentDelayCheckCor = StartCoroutine(CheckDelay());

                tempFoot = footPool.GetObject(footPool.transform).GetComponent<GameSlideLAVA_Foot>();
                tempFoot.transform.position = obj.transform.position;

                if (tempFoot != null)
                    tempFoot.Hit();
            }

            gameSlideLAVA_ObjectControl.GetPercentage();
        }

        protected override void OnEnd()
        {
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.SlideLAVA);
            SoundManager.Instance.StopSound((int)SoundType_GameFX.SlideLAVA_Sizzle);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            footPool.PoolObject(msg.myObject);
        }
    }
}