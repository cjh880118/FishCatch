﻿using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using OpenCVForUnity;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CellBig.Module.VideoDevice
{
    public class VideoDeviceModule : IModule, ISerializationCallbackReceiver
    {
        private class ConnectorInfo
        {
            public Action<Vector2Int, bool> OnConnected;
            public byte[] Buffer;
            public bool Connected;
        }

        [SerializeField]
        private string _settingsPath = "Setting/video_settings.json";
        [SerializeField]
        private VideoDeviceSettings _videoDeviceSettings;
        [SerializeField]
        private KeyCode _saveKey = KeyCode.Space;
        
        private VideoCapture _videoCapture;
        private IVideoDevice _device;
        private readonly Dictionary<object, ConnectorInfo> _connectorMap = new Dictionary<object, ConnectorInfo>();
        private readonly List<byte[]> _buffers = new List<byte[]>();
        private byte[] _frameBuffer;
        private Mat _frameMat;
        private Thread _thread;
        private Coroutine _connectCrt;

#if UNITY_EDITOR
        private void OnPauseChanged(PauseState state)
        {
            if (state == PauseState.Paused)
            {
                AbortThread();
            }
            else
            {
                StartThread();
            }
        }
#endif

        private void SaveSettings()
        {
            string json = JsonUtility.ToJson(_videoDeviceSettings, true);
            File.WriteAllText(Application.streamingAssetsPath + '/' + _settingsPath, json);
        }

        private void LoadSettings()
        {
            if (!File.Exists(Application.streamingAssetsPath + '/' + _settingsPath))
                SaveSettings();

            string json = File.ReadAllText(Application.streamingAssetsPath + '/' + _settingsPath);
            JsonUtility.FromJsonOverwrite(json, _videoDeviceSettings);
        }

        protected override void OnLoadStart()
        {
            SetResourceLoadComplete();

            LoadSettings();
            RegisterCallbacks();

            InitDevice();
            InitModel();

#if UNITY_EDITOR
            EditorApplication.pauseStateChanged += OnPauseChanged;
#endif
        }

        private void OnDestroy()
        {
            AbortThread();
            ReleaseBuffer();
            UnregisterCallbacks();
            
            if (_device != null)
            {
                if (_connectCrt != null)
                    StopCoroutine(_connectCrt);
                _device.Stop();
                _device.Disconnect();
            }

            if (_videoCapture != null)
            {
                _videoCapture.release();
                _videoCapture.Dispose();
            }

#if UNITY_EDITOR
            EditorApplication.pauseStateChanged -= OnPauseChanged;
#endif
        }

        private void InitDevice()
        {
            IVideoDevice device = null;
            switch (_videoDeviceSettings.DeviceAPI)
            {
                case VideoDeviceAPI.UQ3388:
                    device = new UQ3388();
                    break;

                case VideoDeviceAPI.Unity:
                    device = new UnityCam();
                    break;

                case VideoDeviceAPI.OpenCV:
                    device = new OpenCVCam();
                    break;
            }
            _device = device;
            _videoCapture = new VideoCapture(0);
        }

        private void InitModel()
        {
            if (Model.First<VideoDeviceInfoModel>() == null)
                new VideoDeviceInfoModel(_videoDeviceSettings, _device);
        }

        private void InitBuffer()
        {
            int bufferLen = _device.Resolution.x * _device.Resolution.y * 3;
            _frameBuffer = new byte[bufferLen];
            _frameMat = new Mat(_device.Resolution.y, _device.Resolution.x, CvType.CV_8UC3);
        }

        private void ReleaseBuffer()
        {
            if (_frameMat != null && !_frameMat.IsDisposed)
            {
                _frameMat.release();
                _frameMat.Dispose();
            }
        }
        
        private void RegisterCallbacks()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            Message.AddListener<SaveSettings>(OnSaveSettings);
            Message.AddListener<VideoDeviceConnectRequest>(OnConnectRequested);
            Message.AddListener<VideoDeviceDisconnectRequest>(OnDisconnectRequested);
            Message.AddListener<VideoDevicePlayRequest>(OnPlayRequested);
            Message.AddListener<VideoDeviceStopRequest>(OnStopRequested);
            Message.AddListener<RefreshVideoDeviceProps>(OnRefreshPropsRequired);
        }

        private void UnregisterCallbacks()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            Message.RemoveListener<SaveSettings>(OnSaveSettings);
            Message.RemoveListener<VideoDeviceConnectRequest>(OnConnectRequested);
            Message.RemoveListener<VideoDeviceDisconnectRequest>(OnDisconnectRequested);
            Message.RemoveListener<VideoDevicePlayRequest>(OnPlayRequested);
            Message.RemoveListener<VideoDeviceStopRequest>(OnStopRequested);
            Message.RemoveListener<RefreshVideoDeviceProps>(OnRefreshPropsRequired);
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
        {
            if (_connectorMap.Count(pair => pair.Value.Buffer != null) > 0 && _videoDeviceSettings.DeviceAPI == VideoDeviceAPI.Unity && !_device.IsPlaying)
            {
                _device.Play();
            }
        }

        private void OnSaveSettings(SaveSettings msg)
        {
            SaveSettings();
        }

        private void OnConnectRequested(VideoDeviceConnectRequest msg)
        {
            if (!_connectorMap.ContainsKey(msg.Connector))
            {
                _connectorMap.Add(msg.Connector, new ConnectorInfo() { OnConnected = msg.OnConnected });
                if (_connectCrt == null)
                    _connectCrt = StartCoroutine(CheckConnection());
            }
        }

        private void OnDisconnectRequested(VideoDeviceDisconnectRequest msg)
        {
            //if (_device != null && _device.IsConnected)
            //{
            if (_connectorMap.ContainsKey(msg.Connector))
            {
                if (_connectorMap.Count == 1 && _connectorMap[msg.Connector].Connected)
                {
                    StopCoroutine(_connectCrt);
                    _connectCrt = null;

                    AbortThread();
                    ReleaseBuffer();
                    _device.Disconnect();

                    if (!_device.IsConnected)
                    {
                        _device = null;
                        lock (_buffers)
                        {
                            _buffers.Clear();
                        }
                        _connectorMap.Clear();
                    }
                    else if (_device != null)
                    {
                        Debug.LogError($"[{nameof(VideoDeviceModule)}] 비디오 장치 연결 해제 요청에 실패했습니다.");
                    }
                }
                else
                {
                    _connectorMap.Remove(msg.Connector);
                    if (_connectorMap.Count == 0)
                    {
                        StopCoroutine(_connectCrt);
                        _connectCrt = null;
                    }
                }
            }
            //}
        }

        private void OnPlayRequested(VideoDevicePlayRequest msg)
        {
            if (_device == null)
            {
                Debug.LogError($"[{nameof(VideoDeviceModule)}] 비디오 장치가 연결되어있지 않습니다.");
                return;
            }
            else if (!_connectorMap.ContainsKey(msg.Connector))
            {
                Debug.LogError($"[{nameof(VideoDeviceModule)}] 비디오 장치 연결 요청을 먼저 해야합니다.");
                return;
            }
            else if (_connectorMap[msg.Connector].Buffer != null)
            {
                return;
            }

            if (!_device.IsPlaying)
            {
                _device.Play();
            }

            if (_device.IsPlaying)
            {
                lock (_buffers)
                {
                    _buffers.Add(msg.Buffer);
                }
                _connectorMap[msg.Connector].Buffer = msg.Buffer;

                if (_thread == null && _videoDeviceSettings.DeviceAPI != VideoDeviceAPI.Unity)
                {
                    StartThread();
                }
            }
            else
            {
                Debug.LogError($"[{nameof(VideoDeviceModule)}] 비디오 장치 재생 요청에 실패했습니다.");
            }
        }

        private void OnStopRequested(VideoDeviceStopRequest msg)
        {
            if (_device == null)
            {
                Debug.LogError($"[{nameof(VideoDeviceModule)}] 비디오 장치가 연결되어있지 않습니다.");
                return;
            }
            else if (!_connectorMap.ContainsKey(msg.Connector))
            {
                return;
            }
            else if (_connectorMap[msg.Connector].Buffer == null)
            {
                return;
            }

            if (_device.IsPlaying)
            {
                if (_connectorMap.Count(pair => pair.Value.Buffer != null) == 1)
                {
                    AbortThread();
                    _device.Stop();

                    if (!_device.IsPlaying)
                    {
                        lock (_buffers)
                        {
                            _buffers.Clear();
                        }
                        foreach (var pair in _connectorMap)
                            pair.Value.Buffer = null;
                    }
                    else
                    {
                        Debug.LogError($"[{nameof(VideoDeviceModule)}] 비디오 장치 정지 요청에 실패했습니다.");
                    }
                }
                else
                {
                    lock (_buffers)
                    {
                        _buffers.Remove(_connectorMap[msg.Connector].Buffer);
                    }
                    _connectorMap[msg.Connector].Buffer = null;
                }
            }
            else
            {
                lock (_buffers)
                {
                    _buffers.Remove(_connectorMap[msg.Connector].Buffer);
                }
                _connectorMap[msg.Connector].Buffer = null;
            }
        }

        private void OnRefreshPropsRequired(RefreshVideoDeviceProps _)
        {
            UpdateProps();
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            UpdateProps();
        }

        private void UpdateProps()
        {
            if (_videoCapture != null && _videoCapture.isOpened())
            {
                _videoCapture.set(15, _videoDeviceSettings.Exposure);       // Exposure
                _videoCapture.set(21, _videoDeviceSettings.AutoExposure);   // Auto Exposure
                _videoCapture.set(28, _videoDeviceSettings.Focus);          // Focus
                _videoCapture.set(39, _videoDeviceSettings.AutoFocus);      // Auto Focus
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(_saveKey))
            {
                SaveSettings();
            }

            if (_videoDeviceSettings.DeviceAPI == VideoDeviceAPI.Unity)
            {
                ProcessFrame();
            }
        }

        private void StartThread()
        {
            _thread = new Thread(UpdateThread);
            _thread.Start();
        }

        private void AbortThread()
        {
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
                _thread.Join();
            }
        }

        private void UpdateThread()
        {
            while (true)
            {
                ProcessFrame();
            }
        }

        private void ProcessFrame()
        {
            if (_device != null && _device.IsPlaying)
            {
                _device.CopyBuffer(_frameBuffer);
                if (_frameMat.IsDisposed)
                    return;

                _frameMat.put(0, 0, _frameBuffer);
                if (_videoDeviceSettings.FlipX && _videoDeviceSettings.FlipY)
                    Core.flip(_frameMat, _frameMat, -1);
                else if (_videoDeviceSettings.FlipX)
                    Core.flip(_frameMat, _frameMat, 1);
                else if (_videoDeviceSettings.FlipY)
                    Core.flip(_frameMat, _frameMat, 0);

                _frameMat.convertTo(_frameMat, CvType.CV_8UC3, _videoDeviceSettings.Contrast);

                lock (_buffers)
                {
                    foreach (byte[] buffer in _buffers)
                    {
                        lock (buffer)
                        {
                            /*
                             * Mat.get -> avg > 10ms
                             * Marshal.Copy -> avg > 0.15ms
                             */
                            //_frameMat.get(0, 0, buffer);
                            Marshal.Copy((IntPtr)_frameMat.dataAddr(), buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }

        private IEnumerator CheckConnection()
        {
            bool firstConnection = true;
            bool wasConnected = false;

            while (true)
            {
                if (!_device.IsConnected)
                {
                    if (wasConnected)
                        Message.Send(new VideoDeviceEvent(VideoDeviceEvent.EventType.Unplugged, null, "비디오 장치의 연결이 해제 되었습니다."));

                    _device.Connect(_videoDeviceSettings.RequestedResolution);
                    if (_device.IsConnected)
                    {
                        InitBuffer();

                        foreach (var request in _connectorMap.Values)
                        {
                            request.OnConnected?.Invoke(_device.Resolution, request.Connected);
                            request.Connected = true;
                        }

                        if (!_device.IsPlaying && _connectorMap.Count(pair => pair.Value.Buffer != null) > 0)
                            _device.Play();

                        if (!firstConnection)
                            Message.Send(new VideoDeviceEvent(VideoDeviceEvent.EventType.Reconnected, null, "비디오 장치에 재연결 되었습니다."));
                        else
                            firstConnection = false;
                    }
                    else
                    {
                        string msg = "비디오 장치 연결 요청에 실패했습니다.";
                        Message.Send(new VideoDeviceEvent(VideoDeviceEvent.EventType.FailedToConnect, null, msg));
                        Debug.LogError($"[{nameof(VideoDeviceModule)}] {msg}");
                        yield return new WaitForSeconds(_videoDeviceSettings.ReconnectionDelay);
                    }
                }
                else
                {
                    foreach (var request in _connectorMap.Values)
                    {
                        if (!request.Connected)
                        {
                            request.OnConnected?.Invoke(_device.Resolution, false);
                            request.Connected = true;
                        }
                    }

                    yield return null;
                }

                wasConnected = _device.IsConnected;
            }
        }
    }
}