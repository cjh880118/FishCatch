using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class KioskInGameDialog : IDialog
    {
        public Button Start_Btn = null;

        private void Start()
        {
            Start_Btn.onClick.AddListener(OnStartClick);
        }

        void OnStartClick()
        {
            //Message.Send<Event.GotoTitleMsg>(new Event.GotoTitleMsg());
        }
    }
}

