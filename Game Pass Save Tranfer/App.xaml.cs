using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Game_Pass_Save_Tranfer
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
                        Game_Pass_Save_Tranfer.Properties.Resource.UpdateAvailableDialog,
                        Game_Pass_Save_Tranfer.Properties.Resource.UpdateAvailable,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (answer == MessageBoxResult.Yes)
                        gitHub.Update();
                }
            });
        }
    }
}
