using UnityEngine;
using System.Collections;
using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

namespace JHchoi.Contents
{
	public class TitleContent : IContent
	{
        protected override void OnEnter()
		{
            Message.Send<FadeOutMsg>(new FadeOutMsg());

            UI.IDialog.RequestDialogEnter<UI.ScreenTitleDialog>();
            UI.IDialog.RequestDialogEnter<UI.KioskTitleDialog>();
        }

		protected override void OnExit()
		{
            UI.IDialog.RequestDialogExit<UI.ScreenTitleDialog>();
            UI.IDialog.RequestDialogExit<UI.KioskTitleDialog>();
        }

        protected override void AddMessage()
        {
            Message.AddListener<UI.Event.StartClickMsg>(OnStartClick);
        }

        protected override void RemoveMessage()
        {
            Message.RemoveListener<UI.Event.StartClickMsg>(OnStartClick);
        }

        void OnStartClick(UI.Event.StartClickMsg msg)
        {
            StartCoroutine(ChangeScene());
        }

        IEnumerator ChangeScene()
        {
            Message.Send<FadeInMsg>(new FadeInMsg());
            yield return new WaitForSeconds(0.5f);
            Scene.SceneManager.Instance.Load(SceneName.Lobby);
        }
    }
}
