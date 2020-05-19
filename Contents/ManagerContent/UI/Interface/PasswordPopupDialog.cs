using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JHchoi.Models;
using JHchoi.Common;
using JHchoi.UI.Event;

namespace JHchoi.UI
{
    public class PasswordPopupDialog : IDialog
    {
        protected Text result;
        protected InputField inputPassword;
        protected Button Enter;
        protected Button Exit;
        protected PlayContentModel cm;

        protected override void OnLoad()
        {
            cm = Model.First<PlayContentModel>();
        }

        protected override void OnEnter()
        {
            var image = this.transform.Find("Image");
            result = image.Find("Result").GetComponent<Text>();
            result.text = "";
            inputPassword = image.Find("Input").GetComponent<InputField>();
            inputPassword.text = "";
            Enter = image.Find("Enter").GetComponent<Button>();
            Enter.onClick.AddListener(StartGame);
            Exit = image.Find("Exit").GetComponent<Button>();
            Exit.onClick.AddListener(ExitGame);
        }

        protected override void OnExit()
        {
            if (Enter != null)
                Enter.onClick.RemoveListener(StartGame);
            if (Exit != null)
                Exit.onClick.RemoveListener(ExitGame);
        }

        protected virtual void StartGame()
        {
        }

        protected virtual void ExitGame()
        {
        }
    }
}