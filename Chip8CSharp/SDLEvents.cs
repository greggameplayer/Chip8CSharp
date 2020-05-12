using SDL2;
using System.Diagnostics;

namespace Chip8CSharp
{
    class SDLEvents
    {
        public bool running = true;
        public void EventLoop(CPU cpu, Stopwatch frameTimer, Config configObj, Keyboard keyboard)
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
                    keyboard.KeyDown(sdlEvent, configObj, cpu);
                }
                else if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    keyboard.KeyUp(sdlEvent, configObj, cpu);
                }
            }
        }
    }
}
