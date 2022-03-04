using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Xbox_Live_Save_Exporter.UWP
{
    public sealed partial class ExportDialog : ContentDialog
    {
        #region Constructors
        public ExportDialog()
        {
            InitializeComponent();

            var res = ResourceLoader.GetForCurrentView();
            Title = res.GetString("Exporting") + "...";
        }
        #endregion

        #region Methods
        /// <summary>
        /// Display the game
        /// </summary>
        /// <param name="game">The game to display</param>
        public void SetGame(Game game)
        {
            imgGameLogo.Source = game.LogoUWP;
            txtGameTitle.Text = game.DisplayName;
        }

        /// <summary>
        /// Display the statut
        /// </summary>
        /// <param name="statut">The statut to display</param>
        public void SetStatut(string statut) => txtStatut.Text = statut;

        /// <summary>
        /// Display the current progress
        /// </summary>
        /// <param name="progress">The progress to display</param>
        public void SetProgress(double progress) => progressBar.Value = progress;
        #endregion
    }
}
