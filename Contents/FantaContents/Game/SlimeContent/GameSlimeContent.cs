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
    public class GameSlimeContent : IFantaBoxContent
    {
        ObjectPool mGameObjPool = null;

        Coroutine mCor_GameLogic = null;

        GameModel gm;

        float maxPlayTime;
        float currentPlayTime;
        GameObject tempObj;

        public GameSlimeSlime Slime;

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
            ObjectList = new List<GameObject>();
            string scenename = "GameSlime";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            #region resource load
            string prefabnameBlue = scenename + "/Virus_Blue";
            var fullpath_prefabBlue = string.Format("Prefab/{0}", prefabnameBlue);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefabBlue, o => mGameObjPool = OnPostLoadProcess(o, 20)));

            string prefabnameGreen = scenename + "/Virus_Green";
            var fullpath_prefabGreen = string.Format("Prefab/{0}", prefabnameGreen);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefabGreen, o => mGameObjPool = OnPostLoadProcess(o, 20)));

            string prefabnameOrange = scenename + "/Virus_Orange";
            var fullpath_prefabOrange = string.Format("Prefab/{0}", prefabnameOrange);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefabOrange, o => mGameObjPool = OnPostLoadProcess(o, 20)));

            string prefabnameRed = scenename + "/Virus_Red";
            var fullpath_prefabRed = string.Format("Prefab/{0}", prefabnameRed);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefabRed, o => mGameObjPool = OnPostLoadProcess(o, 20)));

            string prefabnameViolet = scenename + "/Virus_Violet";
            var fullpath_prefabViolet = string.Format("Prefab/{0}", prefabnameViolet);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefabViolet, o => mGameObjPool = OnPostLoadProcess(o, 20)));

            string prefabnameYellow = scenename + "/Virus_Yellow";
            var fullpath_prefabYellow = string.Format("Prefab/{0}", prefabnameYellow);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefabYellow, o => mGameObjPool = OnPostLoadProcess(o, 20)));
            #endregion

            SetLoadComplete();
        }

        void SetParentSlime(ObjectPool objPool)
        {
            objPool.transform.SetParent(transform.GetChild(0).Find("SlimeMng").transform);
        }

        protected override void OnEnter()
        {
            ObjectListOn();
            RequestContentEnter<GlobalContent>();
            RequestContentEnter<ReadyContent>();
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));
            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Slime);
            Slime = transform.GetChild(0).Find("SlimeMng").GetComponent<GameSlimeSlime>();
            //TextCtrl = transform.GetChild(0).Find("Score").GetComponent<ScoreTextControl>();

            Slime.Enter();

        }

        protected override void OnExit()
        {   
            ObjectListOff();
        }

       
        protected override void OnPlay()
        {
            mCor_GameLogic = StartCoroutine(Cor_PlayContent_Slime());
        }

        IEnumerator Cor_PlayContent_Slime()
        {
            Slime.CurrTime = currentPlayTime;
            Slime.MaxTime = maxPlayTime;
            while (true)
            {
                yield return null;
            }
        }


        protected override void OnEnd()
        {
            StopCoroutine(Cor_PlayContent_Slime());
            mCor_GameLogic = null;
            Slime.DestroySlime();
            Slime.PoolObject.Clear();
            //Slime.DestroySlime();
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Slime);
        }

        protected override void OnHit(GameObject obj)
        {
            GameSlimeSlimeObj obj_script = obj.transform.GetComponent<GameSlimeSlimeObj>();
            if (obj_script != null)
            {
                tempObj = obj;
                obj_script.Die();
                Message.Send<UI.Event.ADDScore>(new ADDScore(100));
            }
        }

        
        protected override void HitPoint(Vector3 hitPoint)
        {
            if (tempObj != null)
            {
                tempObj.GetComponent<GameSlimeSlimeObj>().HitPoint(hitPoint);
                tempObj = null;
            }
        }
    }
}
