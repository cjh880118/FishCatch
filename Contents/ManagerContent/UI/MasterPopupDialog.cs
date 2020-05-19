using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using JHchoi.Models;
using JHchoi.Common;
using JHchoi.UI.Event;

namespace JHchoi.UI
{
    public class MasterPopupDialog : PasswordPopupDialog
    {
        protected override void StartGame()
        {
            if (string.Compare(inputPassword.text, "cellbig3413",false) >= 0)
            {
                IDialog.RequestDialogExit<CommonManagerDialog>();
                IDialog.RequestDialogEnter<MasterManagerDialog>();
                IDialog.RequestDialogExit<MasterPopupDialog>();
            }
            else
            {
                result.text = "비밀번호가 다릅니다. 다시 확인해주세요.";
            }
        }

        protected override void ExitGame()
        {
            IDialog.RequestDialogExit<MasterPopupDialog>();
        }
    }
}