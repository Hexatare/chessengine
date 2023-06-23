using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChessEngineClassLibrary
{
    public class Cell : INotifyPropertyChanged
    {
        // Property for the color of the cell
        private SolidColorBrush cellColor;
        public SolidColorBrush CellColor
        {
            get { return cellColor; }
            set
            {
                if (cellColor != value)
                {
                    cellColor = value;
                    OnPropertyChanged(nameof(CellColor));
                }
            }
        }

        // Property for the index of the cell
        public int CellIndex { get; set; }

        // Property to hold the piece type on the cell
        public int PieceOnCell { get; set; }

        // Boolean property indicating whether the cell is currently occupied
        public bool CurrentlyOccupied { get; set; }

        // Event to notify subscribers of property changes
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Constructor
        public Cell(SolidColorBrush cellColor, int cellIndex, int pieceOnCell = Piece.Empty)
        {
            CellIndex = cellIndex;
            CellColor = cellColor;
            PieceOnCell = pieceOnCell;

            // Set CurrentlyOccupied based on the piece type
            CurrentlyOccupied = (PieceOnCell != Piece.Empty);
        }
    }
}
