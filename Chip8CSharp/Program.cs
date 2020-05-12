using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using SDL2;
using System.Runtime.InteropServices;
using Sentry;

namespace Chip8CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SentrySdk.Init("https://83625c513509439392b08f862b35c090@o373941.ingest.sentry.io/5191206"))
            {
                ConfigEngine config = new ConfigEngine();

                config.Init(out Config configObj);

                if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
                {
                    Console.WriteLine("SDL failed to init !");
                    return;
                }

                Video video = new Video();
                SDLRendering renderEngine = new SDLRendering();
                CPU cpu = new CPU(video);
                video.clearVBuffer();

                if (args.Length == 1)
                {
                    using (BinaryReader reader =
                              new BinaryReader(new FileStream(args[0], FileMode.Open)))
                    {
                        List<byte> program = new List<byte>();

                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            program.Add(reader.ReadByte());
                        }

                        cpu.LoadProgram(program.ToArray());
                    }
                }
                else
                {
                    Console.WriteLine("Path of the program you want to load : ");
                    string path = Console.ReadLine();
                    Console.Clear();
                    try
                    {
                        using (BinaryReader reader =
                                  new BinaryReader(new FileStream(path, FileMode.Open)))
                        {
                            List<byte> program = new List<byte>();

                            while (reader.BaseStream.Position < reader.BaseStream.Length)
                            {
                                program.Add(reader.ReadByte());
                            }

                            cpu.LoadProgram(program.ToArray());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }


                renderEngine.createWindow(out IntPtr window, configObj);
                video.init(configObj.QueueOffset);
                renderEngine.getDisplayMode(out SDL.SDL_DisplayMode DM);

                renderEngine.verifyWindow(window);

                config.setWindowSizeDynamically(configObj, window, DM);

                renderEngine.createRenderer(window, out IntPtr renderer);
                bool running = true;

                Audio audioEngine = new Audio();

                audioEngine.init(cpu);

                audioEngine.play();

                renderEngine.createDisplayHandle(video.frameBuffer, out GCHandle displayHandle);

                Keyboard keyboard = new Keyboard();
                Stopwatch frameTimer = Stopwatch.StartNew();
                int ticksPer60hz = (int)(Stopwatch.Frequency * 0.016);
                while (running)
                {
                    try
                    {
                        if (!cpu.WaitingForKeyPress)
                            cpu.Step();
                        if (frameTimer.ElapsedTicks > ticksPer60hz)
                        {
                            keyboard.SDLEventLoop(cpu, frameTimer, running);
                            if (renderEngine.getSdlTexture() != IntPtr.Zero)
                                renderEngine.destroyTexture();
                            renderEngine.createRGBSurfaceFrom(displayHandle);
                            renderEngine.createTextureFromSurface(renderer);
                            renderEngine.render(renderer);

                            frameTimer.Restart();
                        }
                        Thread.Sleep(1);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                renderEngine.destroyAll(window, renderer);
            }
        }
    }
}
