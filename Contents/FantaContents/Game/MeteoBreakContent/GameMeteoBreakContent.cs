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
    public class GameMeteoBreakContent : IFantaBoxContent
    {
        Coroutine mCor_GameLogic = null;
        Coroutine mCor_InputMouse = null;

        List<ObjectPool> mGameObjPools = new List<ObjectPool>();
        ObjectPool HitEffectPool;

        protected override void OnLoadStart()
        {
            StartCoroutine(Cor_Load());
        }

        IEnumerator Cor_Load()
        {
            // 게임 씬 로드
            ObjectList = new List<GameObject>();
            string scenename = "GameMeteoBreak";
            var fullpath = string.Format("Scenes/FantaScenes/Fanta/{0}", scenename);
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));

            // 게임 관련 리소스 로딩
            string[] loadObjNames = new string[] { "JupiterObj", "MarsObj", "mercuryObj", "NeptuneObj", "PlutoObj",
                "Roket 1", "Roket 2", "Roket 3", "UFO 1", "UFO 2", "UFO 3", "UFO 4"};

            for (int i = 0; i < loadObjNames.Length; i++)
            {
                var fullpath_prefab = string.Format("Prefab/{0}/{1}", scenename, loadObjNames[i]);
                yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath_prefab,
                    o => mGameObjPools.Add(OnPostLoadProcess(o, 10))));
            }

            string path = string.Format("Prefab/{0}/HoshiEx 1", scenename);
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
                    o => HitEffectPool= OnPostLoadProcess(o, 40)));

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
            SoundManager.Instance.PlaySound((int)SoundType_GameBGM.MeteoBreak);
            mCor_GameLogic = StartCoroutine(Cor_GameLogic());
            mCor_InputMouse = StartCoroutine(Cor_InputMouseCheck());
        }

        // 게임로직 코루틴 삽입
        IEnumerator Cor_GameLogic()
        {
            yield return null;

            while (true)
            {
                int index = Random.Range(0, mGameObjPools.Count);
                var touchobj = mGameObjPools[index].GetObject().GetComponent<GameMeteoBreak_Obj>();

                float fPos = Random.Range(-8f, 8f);
                touchobj.Active(new Vector3(-20.0f, fPos, index < 7 ? -5f : 0), index);

                float Random_Dealy = Random.Range(0, 2f);
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
            SoundManager.Instance.StopSound((int)SoundType_GameBGM.MeteoBreak);
            StopCoroutine(mCor_GameLogic);
            StopCoroutine(mCor_InputMouse);
            mCor_GameLogic = null;
            mCor_InputMouse = null;
        }

        protected override void OnHit(GameObject obj)
        {
            var obj_script = obj.transform.GetComponent<GameMeteoBreak_Obj>();
            if (obj_script != null)
            {
                obj_script.Hit();
                StartCoroutine(ShowEffect(obj_script.transform.localPosition));
            }
        }

        IEnumerator ShowEffect(Vector3 pos)
        {
            var effect = HitEffectPool.GetObject(HitEffectPool.transform);
            effect.SetActive(true);
            effect.transform.localPosition = pos;

            yield return new WaitForSeconds(2.0f);

            HitEffectPool.PoolObject(effect);
        }

        void OnDeactive(Event.GameObjectDeActiveMessage msg)
        {
            mGameObjPools[msg.TypeIndex].PoolObject(msg.myObject);
        }
    }
}
