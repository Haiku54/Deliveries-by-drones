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

namespace PL
{
    /// <summary>
    /// Interaction logic for DisplayDronesList.xaml
    /// </summary>
    public partial class DisplayDronesList : Page
    {
        BlApi.IBL BL;

        public delegate void PackagePage(int id);
        public event PackagePage AddClik;
        public event PackagePage DoubleClik;




        /// <summary>
        /// Initialize the drone list view window
        /// </summary>
        /// <param name="bL"></param>
        public DisplayDronesList() 
        {
            InitializeComponent();
            this.BL = BlApi.BlFactory.GetBL();
       
            DronesListView.ItemsSource = BL.DisplayDroneList();
            StatusSelector.ItemsSource = Enum.GetValues(typeof(BO.DroneStatus));
            WeightSelector.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
        }

        /// <summary>
        /// Displays the list of drones according to the filters of the 2 options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.DroneStatus? status;

            if (StatusSelector.SelectedItem == null) status = null;
            else status = (BO.DroneStatus)StatusSelector.SelectedItem;

            if (WeightSelector.SelectedItem == null)
            {
                switch (status)
                {
                    case BO.DroneStatus.Available:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Available);
                        break;

                    case BO.DroneStatus.Maintenance:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Maintenance);
                        break;

                    case BO.DroneStatus.Shipping:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Shipping);
                        break;

                    case null:
                        DronesListView.ItemsSource = BL.DisplayDroneList();
                        break;
                }
            }
            else
            {
                switch (status)
                {
                    case BO.DroneStatus.Available:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Available && d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem);
                        break;

                    case BO.DroneStatus.Maintenance:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Maintenance && d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem);
                        break;

                    case BO.DroneStatus.Shipping:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Shipping && d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem);
                        break;

                    case null:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem);
                        break;

                }
            }

        }

        /// <summary>
        /// Displays the list of drones according to the filters of the 2 options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.WeightCategories? weight;

            if (WeightSelector.SelectedItem == null) weight = null;
            else weight = (BO.WeightCategories)WeightSelector.SelectedItem;

            if (StatusSelector.SelectedItem == null)
            {
                switch (weight)
                {
                    case BO.WeightCategories.Heavy:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Heavy);
                        break;

                    case BO.WeightCategories.Light:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Light);
                        break;

                    case BO.WeightCategories.Medium:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Medium);
                        break;

                    case null:
                        DronesListView.ItemsSource = BL.DisplayDroneList();
                        break;
                }
            }
            else
            {
                switch (weight)
                {
                    case BO.WeightCategories.Heavy:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Heavy && d.Status == (BO.DroneStatus)StatusSelector.SelectedItem);
                        break;

                    case BO.WeightCategories.Light:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Light && d.Status == (BO.DroneStatus)StatusSelector.SelectedItem);
                        break;

                    case BO.WeightCategories.Medium:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Medium && d.Status == (BO.DroneStatus)StatusSelector.SelectedItem);
                        break;

                    case null:
                        DronesListView.ItemsSource = BL.DisplayDroneListFilter(d => d.Status == (BO.DroneStatus)StatusSelector.SelectedItem);
                        break;

                }
            }
        }

        /// <summary>
        /// By clicking on the button Add Drone - Displays the drone window, and registers for the event of closing the window the function that refreshes the drone list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_New_Drone(object sender, RoutedEventArgs e)
        {
            if (AddClik != null) AddClik(-1);
           
        }

        /// <summary>
        /// A function that refreshes the list of drones
        /// </summary>
        /// <param name="ob"></param>
        private void RefreshListView(object sender, EventArgs e) // עדכון לרשימות
        {
            DronesListView.Items.Refresh();
            if (WeightSelector.SelectedItem == null && StatusSelector.SelectedItem == null) DronesListView.ItemsSource = BL.DisplayDroneList();
            if (WeightSelector.SelectedItem != null) WeightSelector_SelectionChanged(this, null);
            if (StatusSelector.SelectedItem != null) StatusSelector_SelectionChanged(this, null);
        }

        /// <summary>
        /// Double-clicking the drone in the list - Displays the drone window, and registers for the event of closing the window the function that refreshes the drone list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DronesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((BO.DroneToList)DronesListView.SelectedItem != null)
            {
                if (DoubleClik != null) DoubleClik(((BO.DroneToList)DronesListView.SelectedItem).ID);  
            }
            DronesListView.SelectedItems.Clear();
        }

        /// <summary>
        /// Closes the drone list window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Reset the drone list filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            StatusSelector.SelectedItem = null;
            WeightSelector.SelectedItem = null;
        }
        public void RefreshList(int t)
        {
            RefreshListView(this, EventArgs.Empty);
        }
    }
}

