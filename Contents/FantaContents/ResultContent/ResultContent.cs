using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig;
using CellBig.Contents.Event;
using CellBig.UI.Event;

namespace CellBig.Contents
{
	public class ResultContent : IContent
	{
        protected override void OnEnter()
        {
            UI.IDialog.RequestDialogEnter<UI.ResultDialog>();

            Message.Send<IsScoreContentMsg>(new IsScoreContentMsg(pcm.GetCurrentContent().isScore));
        }

        protected override void OnExit()
        {

            UI.IDialog.RequestDialogExit<UI.ResultDialog>();
        }

        protected override void AddMessage()
        {
          
        }
        protected override void RemoveMessage()
        {
        }
    }
}
