using System.IO;
using System;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Reflection;


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
        public int QueueOffset = 0;
        public int DisplayState = 0;
        public int DisplayResolution = 0;
        public int DisplayWidth = 0;
        public int DisplayHeight = 0;
    }

    class ConfigEngine
    {
        string ConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Config.json";

        public void Init(out Config configObj)
        {
            configObj = new Config();

            string JsonResult = JsonConvert.SerializeObject(configObj, Formatting.Indented);

            if (File.Exists(ConfigPath))
            {
                Console.WriteLine("read config file !");
                var tr = new StreamReader(ConfigPath);
                JsonResult = tr.ReadToEnd();
                configObj = JsonConvert.DeserializeObject<Config>(JsonResult);
                tr.Close();
            }
            else
            {
                Console.WriteLine("create config file !");
                using (var tw = new StreamWriter(ConfigPath, true))
                {
                    tw.WriteLine(JsonResult.ToString());
                    tw.Close();
                }
            }
        }
    }
}
