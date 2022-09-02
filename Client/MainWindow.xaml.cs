using PL;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PLClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientMde clientPage;
        public MainWindow()
        {
            InitializeComponent();
            clientPage = new ClientMde();
            new Model.ViewModel();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Hidden;
            clientPage.addClient.Click += Sign_Up_From_ClientMode;
            clientPage.CancelLodin.Click += reternToMain;
            clientPage.Exit.Click += reternToMain;
            this.Frame.Content = clientPage;
        }

        private void Sign_Up_From_ClientMode(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Hidden;
            var page = new DisplayClient(" ");
            this.Frame.Content = page;

        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
        
        }

        private void Sign_Up_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Hidden;
            DisplayClient page = new DisplayClient(" ");
            page.MainWindow += reternToMain;
            this.Frame.Content = page;
        }

        private void reternToMain(object sender, RoutedEventArgs e)
        {
          
            MainGrid.Visibility = Visibility;
            clientPage = new ClientMde();
        }
    }
}
