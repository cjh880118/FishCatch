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

namespace CellBig.Contents
{
    public class GameRoadDestructionContent : IFantaBoxContent
    {
        GameRoadDestruction_ObjectControl gameRoadDestruction_ObjectControl;

        ObjectPool footPool;
        GameRoadDestruction_Foot tempFoot = null;

        GameModel gm;

        protected override void OnLoadStart()
        {
            gm = Model.First<GameModel>();
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;

            ObjectList = new List<GameObject>();
            string scenename = "GameRoadDestruction";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameRoadDestruction_ObjectControl = OnPostLoadProcess(o).GetComponent<GameRoadDestruction_ObjectControl>()));

            string prefabname = scenename + "/Object/Foot";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => footPool = OnPostLoadProcess(o, 20)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.RoadDestruction);
        }

        protected override void OnExit()
        {
            SoundManager.Instance.StopSound((int)SoundType_GameFX.RoadDestruction_TapleRe);

            Message.Send<PoolObjectMsg>(new PoolObjectMsg());

            ObjectListOff();
            ReloadObject();
        }

        void ReloadObject()
        {
            ObjectList.Remove(gameRoadDestruction_ObjectControl.gameObject);
            Destroy(gameRoadDestruction_ObjectControl.gameObject);

            string scenename = "GameRoadDestruction";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameRoadDestruction_ObjectControl = OnPostLoadProcess(o).GetComponent<GameRoadDestruction_ObjectControl>()));
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
            gameRoadDestruction_ObjectControl.GameStart();
        }

        protected override void OnHit(GameObject obj)
        {
            if (!gameRoadDestruction_ObjectControl.isReady && obj.GetComponent<FracturedChunk>() != null)
                return;

            if (tempFoot != null)
            {
                if (Vector3.Distance(tempFoot.transform.position, obj.transform.position) < 1)
                    return;
            }

            if (isDelayCheck)
            {
                contentDelayCheckCor = StartCoroutine(CheckDelay());

                tempFoot = footPool.GetObject(footPool.transform).GetComponent<GameRoadDestruction_Foot>();
                tempFoot.transform.position = obj.transform.position;

                if (tempFoot != null)
                        tempFoot.Hit(isDelayCheck);
            }

            gameRoadDestruction_ObjectControl.GetPercentage();
        }

        protected override void OnEnd()
        {
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.RoadDestruction);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            footPool.PoolObject(msg.myObject);
        }
    }
}