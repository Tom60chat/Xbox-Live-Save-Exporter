using System;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Popups;

namespace Xbox_Live_Save_Exporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsWAP = false;

        public App()
        {
            IsWAP = Package.Current != null;

            if (IsWAP)
                InitStoreVersion();
            else
                InitStandalone();
        }

        // The WPF app is not packaged as a UWP app, we can use GitHub API to check for updates
        void InitStandalone()
        {
            // Check update
            var gitHub = new GitHubHelper("Tom60chat", "Xbox-Live-Save-Exporter");

            Task.Factory.StartNew(async () =>
            {
                if (await gitHub.CheckNewerVersion())
                {
                    var answer = MessageBox.Show(
                        Xbox_Live_Save_Exporter.Properties.Resource.UpdateAvailableDialog,
                        Xbox_Live_Save_Exporter.Properties.Resource.UpdateAvailable,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (answer == MessageBoxResult.Yes)
                        gitHub.Update();
                }
            });
        }

        // The WPF as been packaged as a UWP app, we can use the Windows Store API to check for updates
        void InitStoreVersion()
        {
            // Check update
            Task.Factory.StartNew(async () =>
            {
                var availabilityResult = await Package.Current.CheckUpdateAvailabilityAsync();

                if (availabilityResult.Availability == PackageUpdateAvailability.Available || availabilityResult.Availability == PackageUpdateAvailability.Required || true)
                {
                    var answer = MessageBox.Show(
                        Xbox_Live_Save_Exporter.Properties.Resource.UpdateAvailableDialog,
                        Xbox_Live_Save_Exporter.Properties.Resource.UpdateAvailable,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (answer == MessageBoxResult.Yes)
                        await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?productid=9NK0HLN1PRWB"));
                }
            });
        }
    }
}
