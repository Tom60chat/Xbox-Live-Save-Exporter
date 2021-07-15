using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xbox_Live_Save_Exporter.UWP
{
    public static class Permission
    {
        #region Methods
        /// <summary>
        /// Return if the File System permission is enable
        /// </summary>
        /// <returns>true if the File System permission is enable, else false</returns>
        public static async Task<bool> HavePermission()
        {
            try
            {
                await StorageFolder.GetFolderFromPathAsync(UserDataPaths.GetDefault().LocalAppData);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        /// <summary>
        /// Show a dialog to ask user to enable file system authorization
        /// </summary>
        public static async Task AskPermissionAsync()
        {
            var dialog = new ContentDialog();
            var res = ResourceLoader.GetForCurrentView();

            dialog.Content = res.GetString("PermissionDialog");
            dialog.PrimaryButtonText = res.GetString("Settings");
            dialog.SecondaryButtonText = res.GetString("Close");

            dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
            dialog.SecondaryButtonClick += Dialog_SecondaryButtonClick;

            await dialog.ShowAsync();
        }

        private static async void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //save item
            var deferral = args.GetDeferral();
            args.Cancel = true;
            deferral.Complete();

            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
        }

        private static async void Dialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!await ApplicationView.GetForCurrentView().TryConsolidateAsync()) // The recomanded way
                Application.Current.Exit(); // The not very UWP logic way
        }
        #endregion
    }
}
