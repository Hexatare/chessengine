using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ChessEngine
{
    /// <summary>
    /// Interaktionslogik für SettingDialog.xaml
    /// </summary>
    public partial class SettingDialog : Window
    {

        #region Properties and Members

        /// <summary>
        /// Current Settings of the Game
        /// </summary>
        public GameSettings Settings { get; private set; }

        /// <summary>
        /// List with all Texts for the Mode Combobox
        /// </summary>
        private List<string> modeTxt;

        /// <summary>
        /// List with all Texts for the Color Combobox
        /// </summary>
        private List<string> colorTxt;

        /// <summary>
        /// List with all Texts for the Difficulty Combobox
        /// </summary>
        private List<string> diffTxt;

        /// <summary>
        /// List with all Texts for the Time Combobox
        /// </summary>
        private List<int> timeTxt;
        
        #endregion

        #region Constructor
            
        /// Constructor
        /// </summary>
        public SettingDialog()
        {
            InitializeComponent();

            // Create the List Elements with ist values
            modeTxt = new List<string> { "Mensch", "Computer" };
            colorTxt = new List<string> { "Weiss", "Schwarz" };
            diffTxt = new List<string> { "Leicht", "Mittel", "Schwer" };

            timeTxt = new List<int>();

            foreach (GameTime time in Enum.GetValues(typeof(GameTime)))
            {
                timeTxt.Add( (int)time);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the Values of the Combo Boxes and display Initial Values
        /// </summary>
        /// <param name="Mode">current playmode</param>
        /// <param name="Color">current color of player one</param>
        /// <param name="Difficulty">difficulty, when playmode == computer</param>
        public void SetGameSettings(GameSettings currSettings)
        {
            this.Settings = currSettings;

            CboMode.ItemsSource = modeTxt;
            CboColor.ItemsSource = colorTxt;
            CboDiff.ItemsSource = diffTxt;
            CboTime.ItemsSource = timeTxt;

            CboMode.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(GameMode)), currSettings.Mode);
            CboColor.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(Piece.PColor)), currSettings.Color);
            CboDiff.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(Difficulty)), currSettings.Difficulty);
            CboTime.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(GameTime)), currSettings.TimePlay);
        }

        
        /// <summary>
        /// Ok Button pressed, retreive the selected Values 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdOk_Click(object sender, RoutedEventArgs e)
        {
            // Retreive the selected values
            Settings.Mode = Enum.GetValues<GameMode>()[CboMode.SelectedIndex];
            Settings.Color = Enum.GetValues<Piece.PColor>()[CboColor.SelectedIndex];
            Settings.Difficulty = Enum.GetValues<Difficulty>()[CboDiff.SelectedIndex];
            Settings.TimePlay = Enum.GetValues<GameTime>()[CboTime.SelectedIndex];

            // Return Result == true
            this.DialogResult = true;

            // Close the Dialog
            this.Close();
        }


        /// <summary>
        /// Cancel Button pressed, close the Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        #endregion
    }
}
