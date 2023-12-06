using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace SimpleClicker
{
    class Program
    {
        static Random rand = new Random();
        static Win32.VirtualKeys ToggleClickingKey = Win32.VirtualKeys.Numpad4;
        static Win32.VirtualKeys SuperClickingKey = Win32.VirtualKeys.Numpad5;
        static Win32.VirtualKeys HumanClickingKey = Win32.VirtualKeys.Numpad6;
        
        static Win32.VirtualKeys KeyToSpam = Win32.VirtualKeys.Q;

        enum ClickingType
        {
            Super,
            Human,
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Number that represents time in milliseconds.</returns>
        static int GetHumanInputDelay()
        {
            return rand.Next(15, 60);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("### Simple Clicker ver. {0} ###", Assembly.GetEntryAssembly().GetName().Version.ToString());

            Console.WriteLine("Getting process");
            Process[] processes = Process.GetProcesses();
            Process process = processes.First(p => p.ProcessName == "WowClassic" || p.ProcessName == "Wow-64" || p.ProcessName == "Wow");
            if (process == null)
            {
                Console.WriteLine("No WoW process found!");
                return;
            }
            process.EnableRaisingEvents = true;
            process.Exited += ProcessExitedCallback;

            Console.WriteLine("Getting handle");
            IntPtr handle = process.MainWindowHandle;
            Console.WriteLine("Process handle: {0}", handle.ToString());

            //Console.WriteLine("Press Enter to start");
            //Console.ReadLine();

            ClickingType clickingType = ClickingType.Human;
            Console.WriteLine("Clicking loop...");
            bool toggle = false;
            while (true)
            {
                if (!WindowsManager.IsWindowInFocus(handle))
                    continue;

                if (WindowsManager.IsKeyPressed(SuperClickingKey))
                {
                    if (clickingType != ClickingType.Super)
                        Console.WriteLine("Selected: Super clicking.");
                    clickingType = ClickingType.Super;
                }
                else if (WindowsManager.IsKeyPressed(HumanClickingKey))
                {
                    if (clickingType != ClickingType.Human)
                        Console.WriteLine("Selected: Human clicking.");
                    clickingType = ClickingType.Human;
                }

                if (WindowsManager.IsKeyPressed(ToggleClickingKey))
                {
                    toggle = !toggle;
                    Console.WriteLine($"[{(toggle ? "+" : "-")}] New state: {(toggle ? $"Clicking - [{clickingType}]" : "Not clicking")}.");
                }

                if (!toggle)
                    continue;

                int delay = clickingType == ClickingType.Super ? 0 : GetHumanInputDelay();
                WindowsManager.PressKey(handle, KeyToSpam, delay);
                Thread.Sleep(delay);
            }
        }

        private static void ProcessExitedCallback(object sender, EventArgs e)
        {
            Console.WriteLine("Target process exited");
            Environment.Exit(0);
        }
    }
}