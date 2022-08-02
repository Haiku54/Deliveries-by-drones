using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
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
    /// Interaction logic for DisplayDronesList.xaml
    /// </summary>
    public partial class DisplayDronesList : Page
    {
        BlApi.IBL BL;

        public delegate void DronePage(int id);
        public event DronePage AddClik;
        public event DronePage DoubleClik;

        /// <summary>
        /// Initialize the drone list view page
        /// </summary>
        /// <param name="bL"></param>
        public DisplayDronesList() 
        {
            InitializeComponent();
            this.BL = BlApi.BlFactory.GetBL();
            DronesListView.DataContext = global::Model.ViewModel.drones;
           
            StatusSelector.ItemsSource = Enum.GetValues(typeof(BO.DroneStatus));
            WeightSelector.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
        }

        

        /// <summary>
        /// open drone page of drone selected from the Drone list 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DronesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DronesListView.SelectedItem != null && DoubleClik != null) 
                DoubleClik(((PO.DroneToList)DronesListView.SelectedItem).ID);
            DronesListView.SelectedItems.Clear();
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
        /// reseting the list from changes of filters 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            global::Model.ViewModel.drones.Clear();
            foreach (var item in BL.DisplayDroneList())
            {
                PO.DroneToList d = (PO.DroneToList)item.CopyPropertiesToNew(typeof(PO.DroneToList));
                d.DroneLocation = item.DroneLocation;
                global::Model.ViewModel.drones.Add(d);
            }
            Show_Drones(this, new RoutedEventArgs()); //after filter

        }


        /// <summary>
        /// function that uses the grouping function in Bl that group element of list by their status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show_Drones(object sender, RoutedEventArgs e)
        {
            if (DronesListView != null)
            {
                if (Show_Normally.IsChecked == true)
                {
                    var d = BL.DisplayDroneList();
                    var temp = new ObservableCollection<PO.DroneToList>(global::Model.ViewModel.drones);
                    global::Model.ViewModel.drones.Clear();
                    foreach (var item in d)
                    {
                        PO.DroneToList d1 = (PO.DroneToList)item.CopyPropertiesToNew(typeof(PO.DroneToList));
                        d1.DroneLocation = item.DroneLocation;
                        if (temp.Any(x => x.ID == item.ID)) global::Model.ViewModel.drones.Add(d1);
                    }
                }
                else if (Show_Status.IsChecked == true)
                {
                    var d = BL.DroneGroupbyStatus();
                    var temp = new ObservableCollection<PO.DroneToList>(global::Model.ViewModel.drones);
                    global::Model.ViewModel.drones.Clear();
                    foreach (var group in d)
                    {
                        foreach (BO.DroneToList item in group)
                        {
                            if (temp.Any(x => x.ID == item.ID))
                            {
                                PO.DroneToList d2 = (PO.DroneToList)item.CopyPropertiesToNew(typeof(PO.DroneToList));
                                d2.DroneLocation = item.DroneLocation;
                                global::Model.ViewModel.drones.Add(d2);
                            }
                        }
                    }
                }
              
            }
        }


        /// <summary>
        /// Displays the list of drones according to the filters of the 2 options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.DroneStatus? status;
            global::Model.ViewModel.drones.Clear();

            if (StatusSelector.SelectedItem == null) status = null;
            else status = (BO.DroneStatus)StatusSelector.SelectedItem;

            if (WeightSelector.SelectedItem == null)
            {
                switch (status)
                {
                    case BO.DroneStatus.Available:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Available)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case BO.DroneStatus.Maintenance:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Maintenance)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case BO.DroneStatus.Shipping:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Shipping)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case null:
                        foreach(var item in BL.DisplayDroneList()) global::Model.ViewModel.drones.Add((PO.DroneToList)item.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;
                }
            }
            else
            {
                switch (status)
                {
                    case BO.DroneStatus.Available:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Available && d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case BO.DroneStatus.Maintenance:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Maintenance && d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case BO.DroneStatus.Shipping:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.Status == BO.DroneStatus.Shipping && d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case null:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                }
            }
            global::Model.ViewModel.AddDronesLocations();
        }

        /// <summary>
        /// Displays the list of drones according to the filters of the 2 options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.WeightCategories? weight;
            global::Model.ViewModel.drones.Clear();

            if (WeightSelector.SelectedItem == null) weight = null;
            else weight = (BO.WeightCategories)WeightSelector.SelectedItem;

            if (StatusSelector.SelectedItem == null)
            {
                switch (weight)
                {
                    case BO.WeightCategories.Heavy:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Heavy)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case BO.WeightCategories.Light:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Light)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case BO.WeightCategories.Medium:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Medium)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;
                    case null:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;
                }
            }
            else
            {
                switch (weight)
                {
                    case BO.WeightCategories.Heavy:

                        foreach (var d in BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Heavy && d.Status == (BO.DroneStatus)StatusSelector.SelectedItem)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case BO.WeightCategories.Light:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Light && d.Status == (BO.DroneStatus)StatusSelector.SelectedItem)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case BO.WeightCategories.Medium:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.MaxWeight == BO.WeightCategories.Medium && d.Status == (BO.DroneStatus)StatusSelector.SelectedItem)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));
                        break;

                    case null:
                        foreach (var d in BL.DisplayDroneListFilter(d => d.Status == (BO.DroneStatus)StatusSelector.SelectedItem)) global::Model.ViewModel.drones.Add((PO.DroneToList)d.CopyPropertiesToNew(typeof(PO.DroneToList)));

                        break;
                }
            }
            global::Model.ViewModel.AddDronesLocations();
        }

       


    }
}

