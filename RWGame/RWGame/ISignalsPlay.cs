using System;
using System.Collections.Generic;
using System.Text;

namespace RWGame
{
    interface ISignalsPlay
    {
        void PlayShootSignal();
        void Pause();
        Action OnFinishedPlaying { get; set; }
    }
}
