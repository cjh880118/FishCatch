﻿using System;
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
    public class GameWaterFishContent : IFantaBoxContent
    {
        GameObject gameWaterFish_World;

        ObjectPool footPool;
        GameWaterFish_Foot tempFoot = null;

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
            string scenename = "GameWaterFish";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                        gameWaterFish_World = OnPostLoadProcess(o)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.WaterFish);
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.WaterFish_Valley);
        }

        protected override void OnExit()
        {
            Message.Send<PoolObjectMsg>(new PoolObjectMsg());
            ObjectListOff();
            ReloadObject();
        }

        void ReloadObject()
        {
            ObjectList.Remove(gameWaterFish_World.gameObject);
            Destroy(gameWaterFish_World.gameObject);

            string scenename = "GameWaterFish";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                                    gameWaterFish_World = OnPostLoadProcess(o)));
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

        protected override void OnHit(GameObject obj)
        {
            if (isDelayCheck)
            {
                contentDelayCheckCor = StartCoroutine(CheckDelay());

                tempFoot = footPool.GetObject(footPool.transform).GetComponent<GameWaterFish_Foot>();

                if (tempFoot != null)
                    tempFoot.Hit();

            }
        }

        protected override void HitPoint(Vector3 hitPoint)
        {
            tempFoot.transform.position = hitPoint;
        }

        protected override void OnEnd()
        {
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.WaterFish);
            SoundManager.Instance.StopSound((int)SoundType_GameFX.WaterFish_Valley);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            footPool.PoolObject(msg.myObject);
        }
    }
}