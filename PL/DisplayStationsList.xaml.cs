using Model;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for DisplayStationsList.xaml
    /// </summary>
    public partial class DisplayStationsList : Page
    {
        BlApi.IBL BL;
        
        public delegate void StationPage(int id);
        public event StationPage AddClik;
        public event StationPage DoubleClik;


        /// <summary>
        /// Ctoe with init elements
        /// </summary>
        public DisplayStationsList()
        {
            InitializeComponent();
            this.BL = BlApi.BlFactory.GetBL();
            StationsListView.DataContext = Model.ViewModel.stations;

        }



        /// <summary>
        /// open station page of selected station
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StationsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StationsListView.SelectedItem != null)
            {
                if (DoubleClik != null) DoubleClik(((PO.StationToList)StationsListView.SelectedItem).ID);
            }
            StationsListView.SelectedItems.Clear();
        }


        /// <summary>
        /// function that filter the display of the list with grouping by num slot for example
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show_Stations(object sender, RoutedEventArgs e)
        {
            if (StationsListView != null)
            {
                if (Show_Normally.IsChecked == true)
                {
                    var s = BL.DisplayStationList();
                    var temp = new ObservableCollection<PO.StationToList>(Model.ViewModel.stations);

                    Model.ViewModel.stations.Clear();
                    
                    foreach (var item in s)
                    {
                        if (temp.Any(s => s.ID == item.ID))  //init again
                            Model.ViewModel.stations.Add((PO.StationToList)item.CopyPropertiesToNew(typeof(PO.StationToList)));
                    }
                }
                else if (Show_Open.IsChecked == true)
                {
                    var s = BL.GroupStationByNumSlots();  
                    var temp = new ObservableCollection<PO.StationToList>(Model.ViewModel.stations);
                    Model.ViewModel.stations.Clear();

                    foreach (var group in s)
                    {
                        foreach (BO.StationToList item in group)
                        {
                            if (temp.Any(s => s.ID == item.ID))
                                Model.ViewModel.stations.Add((PO.StationToList)item.CopyPropertiesToNew(typeof(PO.StationToList)));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// adding new station to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_New_Station(object sender, RoutedEventArgs e)
        {
            if(AddClik!=null) AddClik(-1);
        }

        /// <summary>
        /// reset the display list from filters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            Model.ViewModel.stations.Clear();
            foreach (var item in BL.DisplayStationList())
            {
                Model.ViewModel.stations.Add((PO.StationToList)item.CopyPropertiesToNew(typeof(PO.StationToList)));
            }
            Show_Stations(this, new RoutedEventArgs()); 
        }
    }
}
