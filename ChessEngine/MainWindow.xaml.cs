using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using ChessEngineClassLibrary;
using Microsoft.Win32;
using static System.Runtime.InteropServices.JavaScript.JSType;


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

        #endregion


        #region Menu Commands

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


        private void MenuItemUndo_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion


        #region Private Methods

        private static void ShowOpenFileDialog()
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
                    using StreamReader sr = new StreamReader(selectedFilePath);
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        // Process each line of the file
                        Debug.WriteLine(line);
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine("An error occurred while reading the file: " + e.Message);
                }
            }
        }
    
        private static void ShowSaveFileDialog()
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
