﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Module.VideoDevice;

public class IRCam_VideoOption : MonoBehaviour
{
    bool isSet = false;
    VideoDeviceSettings setting;

    bool FlipX;
    bool FlipY;
    double Contrast;
    double Exposure;
    double AutoExposure;
    double Focus;
    double AutoFocus;

    private void Awake()
    {
        setting = Model.First<VideoDeviceInfoModel>().Settings;
    }
    
    private void OnEnable()
    {
        Message.AddListener<JHchoi.UI.Event.OptionReset>(ResetOption);
        if (!isSet)
            StartCoroutine(UiSet());
    }

    private void OnDisable()
    {
        isSet = false;
        Message.RemoveListener<JHchoi.UI.Event.OptionReset>(ResetOption);
    }

    IEnumerator UiSet()
    {
        foreach (var item in GetComponentsInChildren<ValueControll>())
        {
            while (!item.isSet)
            {
                yield return null;
            }
            if (!item.isToggle)
            {
                item.UpDownAction = ValueUp;
                item.ValueChangeAction = ValueChange;
            }
            else
            {
                item.ValueChangeAction = ValueChange;
            }

            SetData(item);
        }
        isSet = true;
        yield return null;
    }

    void InitData()
    {
        FlipX = setting.FlipX;
        FlipY = setting.FlipY;
        Contrast = setting.Contrast;
        Exposure = setting.Exposure;
        AutoExposure = setting.AutoExposure;
        Focus = setting.Focus;
        AutoFocus = setting.AutoFocus;
    }

    void SetData(ValueControll value)
    {
        if (string.Equals(value.gameObject.name, "FlipX"))
            value.SetValue(setting.FlipX);
        else if (string.Equals(value.gameObject.name, "FlipY"))
            value.SetValue(setting.FlipY);
        else if (string.Equals(value.gameObject.name, "Contrast"))
            value.SetValue(setting.Contrast.ToString());
        else if (string.Equals(value.gameObject.name, "Exposure"))
            value.SetValue(setting.Exposure.ToString());
        else if (string.Equals(value.gameObject.name, "AutoExposure"))
            value.SetValue(setting.AutoExposure.ToString());
        else if (string.Equals(value.gameObject.name, "Focus"))
            value.SetValue(setting.Focus.ToString());
        else if (string.Equals(value.gameObject.name, "AutoFocus"))
            value.SetValue(setting.AutoFocus.ToString());
    }

    void ValueUp(bool isUp, ValueControll value)
    {
        Debug.Log(value.gameObject.name);
        var input = value.GetValue();
        if (input == "")
        {
            input = "0";
        }
        double val = double.Parse(input);
        double Acc = 0;
        if (string.Equals(value.gameObject.name, "Contrast"))
            Acc = 0.1f;
        else if (string.Equals(value.gameObject.name, "Exposure"))
            Acc = 0.1f;
        else if (string.Equals(value.gameObject.name, "AutoExposure"))
            Acc = 0.1f;
        else if (string.Equals(value.gameObject.name, "Focus"))
            Acc = 0.1f;
        else if (string.Equals(value.gameObject.name, "AutoFocus"))
            Acc = 0.1f;
        val += isUp ? Acc : -Acc;
        value.SetValue(val.ToString());
    }

    void ValueChange(ValueControll value)
    {
        if (string.Equals(value.gameObject.name, "FlipX"))
        {
            string var = value.GetValue();
            var item = bool.Parse(var);
            setting.FlipX = item;
        }
        else if (string.Equals(value.gameObject.name, "FlipY"))
        {
            string var = value.GetValue();
            var item = bool.Parse(var);
            setting.FlipY = item;
        }
        else if (string.Equals(value.gameObject.name, "Contrast"))
        {
            string var = value.GetValue();
            var item = double.Parse(var);
            setting.Contrast = item;
        }
        else if (string.Equals(value.gameObject.name, "Exposure"))
        {
            string var = value.GetValue();
            var item = double.Parse(var);
            setting.Exposure = item;
        }
        else if (string.Equals(value.gameObject.name, "AutoExposure"))
        {
            string var = value.GetValue();
            var item = double.Parse(var);
            setting.AutoExposure = item;
        }
        else if (string.Equals(value.gameObject.name, "Focus"))
        {
            string var = value.GetValue();
            var item = double.Parse(var);
            setting.Focus = item;
        }
        else if (string.Equals(value.gameObject.name, "AutoFocus"))
        {
            string var = value.GetValue();
            var item = double.Parse(var);
            setting.AutoFocus = item;
        }
    }

    public void ResetOption(JHchoi.UI.Event.OptionReset msg)
    {
        setting.FlipX = FlipX;
        setting.FlipY = FlipY;
        setting.Contrast = Contrast;
        setting.Exposure = Exposure;
        setting.AutoExposure = AutoExposure;
        setting.Focus = Focus;
        setting.AutoFocus = AutoFocus;
    }

}
