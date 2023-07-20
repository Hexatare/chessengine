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
        #region Properties and Members

        /// <summary>
        /// Texts for who is the winner
        /// </summary>
        private readonly string[] TxtGameResult = { "Weiss gewinnt", "Schwarz gewinnt", "Unentschieden" };

        /// <summary>
        /// Texts for Game End Reason
        /// </summary>
        private readonly string[] TxtGameEndReason = { "Nichts", "Aufgabe", "Patt", "Zeit abgelaufen", "Übereinkunft", 
                                                    "50 Zug Regel", "Zuwenig Figuren", "Zuwenig Figuren und Timeout", 
                                                    "Schachmatt"};

        /// <summary>
        /// Index for Draw Reason Text
        /// </summary>
        private readonly int TxtIndexDraw = 2;


        #endregion



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameResults">Gameresults to display</param>
        public GameEndDialog(GameEndEventArgs? gameResults)
        {
            InitializeComponent();

            int gameEndReason = 0;
            
            if(gameResults != null )
                gameEndReason = (int)gameResults.Reason;

            // Setup the Labels
            this.LblMoves.Content = gameResults?.NbrOfMoves.ToString();
            this.CboPiece.ItemsSource = gameResults?.CapturedPieces;
            this.LblTime.Content = gameResults?.TimePlayed.ToString();
            this.LblReason.Content = TxtGameEndReason[gameEndReason];

            // Ermittlung des Gewinners
            if(    (gameResults?.Reason == GameEndReason.Stalemate)
                || (gameResults?.Reason == GameEndReason.Checkmate) )
            {
                this.LblWinner.Content = TxtGameResult[(int)gameResults.Winner];
            }

            if(gameResults?.Reason == GameEndReason.ClockFlagged)
            {
                this.LblWinner.Content = TxtGameResult[ ((int)gameResults.Winner + 1) % 2];
            }

            if (    (gameResults?.Reason == GameEndReason.Resignation)
                 || (gameResults?.Reason == GameEndReason.Aggrement)
                 || (gameResults?.Reason == GameEndReason.FiftyMoveRule)
                 || (gameResults?.Reason == GameEndReason.Insufficient_Material)
                 || (gameResults?.Reason == GameEndReason.TimeOutVsInsufficient_Material))
            {
                this.LblWinner.Content = TxtGameResult[TxtIndexDraw];
            }
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
