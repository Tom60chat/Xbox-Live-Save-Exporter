using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Xbox_Live_Save_Exporter
{
    public class Export
    {
        #region Variables
        /// <summary> Invoked when progress change </summary>
        public EventHandler<double> OnProgress;
        /// <summary> Invoked when a new file is being exported </summary>
        public EventHandler<string> OnExport;
        #endregion

        #region Methods
        /// <summary> Export a game save file to the desired folder </summary>
        /// <param name="game">The game to export</param>
        /// <param name="folder">The destination folder</param>
        /// <returns>true the task is successful, else false</returns>
        public async Task<bool> Start(Game game, StorageFolder folder)
        {
            // DirectoryInfo is much faster than StorageFolder but https://docs.microsoft.com/en-us/archive/blogs/wsdevsol/skip-the-path-stick-to-the-storagefile

            double progres;
            StorageFolder wgs;
            IReadOnlyList<StorageFolder> folders;
            StorageFile file;

            if (folder == null) return false;

            /*var wgs = new DirectoryInfo(Path.Combine(game.DataPath, "SystemAppData\\wgs"));
            var dirs = wgs.GetDirectories();

            if (dirs == null) return false;*/

            try
            {
                wgs = await StorageFolder.GetFolderFromPathAsync(Path.Combine(game.DataPath, "SystemAppData\\wgs"));
                folders = await wgs.GetFoldersAsync();
            }
            catch
            {
                return false;
            }

            var tranferGame = await folder.CreateFolderAsync(game.DisplayName, CreationCollisionOption.OpenIfExists);

            // Loop through every user folder
            for (int u = 0; u < folders.Count; u++)
            {
                double Xl = (double)1 / folders.Count;
                double Xr = Xl * u;
                progres = Xr;

                OnProgress?.Invoke(this, progres);

                var user = folders[u];
                /*var file = new FileInfo(Path.Combine(user.FullName, "containers.index"));

                if (!file.Exists) continue;*/

                try
                {
                    file = await StorageFile.GetFileFromPathAsync(Path.Combine(user.Path, "containers.index"));
                }
                catch
                {
                    continue;
                }

                var container = await Container.TryParse(file);

                if (container == null) continue;

                var tranferUser = await tranferGame.CreateFolderAsync(user.Name, CreationCollisionOption.OpenIfExists);

                // Loop through every save folder
                for (int f = 0; f < container.Folders.Count; f++)
                {
                    double Yl = (double)Xl / container.Folders.Count;
                    double Yr = Yl * f + Xr;
                    progres = Yr;

                    OnProgress?.Invoke(this, progres);

                    var containerFolder = container.Folders[f];
                    var containerFiles = await ContainerFile.TryParse(containerFolder);

                    if (containerFiles == null) continue;

                    var tranferFolder = await tranferUser.CreateFolderAsync(containerFolder.Name, CreationCollisionOption.OpenIfExists);

                    // Loop through every save file
                    for (int s = 0; s < containerFiles.Count; s++)
                    {
                        double Zl = (double)Yl / containerFiles.Count;
                        double Zr = Zl * f + Yr;
                        progres = Zr;

                        OnProgress?.Invoke(this, progres);

                        var containerFile = containerFiles[s];
                        var sourceFile = await StorageFile.GetFileFromPathAsync(containerFile.Path);

                        OnExport?.Invoke(this, containerFile.Name);

                        await sourceFile.CopyAsync(tranferFolder, containerFile.Name, NameCollisionOption.GenerateUniqueName);
                    }
                }
            }

            return true;
        }
        #endregion
    }
}
