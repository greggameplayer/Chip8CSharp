using SDL2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public void KeyDown(SDL.SDL_Event sdlEvent, Config configObj, CPU cpu)
        {
            var key = TestPressedKey((int)sdlEvent.key.keysym.sym, configObj);
            cpu.Keyboard |= (ushort)(1 << key);

            if (cpu.WaitingForKeyPress)
                cpu.KeyPressed((byte)key);
        }

        public void KeyUp(SDL.SDL_Event sdlEvent, Config configObj, CPU cpu)
        {
            var key = TestPressedKey((int)sdlEvent.key.keysym.sym, configObj);
            cpu.Keyboard &= (ushort)~(1 << key);
        }

        public int TestPressedKey(int key, Config configObj)
        {
            int result = 0;
            var @switch = new Functions.Switch
            {
                { () => key == configObj.zero, () => { result = (int)Keys.zero;} },
                { () => key == configObj.one, () => { result = (int)Keys.one;} },
                { () => key == configObj.two, () => { result = (int)Keys.two;} },
                { () => key == configObj.three, () => { result = (int)Keys.three;} },
                { () => key == configObj.four, () => { result = (int)Keys.four;} },
                { () => key == configObj.five, () => { result = (int)Keys.five;} },
                { () => key == configObj.six, () => { result = (int)Keys.six;} },
                { () => key == configObj.seven, () => { result = (int)Keys.seven;} },
                { () => key == configObj.eight, () => { result = (int)Keys.eight;} },
                { () => key == configObj.nine, () => { result = (int)Keys.nine;} },
                { () => key == configObj.A, () => { result = (int)Keys.A;} },
                { () => key == configObj.B, () => { result = (int)Keys.B;} },
                { () => key == configObj.C, () => { result = (int)Keys.C;} },
                { () => key == configObj.D, () => { result = (int)Keys.D;} },
                { () => key == configObj.E, () => { result = (int)Keys.E;} },
                { () => key == configObj.F, () => { result = (int)Keys.F;} }
            };

            @switch.Execute();

            return result;
        }
    }
}
