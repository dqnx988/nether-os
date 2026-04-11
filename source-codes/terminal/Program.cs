using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

const uint WmSetIcon = 0x0080;
const int IconSmall = 0;
const int IconBig = 1;

Console.Title = "NetherOS Terminal";
SetConsoleWindowIcon();

var running = true;

PrintWelcome();

while (running)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("netheros-terminal> ");
    Console.ResetColor();

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    var parts = input.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    var command = parts[0].ToLowerInvariant();
    var argument = parts.Length > 1 ? parts[1] : string.Empty;

    switch (command)
    {
        case "help":
            PrintHelp();
            break;

        case "echo":
            if (argument.ToLowerInvariant() == "help")
            {
                PrintEchoHelp();
            }
            else
            {
                Console.WriteLine(argument);
            }
            break;

        case "clear":
            if (argument.ToLowerInvariant() == "help")
            {
                PrintClearHelp();
            }
            else
            {
                Console.Clear();
            }
            break;

        case "reboot":
            if (argument.ToLowerInvariant() == "help")
            {
                PrintRebootHelp();
            }
            else
            {
                HandleReboot(argument);
            }
            break;

        case "restart":
            if (argument.ToLowerInvariant() == "help")
            {
                PrintRestartHelp();
            }
            else
            {
                HandleRestart();
            }
            break;

        case "exit":
            if (argument.ToLowerInvariant() == "help")
            {
                PrintExitHelp();
            }
            else
            {
                running = false;
            }
            break;

        case "quit":
            if (argument.ToLowerInvariant() == "help")
            {
                PrintExitHelp();
            }
            else
            {
                running = false;
            }
            break;

        case "leave":
            if (argument.ToLowerInvariant() == "help")
            {
                PrintExitHelp();
            }
            else
            {
                running = false;
            }
            break;

        default:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Unknown command: {command}");
            Console.ResetColor();
            Console.WriteLine("Type `help` to list available commands.");
            break;
    }
}

return;

static void PrintWelcome()
{
    var environment = Environment.GetEnvironmentVariable("NETHEROS_ENV") ?? "Production";
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"NetherOS Terminal {environment}");
    Console.WriteLine("------------------------------");
    Console.ResetColor();
    Console.WriteLine("Type `help` to see available commands.");
    Console.WriteLine();
}

static void PrintHelp()
{
    Console.WriteLine("Available commands:");
    Console.WriteLine("  help    - show help");
    Console.WriteLine("  echo    - print the provided text");
    Console.WriteLine("  clear   - clear the console");
    Console.WriteLine("  reboot  - restart the PC");
    Console.WriteLine("  restart - restart the system");
    Console.WriteLine("  exit    - close the application");
    Console.WriteLine("  quit    - close the application");
    Console.WriteLine("  leave   - close the application");
    Console.WriteLine();
    Console.WriteLine("For help with a specific command, type: <command> help");
}

static void PrintEchoHelp()
{
    Console.WriteLine("echo command:");
    Console.WriteLine("  echo <text>  - print the provided text");
}

static void PrintClearHelp()
{
    Console.WriteLine("clear command:");
    Console.WriteLine("  clear  - clear the console");
}

static void PrintRebootHelp()
{
    Console.WriteLine("reboot command:");
    Console.WriteLine("  reboot       - restart the PC after explicit confirmation");
    Console.WriteLine("  reboot uefi  - restart into UEFI firmware settings");
    Console.WriteLine("  reboot winre - restart into Windows Recovery Environment");
}

static void PrintRestartHelp()
{
    Console.WriteLine("restart command:");
    Console.WriteLine("  restart  - restart the system after explicit confirmation");
}

static void HandleRestart()
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Restart system now? Type 'y' to restart, 'n' to cancel");
    Console.ResetColor();

    var confirmation = Console.ReadLine();

    if (!string.Equals(confirmation, "y", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Restart canceled.");
        return;
    }

    try
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "shutdown",
            Arguments = "/r /t 0",
            UseShellExecute = true,
            CreateNoWindow = true
        };

        Process.Start(startInfo);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Failed to start restart: {ex.Message}");
        Console.ResetColor();
    }
}

static void PrintExitHelp()
{
    Console.WriteLine("exit command:");
    Console.WriteLine("  exit   - close the application");
    Console.WriteLine("  quit   - close the application");
    Console.WriteLine("  leave  - close the application");
}

static void HandleReboot(string argument)
{
    string rebootType = argument.ToLowerInvariant();
    string arguments = "/r /t 0";
    string message = "Reboot system now?";

    switch (rebootType)
    {
        case "uefi":
            arguments = "/r /fw /t 0";
            message = "Reboot into UEFI firmware settings now?";
            break;
        case "winre":
            arguments = "/r /o /t 0";
            message = "Reboot into Windows Recovery Environment now?";
            break;
        case "":
            // Normal reboot
            break;
        default:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Unknown reboot option: {rebootType}");
            Console.ResetColor();
            Console.WriteLine("Valid options: reboot, reboot uefi, reboot winre");
            return;
    }

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"{message} Type \"y\" for reboot, \"n\" for cancel");
    Console.ResetColor();

    var confirmation = Console.ReadLine();

    if (!string.Equals(confirmation, "y", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Reboot canceled.");
        return;
    }

    try
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "shutdown",
            Arguments = arguments,
            UseShellExecute = true,
            CreateNoWindow = true
        };

        Process.Start(startInfo);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Failed to start reboot: {ex.Message}");
        Console.ResetColor();
    }
}

static void SetConsoleWindowIcon()
{
    try
    {
        var windowHandle = NativeMethods.GetConsoleWindow();
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }

        var iconPath = Path.Combine(AppContext.BaseDirectory, "img", "logo.png");
        if (!File.Exists(iconPath))
        {
            return;
        }

        using var bitmap = new Bitmap(iconPath);
        var iconHandle = bitmap.GetHicon();

        NativeMethods.SendMessage(windowHandle, WmSetIcon, (IntPtr)IconSmall, iconHandle);
        NativeMethods.SendMessage(windowHandle, WmSetIcon, (IntPtr)IconBig, iconHandle);
    }
    catch
    {
        // Ignore icon loading failures so the shell still starts normally.
    }
}

static class NativeMethods
{
    [DllImport("kernel32.dll")]
    internal static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
