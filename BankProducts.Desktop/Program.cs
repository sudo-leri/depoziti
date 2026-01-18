using Avalonia;
using System;
using System.Globalization;
using System.Threading;

namespace BankProducts.Desktop;

/// <summary>
/// The entry point for the Bank Products Desktop application.
/// </summary>
sealed class Program
{
    /// <summary>
    /// The main entry point of the application.
    /// Configures culture settings and starts the Avalonia desktop application.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    [STAThread]
    public static void Main(string[] args)
    {
        // Set invariant culture for consistent number and date formatting across all threads
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    /// <summary>
    /// Builds and configures the Avalonia application.
    /// </summary>
    /// <returns>An <see cref="AppBuilder"/> configured for the desktop application.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
