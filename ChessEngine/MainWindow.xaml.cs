using ChessEngineClassLibrary;
using ChessEngineClassLibrary.Resources;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        private Board board;


        /// <summary>
        /// Reference to the Game
        /// </summary>
        private Game game;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            // Create Board
            board = new Board(ChessGrid);

            // Create the Game Class
            game = new Game(board);

        }


        #region Command Bindings

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

        private void CommandBindingOpen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Ask for the End of the Game first
            this.MenuItemEndGame_Click(sender, e);

            if (game.gameState != Game.GameState.None)
                return;

            ShowOpenFileDialog();
        }

        private void CommandBindingSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowSaveFileDialog();
        }

        private void ChessMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MenuItemNewGame.IsEnabled = true;
            this.MenuItemSave.IsEnabled = false;
            this.MenuItemEndGame.IsEnabled = false;
            this.MenuItemUndo.IsEnabled = false;
        }


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

            if (game.gameState != Game.GameState.None)
                return;

            // Command to the Board, to start a new Game
            game.SetNewGame(Resource1.DefaultFEN);
            this.MenuItemSave.IsEnabled = true;
            this.MenuItemUndo.IsEnabled = true;
            this.MenuItemEndGame.IsEnabled = true;
        }


        /// <summary>
        /// Ends the current Game, ask the User for Confirmation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemEndGame_Click(object sender, RoutedEventArgs e)
        {
            if(game.gameState != Game.GameState.None)
            {
                MessageBoxResult Result = MessageBox.Show("Wollen Sie das laufende Spiel beenden", "Chess", 
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (Result == MessageBoxResult.Yes)
                {
                    game.EndGame();
                    this.MenuItemEndGame.IsEnabled = false;
                    this.MenuItemSave.IsEnabled=false;
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

            MessageBox.Show("Chess Play, Current Version 0.1\nDeveloped by Noël, written by Amir.",
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
            game.UndoLastMove();
        }

        #endregion


        #region Private Methods

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
                        this.MenuItemUndo.IsEnabled = true;
                        this.MenuItemEndGame.IsEnabled = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("An error occurred while reading the file: " + e.Message);
                }
            }
        }

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

                // TODO: Save the file at the selected file path
            }
        }

        #endregion

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingDialog settingDialog = new SettingDialog();
            settingDialog.Owner = this;
            settingDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Setting the Properties of the Window
            settingDialog.SetValues(new List<string> { "Human", "Computer" },
                                    new List<string> { "White", "Black" },
                                    new List<string> { "Easy", "Intermediate", "Hard" });
            // Show the Dialog
            settingDialog.ShowDialog();

        }
    }
}
