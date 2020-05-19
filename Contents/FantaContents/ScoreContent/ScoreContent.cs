using UnityEngine;
using System.Collections;
using JHchoi.Models;
using JHchoi;
using JHchoi.Contents.Event;
using JHchoi.UI.Event;

namespace JHchoi.Contents
{
	public class ScoreContent : IContent
	{
        protected override void OnEnter()
        {
            // UI OnOFF
            UI.IDialog.RequestDialogEnter<UI.ScoreDialog>();
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.ScoreDialog>();
        }
    }
}
