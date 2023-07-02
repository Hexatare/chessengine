using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChessEngineClassLibrary;


namespace ChessEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Create Board
            Board ChessBoard = new Board(ChessGrid);

            
            //ChessBoard.InitChessBoard(ChessGrid);
        }

 
        /// <summary>
        /// Menu Command to quit the Application, to be confirmed by the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemQuit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show("Wollen Sie das Spiel beenden", "Chess", MessageBoxButton.YesNo);
            if (Result == MessageBoxResult.Yes) { Application.Current.Shutdown(); }
            
        }


        /// <summary>
        /// Binding to the NewGame ShortCut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }



        /// <summary>
        /// Binding to the NewGame ShortCut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Command to the Board, to start a new Game
        }



        /// <summary>
        /// Menu Commnand NewGame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemNewGame_Click(object sender, RoutedEventArgs e)
        {
            // Command to the Board, to start a new Game
        }

        /// <summary>
        /// Shows Information about the Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chess Version 0.1", "About Chess", MessageBoxButton.OK);
        }

    }
}
