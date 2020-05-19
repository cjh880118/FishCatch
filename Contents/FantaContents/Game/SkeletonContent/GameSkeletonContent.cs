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
    public class GameSkeletonContent : IFantaBoxContent
    {
        GameSkeleton_ObjectControl gameSkeleton_ObjectControl;

        Coroutine Cor_GameLogic;

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
            string scenename = "GameSkeleton";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => 
                                gameSkeleton_ObjectControl = OnPostLoadProcess(o).GetComponent<GameSkeleton_ObjectControl>()));

            SetLoadComplete();
        }
        protected override void OnEnter()
        {
            // 게임 시작시 Ready 연출을 시작해 줍니다.
            ObjectListOn();

            RequestContentEnter<GlobalContent>();

            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());
            Message.Send<MainCameraMsg>(new MainCameraMsg(mainCamera));

            StartCoroutine(WaitEndAnimation());

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Skeleton);
        }

        IEnumerator WaitEndAnimation()
        {
            while (!gameSkeleton_ObjectControl.IsEnAnimation())
                yield return null;

            RequestContentEnter<ReadyContent>();
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
            Cor_GameLogic = StartCoroutine(gameSkeleton_ObjectControl.Cor_PlayContent_Skeleton());
        }

        protected override void OnHit(GameObject obj)
        {
            GameSkeleton_Skeleton obj_script = obj.transform.GetComponent<GameSkeleton_Skeleton>();

            if (obj_script != null)
                obj_script.Hit();
        }

        protected override void OnEnd()
        {
            StopCoroutine(Cor_GameLogic);
            Cor_GameLogic = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Skeleton);
        }
    }
}