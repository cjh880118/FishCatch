using UnityEngine;
using System.Collections;
using CellBig.UI.Event;
using CellBig.Contents.Event;
using CellBig.Models;
using CellBig.Common;
using OpenCVForUnity;
using System.Collections.Generic;
using Rect = UnityEngine.Rect;

namespace CellBig.Contents
{
    public abstract class IFantaBoxContent : IContent
    {
        //[SerializeField]
        //protected Camera mainCamera = null;
        [SerializeField]
        protected List<GameObject> ObjectList = null;

        protected Touch tempTouch;
        protected Vector2 touchPos;
        protected List<Rect> rects = new List<Rect>();
        protected Coroutine multiTouchCor;
        // 키넥트 터치 딜레이
        protected Coroutine delayTouchRectCor;


        // 콘텐츠 내부 터치 딜레이 (플로어 콘텐츠만 적용)
        protected Coroutine contentDelayCheckCor;
        protected bool isDelayCheck = true;
        //////////////////

        protected Vector3 lastRayDistance = new Vector3(10000,10000,10000);
        protected Vector3 currentRayDistance = Vector3.zero;

        bool isPlaying;

        protected override void AddMessage()
        {
            Message.AddListener<ReadyGoEndMesg>(OnReadyEnd);
            Message.AddListener<PlayTimeOverMsg>(OnTimeOver);
            Message.AddListener<MultiTouchMsg>(OnMultiTouchMsg);
            Message.AddListener<TouchRectMsg>(OnTouchRect);
            Message.AddListener<NextGameStartMsg>(OnNextGame);

            OnAddMessage();
        }

        protected override void RemoveMessage()
        {
            Message.RemoveListener<ReadyGoEndMesg>(OnReadyEnd);
            Message.RemoveListener<PlayTimeOverMsg>(OnTimeOver);
            Message.RemoveListener<MultiTouchMsg>(OnMultiTouchMsg);
            Message.RemoveListener<TouchRectMsg>(OnTouchRect);
            Message.RemoveListener<NextGameStartMsg>(OnNextGame);

            OnRemoveMessage();
        }

        protected virtual void OnAddMessage() { }
        protected virtual void OnRemoveMessage() { }

        protected abstract void OnPlay();
        protected abstract void OnEnd();
        protected abstract void OnHit(GameObject obj);
        protected virtual void HitPoint(Vector3 hitPoint) { }

        protected virtual void OnMultiTouchMsg(MultiTouchMsg msg)
        {
            multiTouchCor = StartCoroutine(MultiTouch());
        }

        IEnumerator MultiTouch()
        {
            while (true)
            {
                if (Input.touchCount > 0)
                {
                    for (int index = 0; index < Input.touchCount; index++)
                    {
                        tempTouch = Input.GetTouch(index);
                        if (tempTouch.phase == TouchPhase.Began)
                        {
                            touchPos = mainCamera.ScreenToViewportPoint(tempTouch.position);
                            Rect mouseRect = new Rect(touchPos, new Vector3(0.1f, 0.1f));
                            rects.Add(mouseRect);

                            Message.Send<TouchRectMsg>(new TouchRectMsg(rects));

                            rects.Clear();
                        }
                    }
                }

                yield return null;
            }
        }

        protected virtual void OnTouchRect(TouchRectMsg msg)
        {
            if (mainCamera == null)
                return;

            if (delayTouchRectCor == null)
                delayTouchRectCor = StartCoroutine(OnDelayTouchRect(msg));

            //// Bit shift the index of the layer (8) to get a bit mask
            //int layerMask = 1 << 8;
            //// This would cast rays only against colliders in layer 8.
            //// But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            //layerMask = ~layerMask;
            //Rect CurrentRect;
            //RaycastHit latestHit = new RaycastHit();

            //List<Rect> RectList = msg.TouchRects;

            //while (RectList.Count != 0)
            //{
            //    CurrentRect = RectList[0];
            //    float CenterX = CurrentRect.x;
            //    float CenterY = CurrentRect.y;
            //    for (float i = CurrentRect.x; i < CurrentRect.x + CurrentRect.width; i+=0.01f)
            //    {
            //        for (float j = CurrentRect.y; j < CurrentRect.y + CurrentRect.height; j += 0.01f)
            //        {
            //            if (Physics.Raycast(mainCamera.ViewportPointToRay(new Vector3(i, j, 0)), out latestHit, Mathf.Infinity, layerMask))
            //            {
            //                OnHit(latestHit.transform.gameObject);
            //                HitPoint(latestHit.point);
            //            }
            //        }
            //    }
            //    RectList.RemoveAt(0);
            //}
        }

        IEnumerator OnDelayTouchRect(TouchRectMsg msg)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;
            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            Rect CurrentRect;
            RaycastHit latestHit = new RaycastHit();

            List<Rect> RectList = msg.TouchRects;
            //var pcm = Model.First<PlayContentModel>();

            while (RectList.Count != 0)
            {
                CurrentRect = RectList[0];
                float interval = pcm.GetCurrentContent().Rayinterval;
                bool isMultiRay = pcm.GetCurrentContent().isMultiRay;
                float rayDistance = pcm.GetCurrentContent().RayDistance;

                if (isMultiRay)
                {
                    for (float i = CurrentRect.x; i < CurrentRect.x + CurrentRect.width; i += interval)
                    {
                        for (float j = CurrentRect.y; j < CurrentRect.y + CurrentRect.height; j += interval)
                        {
                            if (Physics.Raycast(mainCamera.ViewportPointToRay(new Vector3(i, j, 0)), out latestHit, Mathf.Infinity, layerMask))
                            {
                                OnHit(latestHit.transform.gameObject);
                                HitPoint(latestHit.point);
                            }
                        }
                    }
                }
                else
                {
                    if (Physics.Raycast(mainCamera.ViewportPointToRay(new Vector3(CurrentRect.center.x, CurrentRect.center.y, 0)), out latestHit, Mathf.Infinity, layerMask))
                    {
                        currentRayDistance = new Vector3(latestHit.point.x, 0, latestHit.point.z);

                        if (Vector3.Distance(currentRayDistance, lastRayDistance) > rayDistance)
                        {
                            OnHit(latestHit.transform.gameObject);
                            HitPoint(latestHit.point);
                            lastRayDistance = currentRayDistance;
                        }
                    }
                }
                RectList.RemoveAt(0);

                yield return null;
            }

            yield return new WaitForSeconds(sm.DelayTouch);

            delayTouchRectCor = null;
        }

        void OnReadyEnd(ReadyGoEndMesg msg)
        {
            RequestContentEnter<PlayTimeContent>();

            if(pcm.GetCurrentContent().isScore)
                RequestContentEnter<ScoreContent>();
            RequestContentExit<ReadyContent>();
            OnPlay();

            isPlaying = true;
        }

        void OnTimeOver(PlayTimeOverMsg msg)
        {
            isPlaying = false;

            OnEnd();
            RequestContentExit<PlayTimeContent>();
            if (pcm.GetCurrentContent().isScore)
                RequestContentExit<ScoreContent>();
            RequestContentEnter<ResultContent>();
        }

        void OnNextGame(NextGameStartMsg msg)
        {
            StartCoroutine(ChangeContent());
        }

        IEnumerator ChangeContent()
        {
            Message.Send<FadeInMsg>(new FadeInMsg());
            RequestContentExit<ResultContent>();
            yield return new WaitForSeconds(0.5f);
            var pcm = Model.First<PlayContentModel>();
            string nextContent = pcm.GetNextContentName();
            IContent.RequestContentExit(this.name);
            IContent.RequestContentEnter(nextContent);
        }

        // 씬 프리펩 로드 프로세스
        protected virtual GameObject OnPostLoadProcess(Object o)
        {
            var PrefabInstance = Instantiate(o) as GameObject;
            PrefabInstance.transform.SetParent(transform);
            PrefabInstance.SetActive(false);
            ObjectList.Add(PrefabInstance);
            mainCamera = PrefabInstance.GetComponentInChildren<Camera>();
            return PrefabInstance;
        }

        // 오브젝트 풀 로드 프로세스
        protected virtual ObjectPool OnPostLoadProcess(Object o,int count)
        {
            var PoolObject = new GameObject();
            PoolObject.name = o.name + "Pool";
            PoolObject.transform.SetParent(transform);
            ObjectPool p = PoolObject.AddComponent<ObjectPool>();
            p.PreloadObject(count, o as GameObject);
            ObjectList.Add(PoolObject);
            PoolObject.SetActive(false);
            return p;
        }
        
        protected void ObjectListOn()
        {
            if (ObjectList != null)
            {
                for (int i = 0; i < ObjectList.Count; i++)
                {
                    ObjectList[i].SetActive(true);
                }
            }
        }

        protected void ObjectListOff()
        {
            if (ObjectList != null)
            {
                for (int i = 0; i < ObjectList.Count; i++)
                {
                    ObjectList[i].SetActive(false);
                }
            }
        }

        protected bool IsPlaying()
        {
            return isPlaying;
        }

        protected IEnumerator CheckDelay()
        {
            isDelayCheck = false;

            contentDelayCheckCor = null;

            yield return new WaitForSeconds(0.25f);

            isDelayCheck = true;
        }
    }
}
