using SDL2;
using System;
using System.Diagnostics;

namespace Chip8CSharp
{
    class Keyboard
    {
        public enum Keys
        {
            one = 1,
            two = 2,
            three = 3,
            four = 4,
            five = 5,
            six = 6,
            seven = 7,
            eight = 8,
            nine = 9,
            zero = 0,
            A = 10,
            B = 11,
            C = 12,
            D = 13,
            E = 14,
            F = 15
        }
        public int KeyCodeToKeyIndex(int keycode)
        {
            int keyIndex = 0;
            if (keycode < 58)
                keyIndex = keycode - 48;
            else
                keyIndex = keycode - 87;

            return keyIndex;
        }

        public void SDLEventLoop(CPU cpu, Stopwatch frameTimer, bool running)
        {
            SDL.SDL_Event sdlEvent;
            while (SDL.SDL_PollEvent(out sdlEvent) != 0)
            {
                if (sdlEvent.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    running = false;
                }
                else if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    var key = KeyCodeToKeyIndex((int)sdlEvent.key.keysym.sym);
                    Console.WriteLine(key);
                    cpu.Keyboard |= (ushort)(1 << key);

                    if (cpu.WaitingForKeyPress)
                        cpu.KeyPressed((byte)key);
                }
                else if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    var key = KeyCodeToKeyIndex((int)sdlEvent.key.keysym.sym);
                    cpu.Keyboard &= (ushort)~(1 << key);
                }
            }
        }
    }
}
