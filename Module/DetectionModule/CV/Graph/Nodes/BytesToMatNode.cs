using UnityEngine;
using OpenCVForUnity;

namespace JHchoi.Module.Detection.CV
{
    public class BytesToMatNode : CVNodeBase<Output.Mat, byte[], Mat>
    {
        private readonly Mat _inputMat;
        private readonly Mat _outputMat;
        private readonly Size _size;

        public BytesToMatNode()
        {
            _inputMat = new Mat(Settings.InputResolution.y, Settings.InputResolution.x, CvType.CV_8UC3);
            _outputMat = new Mat(Settings.OutputResolution.y, Settings.OutputResolution.x, CvType.CV_8UC3);
            _size = _outputMat.size();
        }

        ~BytesToMatNode()
        {
            _inputMat.Dispose();
        }
        
        protected override Mat RunImpl(byte[] input, int deltaInterval)
        {
            _inputMat.put(0, 0, input);
            Core.flip(_inputMat, _inputMat, 0);
            Imgproc.resize(_inputMat, _outputMat, _size, 0, 0, Imgproc.INTER_AREA);

            return _outputMat;
        }

        protected override void Copy(Mat from, ref Mat to)
        {
            if (to == null)
                to = from.clone();
            else
                from.copyTo(to);
        }
    }
}
