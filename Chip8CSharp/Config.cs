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
        public int one = (int)Keyboard.Keys.one;
        public int two = (int)Keyboard.Keys.two;
        public int three = (int)Keyboard.Keys.three;
        public int four = (int)Keyboard.Keys.four;
        public int five = (int)Keyboard.Keys.five;
        public int six = (int)Keyboard.Keys.six;
        public int seven = (int)Keyboard.Keys.seven;
        public int eight = (int)Keyboard.Keys.eight;
        public int nine = (int)Keyboard.Keys.nine;
        public int A = (int)Keyboard.Keys.A;
        public int B = (int)Keyboard.Keys.B;
        public int C = (int)Keyboard.Keys.C;
        public int D = (int)Keyboard.Keys.D;
        public int E = (int)Keyboard.Keys.E;
        public int F = (int)Keyboard.Keys.F;
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
                readConfig(ConfigPath, JsonResult, configObj);
                if (configObj.ConfigVersion < CURRENT_CONFIG_VERSION || !(IsPropertyExist(configObj, "ConfigVersion")))
                {
                    File.Delete(ConfigPath);
                    configObj = new Config();
                    JsonResult = JsonConvert.SerializeObject(configObj, Formatting.Indented);

                    Console.WriteLine("rewrite default config file !");
                    writeConfig(ConfigPath, JsonResult);
                    readConfig(ConfigPath, JsonResult, configObj);
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

        public static bool IsPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }

        private void readConfig(string ConfigPath, string JsonResult, Config configObj)
        {
            Console.WriteLine("read config file !");
            var tr = new StreamReader(ConfigPath);
            JsonResult = tr.ReadToEnd();
            configObj = JsonConvert.DeserializeObject<Config>(JsonResult);
            tr.Close();
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
