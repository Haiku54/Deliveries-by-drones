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

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BlApi.IBL BL;
        object content;

        public MainWindow()
        {
            InitializeComponent();
            BL = BlApi.BlFactory.GetBL();
            content = this.Content;
            new Model.ViewModel();
        }


        #region List view buttons

        /// <summary>
        /// Opening Client List page with double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientsListButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayClientsList page = new DisplayClientsList();
                page.AddClik += AddClientPage; // Registration for the event Opening a client adding page
                page.DoubleClik += ClientDisplayPage; // Registration for the event Opening a client display page
                this.Frame.Content = page;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Opening Package List page with double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PackageListButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindowDisplay.Visibility = Visibility.Hidden;
                DisplayPackagesList page = new DisplayPackagesList();
                page.AddClik += AddPackagePage; // Registration for the event Opening a package adding page
                page.DoubleClik += PackageDisplayPage; // Registration for the event Opening a package display page
                this.Frame.Content = page;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Opening Station List page with double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stations_Click(object sender, RoutedEventArgs e)
        {

            DisplayStationsList page = new DisplayStationsList();
            page.AddClik += AddStationPage; // Registration for the event Opening a station adding page
            page.DoubleClik += StationDisplayPage; // Registration for the event Opening a station display page
            this.Frame.Content = page;
        }

        /// <summary>
        /// Opening Drone List page with double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Drones_List_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindowDisplay.Visibility = Visibility.Hidden;
                DisplayDronesList page = new DisplayDronesList();
                page.AddClik += AddDronePage; // Registration for the event Opening a client drone page
                page.DoubleClik += DroneDisplayPage; // Registration for the event Opening a drone display page
                this.Frame.Content = page;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion 


        #region Functions for opening adds pages
        /// <summary>
        /// Opening page for adding a Package 
        /// </summary>
        /// <param name="num"></param>
        private void AddPackagePage(int num) 
        {
            this.Frame.Content = new DisplayPackage();
        }

        /// <summary>
        /// Opening page for adding a Drone 
        /// </summary>
        /// <param name="num"></param>
        private void AddDronePage(int num)
        {
            this.Frame.Content = new DisplayDrone();
        }

        /// <summary>
        /// Opening page for adding a Client
        /// </summary>
        /// <param name="num"></param>
        private void AddClientPage(int num) 
        {
            this.Frame.Content = new DisplayClient();
        }

        /// <summary>
        /// Opening page for adding a Station
        /// </summary>
        /// <param name="num"></param>
        private void AddStationPage(int num) 
        {
            this.Frame.Content = new DisplayStation();
        }
        #endregion


        #region Functions for opening display pages
        /// <summary>
        /// Opening page for Updating a Package 
        /// </summary>
        /// <param name="id"></param>
        private void PackageDisplayPage(int id) 
        {

            var page = new DisplayPackage(id);
            page.ClientPage += ClientDisplayPageFromPackage;
            page.DronePage += DroneDisplayPageFromPackage;
            this.Frame.Content = page;
        }

        /// <summary>
        /// Opening page for Display a Drone 
        /// </summary>
        /// <param name="id"></param>
        private void DroneDisplayPage(int id) 
        {
            if (Model.ViewModel.drones.First(d => d.ID == id).DronePage == null) // If this drone has been opened before, then the old page will open, this is so that it will be possible to stop the background Worker
            {
                var droneDisplayPage = new DisplayDrone(id);
                droneDisplayPage.PackagePage += PackageDisplayFromDrone; // Registration for the event Opening a package page from a drone page
                this.Frame.Content = droneDisplayPage;
                Model.ViewModel.drones.First(d => d.ID == id).DronePage = droneDisplayPage;
            }
            else this.Frame.Content = Model.ViewModel.drones.First(d => d.ID == id).DronePage;
        }

        /// <summary>
        /// Opening page for Display a Client 
        /// </summary>
        /// <param name="id"></param>
        private void ClientDisplayPage(int id) 
        {
            var page = new DisplayClient(id);
            page.PackagePage += PackageDisplayFromClient; // Registration for the event Opening a package page from a client page
            this.Frame.Content = page;
        }

        /// <summary>
        /// Opening page for Display a Station 
        /// </summary>
        /// <param name="id"></param>
        private void StationDisplayPage(int id) 
        {
            var page = new DisplayStation(id);
            page.DronePage += DroneDiplayFromStation; // Registration for the event of opening a drone page from a package page
            this.Frame.Content = page;
        }
        #endregion


        #region Functions for opening pages from other pages
        /// <summary>
        /// Opening a Client page from the package page
        /// </summary>
        /// <param name="id"></param>
        private void ClientDisplayPageFromPackage(int id)
        {
            var page = new DisplayClient(id);
            page.PackagePage += PackageDisplayFromClient; // Registration for the event Opening a package page from a client page
            this.Frame.Content = page;
        }

        /// <summary>
        ///  Opening a Drone page from the package page
        /// </summary>
        /// <param name="id"></param>
        private void DroneDisplayPageFromPackage(int id)
        {
            if (Model.ViewModel.drones.First(d => d.ID == id).DronePage == null) // If this drone has been opened before, then the old page will open, this is so that it will be possible to stop the background Worker
            {
                var page = new DisplayDrone(id);
                page.PackagePage += PackageDisplayFromDrone; // Registration for the event Opening a package page from a drone page
                this.Frame.Content = page;
                Model.ViewModel.drones.First(d => d.ID == id).DronePage = page;
            }
            else this.Frame.Content = Model.ViewModel.drones.First(d => d.ID == id).DronePage;
        }

        /// <summary>
        ///  Opening a Drone page from the Station page
        /// </summary>
        /// <param name="id"></param>
        private void DroneDiplayFromStation(int id)
        {
            if (Model.ViewModel.drones.First(d => d.ID == id).DronePage == null) // If this drone has been opened before, then the old page will open, this is so that it will be possible to stop the background Worker
            {
                var droneDisplayPage = new DisplayDrone(id);
                this.Frame.Content = droneDisplayPage;
                Model.ViewModel.drones.First(d => d.ID == id).DronePage = droneDisplayPage;
            }
            else this.Frame.Content = Model.ViewModel.drones.First(d => d.ID == id).DronePage;
        }

        /// <summary>
        /// Opening a Package page from the Client page
        /// </summary>
        /// <param name="id"></param>
        private void PackageDisplayFromClient(int id)
        {
            var page = new DisplayPackage(id);
            page.DronePage += DroneDisplayPageFromPackage; // Registration for the event Opening a package page from a drone page
            page.ClientPage += ClientDisplayPageFromPackage; // Registration for an event Opening a client page from a package page
            this.Frame.Content = page;
        }

        /// <summary>
        /// Opening a Package page from the Drone page
        /// </summary>
        /// <param name="id"></param>
        private void PackageDisplayFromDrone(int id)
        {
            var page = new DisplayPackage(id);
            page.ClientPage += ClientDisplayPageFromPackage; // Registration for an event Opening a client page from a package page
            page.DronePage += DroneDisplayPageFromPackage;  // Registration for the event Opening a package page from a drone page
            this.Frame.Content = page;
        }
        #endregion


        private void Manager_login_Click(object sender, RoutedEventArgs e)
        {
            Buttons_For_Lists.Visibility = Visibility.Visible;
            MainWindowDisplay.Visibility = Visibility.Hidden;
            Frame.Visibility = Visibility.Visible;
        }

    }
}