﻿using System;
using UnityEngine;

namespace JHchoi.Module.VideoDevice
{
    [Serializable]
    public class VideoDeviceSettings
    {
        public VideoDeviceAPI DeviceAPI;
        public string DeviceName;
        public float ReconnectionDelay;
        public Vector2Int RequestedResolution;

        public bool FlipX;
        public bool FlipY;
        public double Contrast;
        public double Exposure;
        public double AutoExposure;
        public double Focus;
        public double AutoFocus;
    }
}