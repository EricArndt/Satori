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
using System.ComponentModel;
using Model = Satori.Model;

namespace Satori
{
    /// <summary>
    /// </summary>
    public partial class LoadWindow : Window
    {
        private GridViewColumnHeader currentColumn = null;
        private SortAdorner currentAdorner = null;
        public Model.Deck SelectedDeck { get; private set; }



        public LoadWindow()
        {
            InitializeComponent();
            var languages = Model.Language.LoadAllLanguages().OrderBy(language => language.Name).ToList();
            var decks = Model.Deck.LoadAllVisibleDecks().OrderBy(language => language.Name).ToList();

            var any = new Model.Language();
            any.Name = "Any Language";
            any.LanguageID = -1;
            languages.Insert(0, any);

            FrontLanguageComboBox.ItemsSource = languages;
            BackLanguageComboBox.ItemsSource = languages;
            deckList.ItemsSource = decks;

            FrontLanguageComboBox.SelectedIndex = 0;
            BackLanguageComboBox.SelectedIndex = 0;
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            UserSelectedDeck();
            
        }

        private void deckList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UserSelectedDeck();
        }

        private void UserSelectedDeck()
        {
            var selected = deckList.SelectedItem as Model.Deck;

            if (selected != null)
            {
                SelectedDeck = selected;
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SortDeckView_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            string field = column.Tag as string;

            if (currentColumn != null)
            {
                AdornerLayer.GetAdornerLayer(currentColumn).Remove(currentAdorner);
                deckList.Items.SortDescriptions.Clear();
            }

            ListSortDirection direction = ListSortDirection.Ascending;
            if (currentColumn == column && currentAdorner.Direction == direction)
            {
                direction = ListSortDirection.Descending;
            }

            currentColumn = column;
            currentAdorner = new SortAdorner(currentColumn, direction);
            AdornerLayer.GetAdornerLayer(currentColumn).Add(currentAdorner);
            deckList.Items.SortDescriptions.Add(new SortDescription(field, direction));
        }
        
        public class SortAdorner : Adorner
        {
            private readonly static Geometry downArrow = Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");
            private readonly static Geometry upArrow = Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

            public ListSortDirection Direction { get; private set; }

            public SortAdorner(UIElement element, ListSortDirection dir): base(element)
            { 
                Direction = dir; 
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                drawingContext.PushTransform(
                    new TranslateTransform(
                      AdornedElement.RenderSize.Width - 15,
                      (AdornedElement.RenderSize.Height - 5) / 2));

                drawingContext.DrawGeometry(Brushes.Black, null,
                    Direction == ListSortDirection.Ascending ?
                      downArrow : upArrow);

                drawingContext.Pop();
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var frontSelection = FrontLanguageComboBox.SelectedItem as Model.Language;
            if (frontSelection == null || frontSelection.LanguageID == -1)
            {
                FrontLanguageComboBox.SelectedItem = 0;
            }

            var backSelection = BackLanguageComboBox.SelectedItem as Model.Language;
            if (backSelection == null || backSelection.LanguageID == -1)
            {
                BackLanguageComboBox.SelectedItem = 0;
            }

            if (BackLanguageComboBox.SelectedIndex == 0 && FrontLanguageComboBox.SelectedIndex == 0)
            {
                deckList.ItemsSource = Model.Deck.LoadAllVisibleDecks();
            }
            else 
            {
                var BackDeck = BackLanguageComboBox.SelectedItem as Model.Language;
                var FrontDeck = FrontLanguageComboBox.SelectedItem as Model.Language;

                if (FrontDeck != null && FrontDeck.LanguageID == -1)
                {
                    FrontDeck = null;
                }

                if (BackDeck != null && BackDeck.LanguageID == -1)
                {
                    BackDeck = null;
                }

                deckList.ItemsSource = Model.Deck.LoadAllVisibleDecks(FrontDeck, BackDeck);
            }
        }
    }
}
