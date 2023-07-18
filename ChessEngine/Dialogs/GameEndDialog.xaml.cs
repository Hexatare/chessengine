using ChessEngineClassLibrary.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ChessEngine.Dialogs
{
    /// <summary>
    /// Interaktionslogik für GameEndDialog.xaml
    /// </summary>
    public partial class GameEndDialog : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameResults">Gameresults to display</param>
        public GameEndDialog(GameEndEventArgs gameResults)
        {
            InitializeComponent();

            // Setup the Labels
            this.LblWinner.Content = gameResults?.Winner.ToString();
            this.LblMoves.Content = gameResults?.NbrOfMoves.ToString();
            this.CboPiece.ItemsSource = gameResults?.CapturedPieces;
            this.LblTime.Content = gameResults?.TimePlayed.ToString();
            this.LblReason.Content = gameResults?.Reason.ToString();

            this.CboPiece.SelectedIndex = 0;
        }


        /// <summary>
        /// Button Event, wenn das Fenster geschlossen werden soll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
