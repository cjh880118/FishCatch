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
    public class GameWestonContent : IFantaBoxContent
    {
        GameWeston_ObjectControl gameWeston_ObjectControl;
        
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
            string scenename = "GameWeston";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => gameWeston_ObjectControl = OnPostLoadProcess(o).GetComponent<GameWeston_ObjectControl>()));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Weston);
        }

        protected override void OnExit()
        {
            ObjectListOff();
        }

        protected override void OnAddMessage()
        {
        }

        protected override void OnRemoveMessage()
        {
        }

        protected override void OnPlay()
        {
            Message.Send<MultiTouchMsg>(new MultiTouchMsg());
            Cor_GameLogic = StartCoroutine(gameWeston_ObjectControl.Cor_PlayContent_Weston());
        }

        protected override void OnHit(GameObject obj)
        {
            GameWeston_WestonTarget obj_script = obj.transform.GetComponent<GameWeston_WestonTarget>();

            if (obj_script != null)
                obj_script.Hit();
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Weston);
        }
    }
}