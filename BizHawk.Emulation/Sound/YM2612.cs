﻿namespace BizHawk.Emulation.Sound
{
    public sealed class YM2612 : ISoundProvider
    {
        public byte ReadStatus()
        {
            // default status: not BUSY, both timers tripped
            return 3;
        }

        public void Write(int addr, byte value)
        {
            
        }

        public void Reset()
        {
        }

        public void GetSamples(short[] samples)
        {
            // TODO
        }
    }
}
