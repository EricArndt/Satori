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
using System.IO;

namespace Satori
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FlashcardTestWindow : Window
    {
        private bool AnswerTextBoxIsShadowText { get; set; }
        private FlashcardQuiz FQ;

        public bool Running { get; private set; }

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
                QuestionTextBox.Text = currentCard.FrontText;
                SetImageFromCard();
            }
        }

        public FlashcardTestWindow(List<Model.Deck> decks)
        {
            Running = true;
            InitializeComponent();
            FQ = new FlashcardQuiz(decks);
            IterateRun();
            AnswerTextBoxEnableShadowText();
            
        }


        private void AnswerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SubmitButton_Click(sender, e);
            }
        }

        private void answerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AnswerTextBox.Text == "")
            {
                AnswerTextBoxEnableShadowText();
                AnswerTextBoxIsShadowText = true;
            }
            else
            {
                AnswerTextBoxIsShadowText = false;
            }

            AnswerTextBox.CaretIndex = 0;
        }

        private void AnswerTextBoxEnableShadowText()
        {
            AnswerTextBox.Text = "Answer Here";
            AnswerTextBoxIsShadowText = true;
            AnswerTextBox.Foreground = Brushes.Gray;
        }

        private void AnswerTextBoxDisableShadowText()
        {
            AnswerTextBoxIsShadowText = false;
            AnswerTextBox.Foreground = Brushes.Black;
        }

        private void answerTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (AnswerTextBox.Text != "" && AnswerTextBoxIsShadowText)
            {
                AnswerTextBox.Text = "";
                AnswerTextBoxDisableShadowText();
            }
            (sender as TextBox).SelectAll();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var response = FQ.CheckResponse(AnswerTextBox.Text);
            MessageBox.Show(response);
            IterateRun();
        }

        private void IterateRun()
        {
            if (CheckForCompletion())
            {
                LoadCard();
                SetProgressBar();
                SetupTextBoxes();
            }
        }

        private void SetupTextBoxes()
        {
            AnswerTextBox.Clear();
            QuestionTextBox.Focus();
            AnswerTextBox.Focus();
        }

        private void SetProgressBar()
        { 
            progressBar.Value = FQ.CorrectCards;
            progressBar.Maximum = FQ.TotalCards;
        }

        private void LoadCard()
        {
            CurrentCard = FQ.DrawCard();
        }

        private bool CheckForCompletion()
        {
            if(FQ.TotalCards == 0)
            {
                MessageBox.Show("There are no cards");
                Running = false;
                return false;
            }

            if (FQ.CorrectCards == FQ.TotalCards)
            {
                var result = MessageBox.Show("Congratulations you have finished going through all of the flashcards! Would you like to practice them again?",
                     "Flashcards Completed", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    FQ.Reset();
                    return true;
                }
                else 
                {
                    this.Close();
                    return false;
                }
            }

            return true;
        }

        private void SetImageFromCard()
        {
            if (currentCard.Picture != null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(currentCard.Picture);
                bi.EndInit();
                flashcardImage.Source = bi;
            }
            else
            {
                flashcardImage.Source = null;
            }
        }
    }
}
