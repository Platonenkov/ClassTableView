using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Notification.Wpf;
using Notifications.Wpf.Annotations;

namespace ClassTableView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static NotificationManager Notifier { get; } = new NotificationManager(Application.Current.Dispatcher);
        public App()
        {

            const StringComparison strcmp = StringComparison.InvariantCultureIgnoreCase;
            var args = Environment.GetCommandLineArgs();

            AppDomain.CurrentDomain.UnhandledException += OnExceptionUnhandled;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }
        /// <summary>Обработчик неперехваченных исключений</summary>
        private static void OnDispatcherUnhandledException([CanBeNull] object Sender, [NotNull] DispatcherUnhandledExceptionEventArgs E)
        {

#if DEBUG
            var exception_text = E.Exception.ToString();
            var res = MessageBox.Show(exception_text, "Unhandled exception!", MessageBoxButton.OKCancel, E.Handled ? MessageBoxImage.Warning : MessageBoxImage.Error);
            if (res == MessageBoxResult.OK)
                E.Handled = true;

            if (Debugger.IsAttached)
            {
                if (E.Exception is ResourceReferenceKeyNotFoundException)
                {
                    E.Handled = true;
                    MessageBox.Show(exception_text, "Unhandled exception!", MessageBoxButton.OK, E.Handled ? MessageBoxImage.Warning : MessageBoxImage.Error);
                }
                else
                    Debugger.Break();
            }
            else
            {
                MessageBox.Show(exception_text, "Unhandled exception!", MessageBoxButton.OK, E.Handled ? MessageBoxImage.Warning : MessageBoxImage.Error);
            }
#endif
        }

        /// <summary>Обработчик неперехваченных исключений</summary>
        private static void OnExceptionUnhandled([CanBeNull] object Sender, [NotNull] UnhandledExceptionEventArgs E)
        {
            Trace.WriteLine($"Exception:\r\n{E.ExceptionObject}");
            var exception_text = E.ExceptionObject.ToString();
#if DEBUG
            if (Debugger.IsAttached) Debugger.Break();
            else
                MessageBox.Show(E.ExceptionObject.ToString(), "Unhandled exception!", MessageBoxButton.OK, E.IsTerminating ? MessageBoxImage.Error : MessageBoxImage.Warning);
#endif
            Trace.Fail("Необработанное исключение", exception_text);
        }
    }
}
