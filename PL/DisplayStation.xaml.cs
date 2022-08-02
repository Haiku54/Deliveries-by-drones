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
using Model;


namespace PL
{
    /// <summary>
    /// Logique d'interaction pour DisplayStation.xaml
    /// </summary>
    public partial class DisplayStation : Page
    {
        private BlApi.IBL bL;

        public delegate void Navigation(int id);
        public event Navigation DronePage;


        /// <summary>
        /// ctor that display the Station page to add 
        /// </summary>
        public DisplayStation()
        {
            InitializeComponent();
            bL = BlApi.BlFactory.GetBL();
            Model.ViewModel.Station = new Station();
            Model.ViewModel.Station.station = new BO.Station();
            Model.ViewModel.Station.station.StationLocation = new BO.Location();

            MainGrid.DataContext = Model.ViewModel.Station;

            Model.ViewModel.Station.station.StationLocation = new BO.Location();
            Mode.IsChecked = true;   //for visibility of some buttons

        }

        /// <summary>
        /// ctor that display specific station info according to id
        /// </summary>
        /// <param name="id"></param>
        public DisplayStation(int id)
        {
            bL = BlApi.BlFactory.GetBL();
            Model.ViewModel.Station.station = bL.DisplayStation(id);
            InitializeComponent();

            MainGrid.DataContext = Model.ViewModel.Station;
            ChargingDroneList.ItemsSource = Model.ViewModel.Station.station.ChargingDronesList;

        }

        /// <summary>
        /// button to update the station name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Change_Station_Name_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bL.UpdateStationName(Model.ViewModel.Station.station.ID, NameInput.Text);
                MessageBox.Show($"Name have been changed to {NameInput.Text} !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Model.ViewModel.Station.station = bL.DisplayStation(Model.ViewModel.Station.station.ID); 
                Model.ViewModel.stations.First(s => s.ID == Model.ViewModel.Station.station.ID).Name = NameInput.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// button to change the num of charge slot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Change_Slot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int result = Int32.Parse(Charge_slot_input.Text);
                bL.UpdateStationNumCharge(Model.ViewModel.Station.station.ID, result);
                MessageBox.Show($" Number of Charge slot have been updated !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Model.ViewModel.Station.station = bL.DisplayStation(Model.ViewModel.Station.station.ID);  //update the list
                Model.ViewModel.stations.First(s => s.ID == Model.ViewModel.Station.station.ID).AvailableChargingSlots = result - Model.ViewModel.stations.First(s => s.ID == Model.ViewModel.Station.station.ID).BusyChargingSlots;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Button to add a station according to user input in add station page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Station_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bL.AddStation(Model.ViewModel.Station.station);
                MessageBox.Show($"The station was successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.NavigationService.GoBack();

                Model.ViewModel.stations.Add(new PO.StationToList() { ID = Model.ViewModel.Station.station.ID, Name = Model.ViewModel.Station.station.Name, AvailableChargingSlots = Model.ViewModel.Station.station.AvailableChargeSlots, BusyChargingSlots = 0 });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to add station {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// displaying the drone info from the list of charging drone of the station
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChargingDroneList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.ChargingDrone ch = ChargingDroneList.SelectedItem as BO.ChargingDrone;
            if (DronePage != null && ch!=null && ch.ID != 0)
                DronePage(ch.ID);
        }

        /// <summary>
        /// button to go back 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Button to cancel addition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }


    }
}
