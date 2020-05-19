using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JHchoi.Module.VideoDevice;
using OpenCVForUnity;

public class ConnectionTest : MonoBehaviour
{
    [SerializeField]
    private RawImage _image;
    [SerializeField]
    private int _erodeSize;
    [SerializeField]
    private int _dilateSize;
    [SerializeField]
    private double _threshold;
    [SerializeField]
    private double _minContourArea;
    [SerializeField]
    private double _maxContourArea;
    [SerializeField]
    private double _learningRate = -1;

    private byte[] _frameBuffer;
    private Mat _rgbMat;
    private Mat _fgMask;
    private Texture2D _texture;
    private BackgroundSubtractorMOG2 _bg;
    private bool _firstInput = true;
    private double _nextLearningRate;

    private void Awake()
    {
        Message.AddListener<VideoDeviceEvent>(e => Debug.Log(e.Type));
    }

    private void OnEnable()
    {
        Message.Send(new VideoDeviceConnectRequest(this, OnConnected));
    }

    private void OnDisable()
    {
        Message.Send(new VideoDeviceStopRequest(this));
        Message.Send(new VideoDeviceDisconnectRequest(this));
    }

    private void OnConnected(Vector2Int resolution, bool reconnection)
    {
        _frameBuffer = new byte[resolution.x * resolution.y * 3];
        _bg = Video.createBackgroundSubtractorMOG2();
        _rgbMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3);
        _fgMask = new Mat(resolution.y, resolution.x, CvType.CV_8UC1);
        _texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
        _image.texture = _texture;

        Message.Send(new VideoDevicePlayRequest(this, _frameBuffer));
    }

    private void Update()
    {
        if (_frameBuffer != null)
        {
            _rgbMat.put(0, 0, _frameBuffer);
            Core.flip(_rgbMat, _rgbMat, 0);

            _bg.apply(_rgbMat, _fgMask, _nextLearningRate);

            Imgproc.threshold(_fgMask, _fgMask, _threshold, 255, Imgproc.THRESH_BINARY);
            Imgproc.erode(_fgMask, _fgMask, Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(_erodeSize, _erodeSize)));
            Imgproc.dilate(_fgMask, _fgMask, Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(_dilateSize, _dilateSize)));

            List<OpenCVForUnity.Rect> rects = new List<OpenCVForUnity.Rect>();
            List<MatOfPoint> contours = new List<MatOfPoint>();
            Imgproc.findContours(_fgMask, contours, new Mat(), Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
            foreach (var contour in contours)
            {
                double area = Imgproc.contourArea(contour);
                if (area >= _minContourArea && area <= _maxContourArea)
                    rects.Add(Imgproc.boundingRect(contour));
            }
            
            Mat m = _fgMask.clone();
            Imgproc.cvtColor(m, m, Imgproc.COLOR_GRAY2RGB);

            Scalar color = new Scalar(255, 0, 0);
            foreach (var rect in rects)
                Imgproc.rectangle(m, rect, color);

            Utils.fastMatToTexture2D(m, _texture);

            _nextLearningRate = rects.Count > 0 ? 0 : _learningRate;
        }
    }
}
