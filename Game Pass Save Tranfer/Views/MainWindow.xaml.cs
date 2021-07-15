using mveril.WinRT.InitializeWithWindow.WPF;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.Storage.Pickers;
using Windows.System;

namespace Xbox_Live_Save_Exporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        public ObservableCollection<Game> gameCollection { get; private set; }
        #endregion

        #region Constructors
        public MainWindow()
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

            progressRing.Visibility = Visibility.Collapsed; // Kinda lazy to use XAML island, don't want to overload the app
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

            this.InitializeWinRTChild(folderPicker);
            var folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                var exportWindow = new TranferWindow();

                IsEnabled = false;
                exportWindow.Show();

                for (int i = 0; i < lstGames.SelectedItems.Count; i++)
                {
                    var selectedItem = lstGames.SelectedItems[i];

                    if (selectedItem is Game game)
                    {
                        exportWindow.SetGame(game);

                        exportWindow.SetStatut(Properties.Resource.Exporting + "...");

                        var export = new Export();

                        export.OnProgress += (_sender, progress) =>
                        {
                            // TODO; Split progress bar with number of game
                            /*double Xl = (double)1 / (double)lstGames.SelectedItems.Count;
                            double Xr = Xl * i;

                            double Yl = (double)Xl / 1;
                            double Yr = Yl * progress + Xr;*/
                            exportWindow.SetProgress(progress);
                        };
                        export.OnExport += (sender, statut) => exportWindow.SetStatut(Properties.Resource.Exporting + " " + statut);

                        await export.Start(game, folder);
                    }
                }

                exportWindow.Close();
                IsEnabled = true;

                await Launcher.LaunchFolderAsync(folder);
            }
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Properties.Resource.AboutDialog, Properties.Resource.About);
        }
        #endregion
    }
}
