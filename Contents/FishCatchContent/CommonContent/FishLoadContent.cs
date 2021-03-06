﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHchoi.Models;
using JHchoi.Contents.Event;
using JHchoi.UI.Event;

namespace JHchoi.Contents
{
	public class FishLoadContent : IContent
	{
        //GameModel gm;
        //PlayContentModel pcm;
        CommonModel cm;
        protected override void OnLoadStart()
        {
            Screen.SetResolution(1600, 1200, true);
            SetLoadComplete();
        }

        protected override void OnEnter()
		{
            UI.IDialog.RequestDialogEnter<UI.KioskGlobalDialog>();
            UI.IDialog.RequestDialogEnter<UI.ScreenGlobalDialog>();
            sm.PlayTime = pcm.PlayTime;
            StartCoroutine(StartContent());
        }

        IEnumerator StartContent()
        {
            Message.Send<JHchoi.UI.Event.LoadImageChangeMsg>(new LoadImageChangeMsg());
            Message.Send<FadeInMsg>(new FadeInMsg());
            yield return new WaitForSeconds(0.5f);
            var pcm = Model.First<PlayContentModel>();
            IContent.RequestContentEnter(pcm.GetCurrentContent().ContentName);
        }

        protected override void OnExit()
		{
            UI.IDialog.RequestDialogExit<UI.KioskGlobalDialog>();
            UI.IDialog.RequestDialogExit<UI.ScreenGlobalDialog>();
        }
    }
}
