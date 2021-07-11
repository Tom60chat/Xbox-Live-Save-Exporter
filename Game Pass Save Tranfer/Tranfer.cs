using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Game_Pass_Save_Tranfer
{
    public class Export
    {
        public EventHandler<double> OnProgress;

        public async Task<bool> Start(Game game, StorageFolder folder)
        {
            double progres;

            if (folder == null) return false;

            var wgs = new DirectoryInfo(Path.Combine(game.DataPath, "SystemAppData\\wgs"));
            var dirs = wgs.GetDirectories();

            if (dirs == null) return false;

            var tranferGame = await folder.CreateFolderAsync(game.DisplayName, CreationCollisionOption.OpenIfExists);

            // Loop through every user folder
            for (int u = 0; u < dirs.Length; u++)
            {
                progres = (double)u / dirs.Length;
                if (OnProgress != null) OnProgress(this, progres);

                var user = dirs[u];
                var file = new FileInfo(Path.Combine(user.FullName, "containers.index"));

                if (!file.Exists) continue;

                var container = Container.TryParse(file.FullName);

                if (container == null) continue;

                var tranferUser = await tranferGame.CreateFolderAsync(user.Name, CreationCollisionOption.OpenIfExists);

                // Loop through every save folder
                for (int f = 0; f < container.Folders.Count; f++)
                {
                    progres += (double)f / container.Folders.Count * 0.1;
                    if (OnProgress != null) OnProgress(this, progres);

                    var containerFolder = container.Folders[f];
                    var containerFiles = ContainerFile.TryParse(containerFolder);

                    if (containerFiles == null) continue;

                    var tranferFolder = await tranferUser.CreateFolderAsync(containerFolder.Name, CreationCollisionOption.OpenIfExists);

                    // Loop through every save file
                    for (int s = 0; s < containerFiles.Count; s++)
                    {
                        progres += (double)s / containerFiles.Count * 0.1;
                        if (OnProgress != null) OnProgress(this, progres);

                        var containerFile = containerFiles[s];
                        var sourceFile = await StorageFile.GetFileFromPathAsync(containerFile.Path);

                        await sourceFile.CopyAsync(tranferFolder, containerFile.Name, NameCollisionOption.GenerateUniqueName);
                    }
                }
            }

            return true;
        }
    }
}
