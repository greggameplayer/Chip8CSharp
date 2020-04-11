using SDL2;
using System;
using System.Runtime.InteropServices;

namespace Chip8CSharp
{
    public class Video
    {
        const uint SCREEN_WIDTH = 64;
        const uint SCREEN_HEIGHT = 32;
        const uint QUEUE_SIZE = 0x100;
        const uint QUEUE_OFFSET = 0;

        public uint[] videoBuffer = new uint[SCREEN_WIDTH * SCREEN_HEIGHT]; // video buffer
        public uint[] frameBuffer = new uint[SCREEN_WIDTH * SCREEN_HEIGHT]; // used as the real display

        uint offset_count = 0;
        uint offset_limit = QUEUE_OFFSET;
        bool copyFlag = false;
        ushort[] opcodeQueue = new ushort[QUEUE_SIZE];
        uint queuePointer = 0;

        IntPtr sdlSurface, sdlTexture = IntPtr.Zero;

        void enqueue(ushort opcode)
        {
            opcodeQueue[queueMask((int)(queuePointer++))] = opcode;
        }

        void emptyqueue()
        {
            queuePointer = 0;
        }

        void findOpcodefromQueue(ushort opcode)
        {
            for (int i = 0; i < queuePointer; i++)
            {
                if ((opcode == opcodeQueue[queueMask(i)]) && (++offset_count > offset_limit))
                {
                    copyFlag = true;
                    offset_count = 0;
                    return;
                }
            }
        }

        int queueMask(int i)
        {
            if (i > QUEUE_SIZE - 1)
            {
                copyFlag = true;
            }
            return (int)(i % QUEUE_SIZE);
        }

        public void copyToFbuffer()
        {
            for (int i = 0; i < (SCREEN_WIDTH * SCREEN_HEIGHT); i++)
                frameBuffer[i] = videoBuffer[i];
        }

        public void clearVBuffer()
        {
            for (int i = 0; i < (SCREEN_WIDTH * SCREEN_HEIGHT); i++)
                videoBuffer[i] = 0;
            copyToFbuffer();
        }

        public void optimizations(ushort opcode)
        {
            findOpcodefromQueue(opcode);
            if (copyFlag)
            {
                emptyqueue();
                copyToFbuffer();
                copyFlag = false;
            }
            enqueue(opcode);
        }

        public void init(uint queue_offset, out IntPtr window)
        {
            window =
                    SDL.SDL_CreateWindow("Chip8CSharp", 0, 30, 64 * 8, 32 * 8,
                                         SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            this.offset_limit = queue_offset;

            clearVBuffer();
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

        public void createDisplayHandle(out GCHandle displayHandle)
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
