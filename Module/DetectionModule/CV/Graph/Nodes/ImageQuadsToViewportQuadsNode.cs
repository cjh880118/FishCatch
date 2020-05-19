using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

namespace JHchoi.Module.Detection.CV
{
    public class ImageQuadsToViewportQuadsNode : CVNodeBase<Output.ViewportQuads, List<Point>, List<Vector2>>
    {
        private readonly List<Vector2> _points = new List<Vector2>();
        private Mat _pointMat = new Mat(3, 1, CvType.CV_64F);
        private double[] _point = new double[3];

        public ImageQuadsToViewportQuadsNode()
        {

        }

        protected override List<Vector2> RunImpl(List<Point> input, int deltaInterval)
        {
            _points.Clear();

            Vector2Int resolution = Settings.OutputResolution;
            Mat transform = Settings.OuterPerspectiveTransform.inv() * Settings.InnerPerspectiveTransform;
            foreach (var p in input)
            {
                _point[0] = p.x;
                _point[1] = p.y;
                _point[2] = 1.0;

                _pointMat.put(0, 0, _point);
                //_pointMat = outerInv * _pointMat;
                //_pointMat.get(0, 0, _point);
                //_pointMat = inner * (_pointMat / _point[2]);
                //_pointMat.get(0, 0, _point);
                Core.perspectiveTransform(_pointMat, _pointMat, transform);
                _pointMat.get(0, 0, _point);

                Vector2 point = new Vector2()
                {
                    x = (float)_point[0] / resolution.x,
                    y = 1f - (float)_point[1] / resolution.y
                };

                _points.Add(point);
            }

            return _points;
        }

        protected override void Copy(List<Vector2> from, ref List<Vector2> to)
        {
            if (to == null)
                to = new List<Vector2>(from);            
            else
                to.AddRange(from);
        }

        protected override void Clear(List<Vector2> output)
        {
            output.Clear();
        }
    }
}
