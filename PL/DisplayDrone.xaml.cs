using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET;
using Model;


namespace PL
{
    /// <summary>
    /// Interaction logic for DisplayDrone.xaml
    /// </summary>
    public partial class DisplayDrone : Page
    {
        private BlApi.IBL bl;
        Drone Drone = new Drone();
        BackgroundWorker backgroundWorker;
        

        public delegate void Navigation(int id);
        public event Navigation PackagePage;

        /// <summary>
        /// ctor for creating new drone from user input in that page
        /// </summary>
        public DisplayDrone()
        {
            InitializeComponent();
            MainGrid.DataContext = Drone;
            bl = BlApi.BlFactory.GetBL();

            Drone_MaxWeight.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
            Mode.IsChecked = true;

            Drone.drone = new BO.Drone();           //init object 
            Drone.drone.Status = BO.DroneStatus.Maintenance;
            Drone.drone.DronePackageProcess = new BO.PackageProcess();
            Drone.drone.DronePackageProcess.Sender = new BO.ClientPackage();
            Drone.drone.DronePackageProcess.Receiver = new BO.ClientPackage();
            Drone.drone.DronePackageProcess.CollectLocation = new BO.Location();
            Stations_List.ItemsSource = bl.DisplayStationListWitAvailableChargingSlots();
        }

        /// <summary>
        /// ctor to display the drone page of selected item
        /// </summary>
        /// <param name="id"></param>
        public DisplayDrone(int id)
        {
            bl = BlApi.BlFactory.GetBL();
            Drone.drone = bl.DisplayDrone(id);
            InitializeComponent();
            MainGrid.DataContext = Drone;
            bl = BlApi.BlFactory.GetBL();

            //if (Drone.drone.Status == BO.DroneStatus.Shipping)
            //    ShipVisibility.IsChecked = true;
            if (Drone.drone.DronePackageProcess == null) 
                Drone.drone.DronePackageProcess = new BO.PackageProcess();
            
            Drone_MaxWeight.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));


            backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += Simulator_DoWork;
            backgroundWorker.ProgressChanged += Model.ViewModel.UpdateDataInSimulator;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.RunWorkerCompleted += Simulator_RunWorkerCompleted;
        }

        /// <summary>
        /// Sending a drone for charging and displaying an appropriate message and refreshing the display as needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChargeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.ViewModel.ChargeDrone(Drone.drone.ID);
                Drone.drone = bl.DisplayDrone(Drone.drone.ID);
                MessageBox.Show("Drone sent to charge", "Success", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Renaming the drone. Checking the correctness of the name, and sending it to BL and displaying an appropriate message and refreshing the display as needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Change_Name_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush red = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE92617"));
            if (SolidColorBrush.Equals(((SolidColorBrush)DroneModel.BorderBrush).Color, red.Color))
            {
                MessageBox.Show("Please enter correct Name", "Error input", MessageBoxButton.OK, MessageBoxImage.Error);
                DroneModel.Text = Drone.drone.Model;

                Model.ViewModel.drones.First(d => d.ID == Drone.drone.ID).Model = Drone.drone.Model;
            }
            else
            {
                try
                {
                    Model.ViewModel.UpdateDroneName(Drone.drone.ID, DroneModel.Text);
                    Drone.drone = bl.DisplayDrone(Drone.drone.ID);
                    MessageBox.Show("Drone ViewModel have been changed successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Releasing a drone from charging, displaying an appropriate message and refreshing the display as needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReleaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.ViewModel.FinishCharging(Drone.drone.ID);
                Drone.drone = bl.DisplayDrone(Drone.drone.ID);
                MessageBox.Show($"Drone have been unplugged, Battery left: {Drone.drone.Battery}%", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Assigning a drone to a package, displaying an appropriate message and refreshing the display as needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Model.ViewModel.AssociatePackage(Drone.drone.ID);
                bl.packageToDrone(Drone.drone.ID);
                Drone.drone = bl.DisplayDrone(Drone.drone.ID);
                MessageBox.Show("Package have been Associated to drone successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
               

                var d = Model.ViewModel.drones.First(d => d.ID == Drone.drone.ID); d.Status = BO.DroneStatus.Shipping; d.PackageID = Drone.drone.DronePackageProcess.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// open package page of package delivery from the drone page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Package_Process_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int id =  Drone.drone.DronePackageProcess.Id;
            if (PackagePage != null && id != 0)
                PackagePage(id);
        }

        /// <summary>
        /// Closing a Drone window and activating the event 'CloseWindowEvent', for which a refresh function of the drone list is registered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Adding a drone.
        ///Check that the inputs are correct and add.
        ///And display an appropriate message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Drone_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.ViewModel.AddDrone(Drone.drone, ((BO.StationToList)Stations_List.SelectedItem).ID);

                MessageBox.Show($"The Drone was successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to add drone {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Select a station for adding a drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stations_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Stations_List.SelectedItem = Stations_List.SelectedItem.ToString().ElementAt(5);
            StationID.Text = ((BO.StationToList)Stations_List.SelectedItem).ID.ToString();
        }

        /// <summary>
        ///  Change the frame color if the StationID input is incorrect
        ///  And adding the station location if the ID is correct
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StationID_TextChanged(object sender, TextChangedEventArgs e)
        {
            var bc = new BrushConverter();
            if (StationID.Text != null && StationID.Text != string.Empty && (StationID.Text).All(char.IsDigit))
            {
                StationID.BorderBrush = (Brush)bc.ConvertFrom("#FF99B4D1");
            }
            else StationID.BorderBrush = (Brush)bc.ConvertFrom("#FFE92617");

            try
            {
                int id = int.Parse(StationID.Text.ToString());
                Location.Text = $"{bl.DisplayStation(id).StationLocation}";
                
            }
            catch (Exception)
            {
                Location.Text = null;
            }
        }

        /// <summary>
        /// going to previous page click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Cancel insert and close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Running the simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Simulator_Click(object sender, RoutedEventArgs e)
        {
            if (backgroundWorker.IsBusy != true)
            {
                backgroundWorker.RunWorkerAsync(); // Start the asynchronous operation.
                simulator.IsChecked = true;
            }
        }

        /// <summary>
        /// Simulator stop button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancellation_Click(object sender, RoutedEventArgs e)
        {
            if(backgroundWorker.IsBusy)
            {
                MessageBox.Show($"Cancelling Simulator Mode, Please Wait", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                backgroundWorker.CancelAsync();
            }
        }

        /// <summary>
        /// Check function to see if the simulator has stopped
        /// </summary>
        /// <returns></returns>
        bool stop()
        {
            return backgroundWorker.CancellationPending;
        }


        /// <summary>
        /// The update function of the display by the simulator
        /// </summary>
        /// <param name="update"></param>
        /// <param name="id"></param>
        void update(string update, int id)
        {
            Drone.drone = bl.DisplayDrone(Drone.drone.ID);


            var droneToList = Model.ViewModel.drones.First(d => d.ID == Drone.drone.ID);
            droneToList.Battery = Drone.drone.Battery;
            droneToList.DroneLocation = Drone.drone.DroneLocation;
            droneToList.Status = Drone.drone.Status;
            if (Drone.drone.DronePackageProcess != null) droneToList.PackageID = Drone.drone.DronePackageProcess.Id;
            else droneToList.PackageID = 0;


            switch (update)
            {
                case "No packages":
                    backgroundWorker.ReportProgress(1);
                    break;

                case "charging":
                    backgroundWorker.ReportProgress(2,id);

                    break;

                case "Finish charging":
                    backgroundWorker.ReportProgress(3,id);

                    break;

                case "Associate":
                    backgroundWorker.ReportProgress(4,id);

                    break;

                case "PickedUp":
                    backgroundWorker.ReportProgress(5,id);

                    break;

                case "Delivered":
                    backgroundWorker.ReportProgress(6,id);

                    break;

                default:
                    break;
            }
            
        }

        /// <summary>
        /// The operation function of the simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Simulator_DoWork(object sender, DoWorkEventArgs e)
        {
            bl.StartSimulator(Drone.drone.ID, update, stop);
        }

        /// <summary>
        /// Function registered for the simulation event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Simulator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            simulator.IsChecked = false;
        }



        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {

            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            mapView.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            mapView.Zoom = 12;
        }
    }
}

