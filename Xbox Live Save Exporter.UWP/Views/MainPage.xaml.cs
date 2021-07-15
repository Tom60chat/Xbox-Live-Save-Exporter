using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Xbox_Live_Save_Exporter.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Properties
        public ObservableCollection<Game> gameCollection { get; private set; }
        #endregion

        #region Constructors
        public MainPage()
        {
            gameCollection = new ObservableCollection<Game>();
            InitializeComponent();
            _ = InitializeAsync();
        }
        #endregion

        #region Methods
        private async Task InitializeAsync()
        {
            var games = await Game.GetGames();

            foreach (var game in games)
                gameCollection.Add(game);

            progressRing.IsActive = false;
            txtNoSaveFound.Visibility = games.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void lstGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnExport.IsEnabled = lstGames.SelectedItems.Count > 0;
        }

        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
            };
            folderPicker.FileTypeFilter.Add("*");

            var folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                var exportDialog = new ExportDialog();

                IsEnabled = false;
                var operation = exportDialog.ShowAsync();

                for (int i = 0; i < lstGames.SelectedItems.Count; i++)
                {
                    var selectedItem = lstGames.SelectedItems[i];

                    if (selectedItem is Game game)
                    {
                        exportDialog.SetGame(game);

                        var res = ResourceLoader.GetForCurrentView();
                        exportDialog.SetStatut(res.GetString("Exporting") + "...");

                        var export = new Export();

                        export.OnProgress += (_sender, progress) =>
                        {
                            /*double Xl = (double)1 / lstGames.SelectedItems.Count;
                            double Xr = Xl * i;*/
                            exportDialog.SetProgress(progress);
                        };
                        export.OnExport += (_sender, statut) => exportDialog.SetStatut(res.GetString("Exporting") + " " + statut);

                        await export.Start(game, folder);
                    }
                }

                operation.Cancel();
                IsEnabled = true;

                await Launcher.LaunchFolderAsync(folder);
            }
        }

        private async void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            var res = ResourceLoader.GetForCurrentView();
            var dialog = new MessageDialog(res.GetString("AboutDialog"), res.GetString("About"));

            await dialog.ShowAsync();
        }
        #endregion
    }
}
