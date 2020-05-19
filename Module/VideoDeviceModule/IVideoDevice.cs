using UnityEngine;

namespace JHchoi.Module.VideoDevice
{
    public interface IVideoDevice
    {
        void Connect(Vector2Int resolution);
        void Disconnect();
        bool IsConnected { get; }
        void Play();
        void Stop();
        bool IsPlaying { get; }
        void CopyBuffer(byte[] dst);
        Vector2Int Resolution { get; }
    }
}