using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CellBig.Contents.Event;

namespace CellBig.UI
{
    public class KioskGlobalDialog : IDialog
    {
        public Image Fade_Img;
        public Image eventColorImage;

        Coroutine eventColorCor;

        protected override void OnEnter()
        {
            Message.AddListener<Event.FadeInMsg>(OnFadeIn);
            Message.AddListener<Event.FadeOutMsg>(OnFadeOut);

            Message.AddListener<ColorCameraMsg>(OnColorCameraMsg);
        }

        protected override void OnExit()
        {
            Message.RemoveListener<Event.FadeInMsg>(OnFadeIn);
            Message.RemoveListener<Event.FadeOutMsg>(OnFadeOut);
            Message.RemoveListener<ColorCameraMsg>(OnColorCameraMsg);
        }

        void OnFadeIn(Event.FadeInMsg msg)
        {
            if (msg.all || msg.kiosk)
                Fade(true, msg.time);
        }

        void OnFadeOut(Event.FadeOutMsg msg)
        {
            if (msg.all || msg.kiosk)
                Fade(false, msg.time);
        }

        void Fade(bool fadeIn, float time)
        {
            var color = Fade_Img.color;
            color.a = fadeIn ? 0.0f : 1.0f;
            Fade_Img.DOFade(fadeIn ? 1.0f : 0.0f, time);
        }

        void OnColorCameraMsg(ColorCameraMsg msg)
        {
            eventColorImage.color = Color.clear;
            eventColorImage.DOColor(msg.Color, msg.Duration).SetLoops(2, LoopType.Yoyo);

            if (eventColorCor != null)
                StopCoroutine(eventColorCor);

            eventColorCor = StartCoroutine(ColorCameraInit(msg.Duration));
        }

        IEnumerator ColorCameraInit(float time)
        {
            yield return new WaitForSeconds(time*2f);
            eventColorImage.color = Color.clear;
        }
    }
}

