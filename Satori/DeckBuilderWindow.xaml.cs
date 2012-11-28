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
using Model = Satori.Model;
using System.IO;

namespace Satori
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DeckBuilderWindow : Window
    {
        private Model.Deck selectedDeck;
        private Model.Deck SelectedDeck
        {
            get
            { 
                return selectedDeck; 
            }
            set
            { 
                selectedDeck = value;
                SelectedDeckTextBox.Text = selectedDeck.Name;
                CardList = Model.Card.SelectAllCardsByDeckID(selectedDeck.DeckID);
            }
        }

        private List<Model.Card> cardList;
        private List<Model.Card> CardList
        {
            get 
            {
                return cardList;
            }
            set
            {
                cardList = value;
                CardSelector.ItemsSource = cardList;
            }
        }

        private Model.Card currentCard;
        private Model.Card CurrentCard
        {
            get
            {
                return currentCard;
            }
            set 
            {
                currentCard = value;
                if (value != null)
                { 
                    FrontTextBox.Text = currentCard.FrontText;
                    BackTextBox.Text = currentCard.BackText;
                    FrontLanguageComboBox.SelectedIndex = FrontLanguageComboBox.Items.IndexOf(Languages.Find(l => l.LanguageID == currentCard.FrontLanguage));
                    BackLanguageComboBox.SelectedIndex = BackLanguageComboBox.Items.IndexOf(Languages.Find(l => l.LanguageID == currentCard.BackLanguage));
                    SetImageFromCard();
                }
            }
        }

        private void SetImageFromCard()
        {
            if (currentCard.Picture != null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(currentCard.Picture);
                bi.EndInit();
                Image.Source = bi;
            }
            else 
            {
                Image.Source = null;
            }
        }

        private List<Model.Language> Languages { get; set; }

        public DeckBuilderWindow()
        {
            InitializeComponent();
            Languages = Model.Language.LoadAllLanguages().OrderBy(language => language.Name).ToList();

            SetLanguageComboBoxes();
        }

        private void SelectDeckButton_Click(object sender, RoutedEventArgs e)
        {
            var loadWindow = new LoadWindow();
            loadWindow.ShowDialog();
            if (loadWindow.SelectedDeck != null)
            {
                SelectedDeck = loadWindow.SelectedDeck;   
            }
        }

        private void DeleteDeckButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "If you delete this deck all cards in the deck will also be lost. Are you sure you want to delete this deck?",
                "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.OK && selectedDeck != null)
            {
                Model.Deck.DeleteDeck(selectedDeck);
                SelectedDeck = new Model.Deck();
            }
        }

        private void CreateNewDeckButton_Click(object sender, RoutedEventArgs e)
        {
            var DeckNamingWindow = new DeckNamingWindow();
            DeckNamingWindow.ShowDialog();
            if (DeckNamingWindow.DeckName != null)
            {
                int DeckID = Model.Deck.CreateNewDeck(DeckNamingWindow.DeckName);
                SelectedDeck = Model.Deck.LoadByDeckID(DeckID);
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var PictureLoader = new Microsoft.Win32.OpenFileDialog();
            PictureLoader.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            var result = PictureLoader.ShowDialog();

            if (result == true)
            {
                try
                {
                    CurrentCard.Picture = System.IO.File.ReadAllBytes(PictureLoader.FileName);
                    Image.Source = new BitmapImage(new Uri(PictureLoader.FileName));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void AddNewLanguage_Click(object sender, RoutedEventArgs e)
        {
            var languageNamingWindow = new LanguageNamingWindow();
            languageNamingWindow.ShowDialog();
            SetLanguageComboBoxes();
        }

        private void DeleteLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            var languageRemovalWindow = new LanguageRemovalWindow();
            languageRemovalWindow.ShowDialog();
            Languages = Model.Language.LoadAllLanguages().OrderBy(language => language.Name).ToList();
            SetLanguageComboBoxes();
        }

        private void SetLanguageComboBoxes()
        {
            FrontLanguageComboBox.ItemsSource = Languages;
            BackLanguageComboBox.ItemsSource = Languages;
        }

        private void AddCardButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDeck != null)
            { 
                var NewCard = new Model.Card();
                NewCard.FrontText = "New Card";
                NewCard.DeckID = selectedDeck.DeckID;

                CardList.Add(NewCard);
                CardSelector.Items.Refresh();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FrontTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentCard != null)
            { 
               CurrentCard.FrontText = FrontTextBox.Text;
               CardSelector.Items.Refresh();
            }
        }

        private void BackTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentCard != null)
            {
                CurrentCard.BackText = BackTextBox.Text;
            }
        }

        private void FrontLanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentCard != null)
            {
                if (FrontLanguageComboBox.SelectedItem == null)
                {
                    CurrentCard.FrontLanguage = null;
                }
                else
                {
                    CurrentCard.FrontLanguage = (FrontLanguageComboBox.SelectedItem as Model.Language).LanguageID;
                }
                
            }
        }

        private void BackLanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentCard != null)
            {
                if (BackLanguageComboBox.SelectedItem == null)
                {
                    CurrentCard.BackLanguage = null;
                }
                else
                {
                    CurrentCard.BackLanguage = (BackLanguageComboBox.SelectedItem as Model.Language).LanguageID;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDeck != null)
            {
                foreach (var card in CardList)
                {
                    Model.Card.AddCard(card);
                }

                CardList = Model.Card.SelectAllCardsByDeckID(selectedDeck.DeckID);

                Model.Deck.UpdateDeckTimestamp(SelectedDeck);
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteCardButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCard != null)
            {
                CardList.Remove(CurrentCard);

                Model.Card.DeleteCard(CurrentCard);

                CleanupAfterDeletingCard();

                CardSelector.Items.Refresh();
            }
        }

        private void CleanupAfterDeletingCard()
        {
            CurrentCard = null;
            FrontTextBox.Text = "";
            BackTextBox.Text = "";
            FrontLanguageComboBox.SelectedIndex = -1;
            BackLanguageComboBox.SelectedIndex = -1;
        }

        private void CardSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentCard = CardSelector.SelectedItem as Model.Card;
        }
    }
}
