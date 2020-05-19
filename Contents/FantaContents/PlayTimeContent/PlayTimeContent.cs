using UnityEngine;
using System.Collections;
using JHchoi.Models;
using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

namespace JHchoi.Contents
{
	public class PlayTimeContent : IContent
	{
        protected override void OnEnter()
        {
            UI.IDialog.RequestDialogEnter<UI.PlayTimeDialog>();
            var sm = Model.First<SettingModel>();
            Message.Send<PlayTimerStartMsg>(new PlayTimerStartMsg(sm.PlayTime));
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.PlayTimeDialog>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Message.Send<PlaySkipMsg>(new PlaySkipMsg());
        }
    }
}
