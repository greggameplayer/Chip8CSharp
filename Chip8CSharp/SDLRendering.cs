using System;
using System.Runtime.InteropServices;
using SDL2;

namespace Chip8CSharp
{
    class SDLRendering
    {
        IntPtr sdlSurface, sdlTexture = IntPtr.Zero;

        public void createWindow(out IntPtr window)
        {
            window =
                    SDL.SDL_CreateWindow("Chip8CSharp", 0, 30, 64 * 8, 32 * 8,
                                         SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        }

        public void getDisplayMode(out SDL.SDL_DisplayMode DM)
        {
            if (SDL.SDL_GetCurrentDisplayMode(0, out DM) != 0)
            {
                Console.WriteLine("SDL_GetCurrentDisplayMode failed" + SDL.SDL_GetError());
                return;
            }
            SDL.SDL_GetCurrentDisplayMode(0, out DM);
        }

        public void verifyWindow(IntPtr window)
        {
            if (window == IntPtr.Zero)
            {
                Console.WriteLine("SDL could not create a valid window");
                return;
            }
        }

        public void createRenderer(IntPtr window, out IntPtr renderer)
        {
            renderer = SDL.SDL_CreateRenderer(
                    window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine("SDL could not create a valid renderer");
                return;
            }
        }

        public IntPtr getSdlSurface()
        {
            return sdlSurface;
        }

        public void setSdlSurface(IntPtr sdlSurfaceSetter)
        {
            sdlSurface = sdlSurfaceSetter;
        }

        public IntPtr getSdlTexture()
        {
            return sdlTexture;
        }

        public void render(IntPtr renderer)
        {
            SDL.SDL_RenderClear(renderer);
            SDL.SDL_RenderCopy(renderer, sdlTexture, IntPtr.Zero, IntPtr.Zero);
            SDL.SDL_RenderPresent(renderer);
        }

        public void destroyAll(IntPtr window, IntPtr renderer)
        {
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);
        }

        public void destroyTexture()
        {
            SDL.SDL_DestroyTexture(sdlTexture);
        }

        public void createDisplayHandle(uint[] frameBuffer, out GCHandle displayHandle)
        {
            displayHandle = GCHandle.Alloc(frameBuffer, GCHandleType.Pinned);
        }

        public void createRGBSurfaceFrom(GCHandle displayHandle)
        {
            sdlSurface = SDL.SDL_CreateRGBSurfaceFrom(
                             displayHandle.AddrOfPinnedObject(), 64, 32, 32, 64 * 4,
                              0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);
        }

        public void createTextureFromSurface(IntPtr renderer)
        {
            sdlTexture = SDL.SDL_CreateTextureFromSurface(renderer, sdlSurface);
        }
    }
}
