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
using CellBig.Contents.Event;

namespace CellBig.Contents
{
    public class GameFireFighterContent : IFantaBoxContent
    {
        ObjectPool mGameObjPool = null;
        Coroutine Cor_GameLogic = null;

        GameModel gm;

        int CountRandomObject = 0;
        float maxPlayTime;
        float currentPlayTime;

        public GameFireFighterTruck Truck;
        public GameFireFighterTruckCam TruckCam;
        public GameFireFighterBuild Build;
        public GameFireFighterFire Fire;

        Coroutine m_pCor_Next = null;
        GameObject tempObj;
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
            string scenename = "GameFireFighter";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            // 게임 관련 리소스 로딩
            string prefabname = scenename + "/FireFighter_Fire";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mGameObjPool = OnPostLoadProcess(o, Random.Range(3, 15))));
            
            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            ObjectListOn();
            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.FireFighter);
            RequestContentEnter<GlobalContent>();
            RequestContentEnter<ReadyContent>();
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));

            Message.AddListener<GameFireFighterNextLevelMsg>(NextLevel);

            #region Initialize
            Truck = transform.GetChild(0).Find("FireTruck01").GetComponent<GameFireFighterTruck>();
            TruckCam = mainCamera.GetComponent<GameFireFighterTruckCam>();
            Build = transform.GetChild(0).Find("BG").GetComponent<GameFireFighterBuild>();
            Fire = transform.GetChild(0).Find("FireMng").GetComponent<GameFireFighterFire>();

            Fire.m_pViewCamera = mainCamera;

            mGameObjPool.transform.SetParent(Fire.transform);

            Truck.Enter();
            Build.Enter();
            Fire.SetupList();
            #endregion
        }

        protected override void OnExit()
        {
            AllDie();
            if (m_pCor_Next != null)
            {
                StopCoroutine(m_pCor_Next);
                m_pCor_Next = null;
            }
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
                yield return null;
            }
        }

        void NextLevel(GameFireFighterNextLevelMsg msg)
        {
            Fire.FireCount = 0;
            if (m_pCor_Next != null)
            {
                StopCoroutine(m_pCor_Next);
                m_pCor_Next = null;
            }
            //Fire.AllDie();
            m_pCor_Next = StartCoroutine(Cor_Next());
        }

        IEnumerator Cor_Next()
        {
            
            SoundManager.Instance.StopSound((int)SoundType_GameFX.FireFighter_Burning);
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.FireFighter_Siren);
            TruckCam.DeActive();
            Fire.AllDie();
            Truck.MoveNext(Build.GetNextTileTruckPos().transform.position.x);
            while (Truck.m_eState != GameFireFighterTruck.E_FireTruckState.E_IDLE)
            {
                Debug.LogError("ghighig");
                yield return null;
            }

            Build.SetNextTile();
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.FireFighter_Burning);
            Fire.CreateObject(Random.Range(3, 15));
            yield return new WaitForSeconds(1.0f);
            Build.MoveTile();

            m_pCor_Next = null;

            yield return null;
        }

        void AllDie()
        {
            for (int i = 0; i < mGameObjPool.transform.childCount; i++)
            { 
                mGameObjPool.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        protected override void OnHit(GameObject obj)
        {
            GameFireFighterFireObj obj_script = obj.transform.GetComponent<GameFireFighterFireObj>();
            if (obj_script != null)
            {
                tempObj = obj;
                Message.Send<ADDScore>(new ADDScore(100));
                obj_script.Hit();
            }
        }


        protected override void HitPoint(Vector3 hitPoint)
        {
            if (tempObj != null)
            {
                tempObj.GetComponent<GameFireFighterFireObj>().HitPoint(hitPoint);
                tempObj = null;
            }
        }

        protected override void OnEnd()
        {
            Fire.Destroy();
            //Build.Destroy();
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.FireFighter);
            SoundManager.Instance.StopSound((int)SoundType_GameFX.FireFighter_Burning);
            Message.RemoveListener<GameFireFighterNextLevelMsg>(NextLevel);
        }
    }
}
