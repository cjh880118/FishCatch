﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Models;
using CellBig.Common;
using CellBig.UI.Event;

namespace CellBig.UI.Event
{
    public class OptionReset : Message
    {

    }
}


namespace CellBig.UI
{
    public class IRCameraSensorSettingDialog : IDialog
    {
        IRCam_DetectionOption detection;
        IRCam_VideoOption video;
        SavePopupDialog popup;

        Button ResetButton;
        Button SaveButton;
        Button ExitButton;
        bool isSet = false;

        protected override void OnLoad()
        {
            if (!isSet)
                UiSet();
        }

        protected override void OnEnter()
        {
             popup.gameObject.SetActive(false);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUnload()
        {
        }

        void UiSet()
        {
            detection = dialogView.transform.Find("Options").GetComponentInChildren<IRCam_DetectionOption>();
            video = dialogView.transform.Find("Options").GetComponentInChildren<IRCam_VideoOption>();
            ResetButton = dialogView.transform.Find("DownButtons").Find("Reset").GetComponent<Button>();
            ResetButton.onClick.AddListener(ResetAction);
            SaveButton = dialogView.transform.Find("DownButtons").Find("Save").GetComponent<Button>();
            SaveButton.onClick.AddListener(SaveAction);
            ExitButton = dialogView.transform.Find("DownButtons").Find("Exit").GetComponent<Button>();
            ExitButton.onClick.AddListener(ExitAction);
            popup = dialogView.transform.Find("SavePopupDialog").GetComponent< SavePopupDialog>();
           

            popup.SaveFunc += SaveAction;
            popup.UnsaveFunc += ResetAction;
            popup.SaveFunc += ExitDialog;
            popup.UnsaveFunc += ExitDialog;

            isSet = true;
        }

        void ResetAction()
        {
            Message.Send<OptionReset>(new OptionReset());
        }

        void SaveAction()
        {
            Message.Send<CellBig.Module.Detection.SaveSettings>(new CellBig.Module.Detection.SaveSettings());
            Message.Send<CellBig.Module.VideoDevice.SaveSettings>(new CellBig.Module.VideoDevice.SaveSettings());
        }

        void ExitDialog()
        {
            IDialog.RequestDialogExit<IRCameraSensorSettingDialog>();
            IDialog.RequestDialogEnter<CommonManagerDialog>();
        }

        void ExitAction()
        {
            popup.gameObject.SetActive(true);
        }
    }
}