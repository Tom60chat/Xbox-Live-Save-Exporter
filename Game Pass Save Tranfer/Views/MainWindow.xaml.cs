using mveril.WinRT.InitializeWithWindow.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Storage.Pickers;
using Windows.System;

namespace Game_Pass_Save_Tranfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Game> gameCollection { get; set; }

        public MainWindow()
        {
            gameCollection = new ObservableCollection<Game>();
            InitializeComponent();

            var games = SaveGameHelper.GetGames();

            foreach (var game in games)
                gameCollection.Add(game);
        }

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
                var tranferWindow = new TranferWindow();

                tranferWindow.Show();

                foreach (var selectedItem in lstGames.SelectedItems)
                {
                    if (selectedItem is Game game)
                    {
                        tranferWindow.SetGame(game);

                        tranferWindow.SetStatut("Transfering...");

                        var transfer = new Export();

                        transfer.OnProgress += (sender, progress) => tranferWindow.SetProgress(progress);

                        await transfer.Start(game, folder);
                    }
                }

                tranferWindow.Close();
                await Launcher.LaunchFolderAsync(folder);
            }
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Made by Tom Oliver" + Environment.NewLine +
                "This program is not not affiliated with Microsoft and these subsidiaries.",
                "About");
        }
    }
}
