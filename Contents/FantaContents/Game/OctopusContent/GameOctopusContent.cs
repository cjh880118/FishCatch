using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public class GameOctopusContent : IFantaBoxContent
    {
        ObjectPool mGameObjPoolVase = null;
        ObjectPool mGameobjPoolOctopus = null;
        ObjectPool mGameObjPoolShit = null;

        Coroutine Cor_GameLogic = null;

        GameModel gm;

        float maxPlayTime;
        float currentPlayTime;

        public Transform VaseParent;
        public Transform OctopusParent;
        public Transform ShitParent;

        public GameOctopusShit Shit;
        public GameOctopusVase Vase;
        public GameOctopus Octopus;
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
            string scenename = "GameOctopus";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            // 슈팅게임 관련 리소스 로딩
            string prefabnameVase = scenename + "/Octopus_Vase";
            var fullpath_prefab_vase = string.Format("Prefab/{0}", prefabnameVase);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab_vase, o => mGameObjPoolVase = OnPostLoadProcess(o, 50)));
            
            string prefabnameOctopus = scenename + "/Octopus";
            var fullpath_prefab_octopus = string.Format("Prefab/{0}", prefabnameOctopus);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab_octopus, o => mGameobjPoolOctopus = OnPostLoadProcess(o, 50)));

            string prefabnameShit = scenename + "/Octopus_Shit";
            var fullpath_prefab_Shit = string.Format("Prefab/{0}", prefabnameShit);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab_Shit, o => mGameObjPoolShit = OnPostLoadProcess(o, 50)));

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
            #region Initialize
            VaseParent = transform.GetChild(0).Find("OctopusVase");
            OctopusParent = transform.GetChild(0).Find("Octopus");
            ShitParent = transform.GetChild(0).Find("OctopusShit");

            mGameObjPoolVase.transform.SetParent(VaseParent);
            mGameobjPoolOctopus.transform.SetParent(OctopusParent);
            mGameObjPoolShit.transform.SetParent(ShitParent);

            Shit = ShitParent.GetComponent<GameOctopusShit>();
            Vase = VaseParent.GetComponent<GameOctopusVase>();
            Octopus = OctopusParent.GetComponent<GameOctopus>();

            Shit.mCamera = mainCamera;
            Vase.mCamera = mainCamera;
            Octopus.mCamera = mainCamera;
            #endregion
        }

        protected override void OnExit()
        {
            maxPlayTime = gm.setting.Model.PlayTime;
            currentPlayTime = gm.setting.Model.PlayTime;

            Octopus.Destroy();
            Shit.Destroy();
            Vase.Destroy();
            StopCoroutine(Cor_PlayContent());

            ObjectListOff();
        }

        protected override void OnPlay()
        {
            Cor_GameLogic = StartCoroutine(Cor_PlayContent());

            Shit.Enter();
            Vase.Enter();
            Octopus.Enter();
        }

        IEnumerator Cor_PlayContent()
        {
            while (true)
            {
                Vase.CurrGameTime = currentPlayTime;
                Vase.MaxGameTime = maxPlayTime;
                yield return null;
            }
        }

        GameObject tempObj;
        protected override void OnHit(GameObject obj)
        {
            // octopus
            if (obj.transform.GetComponent<GameOctopusObj>() != null)
            {
                GameOctopusObj obj_Script_octopus = obj.transform.GetComponent<GameOctopusObj>();
                if (obj_Script_octopus != null)
                    obj_Script_octopus.Hit();
            }
            else if (obj.transform.GetComponent<GameOctopusVaseObj>() != null)
            {
                GameOctopusVaseObj obj_Script_vase = obj.transform.GetComponent<GameOctopusVaseObj>();
                if (obj_Script_vase != null && obj_Script_vase.m_pCollider.enabled == true)
                {
                    tempObj = obj;
                    obj_Script_vase.Die(true);
                }
            }
        }

        protected override void HitPoint(Vector3 hitPoint)
        {
            if (tempObj != null)
            {
                tempObj.GetComponent<GameOctopusVaseObj>().HitPoint(hitPoint);
                tempObj = null;
            }
        }

        protected override void OnEnd()
        {
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Octopus);
            
        }
    }
}
