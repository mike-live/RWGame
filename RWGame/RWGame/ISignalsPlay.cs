using System;

namespace RWGame
{
    interface ISignalsPlay
    {
        void PlayShootSignal();
        void Pause();
        Action OnFinishedPlaying { get; set; }
    }
}
