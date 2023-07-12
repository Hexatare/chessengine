using ChessEngineClassLibrary.Pieces;
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

namespace ChessEngine
{
    /// <summary>
    /// Interaktionslogik für PromotionDialog.xaml
    /// </summary>
    public partial class PromotionDialog : Window
    {
        #region Properies and Members

        /// <summary>
        /// List with all aviable Pieces for Promotion
        /// </summary>
        private List<string> pieces;

        /// <summary>
        /// The selected promotion Piece
        /// </summary>
        public Piece.PType SelectedPiece { set; get; }

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public PromotionDialog()
        {
            InitializeComponent();

            pieces = new List<string>() { "Dame", "Turm", "Springer", "Läufer" };

            CboPiece.ItemsSource = pieces;
            CboPiece.SelectedIndex = 0;
        }


        /// <summary>
        /// Ok Button pressed, retreive the selected Values 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdOk_Click(object sender, RoutedEventArgs e)
        {
            // Get Selected Index
            switch ( CboPiece.SelectedIndex)
            {
                case 0:
                    SelectedPiece = Piece.PType.Queen;
                    break;
                case 1:
                    SelectedPiece = Piece.PType.Rook;
                    break;
                case 2:
                    SelectedPiece = Piece.PType.Knight;
                    break;
                case 3: 
                    SelectedPiece = Piece.PType.Knight;
                    break;
            }

            // Return Result == true
            this.DialogResult = true;

            // Close the Dialog
            this.Close();
        }
    }
}
