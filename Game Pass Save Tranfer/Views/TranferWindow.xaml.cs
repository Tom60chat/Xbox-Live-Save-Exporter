using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Game_Pass_Save_Tranfer
{
    /// <summary>
    /// Logique d'interaction pour TranferWindow.xaml
    /// </summary>
    public partial class TranferWindow : Window
    {
        public TranferWindow()
        {
            InitializeComponent();
        }

        public void SetGame(Game game)
        {
            imgGameLogo.Source = new BitmapImage(new Uri(game.Logo));
            txtGameTitle.Text = game.DisplayName;
        }

        public void SetStatut(string statut) => txtStatut.Text = statut;
        public void SetProgress(double progress) => progressBar.Value = progress;
    }
}
