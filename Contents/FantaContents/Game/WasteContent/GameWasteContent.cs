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

namespace CellBig.Contents
{
    public class GameWasteContent : IFantaBoxContent
    {
        ObjectPool mGameObjectPool = null;

        Coroutine Cor_GameLogic = null;

        GameModel gm;

        float maxPlayTime;
        float currentPlayTime;

        public GameWasteTrash Trash;
        float CurrentTimerUpdate;
        int CurrentCount;

        public bool AllStart;
        public float MaxTimerUpdate;
        public int SpawnSizeMin = 0;
        public int SpawnSizeMax = 1;

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
            string scenename = "GameWaste";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            // 슈팅게임 관련 리소스 로딩
            string prefabname = scenename + "/Trash_Prefab";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mGameObjectPool = OnPostLoadProcess(o, 50)));

            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            ObjectListOn();
            RequestContentEnter<GlobalContent>();
            RequestContentEnter<ReadyContent>();
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));
            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Octopus);

            Trash = transform.GetChild(0).Find("TrashMng").GetComponent<GameWasteTrash>();
            //TextCtrl = transform.GetChild(0).Find("Score").GetComponent<ScoreTextControl>();
            Trash.ObjectPool = mGameObjectPool;
            mGameObjectPool.transform.SetParent(Trash.transform);
            Trash.Setup();

            CurrentCount = 0;
            CurrentTimerUpdate = MaxTimerUpdate;
        }

        protected override void OnExit()
        {
            Trash.Destroy();
            ObjectListOff();
        }

        protected override void OnPlay()
        {
            Cor_GameLogic = StartCoroutine(Cor_PlayContent());
        }

        IEnumerator Cor_PlayContent()
        {
            while(true)
            {
                CurrentTimerUpdate -= 0.1f * Time.deltaTime;
                if (CurrentTimerUpdate < 0)
                {
                    Trash.Create();
                    CurrentTimerUpdate = MaxTimerUpdate;
                }
                //CurrentCount = Trash.ReturnCurrentObjList().Count;
                yield return null;
            }
        }

        GameObject tempObj;
        protected override void OnHit(GameObject obj)
        {
            if (obj.transform.parent.GetComponent<GameWasteTrashObj>() != null)
            {
                tempObj = obj;
                obj.transform.parent.GetComponent<GameWasteTrashObj>().Hit();
            }
        }

        protected override void HitPoint(Vector3 hitPoint)
        {
            if (tempObj != null)
            {
                if (tempObj.transform.parent.GetComponent<GameWasteTrashObj>() != null)
                {
                    tempObj.transform.parent.GetComponent<GameWasteTrashObj>().AfterHit(hitPoint);
                    tempObj = null;
                }
            }
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Octopus);

        }
    }
}