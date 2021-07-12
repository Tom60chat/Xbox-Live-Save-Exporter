using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Management.Deployment;

namespace Game_Pass_Save_Tranfer
{
    public class Game
    {
        #region Constructors
        public Game(string displayName, string logo, string dataPath)
        {
            DisplayName = displayName;
            Logo = logo;
            DataPath = dataPath;
        }
        #endregion

        #region Variables
        /// <summary>
        /// Path to the packages inside the user local folder
        /// </summary>
        public static string LocalPackagesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages");
        #endregion

        #region Properties
        /// <summary> Game display name </summary>
        public string DisplayName { get; private set; }
        /// <summary> Game logo </summary>
        public string Logo { get; private set; }
        /// <summary> Game path to its application data </summary>
        public string DataPath { get; private set; }
        #endregion

        #region Methods
        /// <summary> Retrieves all Xbox Live Cloud compatible games </summary>
        /// <returns>The list of games found</returns>
        public static IReadOnlyList<Game> GetGames()
        {
            var packageManager = new PackageManager();
            var games = new List<Game>();
            var packageDirectorys = new DirectoryInfo(LocalPackagesPath);

            if (packageDirectorys.Exists)
            {
                foreach (var packageDirectory in packageDirectorys.GetDirectories())
                {
                    var wgs = new DirectoryInfo(Path.Combine(packageDirectory.FullName, "SystemAppData\\wgs")); // wgs is for the Xbox Live cloud save folder

                    if (wgs.Exists)
                    {
                        var packages = packageManager.FindPackagesForUser(string.Empty, packageDirectory.Name);

                        if (packages != null)
                        {
                            var package = packages.FirstOrDefault();

                            if (package != null)
                            {
                                var game = new Game(package.DisplayName, package.Logo.AbsoluteUri, packageDirectory.FullName);
                                games.Add(game);
                            }
                        }
                    }
                }
            }

            return games;
        }
        #endregion
    }
}
