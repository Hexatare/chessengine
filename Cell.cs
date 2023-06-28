using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        // The Grid that is shown on the frontend
        public Grid CellGrid { get; set; }

        // Property for the index of the cell
        public int CellIndex { get; set; }

        // Property to hold the piece type on the cell
        public Piece PieceOnCell { get; set; }

        // Boolean property indicating whether the cell is currently occupied
        public bool CurrentlyOccupied { get; set; }

        // Event to notify subscribers of property changes
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Constructor
        public Cell(SolidColorBrush cellColor, int cellIndex, string? pieceColor, Grid cellGrid, Grid chessGrid, string? pieceType = null)
        {
            CellGrid = cellGrid;
            CellIndex = cellIndex;
            CellColor = cellColor;

            if (pieceColor != null)
            {
                
            }
            else
            {
                PieceOnCell = null;
                CurrentlyOccupied = false;
            }
        }
    }
}
