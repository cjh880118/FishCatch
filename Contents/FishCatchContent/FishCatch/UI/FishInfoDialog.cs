using JHchoi.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class FishInfoDialog : IDialog
    {
        public GameObject board;

        protected override void OnEnter()
        {
            for (int i = 0; i < board.transform.childCount; i++)
                board.transform.GetChild(i).gameObject.SetActive(false);

            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<CatchPlateSuccessMsg>(CatchPlateSuccess);
        }

        private void CatchPlateSuccess(CatchPlateSuccessMsg msg)
        {
            if (board.transform.GetChild(msg.playerIndex).gameObject.activeSelf)
                board.transform.GetChild(msg.playerIndex).gameObject.SetActive(false);

            board.transform.GetChild(msg.playerIndex).gameObject.SetActive(true);
            board.transform.GetChild(msg.playerIndex).GetChild(0).GetComponent<Text>().text = msg.fishName;
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<CatchPlateSuccessMsg>(CatchPlateSuccess);
        }
    }
}