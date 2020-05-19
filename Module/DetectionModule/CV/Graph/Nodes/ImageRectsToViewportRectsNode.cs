using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class ImageRectsToViewportRects : CVNodeBase<Output.ViewportRects, List<OpenCVForUnity.Rect>, List<UnityEngine.Rect>>
    {
        private readonly List<UnityEngine.Rect> _rects = new List<UnityEngine.Rect>();
        private Mat _minMat = new Mat(3, 1, CvType.CV_64F);
        private Mat _maxMat = new Mat(3, 1, CvType.CV_64F);
        private double[] _minPoint = new double[3];
        private double[] _maxPoint = new double[3];

        public ImageRectsToViewportRects()
        {

        }

        protected override List<UnityEngine.Rect> RunImpl(List<OpenCVForUnity.Rect> input, int deltaInterval)
        {
            _rects.Clear();

            Vector2Int resolution = Settings.OutputResolution;
            Mat transform = Settings.OuterPerspectiveTransform.inv() * Settings.InnerPerspectiveTransform;
            foreach (var imageRect in input)
            {
                _minPoint[0] = imageRect.x;
                _minPoint[1] = imageRect.y;
                _minPoint[2] = 1.0;
                _maxPoint[0] = imageRect.x + imageRect.width;
                _maxPoint[1] = imageRect.y + imageRect.height;
                _maxPoint[2] = 1.0;

                _minMat.put(0, 0, _minPoint);
                _maxMat.put(0, 0, _maxPoint);
                //_minMat = outerInv * _minMat;
                //_maxMat = outerInv * _maxMat;
                //_minMat.get(0, 0, _minPoint);
                //_maxMat.get(0, 0, _maxPoint);
                //_minMat = inner * (_minMat / _minPoint[2]);
                //_maxMat = inner * (_maxMat / _minPoint[2]);
                Core.perspectiveTransform(_minMat, _minMat, transform);
                Core.perspectiveTransform(_maxMat, _maxMat, transform);
                _minMat.get(0, 0, _minPoint);
                _maxMat.get(0, 0, _maxPoint);

                UnityEngine.Rect viewportRect = new UnityEngine.Rect();
                viewportRect.x = (float)(_minPoint[0] / resolution.x);
                viewportRect.y = 1f - (float)(_maxPoint[1] / resolution.y);
                viewportRect.xMax = (float)(_maxPoint[0] / resolution.x);
                viewportRect.yMax = 1f - (float)(_minPoint[1] / resolution.y);
                _rects.Add(viewportRect);
            }

            return _rects;
        }

        protected override void Copy(List<UnityEngine.Rect> from, ref List<UnityEngine.Rect> to)
        {
            if (to == null)
                to = new List<UnityEngine.Rect>(from);
            else
                to.AddRange(from);
        }

        protected override void Clear(List<UnityEngine.Rect> output)
        {
            output.Clear();
        }
    }
}
