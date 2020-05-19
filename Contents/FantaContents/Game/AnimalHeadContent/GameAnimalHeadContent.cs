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
    public class GameAnimalHeadContent : IFantaBoxContent
    {
        ObjectPool mGameObjPool = null;

        Coroutine Cor_GameLogic = null;
        //Coroutine Cor_InputMouse = null;

        GameModel gm;

        float maxPlayTime;
        float currentPlayTime;

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
            string scenename = "GameAnimalHead";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            // 슈팅게임 관련 리소스 로딩
            string prefabname = scenename + "/AnimalHead";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mGameObjPool = OnPostLoadProcess(o, 30)));

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
        }

        protected override void OnExit()
        {
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
            Cor_GameLogic = StartCoroutine(Cor_PlayContent_AnimalHead());
            Message.Send<MultiTouchMsg>(new MultiTouchMsg());

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.AnimalHead);
        }

        IEnumerator Cor_PlayContent_AnimalHead()
        {
            while (true)
            {
                int CountRandomObject = Random.Range(1, 2);

                for (int i = 0; i < CountRandomObject; i++)
                    mGameObjPool.GetObject(mGameObjPool.transform);

                float Random_Dealy = Random.Range(0.1f, 0.5f);
                yield return new WaitForSeconds(Random_Dealy);

                yield return null;
            }
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.AnimalHead);
        }

        protected override void OnHit(GameObject obj)
        {
            GameAnimalHead_AnimalHead obj_script = obj.transform.GetComponent<GameAnimalHead_AnimalHead>();

            if (obj_script != null)
                obj_script.Die();
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            mGameObjPool.PoolObject(msg.myObject);
        }
    }
}