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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Model = Satori.Model;

namespace Satori
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool AnswerTextBoxIsShadowText { get; set; }
        private List<Model.Deck> Decks { get;  set; }

        public MainWindow()
        {
            InitializeComponent();
            Decks = new List<Model.Deck>();
            SelectedDecksList.ItemsSource = Decks;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var loadWindow = new LoadWindow();
            Decks = new List<Model.Deck>();
            loadWindow.ShowDialog();

            AddSelectedDeckToList(loadWindow);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeckBuilder_Click(object sender, RoutedEventArgs e)
        {
            var DeckBuilderWindow = new DeckBuilderWindow();
            DeckBuilderWindow.ShowDialog();
        }


        private void LoadAdditional_Click(object sender, RoutedEventArgs e)
        {
            var loadWindow = new LoadWindow();
            loadWindow.ShowDialog();
            AddSelectedDeckToList(loadWindow);
        }

        private void AddSelectedDeckToList(LoadWindow loadWindow)
        { 
            if (loadWindow.SelectedDeck != null)
            {
                Decks.Add(loadWindow.SelectedDeck);
                SelectedDecksList.ItemsSource = Decks;
                SelectedDecksList.Items.Refresh();
            }
        }

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            if (Decks.Count == 0)
            {
                MessageBox.Show("No decks are loaded");
                return;
            }
            var ftw = new FlashcardTestWindow(Decks);

            if (ftw.Running)
            {
                ftw.ShowDialog();
            }
            
        }
    }
}
