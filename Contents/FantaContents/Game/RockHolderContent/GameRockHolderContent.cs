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

public enum RockType
{
    Rock_1,
    Rock_2,
}

namespace JHchoi.Contents
{
    public class GameRockHolderContent : IFantaBoxContent
    {
        GameRockHolder_ObjectControl gameRockHolder_ObjectControl;

        ObjectPool rock1_Pool;
        ObjectPool rock2_Pool;

        public List<GameRockHolder_Rock> rockList = new List<GameRockHolder_Rock>();

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
            string scenename = "GameRockHolder";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameRockHolder_ObjectControl = OnPostLoadProcess(o).GetComponent<GameRockHolder_ObjectControl>()));

            string prefabname = scenename + "/Object/Rock_1";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => rock1_Pool = OnPostLoadProcess(o, 6)));

            prefabname = scenename + "/Object/Rock_2";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => rock2_Pool = OnPostLoadProcess(o, 6)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.RockHolder);
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.RockHolder_Valley);
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
            Cor_GameLogic = StartCoroutine(Cor_PlayContent_RockHolder());
        }

        IEnumerator Cor_PlayContent_RockHolder()
        {
            CreateRock(4);

            while(true)
            {
                yield return new WaitForSeconds(5.0f);

                if(rockList.Count < 6)
                    CreateRock();

                yield return null;
            }
        }

        void CreateRock(int count = 1)
        {
            for (int index = 0; index < count; index++)
            {
                GameRockHolder_Rock tempRock = null;

                int randomRock = UnityEngine.Random.Range(0, Enum.GetNames(typeof(RockType)).Length);

                if (randomRock == 0)
                    tempRock = rock1_Pool.GetObject(rock1_Pool.transform).GetComponent<GameRockHolder_Rock>();
                else if (randomRock == 1)
                    tempRock = rock2_Pool.GetObject(rock2_Pool.transform).GetComponent<GameRockHolder_Rock>();

                rockList.Add(tempRock);

                gameRockHolder_ObjectControl.rockList = rockList;
                StartCoroutine(gameRockHolder_ObjectControl.SetSpawn(tempRock));
            }
        }

        protected override void OnHit(GameObject obj)
        {
            GameRockHolder_Rock tempRock = obj.GetComponent<GameRockHolder_Rock>();

            if (isDelayCheck)
            {
                contentDelayCheckCor = StartCoroutine(CheckDelay());

                if (tempRock != null)
                    tempRock.Hit();
            }
        }

        protected override void OnEnd()
        {
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.RockHolder);
            SoundManager.Instance.StopSound((int)SoundType_GameFX.RockHolder_Valley);

            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            if (msg.TypeIndex == (int)RockType.Rock_1)
                rock1_Pool.PoolObject(msg.myObject);
            else if (msg.TypeIndex == (int)RockType.Rock_1)
                rock2_Pool.PoolObject(msg.myObject);

            rockList.Remove(msg.myObject.GetComponent<GameRockHolder_Rock>());
        }
    }
}