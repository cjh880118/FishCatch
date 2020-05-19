using UnityEngine;
using System.Collections;
using JHchoi.Models;
using JHchoi;
using JHchoi.Contents.Event;
using JHchoi.UI.Event;

namespace JHchoi.Contents
{
	public class ReadyContent : IContent
	{
        protected override void OnEnter()
        {
            // UI OnOFF
            UI.IDialog.RequestDialogEnter<UI.ReadyDialog>();
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.ReadyDialog>();
        }
    }
}
