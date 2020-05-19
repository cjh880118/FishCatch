using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.UI.Event;

namespace JHchoi.Contents
{
	public class InGameContent : IContent
	{
        protected override void OnEnter()
        {
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());

            AddMessage();
            UI.IDialog.RequestDialogEnter<UI.KioskInGameDialog>();
            UI.IDialog.RequestDialogEnter<UI.ScreenInGameDialog>();
        }

        protected override void OnExit()
        {
            RemoveMessage();
            UI.IDialog.RequestDialogExit<UI.KioskInGameDialog>();
            UI.IDialog.RequestDialogExit<UI.ScreenInGameDialog>();
        }

        void AddMessage()
        {
            Message.AddListener<GotoTitleMsg>(OnGotoTitle);
        }

        void RemoveMessage()
        {
            Message.RemoveListener<GotoTitleMsg>(OnGotoTitle);
        }

        void OnGotoTitle(UI.Event.GotoTitleMsg msg)
        {
            StartCoroutine(ChangeScene());
        }

        IEnumerator ChangeScene()
        {
            Message.Send<UI.Event.FadeInMsg>(new UI.Event.FadeInMsg());
            yield return new WaitForSeconds(0.5f);
            Scene.SceneManager.Instance.Load(SceneName.Title);
        }
    }
}
