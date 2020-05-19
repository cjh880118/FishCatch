using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.UI
{
    public class OXDialog : IDialog
    {
        public GameObject[] player;
        protected override void OnEnter()
        {
            AddMessage();
            ResetPannel();
        }

        void ResetPannel()
        {
            foreach(var o in player)
            {
                for(int i = 0; i < o.transform.childCount; i++)
                {
                    o.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        private void AddMessage()
        {
            Message.AddListener<CatchFoodPlayerMsg>(CatchFoodPlayer);
        }

        private void CatchFoodPlayer(CatchFoodPlayerMsg msg)
        {
            player[msg.playerIndex].SetActive(false);

            if (msg.isRight)
            {
                player[msg.playerIndex].transform.GetChild(0).gameObject.SetActive(false);
                player[msg.playerIndex].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                player[msg.playerIndex].transform.GetChild(0).gameObject.SetActive(true);
                player[msg.playerIndex].transform.GetChild(1).gameObject.SetActive(false);
            }

            player[msg.playerIndex].SetActive(true);
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<CatchFoodPlayerMsg>(CatchFoodPlayer);
        }
    }
}