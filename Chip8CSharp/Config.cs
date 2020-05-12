using System.IO;
using System;
using SDL2;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Dynamic;
using System.Collections.Generic;

namespace Chip8CSharp
{
    public enum DisplayState
    {
        Windowed,
        Fullscreen
    }

    public enum DisplayResolution
    {
        Available,
        Specific
    }

    class Config
    {
        public int ConfigVersion = 2;
        public int QueueOffset = 0;
        public int DisplayState = 0;
        public int DisplayResolution = 0;
        public int DisplayWidth = 0;
        public int DisplayHeight = 0;
        public int one = 49;
        public int two = 50;
        public int three = 51;
        public int four = 52;
        public int five = 53;
        public int six = 54;
        public int seven = 55;
        public int eight = 56;
        public int nine = 57;
        public int zero = 48;
        public int A = 97;
        public int B = 98;
        public int C = 99;
        public int D = 100;
        public int E = 101;
        public int F = 102;
    }

    class ConfigEngine
    {
        const int CURRENT_CONFIG_VERSION = 2;
        string ConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Config.json";

        public void Init(out Config configObj)
        {
            configObj = new Config();

            string JsonResult = JsonConvert.SerializeObject(configObj, Formatting.Indented);

            if (File.Exists(ConfigPath))
            {
                configObj = readConfig(ConfigPath, JsonResult);
                if (configObj.ConfigVersion < CURRENT_CONFIG_VERSION)
                {
                    File.Delete(ConfigPath);
                    configObj = new Config();
                    JsonResult = JsonConvert.SerializeObject(configObj, Formatting.Indented);

                    Console.WriteLine("rewrite default config file !");
                    writeConfig(ConfigPath, JsonResult);
                    configObj = readConfig(ConfigPath, JsonResult);
                }
            }
            else
            {
                Console.WriteLine("create config file !");
                writeConfig(ConfigPath, JsonResult);
            }
        }

        public void setWindowSizeDynamically(Config configObj, IntPtr window, SDL.SDL_DisplayMode DM)
        {
            if (configObj.DisplayState == (int)DisplayState.Windowed && configObj.DisplayResolution == (int)DisplayResolution.Available)
                SDL.SDL_SetWindowSize(window, DM.w, DM.h - 70);
            else if (configObj.DisplayState == (int)DisplayState.Fullscreen && configObj.DisplayResolution == (int)DisplayResolution.Available)
                SDL.SDL_SetWindowSize(window, DM.w, DM.h);
            else if (configObj.DisplayResolution != (int)DisplayResolution.Available)
                SDL.SDL_SetWindowSize(window, configObj.DisplayWidth, configObj.DisplayHeight);
        }

        private Config readConfig(string ConfigPath, string JsonResult)
        {
            Console.WriteLine("read config file !");
            var tr = new StreamReader(ConfigPath);
            JsonResult = tr.ReadToEnd();
            tr.Close();

            return JsonConvert.DeserializeObject<Config>(JsonResult);
        }

        private void writeConfig(string ConfigPath, string JsonResult)
        {
            using (var tw = new StreamWriter(ConfigPath, true))
            {
                tw.WriteLine(JsonResult.ToString());
                tw.Close();
            }
        }
    }
}
