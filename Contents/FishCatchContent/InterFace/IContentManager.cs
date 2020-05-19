using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;
using CellBig.Constants;
using CellBig.Module.Detection.CV.Output;
using System;
using CellBig.UI.Event;
using CellBig.Constants.FishCatch;
using UnityEngine.SceneManagement;

namespace CellBig.Contents
{
    public abstract class IContentManager : IContent
    {
        public float tempDistance = 0.15f;
        protected CommonModel cm;
        protected FishModel fm;
        protected List<GameObject> listGameObject = new List<GameObject>();
        protected List<Vector2> listVec2 = new List<Vector2>();
        protected List<Coroutine> listCorSound = new List<Coroutine>();
        protected float rectInputDelayCheck;
        protected bool isTestOn;
        Coroutine corRectInputDelay;
        Coroutine corInputKey;

        protected override void OnLoadStart()
        {
            pcm = Model.First<PlayContentModel>();
            cm = Model.First<CommonModel>();
            fm = Model.First<FishModel>();
            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            SceneManager.LoadScene(pcm.GetCurrentContent().ContentName);
            SceneManager.sceneLoaded += Onloaded;
        }

        protected void Onloaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            isTestOn = false;
            StartCoroutine(LoadObject());
            corRectInputDelay = StartCoroutine(RectInputDelay());
            corInputKey = StartCoroutine(InputKey());
            PlaySound();
        }

        protected abstract IEnumerator LoadObject();

        protected virtual void ObjectLoadComplete()
        {
            Debug.Log("로드 컴플릿 타이머 시작");
            Message.Send<FadeOutMsg>(new FadeOutMsg());
            sm.PlayTime = pcm.PlayTime;
            RequestContentEnter<PlayTimeContent>();
        }

        protected abstract void PlaySound();

        protected IEnumerator SoundCorLoop(float sec, SoundFishCatch sound)
        {
            while (true)
            {
                yield return new WaitForSeconds(sec);

                SoundManager.Instance.PlaySound((int)sound);

                yield return new WaitForSeconds(1.0f);

                while (SoundManager.Instance.IsPlaySound((int)sound))
                {
                    yield return null;
                }
            }
        }

        protected override void AddMessage()
        {
            Message.AddListener<Module.Detection.CV.Output.ViewportCircles>(InputVector2);
            Message.AddListener<RectPositionMsg>(RectPosition);
            Message.AddListener<PlayTimeOverMsg>(OnTimeOver);
        }

        private void InputVector2(ViewportCircles msg)
        {
            if (msg.Value.Count <= 0)
                return;

            for (int i = 0; i < msg.Value.Count; i++)
                Log.Instance.log("인아웃 설정 x : " + msg.Value[i].x + "/ y : " + msg.Value[i].y);
            //Log.Instance.log("입력 :" + DateTime.Now.ToString());
            //Debug.Log("InputVector2 " + msg.Value.Count);
            //for (int i = 0; i < msg.Value.Count; i++)
            //{
            //    Debug.Log("Content x : " + msg.Value[i].x + " y : " + msg.Value[i].y);
            //}

            StartCoroutine(ViewPortCheck(msg.Value));
        }

        protected abstract void RectPosition(RectPositionMsg msg);


        private void OnTimeOver(PlayTimeOverMsg msg)
        {
            StartCoroutine(ChangeContent());
        }

        IEnumerator ChangeContent()
        {
            Debug.Log("타임오버");
            var pcm = Model.First<PlayContentModel>();
            IContent.RequestContentExit(pcm.GetCurrentContent().ContentName);
            Message.Send<CellBig.UI.Event.LoadImageChangeMsg>(new LoadImageChangeMsg());
            Message.Send<FadeInMsg>(new FadeInMsg());
            yield return new WaitForSeconds(0.5f);
            string nextContent = pcm.GetNextContentName();
            IContent.RequestContentEnter(nextContent);
        }

        protected abstract IEnumerator RectInputDelay();

        IEnumerator InputKey()
        {
            while (true)
            {
                yield return null;
                if (Input.GetKeyDown(KeyCode.A))
                {
                    isTestOn = !isTestOn;
                }

                if (isTestOn)
                {
                    rectInputDelayCheck = 0;
                    listVec2.Clear();
                    Vector2 vec2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                    listVec2.Add(vec2);
                    StartCoroutine(ViewPortCheck(listVec2));
                }

                if (Input.touchCount > 0)
                {
                    rectInputDelayCheck = 0;
                    listVec2.Clear();
                    Touch touch = Input.GetTouch(0);
                    Vector2 vec2 = Camera.main.ScreenToViewportPoint(touch.position);
                    listVec2.Add(vec2);
                    StartCoroutine(ViewPortCheck(listVec2));
                }
            }
        }

        protected abstract IEnumerator ViewPortCheck(List<Vector2> vec2List);

        protected override void OnExit()
        {
            RemoveMessage();
            RequestContentExit<PlayTimeContent>();
            CoroutineCheckStop(corRectInputDelay);
            CoroutineCheckStop(corInputKey);

            foreach (var o in listCorSound)
            {
                if (o != null)
                    StopCoroutine(o);
            }

            listCorSound.Clear();
            SoundManager.Instance.StopAllSound();
            SceneManager.sceneLoaded -= Onloaded;
            foreach (var o in listGameObject)
                Destroy(o);
        }

        protected void CoroutineCheckStop(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        protected override void RemoveMessage()
        {
            Message.RemoveListener<Module.Detection.CV.Output.ViewportCircles>(InputVector2);
            Message.RemoveListener<RectPositionMsg>(RectPosition);
            Message.RemoveListener<PlayTimeOverMsg>(OnTimeOver);
        }
    }
}