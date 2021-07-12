using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Game_Pass_Save_Tranfer
{
    /// <summary>
    /// Logique d'interaction pour TranferWindow.xaml
    /// </summary>
    public partial class TranferWindow : Window
    {
        #region Constructors
        public TranferWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Display the game
        /// </summary>
        /// <param name="game">The game to display</param>
        public void SetGame(Game game)
        {
            imgGameLogo.Source = new BitmapImage(new Uri(game.Logo));
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
