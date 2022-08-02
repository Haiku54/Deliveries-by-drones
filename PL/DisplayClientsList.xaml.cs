using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Model;

namespace PL
{
    /// <summary>
    /// Interaction logic for DisplayClientsList.xaml
    /// </summary>
    public partial class DisplayClientsList : Page
    {
        BlApi.IBL BL;

        public delegate void ClientPage(int id);
        public event ClientPage AddClik;
        public event ClientPage DoubleClik;


        /// <summary>
        /// ctor to display client list and initlialize datacontext and observable list
        /// </summary>
        public DisplayClientsList()
        {
            InitializeComponent();
            this.BL = BlApi.BlFactory.GetBL();
            ClientListView.DataContext = Model.ViewModel.clients;
        }


        /// <summary>
        /// add new client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_New_Client(object sender, RoutedEventArgs e)
        {
            AddClik(-1);
        }


        /// <summary>
        /// opening the selected client page from the list 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientListView.SelectedItem != null) DoubleClik(((PO.ClientToList)ClientListView.SelectedItem).Id);
            ClientListView.SelectedItems.Clear();
        }

    }
}
