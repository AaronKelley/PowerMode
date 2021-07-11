using System;
using System.Configuration;
using System.Runtime.InteropServices;

namespace PowerMode
{
    /// <summary>
    /// This program allows for setting the Windows "power mode" or "power slider" value from the command line.
    /// </summary>
    class SetPowerMode
    {
        /// <summary>
        /// Execution starts here.
        /// </summary>
        /// <param name="args">Command line parameters.</param>
        /// <returns>Error status; 0 = success, non-zero = failure.</returns>
        static int Main(string[] args)
        {
            try
            {
                // Read from App.config.
                ReadConfig();

                if (args.Length == 0)
                {
                    // Report the current power mode.
                    uint result = PowerGetEffectiveOverlayScheme(out Guid currentMode);
                    if (result == 0)
                    {
                        Console.WriteLine(currentMode);
                        if (currentMode == PowerMode.BetterBattery)
                        {
                            Console.WriteLine("Better battery");
                        }
                        else if (currentMode == PowerMode.BetterPerformance)
                        {
                            Console.WriteLine("Better performance");
                        }
                        else if (currentMode == PowerMode.BestPerformance)
                        {
                            Console.WriteLine("Best performance");
                        }
                    }
                    else
                    {
                        return (int)result;
                    }
                }
                else if (args.Length == 1)
                {
                    // Attempt to set the power mode.
                    string parameter = args[0].ToLower();
                    Guid powerMode;

                    if (parameter == "/?" || parameter == "-?")
                    {
                        Usage();
                        return 1;
                    }
                    else if (parameter == "BetterBattery".ToLower())
                    {
                        powerMode = PowerMode.BetterBattery;
                    }
                    else if (parameter == "BetterPerformance".ToLower())
                    {
                        powerMode = PowerMode.BetterPerformance;
                    }
                    else if (parameter == "BestPerformance".ToLower())
                    {
                        powerMode = PowerMode.BestPerformance;
                    }
                    else
                    {
                        try
                        {
                            powerMode = new Guid(parameter);
                        }
                        catch (Exception)
                        {
                            Console.Error.WriteLine("Failed to parse GUID.\n");
                            Usage();
                            return 1;
                        }
                    }

                    return (int)PowerSetActiveOverlayScheme(powerMode);
                }
                else
                {
                    Usage();
                    return 1;
                }
            }
            catch (Exception exception)
            {
                // Print error information to the console.
                Console.Error.WriteLine("{0}: {1}\n{2}", exception.GetType(), exception.Message, exception.StackTrace);
                Console.WriteLine();
                Usage();
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Print a usage message to the console.
        /// </summary>
        private static void Usage()
        {
            Console.WriteLine(
                    "PowerMode (GPLv3); used to set the active power mode on Windows 10, version 1709 or later\n" +
                    "https://github.com/AaronKelley/PowerMode\n" +
                    "\n" +
                    "  PowerMode                    Report the current power mode\n" +
                    "  PowerMode BetterBattery      Set the system to \"better battery\" mode\n" +
                    "  PowerMode BetterPerformance  Set the system to \"better performance\" mode\n" +
                    "  PowerMode BestPerformance    Set the system to \"best performance\" mode\n" +
                    "  PowerMode <GUID>             Set the system to the mode identified by the GUID"
                );
        }

        private static void ReadConfig()
        {
            if (ConfigurationManager.AppSettings["BetterBatteryGuid"] != null)
            {
                PowerMode.BetterBattery = new Guid(ConfigurationManager.AppSettings["BetterBatteryGuid"]);
            }
            if (ConfigurationManager.AppSettings["BetterPerformanceGuid"] != null)
            {
                PowerMode.BetterPerformance = new Guid(ConfigurationManager.AppSettings["BetterPerformanceGuid"]);
            }
            if (ConfigurationManager.AppSettings["BestPerformanceGuid"] != null)
            {
                PowerMode.BestPerformance = new Guid(ConfigurationManager.AppSettings["BestPerformanceGuid"]);
            }
        }

        /// <summary>
        /// Contains GUID constants for the different power modes.
        /// </summary>
        /// <seealso cref="https://docs.microsoft.com/en-us/windows-hardware/customize/desktop/customize-power-slider"/>
        private static class PowerMode
        {
            /// <summary>
            /// Better Battery mode.
            /// </summary>
            public static Guid BetterBattery = new Guid("961cc777-2547-4f9d-8174-7d86181b8a7a");

            /// <summary>
            /// Better Performance mode.
            /// </summary>
            public static Guid BetterPerformance = new Guid("3af9B8d9-7c97-431d-ad78-34a8bfea439f");

            /// <summary>
            /// Best Performance mode.
            /// </summary>
            public static Guid BestPerformance = new Guid("ded574b5-45a0-4f42-8737-46345c09c238");
        }

        /// <summary>
        /// Retrieves the active overlay power scheme and returns a GUID that identifies the scheme.
        /// </summary>
        /// <param name="EffectiveOverlayPolicyGuid">A pointer to a GUID structure.</param>
        /// <returns>Returns zero if the call was successful, and a nonzero value if the call failed.</returns>
        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
        private static extern uint PowerGetEffectiveOverlayScheme(out Guid EffectiveOverlayPolicyGuid);

        /// <summary>
        /// Sets the active power overlay power scheme.
        /// </summary>
        /// <param name="OverlaySchemeGuid">The identifier of the overlay power scheme.</param>
        /// <returns>Returns zero if the call was successful, and a nonzero value if the call failed.</returns>
        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
        private static extern uint PowerSetActiveOverlayScheme(Guid OverlaySchemeGuid);
    }
}
