using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using SDL2;
using System.Runtime.InteropServices;

namespace Chip8CSharp {
class Program {
  static void Main(string[] args) {
    if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0) {
      Console.WriteLine("SDL failed to init !");
      return;
    }

    CPU cpu = new CPU();

    if (args.Length == 1) {
      using(BinaryReader reader =
                new BinaryReader(new FileStream(args[0], FileMode.Open))) {
        List<byte>program = new List<byte>();

        while (reader.BaseStream.Position < reader.BaseStream.Length) {
          // program.Add((ushort)((reader.ReadByte() << 8) |
          // reader.ReadByte()));
          program.Add(reader.ReadByte());
        }

        cpu.LoadProgram(program.ToArray());
      }
    } else {
      Console.WriteLine("Path of the program you want to load : ");
      string path = Console.ReadLine();
      Console.Clear();
      try {
        using(BinaryReader reader =
                  new BinaryReader(new FileStream(path, FileMode.Open))) {
          List<byte>program = new List<byte>();

          while (reader.BaseStream.Position < reader.BaseStream.Length) {
            // program.Add((ushort)((reader.ReadByte() << 8) |
            // reader.ReadByte()));
            program.Add(reader.ReadByte());
          }

          cpu.LoadProgram(program.ToArray());
        }
      } catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }

    IntPtr window =
        SDL.SDL_CreateWindow("Chip8CSharp", 128, 128, 64 * 8, 32 * 8,
                             SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

    if (window == IntPtr.Zero) {
      Console.WriteLine("SDL could not create a valid window");
      return;
    }

    IntPtr renderer = SDL.SDL_CreateRenderer(
        window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

    if (renderer == IntPtr.Zero) {
      Console.WriteLine("SDL could not create a valid renderer");
      return;
    }

    SDL.SDL_Event sdlEvent;
    bool running = true;

    int sample = 0;
    int beepSamples = 0;

    SDL.SDL_AudioSpec audioSpec = new SDL.SDL_AudioSpec();
    audioSpec.channels = 1;
    audioSpec.freq = 44100;
    audioSpec.samples = 256;
    audioSpec.format = SDL.AUDIO_S8;
    audioSpec.callback =
        new SDL.SDL_AudioCallback((userdata, stream, length) => {
          if (cpu == null)
            return;

          sbyte[] waveData = new sbyte[length];

          for (int i = 0; i < waveData.Length && cpu.SoundTimer > 0;
               i++, beepSamples++) {
            if (beepSamples == 734) {
              beepSamples = 0;
              cpu.SoundTimer--;
            }

            waveData[i] =
                (sbyte)(127 * Math.Sin(sample * Math.PI * 2 * 1000 / 44100));
            sample++;
          }

          byte[] byteData = (byte[])(Array) waveData;

          Marshal.Copy(byteData, 0, stream, byteData.Length);
        });

    SDL.SDL_OpenAudio(ref audioSpec, IntPtr.Zero);
    SDL.SDL_PauseAudio(0);

    IntPtr sdlSurface, sdlTexture = IntPtr.Zero;
    Stopwatch frameTimer = Stopwatch.StartNew();

    while (running) {
      try {
        if (!cpu.WaitingForKeyPress)
          cpu.Step();
        while (SDL.SDL_PollEvent(out sdlEvent) != 0) {
          if (sdlEvent.type == SDL.SDL_EventType.SDL_QUIT) {
            running = false;
          } else if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN) {
            var key = KeyCodeToKeyIndex((int) sdlEvent.key.keysym.sym);
            cpu.Keyboard |= (ushort)(1 << key);

            if (cpu.WaitingForKeyPress)
              cpu.KeyPressed((byte) key);
          } else if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYUP) {
            var key = KeyCodeToKeyIndex((int) sdlEvent.key.keysym.sym);
            cpu.Keyboard &= (ushort) ~(1 << key);
          }
        }

        if (frameTimer.ElapsedMilliseconds > 16) {
          var displayHandle = GCHandle.Alloc(cpu.Display, GCHandleType.Pinned);

          if (sdlTexture != IntPtr.Zero)
            SDL.SDL_DestroyTexture(sdlTexture);

          sdlSurface = SDL.SDL_CreateRGBSurfaceFrom(
              displayHandle.AddrOfPinnedObject(), 64, 32, 32, 64 * 4,
              0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
          sdlTexture = SDL.SDL_CreateTextureFromSurface(renderer, sdlSurface);

          displayHandle.Free();

          SDL.SDL_RenderClear(renderer);
          SDL.SDL_RenderCopy(renderer, sdlTexture, IntPtr.Zero, IntPtr.Zero);
          SDL.SDL_RenderPresent(renderer);

          frameTimer.Restart();
        }
        Thread.Sleep(1);
      } catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }
    SDL.SDL_DestroyRenderer(renderer);
    SDL.SDL_DestroyWindow(window);
  }

  private static int KeyCodeToKeyIndex(int keycode) {
    int keyIndex = 0;
    if (keycode < 58)
      keyIndex = keycode - 48;
    else
      keyIndex = keycode - 87;

    return keyIndex;
  }
}
}
