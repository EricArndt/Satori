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
using System.Data.SqlClient;

namespace Satori
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LanguageRemovalWindow : Window
    {
        public LanguageRemovalWindow()
        {
            InitializeComponent();

            LanguageRemovalComboBox.ItemsSource = Model.Language.LoadAllLanguages().OrderBy(language => language.Name).ToList();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if(LanguageRemovalComboBox.SelectedItem != null)
            {
                    if(Model.Language.DeleteLanguageByID(LanguageRemovalComboBox.SelectedItem as Model.Language))
                    {
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("The selected language could not be deleted because the language is used on at least one card.", 
                            "Could Not Delete Language", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
            }
        }
    }
}
