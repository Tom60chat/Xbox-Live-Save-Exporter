using mveril.WinRT.InitializeWithWindow.WPF;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Windows.Storage.Pickers;
using Windows.System;

namespace Game_Pass_Save_Tranfer
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

            var games = Game.GetGames();

            foreach (var game in games)
                gameCollection.Add(game);
        }
        #endregion

        #region Methods
        private void lstGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnExport.IsEnabled = e.AddedItems.Count > 0;
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

                exportWindow.Show();

                foreach (var selectedItem in lstGames.SelectedItems)
                {
                    if (selectedItem is Game game)
                    {
                        exportWindow.SetGame(game);

                        exportWindow.SetStatut(Properties.Resource.Exporting + "...");

                        var export = new Export();

                        export.OnProgress += (sender, progress) => exportWindow.SetProgress(progress);
                        export.OnExport += (sender, statut) => exportWindow.SetStatut(statut);

                        await export.Start(game, folder);
                    }
                }

                exportWindow.Close();
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
