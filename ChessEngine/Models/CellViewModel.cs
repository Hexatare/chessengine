using ChessEngineClassLibrary.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static ChessEngineClassLibrary.Cell;

namespace ChessEngine.Models
{

    /// <summary>
    /// Class to display the current view of a Cell
    /// </summary>
    public class CellViewModel
    {

        #region Properties and Members

        // Background Color for this Cell
        private Color cellBackgroundColor;

        /// <summary>
        /// Current Image displayed on the cell
        /// </summary>
        private Image? currentImage;

        /// <summary>
        /// The Border of the Cell
        /// </summary>
        private readonly Border cellBorder;

        /// <summary>
        /// Graphical Element, that is placed in the Board
        /// </summary>
        public Grid grid { set; get; }

        /// <summary>
        /// Property for the index of the cell
        /// </summary>
        public int Index { get; set; }


        /// <summary>
        /// A public Eventdelegate for a Cell that is selected
        /// </summary>
        public event EventHandler? CellSelected;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the visual representation of a Cell on the board
        /// </summary>
        /// <param name="cellColor">Background Color of the cell</param>
        /// <param name="cellIndex">Index of the Cell</param>
        public CellViewModel(CellColor cellColor, int cellIndex)
        {
            Index = cellIndex;

            // Create the Border
            cellBorder = new Border();
            cellBorder.BorderBrush = Brushes.Green;
            cellBorder.BorderThickness = new Thickness(0);
            cellBorder.Name = "CellBorder";

            // Generate the grid
            grid = new Grid();

            // Action, when Mouse Button is pushed
            grid.MouseLeftButtonUp += Grid_MouseLeftButtonUp;

            // Set the color of the Cell
            if (cellColor == CellColor.White)
                cellBackgroundColor = Colors.Beige;
            else
                cellBackgroundColor = Colors.Brown;

            // Set the Background of the Cell
            grid.Background = new SolidColorBrush(cellBackgroundColor);

            // Add the Boarder to the grid
            grid.Children.Add(cellBorder);

        }

        #endregion

        #region Methods

        /// <summary>
        /// Eventhandler for Cell Selection
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnCellSelected(EventArgs e)
        {
            CellSelected?.Invoke(this, e);
        }


        /// <summary>
        /// Set the Color of the Cell Border
        /// </summary>
        /// <param name="borderColor"></param>
        public void SetBorderColor(CellBorderColor borderColor)
        {
            if (borderColor == CellBorderColor.None)
            {
                cellBorder.BorderThickness = new Thickness(0);
                return;
            }
            else if (borderColor == CellBorderColor.Yellow)
                cellBorder.BorderBrush = Brushes.Yellow;
            else if (borderColor == CellBorderColor.Green)
                cellBorder.BorderBrush = Brushes.Green;
            else if (borderColor == CellBorderColor.Red)
                cellBorder.BorderBrush = Brushes.Red;

            // Set the Border of the Cell
            cellBorder.BorderThickness = new Thickness(3);
        }


        /// <summary>
        /// Sets the Image on the Cell
        /// </summary>
        /// <param name="image">Image to show</param>
        public void SetImage(Image image)
        {
            this.currentImage = image;
            grid.Children.Add(image);
        }


        /// <summary>
        /// Removes the Image from the Cell 
        /// </summary>
        public void RemoveImge()
        {
            if (currentImage != null)
            {
                grid.Children.Remove(currentImage);
            }
        }


        /// <summary>
        /// Mouse Event, wenn the Cell is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Notify all Eventhandler on the Cell

            //----- Test -----
            // Start fetching the weather forecast asynchronously.
            if(e != null)
            {
                var mouseEvDel = new MouseEventDelegat(OnCellSelected);
                mouseEvDel.Invoke(e);
            }

        }

        private delegate void MouseEventDelegat(EventArgs e);


        #endregion

    }
}
