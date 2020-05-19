using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;
using CellBig.Common;
using DG.Tweening;
using System.Collections.Generic;
using CellBig.Module;
using CellBig.Constants;
using Rect = UnityEngine.Rect;
using OpenCVForUnity;

namespace CellBig.Contents
{
    public class GameMaskContent : IFantaBoxContent
    {
        Coroutine CorInputMouse = null;
        ObjectPool GameObjPool = null;

        int TouchCount = 0;

        protected override void OnLoadStart()
        {
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            // 게임 씬 로드
            ObjectList = new List<GameObject>();
            string scenename = "GameMask";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            var fullpath_prefab = string.Format("Prefab/{0}/Mask", scenename);
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab,
                o => GameObjPool = OnPostLoadProcess(o, 20)));

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
            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.Mask);
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
            // 실제 로직을 돌리고
            SetMaskPattern();
            CorInputMouse = StartCoroutine(Cor_InputMouseCheck());
        }

        // 게임로직 코루틴 삽입
        void SetMaskPattern()
        {
            TouchCount = 0;

            var mm = Model.First<MaskPatternModel>();
            var pattren = mm.GetPatternInfo();
            float width_Interval = 0.5f;
            float height_Interval = 0.65f;

            float startX = -1.0f * (((float)pattren.Lists[0].Count * width_Interval) / 2.0f);
            float startY = 1.0f * (((float)pattren.Lists.Count * height_Interval) / 2.0f);

            for (int i = 0; i < pattren.Lists.Count; i++)
            {
                var info = pattren.Lists[i];
                for (int k = 0; k < info.Count; k++)
                {
                    if (info[k] == 1)
                    {
                        Vector3 pos = new Vector3(startX + k * width_Interval, startY - i * height_Interval, 0.0f);
                        GameMask_Obj touchobj = GameObjPool.GetObject(GameObjPool.transform).GetComponent<GameMask_Obj>();
                        touchobj.Active(pos);

                        TouchCount++;
                    }
                }
            }
        }

        protected IEnumerator Cor_InputMouseCheck()
        {
            List<Rect> rects = new List<Rect>();
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //마우스 눌렀을때 해당위치에 Rect생성
                    rects.Clear();
                    Vector2 screenPoint = mainCamera.ScreenToViewportPoint(Input.mousePosition);
                    UnityEngine.Rect mouseRect = new UnityEngine.Rect(screenPoint, new Vector2(0.1f, 0.1f));
                    rects.Add(mouseRect);

                    Message.Send<TouchRectMsg>(new TouchRectMsg(rects));
                }
                yield return null;
            }
        }

        protected override void OnEnd()
        {
            StopCoroutine(CorInputMouse);
            CorInputMouse = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.Mask);
        }

        protected override void OnHit(GameObject obj)
        {
            var obj_script = obj.transform.GetComponent<GameMask_Obj>();
            if (obj_script != null)
            {
                obj_script.Hit();
            }
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            GameObjPool.PoolObject(msg.myObject);

            TouchCount--;
            if (TouchCount <= 0 && CorInputMouse != null)
                SetMaskPattern();
        }
    }
}
