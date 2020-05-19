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

public enum AirBorneType
{
    Cloud,
    Planet,
}

namespace JHchoi.Contents
{
    public class GameSpaceJumpContent : IFantaBoxContent
    {
        GameSpaceJump_ObjectControl gameSpaceJump_ObjectControl;

        ObjectPool cloudPool;
        ObjectPool planetPool;

        public GameSpaceJump_AirBorne currentAirBorne;

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
            string scenename = "GameSpaceJump";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameSpaceJump_ObjectControl = OnPostLoadProcess(o).GetComponent<GameSpaceJump_ObjectControl>()));

            string prefabname = scenename + "/Object/Cloud";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => cloudPool = OnPostLoadProcess(o, 5)));

            prefabname = scenename + "/Object/Planet";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => planetPool = OnPostLoadProcess(o, 5)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.SpaceJump);
        }

        protected override void OnExit()
        {
            gameSpaceJump_ObjectControl.StopAllSpaceSound();

            Message.Send<PoolObjectMsg>(new PoolObjectMsg());

            ObjectListOff();
        }

        protected override void OnAddMessage()
        {
            Message.AddListener<AirBorneInitMsg>(OnAirBorneInitMsg);
            Message.AddListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnRemoveMessage()
        {
            Message.RemoveListener<AirBorneInitMsg>(OnAirBorneInitMsg);
            Message.RemoveListener<Event.GameObjectDeActiveMessage>(OnDeactive);
        }

        protected override void OnPlay()
        {
            Message.Send<MultiTouchMsg>(new MultiTouchMsg());

            gameSpaceJump_ObjectControl.GameStart();

            Cor_GameLogic = StartCoroutine(CreateAirBorne());
        }

        IEnumerator CreateAirBorne()
        {
            while(true)
            {
                while(!gameSpaceJump_ObjectControl.isReady)
                    yield return null;

                if (gameSpaceJump_ObjectControl.currentPointX < 3.3f)
                    currentAirBorne = cloudPool.GetObject(cloudPool.transform).GetComponent<GameSpaceJump_AirBorne>();
                else
                    currentAirBorne = planetPool.GetObject(planetPool.transform).GetComponent<GameSpaceJump_AirBorne>();

                StartCoroutine(gameSpaceJump_ObjectControl.SetSpawn(currentAirBorne));

                while (currentAirBorne != null)
                    yield return null;

                yield return null;
            }
        }

        protected override void OnHit(GameObject obj)
        {
            GameSpaceJump_AirBorne obj_script = obj.transform.GetComponent<GameSpaceJump_AirBorne>();

            if (obj_script != null)
                obj_script.Hit();

            gameSpaceJump_ObjectControl.MoveUp();
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.SpaceJump);
            gameSpaceJump_ObjectControl.StopAllSpaceSound();
        }

        void OnAirBorneInitMsg(AirBorneInitMsg msg)
        {
            currentAirBorne = null;
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            if(msg.TypeIndex == (int)AirBorneType.Cloud)
                cloudPool.PoolObject(msg.myObject);
            else if (msg.TypeIndex == (int)AirBorneType.Planet)
                planetPool.PoolObject(msg.myObject);
        }
    }
}