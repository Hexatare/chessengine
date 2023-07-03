using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaktionslogik für SettingDialog.xaml
    /// </summary>
    public partial class SettingDialog : Window
    {

        public SettingDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the Values of the Combo Boxes and display Initial Values
        /// </summary>
        /// <param name="Mode">current playmode</param>
        /// <param name="Color">current color of player one</param>
        /// <param name="Difficulty">difficulty, when playmode == computer</param>
        public void SetValues(List<string> Mode, List<string> Color, List<string> Difficulty)
        {
            CboMode.ItemsSource = Mode;
            CboColor.ItemsSource = Color;
            CboDiff.ItemsSource = Difficulty;

            CboMode.SelectedIndex = 0;
            CboColor.SelectedIndex = 0;
            CboDiff.SelectedIndex = 0;
        }

        
        /// <summary>
        /// Ok Button pressed, retreive the selected Values 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdOk_Click(object sender, RoutedEventArgs e)
        {
            // Retreive the selected values
            int modeIndex = CboMode.SelectedIndex;
            int colorIndex = CboColor.SelectedIndex;
            int difficultyIndex = CboDiff.SelectedIndex;

            Debug.WriteLine("Mode: " + modeIndex +" Color: " + colorIndex + " Difficulty: " +  difficultyIndex);

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
            this.Close();
        }
    }
}
