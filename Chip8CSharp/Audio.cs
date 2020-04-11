using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SDL2;

namespace Chip8CSharp
{
    class Audio
    {
        int sample = 0;
        int beepSamples = 0;

        SDL.SDL_AudioSpec audioSpec = new SDL.SDL_AudioSpec();

        public void init(CPU cpu)
        {
            audioSpec.channels = 1;
            audioSpec.freq = 44100;
            audioSpec.samples = 256;
            audioSpec.format = SDL.AUDIO_S8;
            audioSpec.callback =
                new SDL.SDL_AudioCallback((userdata, stream, length) =>
                {
                    if (cpu == null)
                        return;

                    sbyte[] waveData = new sbyte[length];

                    for (int i = 0; i < waveData.Length && cpu.SoundTimer > 0;
                   i++, beepSamples++)
                    {
                        if (beepSamples == 730)
                        {
                            beepSamples = 0;
                            cpu.SoundTimer--;
                        }

                        waveData[i] =
                      (sbyte)(127 * Math.Sin(sample * Math.PI * 2 * 604.1 / 44100));
                        sample++;
                    }

                    byte[] byteData = (byte[])(Array)waveData;

                    Marshal.Copy(byteData, 0, stream, byteData.Length);
                });
        }

        public void play()
        {
            SDL.SDL_OpenAudio(ref audioSpec, IntPtr.Zero);
            SDL.SDL_PauseAudio(0);
        }
    }
}
