using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Constants
{
    public enum LocalizingType
    {
        KR,
        JP,
        EN,
        CH,
    }

    public enum AnswerType
    {
        X = 0,
        O,
    }

    public enum ResultType
    {
        Bed = 0,
        Good,
        VeryGood,
        Max,
    }

    public enum E_OPENCV_MOD
    {
        E_KINECT = 0,
        E_WEBCAM,
        E_REALSENSE,
        E_MAX,
    }
}
