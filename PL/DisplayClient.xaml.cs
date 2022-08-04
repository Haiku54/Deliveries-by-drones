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
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsPresentation;
using Model;

namespace PL
{
    /// <summary>
    /// Logique d'interaction pour DisplayClient.xaml
    /// </summary>
    /// 
    public partial class DisplayClient : Page
    {
        BlApi.IBL bL;

        public delegate void Navigation(int id);
        public event Navigation PackagePage;

        public delegate void Navigation_(object sender, RoutedEventArgs e);
        public event Navigation_ MainWindow; //only in display client data (MainWindow of client mode)

        /// <summary>
        /// ctor for adding a client to the list according to user input
        /// </summary>
        public DisplayClient()
        {
            InitializeComponent();
            MainGrid.DataContext = Model.ViewModel.Client;
            bL = BlApi.BlFactory.GetBL();

            Mode.IsChecked = true; // for visibility
            Model.ViewModel.Client.client = new BO.Client(); //init objects
            Model.ViewModel.Client.client.ClientLocation = new BO.Location();
        }

        /// <summary>
        /// ctor for displaying selected client data 
        /// </summary>
        /// <param name="id"></param>
        public DisplayClient(int id)
        {
            bL = BlApi.BlFactory.GetBL();
            Model.ViewModel.Client.client = bL.DisplayClient(id);
            InitializeComponent();

            MainGrid.DataContext = Model.ViewModel.Client;

        }

        /// <summary>
        /// ctor for new client that sign up *for the bonus*
        /// </summary>
        /// <param name="s"></param>
        public DisplayClient(string s)
        {
            InitializeComponent();
            bL = BlApi.BlFactory.GetBL();
            Model.ViewModel.Client = new Client();
            DataContext = Model.ViewModel.Client;
            Sign_up.Visibility = Visibility.Visible;
            Cancel_Sign_up.Visibility = Visibility.Visible;
            Add_Client_Button.Visibility = Visibility.Hidden;
            cancel.Visibility = Visibility.Hidden;
            Mode.IsChecked = true;
            
            
            Model.ViewModel.Client.client = new BO.Client();
            Model.ViewModel.Client.client.ClientLocation = new BO.Location();

        }


        /// <summary>
        /// functionn that updates client name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateName_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                bL.UpdateClient(Model.ViewModel.Client.client.ID, Client_Name.Text, "");

                MessageBox.Show("Client Name have been Changed Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Model.ViewModel.Client.client = bL.DisplayClient(Model.ViewModel.Client.client.ID);

                Model.ViewModel.clients.First(c => c.Id == Model.ViewModel.Client.client.ID).Name = Model.ViewModel.Client.client.Name;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// function to update phone number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePhone_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                bL.UpdateClient(Model.ViewModel.Client.client.ID, "", Client_Phone.Text);
                MessageBox.Show("Client Phone have been Changed Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Model.ViewModel.Client.client = bL.DisplayClient(Model.ViewModel.Client.client.ID);

                Model.ViewModel.clients.First(c => c.Id == Model.ViewModel.Client.client.ID).Phone = Model.ViewModel.Client.client.Phone;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        ///  Back click button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }


        /// <summary>
        /// cancel adding click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null && this.NavigationService.CanGoBack) this.NavigationService.GoBack();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sign_up_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow != null) MainWindow(this, new RoutedEventArgs());
            if (this.NavigationService != null && this.NavigationService.CanGoBack) this.NavigationService.GoBack();
        }

        /// <summary>
        /// button to add a client with datat inputed in the client page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Client_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bL.AddClient(Model.ViewModel.Client.client);
                MessageBox.Show($"The Client was successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                if (MainWindow != null) MainWindow(this, new RoutedEventArgs());

                Model.ViewModel.clients.Add((PO.ClientToList)bL.GetClientToList(Model.ViewModel.Client.client.ID).CopyPropertiesToNew(typeof(PO.ClientToList)));
                if (this.NavigationService != null && this.NavigationService.CanGoBack) this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to add Client {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        /// <summary>
        /// open client sended packages from list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SenderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.PackageAtClient p = SenderPackageList.SelectedItem as BO.PackageAtClient;
            if (p != null && PackagePage != null && p.Id != 0)
                PackagePage(p.Id);
        }

        /// <summary>
        /// open client received packages from list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiverList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.PackageAtClient p = ReceiverPackageList.SelectedItem as BO.PackageAtClient;
            if (PackagePage != null && p.Id != 0)
                PackagePage(p.Id);

        }

        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {

            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            // choose your provider here
            mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            mapView.MinZoom = 2;
            mapView.MaxZoom = 17;

            // whole world zoom
            mapView.Zoom = 12;
            // lets the map use the mousewheel to zoom
            mapView.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            // lets the user drag the map
            mapView.CanDragMap = true;
            // lets the user drag the map with the left mouse button
            mapView.DragButton = MouseButton.Left;
            mapView.Position = new PointLatLng(31.761804840069285, 35.189091442003786);
        }

 


    }
}
