using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Xbox_Live_Save_Exporter
{
    public class Game
    {
        #region Constructors
        private Game() { }
        /*private Game(string displayName, string logo, string dataPath)
        {
            DisplayName = displayName;
            Logo = logo;
            DataPath = dataPath;
        }*/
        #endregion

        #region Variables
        /// <summary>
        /// Path to the packages inside the user local folder
        /// </summary>
        public static string LocalPackagesPath = Path.Combine(UserDataPaths.GetDefault().LocalAppData, "Packages");
        #endregion

        #region Properties
        /// <summary> Game display name </summary>
        public string DisplayName { get; private set; }
        /// <summary> Game logo </summary>
        public string Logo { get; private set; }
        /// <summary> Game logo </summary>
        public ImageSource LogoUWP { get; private set; }
        /// <summary> Game logo </summary>
        public string DataPath { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Create a new instance of the class Game
        /// </summary>
        /// <param name="displayName">Display name</param>
        /// <param name="logo">Logo path</param>
        /// <param name="dataPath">Application data path</param>
        /// <returns>The instance</returns>
        public static async Task<Game> Build(string displayName, string logo, string dataPath)
        {
            var game = new Game(); // displayName, logo, dataPath);

            game.DisplayName = displayName;
            game.Logo = logo;
            game.DataPath = dataPath;

            try
            {
                BitmapImage image = new BitmapImage();
                var storageFile = await StorageFile.GetFileFromPathAsync(logo);
                using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
                    await image.SetSourceAsync(stream);

                game.LogoUWP = image;
            }
            catch { }

            return game;
        }

        /// <summary> Retrieves all Xbox Live Cloud compatible games </summary>
        /// <returns>The list of games found</returns>
        public static async Task<IReadOnlyList<Game>> GetGames()
        {
            var packageManager = new PackageManager();
            var games = new List<Game>();
            // var packageDirectorys = new DirectoryInfo(LocalPackagesPath.Replace("file:///", string.Empty));

            StorageFolder packageDirectorys;
            IStorageItem wgs;

            try
            {
                packageDirectorys = await StorageFolder.GetFolderFromPathAsync(LocalPackagesPath);
            }
            catch
            {
                var dialog = new MessageDialog("We can't get acces to your saves file, you can't probably do nothings about.\nSorry");
                return games;
            }

            /*if (packageDirectorys.Exists)
            {*/
                foreach (var packageDirectory in await packageDirectorys.GetFoldersAsync())
                {
                // TryGetItemAsync allow better performance than try catch GetFolderFromPathAsync
                wgs = await packageDirectory.TryGetItemAsync("SystemAppData\\wgs");
                if (wgs != null)
                {
                    //var wgs = new DirectoryInfo(Path.Combine(packageDirectory.FullName, "SystemAppData\\wgs")); // wgs is for the Xbox Live cloud save folder

                    /*if (wgs.Exists)
                    {*/

                    var packages = packageManager.FindPackagesForUser(string.Empty, packageDirectory.Name);

                    if (packages != null)
                    {
                        var package = packages.FirstOrDefault();

                        if (package != null)
                        {
                            var game = await Build(package.DisplayName, package.Logo.AbsoluteUri, packageDirectory.Path);
                            games.Add(game);
                        }
                    }
                }
                }
            //}

            return games;
        }
        #endregion
    }
}
