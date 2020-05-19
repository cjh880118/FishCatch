using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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

public enum SnowObjectType
{
    Snow,
    Foot,
}

namespace CellBig.Contents
{
    public class GameSlideSnowContent : IFantaBoxContent
    {
        GameSlideSnow_ObjectControl gameSlideSnow_ObjectControl;

        int snowCount = 400;
        ObjectPool snowPool;
        ObjectPool footPool;
        GameSlideSnow_Foot tempFoot = null;

        List<GameSlideSnow_Snow> snowsList = new List<GameSlideSnow_Snow>();

        GameModel gm;

        Coroutine Cor_GameLogic;

        protected override void OnLoadStart()
        {
            gm = Model.First<GameModel>();
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;

            ObjectList = new List<GameObject>();
            string scenename = "GameSlideSnow";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameSlideSnow_ObjectControl = OnPostLoadProcess(o).GetComponent<GameSlideSnow_ObjectControl>()));

            string prefabname = scenename + "/Object/Snow";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => snowPool = OnPostLoadProcess(o, snowCount)));

            prefabname = scenename + "/Object/Foot";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.SlideSnow);

            Cor_GameLogic = StartCoroutine(CreateSnows());
        }

        protected override void OnExit()
        {
            Message.Send<PoolObjectMsg>(new PoolObjectMsg());
            ObjectListOff();
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
        }

        IEnumerator CreateSnows()
        {
            Vector3 pPos = Vector3.zero;

            for (int i = 0; i < snowCount; i++)
            {
                GameSlideSnow_Snow tempSnow = snowPool.GetObject(snowPool.transform).GetComponent<GameSlideSnow_Snow>();
                tempSnow.transform.localPosition = Vector3.zero;
                snowsList.Add(tempSnow);
                gameSlideSnow_ObjectControl.snowsList = snowsList;
                StartCoroutine(gameSlideSnow_ObjectControl.SetSpawn(tempSnow));
            }

            yield return null;
        }

        protected override void OnHit(GameObject obj)
        {
            if (tempFoot != null)
            {
                if (Vector3.Distance(tempFoot.transform.position, obj.transform.position) < 0.25f)
                    return;
            }

            if (isDelayCheck)
            {
                contentDelayCheckCor = StartCoroutine(CheckDelay());

                tempFoot = footPool.GetObject(footPool.transform).GetComponent<GameSlideSnow_Foot>();
                tempFoot.transform.position = obj.transform.position;
                tempFoot.Hit();

            }
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;
            snowsList.Clear();

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.SlideSnow);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            if (msg.TypeIndex == (int)SnowObjectType.Snow)
                snowPool.PoolObject(msg.myObject);
            else if (msg.TypeIndex == (int)SnowObjectType.Foot)
                footPool.PoolObject(msg.myObject);
        }
    }
}