using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Module;
using CellBig.Module.Detection;
using CellBig.Module.Detection.CV;

public class IRCam_DetectionOption : MonoBehaviour
{
    bool isSet = false;
    CVSettings setting;

    int _ErodeSize;
    int _ErodeIterations;
    int _DilateSize;
    int _DilateIterations;
    int _Threshold;
    double _MinContourSize;
    double _MaxContourSize;
    float _CircleSamplingRate;
    float _CircleValidRate;
    float _CircleErrorThreshold;
    double _QuadEpsilonMultiplier;

    private void Awake()
    {
    }
    
    private void OnEnable()
    {
        Message.AddListener<CellBig.UI.Event.OptionReset>(ResetOption);
        if (!isSet)
            StartCoroutine(UiSet());
    }

    private void OnDisable()
    {
        Message.RemoveListener<CellBig.UI.Event.OptionReset>(ResetOption);
    }

    IEnumerator UiSet()
    {
        setting = Model.First<DetectionInfoModel>().CVSettings;

        InitData();
        var child = GetComponentsInChildren<ValueControll>();

        foreach (var item in child)
        {
            while(!item.isSet)
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
        _ErodeSize = setting.ErodeSize;
        _ErodeIterations = setting.ErodeIterations;
        _DilateSize = setting.DilateSize;
        _DilateIterations = setting.DilateIterations;
        _Threshold = setting.Threshold;
        _MinContourSize = setting.MinContourSize;
        _MaxContourSize = setting.MaxContourSize;
        _CircleSamplingRate = setting.CircleSamplingRate;
        _CircleValidRate = setting.CircleValidRate;
        _CircleErrorThreshold = setting.CircleErrorThreshold;
        _QuadEpsilonMultiplier = setting.QuadEpsilonMultiplier;
    }

    void SetData(ValueControll value)
    {
        if (string.Equals(value.gameObject.name, "erodeSize"))
            value.SetValue(_ErodeSize.ToString());
        else if (string.Equals(value.gameObject.name, "erodeIterations"))
            value.SetValue(_ErodeIterations.ToString());
        else if (string.Equals(value.gameObject.name, "dilateSize"))
            value.SetValue(_DilateSize.ToString());
        else if (string.Equals(value.gameObject.name, "dilateIterations"))
            value.SetValue(_DilateIterations.ToString());
        else if (string.Equals(value.gameObject.name, "threshold"))
            value.SetValue(_Threshold.ToString());
        else if (string.Equals(value.gameObject.name, "minContourSize"))
            value.SetValue(_MinContourSize.ToString());
        else if (string.Equals(value.gameObject.name, "maxContourSize"))
            value.SetValue(_MaxContourSize.ToString());
        else if (string.Equals(value.gameObject.name, "circleSamplingRate"))
            value.SetValue(_CircleSamplingRate.ToString());
        else if (string.Equals(value.gameObject.name, "circleValidRate"))
            value.SetValue(_CircleValidRate.ToString());
        else if (string.Equals(value.gameObject.name, "circleErrorThreshold"))
            value.SetValue(_CircleErrorThreshold.ToString());
        else if (string.Equals(value.gameObject.name, "quadEpsilonMultiplier"))
            value.SetValue(_QuadEpsilonMultiplier.ToString());
    }

    void ValueUp(bool isUp , ValueControll value)
    {
        Debug.Log(value.gameObject.name);
        var input = value.GetValue();
        if (input == "")
        {
            input = "0";
        }

        float val = float.Parse(input);
        float Acc = 0;
        if (string.Equals(value.gameObject.name, "erodeSize"))
            Acc = 1;
        else if (string.Equals(value.gameObject.name, "erodeIterations"))
            Acc = 1;
        else if (string.Equals(value.gameObject.name, "dilateSize"))
            Acc = 1;
        else if (string.Equals(value.gameObject.name, "dilateIterations"))
            Acc = 1;
        else if (string.Equals(value.gameObject.name, "threshold"))
            Acc = 1;
        else if (string.Equals(value.gameObject.name, "minContourSize"))
            Acc = 0.1f;
        else if (string.Equals(value.gameObject.name, "maxContourSize"))
            Acc = 0.1f;
        else if (string.Equals(value.gameObject.name, "circleSamplingRate"))
            Acc = 0.1f;
        else if (string.Equals(value.gameObject.name, "circleValidRate"))
            Acc = 0.1f;
        else if (string.Equals(value.gameObject.name, "circleErrorThreshold"))
            Acc = 0.1f;
        else if (string.Equals(value.gameObject.name, "quadEpsilonMultiplier"))
            Acc = 0.1f;
        val += isUp ? Acc : -Acc;
        value.SetValue(val.ToString());
    }

    void ValueChange(ValueControll value)
    {
        if (string.Equals(value.gameObject.name, "erodeSize"))
        {
            string var = value.GetValue();
            var size = int.Parse(var);
            setting.ErodeSize = size;
        }
        else if (string.Equals(value.gameObject.name, "erodeIterations"))
        {
            string var = value.GetValue();
            var size = int.Parse(var);
            setting.ErodeIterations = size;
        }
        else if (string.Equals(value.gameObject.name, "dilateSize"))
        {
            string var = value.GetValue();
            var size = int.Parse(var);
            setting.DilateSize = size;
        }
        else if (string.Equals(value.gameObject.name, "dilateIterations"))
        {
            string var = value.GetValue();
            var size = int.Parse(var);
            setting.DilateIterations = size;
        }
        else if (string.Equals(value.gameObject.name, "threshold"))
        {
            string var = value.GetValue();
            var size = int.Parse(var);
            setting.Threshold = size;
        }
        else if (string.Equals(value.gameObject.name, "minContourSize"))
        {
            string var = value.GetValue();
            var size = double.Parse(var);
            setting.MinContourSize = size;
        }
        else if (string.Equals(value.gameObject.name, "maxContourSize"))
        {
            string var = value.GetValue();
            var size = double.Parse(var);
            setting.MaxContourSize = size;
        }
        else if (string.Equals(value.gameObject.name, "circleSamplingRate"))
        {
            string var = value.GetValue();
            var size = float.Parse(var);
            setting.CircleSamplingRate = size;
        }
        else if (string.Equals(value.gameObject.name, "circleValidRate"))
        {
            string var = value.GetValue();
            var size = float.Parse(var);
            setting.CircleValidRate = size;
        }
        else if (string.Equals(value.gameObject.name, "circleErrorThreshold"))
        {
            string var = value.GetValue();
            var size = float.Parse(var);
            setting.CircleErrorThreshold = size;
        }
        else if (string.Equals(value.gameObject.name, "quadEpsilonMultiplier"))
        {
            string var = value.GetValue();
            var size = double.Parse(var);
            setting.QuadEpsilonMultiplier = size;
        }
    }

    public void ResetOption(CellBig.UI.Event.OptionReset msg)
    {
       setting.ErodeSize= _ErodeSize;
       setting.ErodeIterations= _ErodeIterations;
       setting.DilateSize= _DilateSize;
       setting.DilateIterations= _DilateIterations;
       setting.Threshold= _Threshold;
       setting.MinContourSize= _MinContourSize;
       setting.MaxContourSize= _MaxContourSize;
       setting.CircleSamplingRate= _CircleSamplingRate;
       setting.CircleValidRate= _CircleValidRate;
       setting.CircleErrorThreshold= _CircleErrorThreshold;
       setting.QuadEpsilonMultiplier= _QuadEpsilonMultiplier;
    }

}
