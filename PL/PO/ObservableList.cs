﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BlApi;


namespace Model
{
    public class ObservableList
    {
        private BlApi.IBL bl;


        public static ObservableCollection<PO.StationToList> stations = new ObservableCollection<PO.StationToList>();
        public static ObservableCollection<PO.DroneToList> drones = new ObservableCollection<PO.DroneToList>();
        public static ObservableCollection<PO.PackageToList> packages = new ObservableCollection<PO.PackageToList>();
        public static ObservableCollection<PO.ClientToList> clients = new ObservableCollection<PO.ClientToList>();




        public ObservableList()
        {
            this.bl = BlApi.BlFactory.GetBL();


            foreach (var item in bl.DisplayStationList())
            {
                PO.StationToList s = (PO.StationToList)item.CopyPropertiesToNew(typeof(PO.StationToList));
                stations.Add(s);
            }

            foreach (var item in bl.DisplayDroneList())
            {
                PO.DroneToList d = (PO.DroneToList)item.CopyPropertiesToNew(typeof(PO.DroneToList));
                drones.Add(d);
            }
            foreach (var item in bl.DisplayClientList())
            {
                PO.ClientToList c = (PO.ClientToList)item.CopyPropertiesToNew(typeof(PO.ClientToList));
                clients.Add(c);
            }

            foreach (var item in bl.DisplayPackageList())
            {
                PO.PackageToList p = (PO.PackageToList)item.CopyPropertiesToNew(typeof(PO.PackageToList));
                packages.Add(p);
            }



        }

        public void ClearDroneList()
        {
            drones.Clear();
        }




    }


 
}
