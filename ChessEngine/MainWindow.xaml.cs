using ChessEngine.Dialogs;
using ChessEngine.Models;
using ChessEngineClassLibrary;
using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using ChessEngineClassLibrary.Resources;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;


namespace ChessEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Properties and private Members

        /// <summary>
        /// Reference to the board
        /// </summary>
        private readonly Board board;

        /// <summary>
        /// Reference to the Game
        /// </summary>
        private readonly Game game;

        /// <summary>
        /// The ViewModel for the Board
        /// </summary>
        private readonly MainWindowViewModel viewModel;
           
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the Main Window, which create the Board and the Game Class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Create Board
            board = new Board();

            // Create the Game Class
            game = new Game(board);

            viewModel = new MainWindowViewModel();
            viewModel.CreateCells(this.ChessGrid, this.board, this.game);

            // Register Eventhandler for Promotion Dlg
            game.PromotionEvent += new EventHandler(PromotionDlgEvent);

            // Register Eventhandler for End Game Dialog
            game.EndGameEvent += new EventHandler(GameEndDlgEvent);   

        }

        #endregion

        #region Command Bindings

        /// <summary>
        /// Binding to the NewGame ShortCut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (game.ActGameState != GameState.None)
                e.CanExecute = false;
            else
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
            this.MenuItemNewGame_Click(sender, e);
        }


        /// <summary>
        /// Binding to the Open ShortCut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingOpen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (game.ActGameState != GameState.None)
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }
        

        /// <summary>
        /// Command Short Cut for opening a saved Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Ask for the End of the Game first
            this.MenuItemEndGame_Click(sender, e);
            ShowOpenFileDialog();
        }


        /// <summary>
        /// Binding to the Open ShortCut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (game.ActGameState != GameState.None)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }


        /// <summary>
        /// Command Short Cut to save the current Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowSaveFileDialog();
        }


        /// <summary>
        /// Action Method, when the Window ist loaded and shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChessMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MenuItemNewGame.IsEnabled = true;
            this.MenuItemSave.IsEnabled = false;
            this.MenuItemEndGame.IsEnabled = false;
            this.MenuItemSettings.IsEnabled = true;
            //this.MenuItemUndo.IsEnabled = false;
        }


        /// <summary>
        /// Method is called, when the user presses the window close buttion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChessMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show("Wollen Sie das Programm beenden", "Chess", MessageBoxButton.YesNo, MessageBoxImage.Question);
    
            if (Result == MessageBoxResult.No)
                e.Cancel = true;
        }


        #endregion

        #region Menu Commands

        /// <summary>
        /// Menu Commnand NewGame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemNewGame_Click(object sender, RoutedEventArgs e)
        {
            // Ask for the End of the Game first
            this.MenuItemEndGame_Click(sender, e);

            if (game.ActGameState != GameState.None)
                return;

            // Command to the Board, to start a new Game
            game.SetNewGame(Resource1.DefaultFEN);
            this.MenuItemSave.IsEnabled = true;
            //this.MenuItemUndo.IsEnabled = true;
            this.MenuItemEndGame.IsEnabled = true;
            this.MenuItemSettings.IsEnabled = false;
        }


        /// <summary>
        /// Ends the current Game, ask the User for Confirmation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemEndGame_Click(object sender, RoutedEventArgs e)
        {
            if(game.ActGameState != GameState.None)
            {
                MessageBoxResult Result = MessageBox.Show("Wollen Sie das laufende Spiel beenden", "Chess", 
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (Result == MessageBoxResult.Yes)
                {
                    game.SetGameEnd();
                    this.MenuItemEndGame.IsEnabled = false;
                    this.MenuItemSave.IsEnabled=false;
                    this.MenuItemSettings.IsEnabled = true;
                }
            }
        }


        /// <summary>
        /// Menu Command to quit the Application, to be confirmed by the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        /// <summary>
        /// Shows Information about the Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBoxButton button = MessageBoxButton.OK;

            MessageBox.Show("Chess Play, Current Version 0.1\nDeveloped and written by Noël.",
                "About Chess", button, icon);
        }


        /// <summary>
        /// Menu Command to undo the last move on the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemUndo_Click(object sender, RoutedEventArgs e)
        {
            // Undo the last move of the player
            //game.UndoLastMove();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Shows the standard Windows Open File Dialog
        /// </summary>
        private void ShowOpenFileDialog()
        {

            // Create an instance of the OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set properties of the dialog
            openFileDialog.Title = "Select a File"; // Dialog title
            openFileDialog.Filter = "Text Files (*.txt)|*.txt"; // File filter
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Initial directory

            // Show the dialog and check if the user clicked the "OK" button
            if (openFileDialog.ShowDialog() == true)
            {
                // Retrieve the selected file path
                string selectedFilePath = openFileDialog.FileName;

                try
                {
                    using StreamReader? sr = new StreamReader(selectedFilePath);
                    string line;

                    if ((line = sr.ReadLine()) != null)
                    {
                        game.SetNewGame(line);
                        this.MenuItemSave.IsEnabled = true;
                        //this.MenuItemUndo.IsEnabled = true;
                        this.MenuItemEndGame.IsEnabled = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("An error occurred while reading the file: " + e.Message);
                }
            }
        }


        /// <summary>
        /// Shows the standard Windows File Save Dialog
        /// </summary>
        private void ShowSaveFileDialog()
        {
            // Create an instance of the SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set properties of the dialog
            saveFileDialog.Title = "Save File"; // Dialog title
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"; // File filter
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Initial directory
            saveFileDialog.FileName = "Untitled.txt"; // Default file name

            // Show the dialog and check if the user clicked the "OK" button
            if (saveFileDialog.ShowDialog() == true)
            {
                // Retrieve the selected file path
                string selectedFilePath = saveFileDialog.FileName;

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog.FileName != "")
                {
                    // Saves the Image via a FileStream created by the OpenFile method.
                    StreamWriter fs = new StreamWriter(saveFileDialog.OpenFile());
                    fs.Write(game.GetFenString());
                    fs.Flush();
                    fs.Close();
                }
            }
        }


        /// <summary>
        /// Shows the Game Settings Dialog and saves the Value to the Game Class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingDialog settingDialog = new SettingDialog();
            settingDialog.Owner = this;
            settingDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Setting the Properties of the Window
            settingDialog.SetGameSettings(game.CurrGameSettings);

            // Show the Dialog
            Nullable<bool> dialogResult = settingDialog.ShowDialog();

            if(dialogResult == true) 
            {
                game.CurrGameSettings = settingDialog.Settings;
            }
        }


        /// <summary>
        /// Delegate for the show Promotion Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PromotionDlgEvent(object sender, EventArgs e)
        {
            PromotionDialog promotionDialog = new PromotionDialog();
            promotionDialog.Owner = this;
            promotionDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Show the Dialog
            Nullable<bool> dialogResult = promotionDialog.ShowDialog();

            game.PromotionPiece = promotionDialog.SelectedPiece;
        }


        /// <summary>
        /// Delegate for the show Promotion Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEndDlgEvent(object sender, EventArgs e)
        {
            GameEndEventArgs gameResult = (GameEndEventArgs)e;
            GameEndDialog gameEndDialog = new GameEndDialog(gameResult);
            gameEndDialog.Owner = this;
            gameEndDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            gameEndDialog.Show();
        }


        #endregion
    }
}
