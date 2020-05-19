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

public enum FallenObjectType
{
    Leaf,
    Foot,
}

namespace JHchoi.Contents
{
    public class GameFallenContent : IFantaBoxContent
    {
        GameFallen_ObjectControl gameFallen_ObjectControl;

        int leafCount = 1000;

        ObjectPool leafPool;
        ObjectPool footPool;
        GameFallen_Foot tempFoot = null;

        List<GameFallen_Leaf> leavesList = new List<GameFallen_Leaf>();

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
            string scenename = "GameFallen";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o =>
                                                gameFallen_ObjectControl = OnPostLoadProcess(o).GetComponent<GameFallen_ObjectControl>()));

            string prefabname = scenename + "/Object/Leaf";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => leafPool = OnPostLoadProcess(o, leafCount)));

            prefabname = scenename + "/Object/Foot";
            fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => footPool = OnPostLoadProcess(o, 3)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Fallen);

            Cor_GameLogic = StartCoroutine(CreateLeaves());
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

        IEnumerator CreateLeaves()
        {
            Vector3 pPos = Vector3.zero;

            for (int i = 0; i < leafCount; i++)
            {
                GameFallen_Leaf tempLeaf = leafPool.GetObject(leafPool.transform).GetComponent<GameFallen_Leaf>();
                tempLeaf.transform.localPosition = Vector3.zero;
                leavesList.Add(tempLeaf);
                gameFallen_ObjectControl.leavesList = leavesList;
                StartCoroutine(gameFallen_ObjectControl.SetSpawn(tempLeaf));
            }

            yield return null;
        }
        
        protected override void OnHit(GameObject obj)
        {
            if (tempFoot != null)
            {
                if (Vector3.Distance(tempFoot.transform.position, obj.transform.position) < 1)
                    return;
            }

            if (isDelayCheck)
            {
                contentDelayCheckCor = StartCoroutine(CheckDelay());

                tempFoot = footPool.GetObject(footPool.transform).GetComponent<GameFallen_Foot>();
                tempFoot.transform.position = obj.transform.position;
                tempFoot.Hit();

                Message.Send<ADDScore>(new ADDScore(100));
            }
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;
            leavesList.Clear();

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Fallen);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            if(msg.TypeIndex == (int)FallenObjectType.Leaf)
                leafPool.PoolObject(msg.myObject);
            else if (msg.TypeIndex == (int)FallenObjectType.Foot)
                footPool.PoolObject(msg.myObject);
        }
    }
}