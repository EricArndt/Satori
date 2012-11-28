using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Satori
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DeckNamingWindow : Window
    {
        public string DeckName { get; private set; }

        public DeckNamingWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DeckName = DeckNameTextBox.Text;
            if (DeckName != "")
            { 
                this.Close();
            }
        }

        private void DeckNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OK_Click(sender, e);
            }
        }
    }
}
