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
        
        static Win32.VirtualKeys SpellSwitchKey = Win32.VirtualKeys.Numpad2;
        static Win32.VirtualKeys InteractSwitchKey = Win32.VirtualKeys.Numpad3;

        static Win32.VirtualKeys SpellKeyToSpam = Win32.VirtualKeys.Q;
        static Win32.VirtualKeys InteractKeyToSpam = Win32.VirtualKeys.Numpad0;

        /// <summary>
        /// Added delay between clicks in milliseconds.
        /// </summary>
        static int? AddedDelayBetweenClicks = null;

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

            bool ignoreFocusCheck = false;
            foreach (var arg in args)
            {
                if (arg == "--ignorefocuscheck")
                {
                    Console.WriteLine("Warning: window focus check is ignored!");
                    ignoreFocusCheck = true;
                }
                else if (arg.StartsWith("--addeddelay="))
                {
                    AddedDelayBetweenClicks = int.Parse(arg.Substring("--addeddelay=".Length).Trim());
                    Console.WriteLine($"Added delay: {AddedDelayBetweenClicks} ms.");
                }
            }
            if (!ignoreFocusCheck)
                Console.WriteLine("To allow the clicker to send commands to the target window when it is not in focus use the --ignorefocuscheck argument.");
            if (AddedDelayBetweenClicks is null)
            {
                Console.WriteLine("To use added delay between clicks use use the --addeddelay= argument. Example: --addeddelay=50.");
                AddedDelayBetweenClicks = 0;
            }

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
            Win32.VirtualKeys key = SpellKeyToSpam;
            Console.WriteLine($"[!] Default clicking mode: {clickingType}. To switch: [{ClickingType.Super} = {SuperClickingKey}], [{ClickingType.Human} = {HumanClickingKey}].");
            Console.WriteLine($"[!] Default key to spam: {key}. To switch: [{SpellSwitchKey} to spam {SpellKeyToSpam}], [{InteractSwitchKey} to spam {InteractKeyToSpam}].");
            Console.WriteLine($"[!] Toggle key: [{ToggleClickingKey}]. Game window should be in focus.");

            Console.WriteLine("Clicking loop...");
            bool toggle = false;
            while (true)
            {
                if (!ignoreFocusCheck && !WindowsManager.IsWindowInFocus(handle))
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

                if (WindowsManager.IsKeyPressed(SpellSwitchKey))
                {
                    var oldKey = key;
                    key = SpellKeyToSpam;
                    if (key != oldKey)
                        Console.WriteLine($"[S] Key: {key}.");
                }
                else if (WindowsManager.IsKeyPressed(InteractSwitchKey))
                {
                    var oldKey = key;
                    key = InteractKeyToSpam;
                    if (key != oldKey)
                        Console.WriteLine($"[I] Key: {key}.");
                }

                if (WindowsManager.IsKeyPressed(ToggleClickingKey))
                {
                    toggle = !toggle;
                    Console.WriteLine($"[{(toggle ? "+" : "-")}] New state: {(toggle ? $"Clicking - [{clickingType}]" : "Not clicking")}.");
                }

                if (!toggle)
                    continue;

                int delay = clickingType == ClickingType.Super ? 0 : GetHumanInputDelay();
                WindowsManager.PressKey(handle, key, delay);
                Thread.Sleep(delay + AddedDelayBetweenClicks.Value);
            }
        }

        private static void ProcessExitedCallback(object sender, EventArgs e)
        {
            Console.WriteLine("Target process exited");
            Environment.Exit(0);
        }
    }
}