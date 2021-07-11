using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Management.Deployment;

namespace Game_Pass_Save_Tranfer
{
    class SaveGameHelper
    {
        public static string LocalPackagesPath = Path.Combine(Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData), "Packages");

        public static IReadOnlyList<Game> GetGames()
        {
            PackageManager packageManager = new PackageManager();
            var games = new List<Game>();
            var packageDirectorys = new DirectoryInfo(LocalPackagesPath);

            if (packageDirectorys.Exists)
            {


                foreach (var packageDirectory in packageDirectorys.GetDirectories())
                {
                    var wgs = new DirectoryInfo(Path.Combine(packageDirectory.FullName, "SystemAppData\\wgs")); // wgs is for the Xbox Live cloud save file
                    if (wgs.Exists)
                    {
                        var packages = packageManager.FindPackagesForUser(string.Empty, packageDirectory.Name);

                        if (packages != null)
                        {
                            var package = packages.FirstOrDefault();

                            if (package != null)
                            {
                                var game = new Game();//(package.DisplayName, package.Logo.AbsoluteUri);
                                game.DisplayName = package.DisplayName;
                                game.Logo = package.Logo.AbsoluteUri;
                                game.DataPath = packageDirectory.FullName;
                                games.Add(game);
                            }
                        }
                    }
                }
            }


            return games;
        }
    }
}
