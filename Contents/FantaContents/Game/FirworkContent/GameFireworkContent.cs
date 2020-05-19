using UnityEngine;
using System.Collections;
using JHchoi.Models;
using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;
using JHchoi.Common;
using DG.Tweening;
using System.Collections.Generic;
using JHchoi.Module;
using JHchoi.Constants;
using DG.Tweening;
using Rect = UnityEngine.Rect;
using OpenCVForUnity;

namespace JHchoi.Contents
{
    public class GameFireworkContent : IFantaBoxContent
    {
        readonly Vector3[] StartPosititon = new Vector3[5]
            { new Vector3(-50,-60,0),new Vector3(-30,-60,0),new Vector3(0,-60,0),new Vector3(30,-60,0),new Vector3(50,-60,0)};

        Coroutine mCor_GameLogic = null;
        Coroutine mCor_InputMouse = null;

        ObjectPool mGameObjPool;
       
        protected override void OnLoadStart()
        {
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            yield return null;
            // 게임 씬 로드
            ObjectList = new List<GameObject>();
            string scenename = "GameFirework";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            // 게임 관련 리소스 로딩
            string prefabname = scenename + "/Firework";
            var fullpath_prefab = string.Format("Prefab/{0}", prefabname);           
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab, o => mGameObjPool = OnPostLoadProcess(o,40)));

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

            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.FireWork);
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
            mCor_GameLogic = StartCoroutine(Cor_GameLogic());
            mCor_InputMouse = StartCoroutine(Cor_InputMouseCheck());

            SoundManager.Instance.PlaySound((int)SoundType_GameFX.FireWork_Move);
        }

        // 게임로직 코루틴 삽입
        IEnumerator Cor_GameLogic()
        {
            yield return null;

            while (true)
            {
                int CountRandomObject = Random.Range(5, 10);

                for (int i = 0; i < CountRandomObject; i++)
                {
                    GameFirework_Firework touchobj = mGameObjPool.GetObject(mGameObjPool.transform).GetComponent<GameFirework_Firework>();
                    Vector3 pos = new Vector3(Random.Range(-500, 500), -280, 0);
                    touchobj.transform.localPosition = pos;
                    touchobj.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-45, 45));
                    touchobj.Active();
                }

                int Random_Dealy = Random.Range(1, 3);

                yield return new WaitForSeconds(Random_Dealy);
            }
        }

        IEnumerator Cor_InputMouseCheck()
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
            StopCoroutine(mCor_GameLogic);
            StopCoroutine(mCor_InputMouse);

            mCor_GameLogic = null;
            mCor_InputMouse = null;

            SoundManager.Instance.StopSound((int)SoundType_GameBGM.FireWork);
            SoundManager.Instance.StopSound((int)SoundType_GameFX.FireWork_Move);
        }

        protected override void OnHit(GameObject obj)
        {
            GameFirework_Firework obj_script = obj.transform.GetComponent<GameFirework_Firework>();

            if (obj_script != null)
                obj_script.Hit();
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            mGameObjPool.PoolObject(msg.myObject);
        }
    }
}
