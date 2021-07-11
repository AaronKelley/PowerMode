# PowerMode

![Windows 10 power flyout](https://snz04pap001files.storage.live.com/y4mZDCQLkTmNPjNizvLlH3t7VcJU4ubfekfsokuyjXTXXryUVGqOFRn81YvSkXAjlRs2fLUzPoWg96sdydaiPXIwxUDQcu6ogiBfeCAbxKI-V9bvqWp8MJh6GnOWDnRrKTrsUykzwwN62Eo7jyfDssl6uBCcbSg1FhzAqRvqzMfBrHkY2lB6_95Kqy3hQozGpnH?width=360&height=343&cropmode=none)

This program allows you to adjust the Windows "power mode" from the command line.  (The power mode an also be adjusted from the taskbar power flyout in Windows 10, or from the Settings application on Windows 11.)

## Requirements

 - Windows 10, version 1709 or later
 - .NET Framework 4.7.1 (should be be already installed on applicable versions of Windows)

## Use

From the command line:
|Command|Result|
|--|--|
|`PowerMode.exe`|Report the current power mode|
|`PowerMode.exe BetterBattery`|Set the system to "Better Battery" mode|
|`PowerMode.exe BetterPerformance`|Set the system to "Better Performance" mode|
|`PowerMode.exe BestPerformance`|Set the system to "Best Performance" mode|
|`PowerMode.exe <GUID>`|Set the system to the power mode defined by the provided GUID|

## Configuration
The application is preconfigured with power mode GUIDs that apply to a standard, stock Windows 10 installation.  (The default GUIDs are specified [here](https://docs.microsoft.com/en-us/windows-hardware/customize/desktop/customize-power-slider).)  If the power modes are not being set appropriately, you may need to check and see what the GUIDs are supposed to be and update the configuration file accordingly.

To check the GUIDs, open `regedit` and navigate to the key:
`HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes`

Look at the values `ActiveOverlayAcPowerScheme` and `ActiveOverlayDcPowerScheme`, which report the current power mode setting for AC power and battery power, respectively.  Check what GUIDs appear in this value when the power slider is set to different positions, and update the file `PowerMode.exe.config` accordingly.

## How it works

This program uses an undocumented Windows API method to change the Windows power mode.  To adjust the power mode in your own .NET application, include this PInvoke signature:

    [DllImportAttribute("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
    private static extern uint PowerSetActiveOverlayScheme(Guid OverlaySchemeGuid);

Then make the call to adjust the power setting like this:

    PowerSetActiveOverlayScheme(new Guid("ded574b5-45a0-4f42-8737-46345c09c238"));

The current power mode can be retrieved using either of these PInvoke methods:

    [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetActualOverlayScheme")]
    public static extern uint PowerGetActualOverlayScheme(out Guid ActualOverlayGuid);
    
    [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
    public static extern uint PowerGetEffectiveOverlayScheme(out Guid EffectiveOverlayGuid);

(I do not know what the difference between them is supposed to be; they each returned the same value every time during my testing.)  Calling the methods looks like:

    if (PowerGetEffectiveOverlayScheme(out Guid activeScheme) == 0)
    {
        Console.WriteLine(activeScheme);
    }

All of these methods return zero on success and non-zero on failure.

There is one other relevant method in `powrprof.dll` called `PowerGetOverlaySchemes`, which I presume would allow you to retrieve all of the available power modes and GUIDs.  It appears to take three parameters, and I have not taken the time to figure out how it works.