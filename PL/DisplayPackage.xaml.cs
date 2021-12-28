﻿using System;
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
    /// Logique d'interaction pour DisplayPackage.xaml
    /// </summary>
    public partial class DisplayPackage : Page
    {

        private Model.PL pL;
        Package Package = new Package();

        public delegate void Navigation(int id);
        public event Navigation Back; //Event Back to Previous Page (For List Refresh)
        public event Navigation ClientPage; //New page opening event from the current page
        public event Navigation DronePage; //New page opening event from the current page

        public DisplayPackage()
        {
            InitializeComponent();
            this.pL = new Model.PL();
            MainGrid.DataContext = Package;
            Package_Priority.ItemsSource = Enum.GetValues(typeof(BO.Priorities));
            Package_Weight.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
            ClientsList.ItemsSource = pL.getClientList();
            ClientsList2.ItemsSource = pL.getClientList();

            Package.package = new BO.Package();

        }

        /// <summary>
        /// Constructor for display and add operations
        /// </summary>
        /// <param name="id">ID of package</param>
        public DisplayPackage(int id)
        {
            this.pL = new Model.PL();
            Package.package = pL.GetPackage(id);
            InitializeComponent();

            if (Package.package.Associated == null) Package.package.DroneOfPackage = new BO.DroneOfPackage();
            MainGrid.DataContext = Package;
            Package_Priority.ItemsSource = Enum.GetValues(typeof(BO.Priorities));
            Package_Weight.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
        }


        /// <summary>
        /// Package delete button - only if it has not yet been associated with a drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Package_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pL.DeletePackage(Package.package.ID);
                if (Back != null) Back(-1);
                MessageBox.Show($"The package has been deleted !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to delete package {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Add package button, display user message and navigate back to previous page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Package_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pL.AddPackage(Package.package);
                if (Back != null) Back(-1);
                MessageBox.Show($"The package was successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to add package {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Back to previous mode button - and refresh list if needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Back != null) Back(-1);
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Package collection button - by the drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pL.PickUpPackage(Package.package.DroneOfPackage.Id);
                MessageBox.Show($"The package has been collected !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Package.package = pL.GetPackage(Package.package.ID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to PickUp package {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        /// <summary>
        /// Package delivery button to the customer - by the drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeliverButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pL.DeliveredToClient(Package.package.DroneOfPackage.Id);
                MessageBox.Show($"The package was delivered to the Client !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Package.package = pL.GetPackage(Package.package.ID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to Deliver package {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }



        /// <summary>
        /// Cancel Add button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, RoutedEventArgs e)
        {
            if (Back != null) Back(-1);
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Button for displaying a sending customer's page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sender_Page_Click(object sender, RoutedEventArgs e)
        {
            if(ClientPage!=null)ClientPage(int.Parse(idSender.Text));
        }

        /// <summary>
        /// Customer Page Receive Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reciver_Page_Click(object sender, RoutedEventArgs e)
        {
            if (ClientPage != null) ClientPage(int.Parse(idReciver.Text));
        }

        /// <summary>
        /// Button display page of the drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Drone_Page_Click(object sender, RoutedEventArgs e)
        {
            if (DronePage != null && int.Parse(Drone_id.Text)!=0)
                DronePage(int.Parse(Drone_id.Text));
        }

    } 
}
