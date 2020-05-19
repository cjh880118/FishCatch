using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Intel.RealSense;
using System;
using UnityEngine.Events;
using System.Threading;

// 리얼센스 뎁스 맵 반환
public class RsDepthSourceManager : MonoBehaviour
{
    private static TextureFormat Convert(Format lrsFormat)
    {
        switch (lrsFormat)
        {
            case Format.Z16: return TextureFormat.R16;
            case Format.Disparity16: return TextureFormat.R16;
            case Format.Rgb8: return TextureFormat.RGB24;
            case Format.Rgba8: return TextureFormat.RGBA32;
            case Format.Bgra8: return TextureFormat.BGRA32;
            case Format.Y8: return TextureFormat.Alpha8;
            case Format.Y16: return TextureFormat.R16;
            case Format.Raw16: return TextureFormat.R16;
            case Format.Raw8: return TextureFormat.Alpha8;
            case Format.Yuyv:
            case Format.Bgr8:
            case Format.Raw10:
            case Format.Xyz32f:
            case Format.Uyvy:
            case Format.MotionRaw:
            case Format.MotionXyz32f:
            case Format.GpioRaw:
            case Format.Any:
            default:
                throw new Exception(string.Format("{0} librealsense format: " + lrsFormat + ", is not supported by Unity"));
        }
    }

    private static int BPP(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.ARGB32:
            case TextureFormat.BGRA32:
            case TextureFormat.RGBA32:
                return 32;
            case TextureFormat.RGB24:
                return 24;
            case TextureFormat.R16:
                return 16;
            case TextureFormat.R8:
            case TextureFormat.Alpha8:
                return 8;
            default:
                throw new ArgumentException("unsupported format {0}", format.ToString());

        }
    }

    

    public ushort[] ResizeTexture(ushort[] origin, int originW, int originH, int w, int h)
    {
        ushort[] resizeBuffer = new ushort[w * h];

        float incX = (1.0f / (float)w);
        float incY = (1.0f / (float)h);
        int originLength = originW * originH;
        for (int px = 0; px < resizeBuffer.Length; px++)
        {
            float normalX = (px % w) * incX;
            float normalY = (px / w) * incY;

            int originX = (int)(normalX * originW);//Clamp(normalX * originW, 0, originW - 1);
            int originY = (int)(normalY * originH);//Clamp(normalY * originH, 0, originH - 1);
            int index = (int)(originY * originW + (originW - originX - 1));//Clamp(originY * originW + originX, 0, originLength - 1);
                                                                           //              originY * originW + originX
            resizeBuffer[px] = origin[index];
        }
        return resizeBuffer;
    }

    

//    void Awake()
//    {
//        Init();
//    }
//    void Init()
//    {
//#if REALSENCE_B
//        threadId = Thread.CurrentThread.ManagedThreadId;
//        _videoStreamFilter = new RsVideoStreamRequest() { Stream = Stream.Depth, Format = Format.Z16, StreamIndex = 0 };
//        _currVideoStreamFilter = _videoStreamFilter.Clone();

//        mRsDevice.OnStart += OnStartStreaming;
//        mRsDevice.OnStop += OnStopStreaming;
//#else
//            //_Sensor = KinectSensor.GetDefault();
//            //if (_Sensor != null)
//            //{
//            //    _Reader = _Sensor.DepthFrameSource.OpenReader();
//            //    _Data = new ushort[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];

//            //    if (!_Sensor.IsOpen)
//            //    {
//            //        _Sensor.Open();
//            //    }
//            //}
//#endif
//    }

//    void Update()
//    {
//#if REALSENCE_B
//        if (!_currVideoStreamFilter.Equals(_videoStreamFilter))
//        {
//            ResetTexture(_videoStreamFilter);
//        }
//        if (EventF.WaitOne(0))
//        {
//            try

//            {
//                if (byte_Data != null)
//                    texture2D_.LoadRawTextureData(byte_Data);
//            }
//            catch
//            {
//                OnStopStreaming();
//                Debug.LogError("Error loading texture data, check texture and stream formats");
//                throw;
//            }
//            texture2D_.Apply();
//        }
//#else
//            if (_Reader != null)
//            {
//                var frame = _Reader.AcquireLatestFrame();
//                if (frame != null)
//                {
//                    frame.CopyFrameDataToArray(_Data);
//                    frame.Dispose();
//                    frame = null;
//                }
//            }
//            else
//            {
//                Init();
//            }
//#endif
//    }

//    void OnApplicationQuit()
//    {
//        //if (_Reader != null)
//        //{
//        //    _Reader.Dispose();
//        //    _Reader = null;
//        //}

//        //if (_Sensor != null)
//        //{
//        //    if (_Sensor.IsOpen)
//        //    {
//        //        _Sensor.Close();
//        //    }

//        //    _Sensor = null;
//        //}
//    }

//    private void UpdateData(Frame frame)
//    {
//        var vidFrame = frame as VideoFrame;
//        byte_Data = byte_Data ?? new byte[vidFrame.Stride * vidFrame.Height];
//        ushort[] shortOrg = new ushort[byte_Data.Length / 2];
//        vidFrame.CopyTo(byte_Data);

//        Buffer.BlockCopy(byte_Data, 0, shortOrg, 0, byte_Data.Length);
//        ushor_Data = ResizeTexture(shortOrg, 640, 480, 512, 424);

        
       
//        if ((vidFrame as Frame) != frame)
//            vidFrame.Dispose();
//    }

//    private void ResetTexture(RsVideoStreamRequest vsr)
//    {
//        if (texture2D_ != null)
//        {
//            Destroy(texture2D_);
//        }

//        texture2D_ = new Texture2D(vsr.Width, vsr.Height, Convert(vsr.Format), false, true)
//        {
//            wrapMode = TextureWrapMode.Clamp,
//            filterMode = filterMode
//        };

//        _currVideoStreamFilter = vsr.Clone();

//        texture2D_.Apply();
//    }

//#if REALSENCE_B
//    //protected virtual void OnStopStreaming()
//    //{
//    //    mRsDevice.OnNewSample -= OnNewSampleUnityThread;
//    //    mRsDevice.OnNewSample -= OnNewSampleThreading;

//    //    mRsDevice.OnNewSample -= OnNewSample; /// 

//    //    if (q != null)
//    //    {
//    //        q.Dispose();
//    //        q = null;
//    //    }

//    //    EventF.Reset();
//    //    byte_Data = null;
//    //}

//    //protected virtual void OnStartStreaming(PipelineProfile activeProfile)
//    //{
//    //    if (mRsDevice.processMode == RsDevice.ProcessMode.UnityThread)
//    //    {
//    //        UnityEngine.Assertions.Assert.AreEqual(threadId, Thread.CurrentThread.ManagedThreadId);
//    //        mRsDevice.OnNewSample += OnNewSampleUnityThread;
//    //    }
//    //    else
//    //    {

//    //        mRsDevice.OnNewSample += OnNewSampleThreading;
//    //    }
//    //}

//    private void OnNewSampleThreading(Frame frame)
//    {
//        if (HasRequestConflict(frame))
//            return;
//        if (HasTextureConflict(frame))
//            return;
//        UpdateData(frame);
//        EventF.Set();
//    }

//    private void OnNewSampleUnityThread(Frame frame)
//    {

//        var vidFrame = frame as VideoFrame;


//        if (HasRequestConflict(vidFrame))
//            return;
//        if (HasTextureConflict(frame))
//            return;

//        UnityEngine.Assertions.Assert.AreEqual(threadId, Thread.CurrentThread.ManagedThreadId);

//        texture2D_.LoadRawTextureData(vidFrame.Data, vidFrame.Stride * vidFrame.Height);

//        if ((vidFrame as Frame) != frame)
//            vidFrame.Dispose();

//        EventF.Set();
//    }

//    private bool HasRequestConflict(Frame frame)
//    {
//        if (!(frame is VideoFrame))
//        {
//            Debug.LogError("NOT VideoFrame");
//            return true;
//        }
//        VideoFrame vf = frame as VideoFrame;
//        if (_videoStreamFilter.Stream != vf.Profile.Stream ||
//            _videoStreamFilter.Format != vf.Profile.Format ||
//            (_videoStreamFilter.StreamIndex != vf.Profile.Index && _videoStreamFilter.StreamIndex != 0))
//            return true;

//        return false;
//    }

//    private bool HasTextureConflict(Frame frame)
//    {
//        var vidFrame = frame as VideoFrame;
//        if (_videoStreamFilter.Width == vidFrame.Width && _videoStreamFilter.Height == vidFrame.Height && _videoStreamFilter.Format == vidFrame.Profile.Format)
//            return false;
//        _videoStreamFilter.CopyProfile(vidFrame);
//        byte_Data = null;
//        return true;
//    }
//#endif

    // ======================================== 현철님 =======================================


    public Stream _stream;
    public Format _format;
    public int _streamIndex;
    public FilterMode filterMode = FilterMode.Point;

    public RsFrameProvider Source;

   // private RsVideoStreamRequest _videoStreamFilter;
 //   private RsVideoStreamRequest _currVideoStreamFilter;
    public Texture2D texture2D_;

  //  readonly AutoResetEvent EventF = new AutoResetEvent(false);
    protected int threadId;
    protected bool bound;
    private byte[] byte_Data; // 리얼센스 데이터용
    private ushort[] ushor_Data; // 뎁스데이터용





   

    [System.Serializable]
    public class TextureEvent : UnityEvent<Texture> { }
    

    [Space]
    public TextureEvent textureBinding;

    FrameQueue q;
    Predicate<Frame> matcher;

    void Start()
    {
        Source.OnStart += OnStartStreaming;
        Source.OnStop += OnStopStreaming;

       // Source.ActiveProfile.
    }

    void OnDestroy()
    {
        if (texture2D_ != null)
        {
            Destroy(texture2D_);
            texture2D_ = null;
        }

        if (q != null)
        {
            q.Dispose();
        }
    }

    protected void OnStopStreaming()
    {
        Source.OnNewSample -= OnNewSample;
        if (q != null)
        {
            q.Dispose();
            q = null;
        }
        byte_Data = null;

    }

    public void OnStartStreaming(PipelineProfile activeProfile)
    {
        q = new FrameQueue(1);
        matcher = new Predicate<Frame>(Matches);
        Source.OnNewSample += OnNewSample;
    }

    private bool Matches(Frame f)
    {
        using (var p = f.Profile)
            return p.Stream == _stream && p.Format == _format && p.Index == _streamIndex;
    }

    void OnNewSample(Frame frame)
    {
        try
        {
            if (frame.IsComposite)
            {
                using (var fs = frame.As<FrameSet>())
                using (var f = fs.FirstOrDefault(matcher))
                {
                    if (f != null)
                        q.Enqueue(f);
                    return;
                }
            }

            if (!matcher(frame))
                return;

            using (frame)
            {
                q.Enqueue(frame);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            // throw;
        }

    }

    bool HasTextureConflict(VideoFrame vf)
    {
        return !texture2D_ ||
            texture2D_.width != vf.Width ||
            texture2D_.height != vf.Height ||
            BPP(texture2D_.format) != vf.BitsPerPixel;
    }

    protected void LateUpdate()
    {

        if (q != null)
        {

            VideoFrame frame;
            if (q.PollForFrame<VideoFrame>(out frame))
                using (frame)
                {
                    ProcessFrame(frame);
                    var vidFrame = frame as VideoFrame;
                    byte_Data = byte_Data ?? new byte[vidFrame.Stride * vidFrame.Height];
                    ushort[] shortOrg = new ushort[byte_Data.Length / 2];
                    vidFrame.CopyTo(byte_Data);

                    Buffer.BlockCopy(byte_Data, 0, shortOrg, 0, byte_Data.Length);
                    ushor_Data = shortOrg;
                    //Debug.Log(ushor_Data.Length);
                    //Debug.Log("asd:" + ushor_Data[100]);
                    //   ushor_Data = ResizeTexture(shortOrg, 640, 480, 640, 480);

                }

            
           
        }
    }

    private void ProcessFrame(VideoFrame frame)
    {

        if (HasTextureConflict(frame))
        {
            if (texture2D_ != null)
            {
                Destroy(texture2D_);
            }

            using (var p = frame.Profile)
            {
                bool linear = (QualitySettings.activeColorSpace != ColorSpace.Linear)
                    || (p.Stream != Stream.Color && p.Stream != Stream.Infrared);
                texture2D_ = new Texture2D(frame.Width, frame.Height, Convert(p.Format), false, linear)
                {
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = filterMode
                };
            }

            textureBinding.Invoke(texture2D_);
        }

        texture2D_.LoadRawTextureData(frame.Data, frame.Stride * frame.Height);
        texture2D_.Apply();
    }

    public ushort[] GetData() // 실질적인 뎁스 데이터 
    {
        return ushor_Data;
    }








    // ================================================= 내코드 =====================================================
    //private static TextureFormat Convert(Format lrsFormat)
    //{
    //    switch (lrsFormat)
    //    {
    //        case Format.Z16: return TextureFormat.R16;
    //        case Format.Disparity16: return TextureFormat.R16;
    //        case Format.Rgb8: return TextureFormat.RGB24;
    //        case Format.Rgba8: return TextureFormat.RGBA32;
    //        case Format.Bgra8: return TextureFormat.BGRA32;
    //        case Format.Y8: return TextureFormat.Alpha8;
    //        case Format.Y16: return TextureFormat.R16;
    //        case Format.Raw16: return TextureFormat.R16;
    //        case Format.Raw8: return TextureFormat.Alpha8;
    //        case Format.Disparity32: return TextureFormat.RFloat;
    //        case Format.Yuyv:
    //        case Format.Bgr8:
    //        case Format.Raw10:
    //        case Format.Xyz32f:
    //        case Format.Uyvy:
    //        case Format.MotionRaw:
    //        case Format.MotionXyz32f:
    //        case Format.GpioRaw:
    //        case Format.Any:
    //        default:
    //            throw new ArgumentException(string.Format("librealsense format: {0}, is not supported by Unity", lrsFormat));
    //    }
    //}

    //private static int BPP(TextureFormat format)
    //{
    //    switch (format)
    //    {
    //        case TextureFormat.ARGB32:
    //        case TextureFormat.BGRA32:
    //        case TextureFormat.RGBA32:
    //            return 32;
    //        case TextureFormat.RGB24:
    //            return 24;
    //        case TextureFormat.R16:
    //            return 16;
    //        case TextureFormat.R8:
    //        case TextureFormat.Alpha8:
    //            return 8;
    //        default:
    //            throw new ArgumentException("unsupported format {0}", format.ToString());

    //    }
    //}

    //public RsFrameProvider Source;

    //[System.Serializable]
    //public class TextureEvent : UnityEvent<Texture> { }

    //public Stream _stream;
    //public Format _format;
    //public int _streamIndex;

    //public FilterMode filterMode = FilterMode.Point;

    //public Texture2D texture;


    //[Space]
    //public TextureEvent textureBinding;

    //FrameQueue q;
    //Predicate<Frame> matcher;

    //void Start()
    //{
    //    Source.OnStart += OnStartStreaming;
    //    Source.OnStop += OnStopStreaming;
    //}

    //void OnDestroy()
    //{
    //    if (texture != null)
    //    {
    //        Destroy(texture);
    //        texture = null;
    //    }

    //    if (q != null)
    //    {
    //        q.Dispose();
    //    }
    //}

    //protected void OnStopStreaming()
    //{
    //    Source.OnNewSample -= OnNewSample;
    //    if (q != null)
    //    {
    //        q.Dispose();
    //        q = null;
    //    }
    //}

    //public void OnStartStreaming(PipelineProfile activeProfile)
    //{
    //    q = new FrameQueue(1);
    //    matcher = new Predicate<Frame>(Matches);
    //    Source.OnNewSample += OnNewSample;
    //}

    //private bool Matches(Frame f)
    //{
    //    using (var p = f.Profile)
    //        return p.Stream == _stream && p.Format == _format && p.Index == _streamIndex;
    //}

    //void OnNewSample(Frame frame)
    //{
    //    try
    //    {
    //        if (frame.IsComposite)
    //        {
    //            using (var fs = frame.As<FrameSet>())
    //            using (var f = fs.FirstOrDefault(matcher))
    //            {
    //                if (f != null)
    //                    q.Enqueue(f);
    //                return;
    //            }
    //        }

    //        if (!matcher(frame))
    //            return;

    //        using (frame)
    //        {
    //            q.Enqueue(frame);
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogException(e);
    //        // throw;
    //    }

    //}

    //bool HasTextureConflict(VideoFrame vf)
    //{
    //    return !texture ||
    //        texture.width != vf.Width ||
    //        texture.height != vf.Height ||
    //        BPP(texture.format) != vf.BitsPerPixel;
    //}

    //protected void LateUpdate()
    //{

    //    if (q != null)
    //    {
    //        VideoFrame frame;
    //        if (q.PollForFrame<VideoFrame>(out frame))
    //            using (frame)
    //                ProcessFrame(frame);
    //    }
    //}

    //private void ProcessFrame(VideoFrame frame)
    //{

    //    if (HasTextureConflict(frame))
    //    {
    //        if (texture != null)
    //        {
    //            Destroy(texture);
    //        }

    //        using (var p = frame.Profile)
    //        {
    //            bool linear = (QualitySettings.activeColorSpace != ColorSpace.Linear)
    //                || (p.Stream != Stream.Color && p.Stream != Stream.Infrared);
    //            texture = new Texture2D(frame.Width, frame.Height, Convert(p.Format), false, linear)
    //            {
    //                wrapMode = TextureWrapMode.Clamp,
    //                filterMode = filterMode
    //            };
    //        }

    //        textureBinding.Invoke(texture);
    //    }

    //    texture.LoadRawTextureData(frame.Data, frame.Stride * frame.Height);
    //    texture.Apply();
    //}




}
