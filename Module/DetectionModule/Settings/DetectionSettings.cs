﻿using System;
using UnityEngine;
using JHchoi.Module.Detection.CV;

namespace JHchoi.Module.Detection
{
    [Serializable]
    public class DetectionSettings
    {
        [SerializeField]
        private BaseSettings _base;
        [SerializeField]
        private CVSettings _cv;

        public void Setup(Vector2Int resolution)
        {
            _cv.Setup(resolution);
        }

        public void Unsetup()
        {
            _cv.Unsetup();
        }

        public void Update()
        {
            _cv.Update();
        }

        public BaseSettings Base => _base;
        public CVSettings CV => _cv;
    }
}