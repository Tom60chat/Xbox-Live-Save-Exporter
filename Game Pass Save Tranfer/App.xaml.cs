using System.Threading.Tasks;
using System.Windows;

namespace Xbox_Live_Save_Exporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
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
    }
}
