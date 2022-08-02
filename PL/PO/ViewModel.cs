using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BlApi;
enum updates
{
   aboutDrone, NoPackages, Charging, FinishCharging, Associate, PickedUp, Delivered
}

namespace Model
{
    public class ViewModel  //class used to use observable collection type for the list of object
    {
        static BlApi.IBL bl;
        static private Random random = new Random();

        public static ObservableCollection<PO.StationToList> stations = new ObservableCollection<PO.StationToList>();
        public static ObservableCollection<PO.DroneToList> drones = new ObservableCollection<PO.DroneToList>();
        public static ObservableCollection<PO.ClientToList> clients = new ObservableCollection<PO.ClientToList>();
        public static ObservableCollection<PO.PackageToList> packages = new ObservableCollection<PO.PackageToList>();
      

        public static Station Station { get; set; }  
        public static Package Package { get; set; }
        public static Client Client { get; set; }


        public ViewModel()
        {
            bl = BlApi.BlFactory.GetBL();

            //packages = new ObservableCollection<PO.PackageToList>();

            Station = new Station();
            Package = new Package();
            Client = new Client();

            updateStationsList();
            updatePackagesList();
            updateDronesList();
            updateClientsList();

        }

        public static void AddDronesLocations()
        {
            BlApi.IBL bl = BlApi.BlFactory.GetBL();
            foreach (var item in bl.DisplayDroneList())
            {
                if (drones.Any(d=> d.ID == item.ID))
                {
                    var droneToList = drones.First(d => d.ID == item.ID);
                    droneToList.DroneLocation = item.DroneLocation;
                }
            }
        }

        private static void updateStationsList()
        {
            stations.Clear();
            foreach (var item in bl.DisplayStationList())  //getting the list to the observable
            {
                PO.StationToList s = (PO.StationToList)item.CopyPropertiesToNew(typeof(PO.StationToList));
                stations.Add(s);
            }
        }

        private static void updateDronesList()
        {
            drones.Clear();
            foreach (var item in bl.DisplayDroneList())
            {

                PO.DroneToList d = (PO.DroneToList)item.CopyPropertiesToNew(typeof(PO.DroneToList));
                d.DroneLocation = item.DroneLocation;
                drones.Add(d);
            }
        }

        private static void updateClientsList()
        {
            clients.Clear();
            foreach (var item in bl.DisplayClientList())
            {
                PO.ClientToList c = (PO.ClientToList)item.CopyPropertiesToNew(typeof(PO.ClientToList));
                clients.Add(c);
            }
        }


        private static void updatePackagesList()
        {
            packages.Clear();
            foreach (var item in bl.DisplayPackageList())
            {
                PO.PackageToList p = (PO.PackageToList)item.CopyPropertiesToNew(typeof(PO.PackageToList));
                packages.Add(p);
            }
        }





        internal static void updateData(object sender, ProgressChangedEventArgs e)
        {
            int id = 0;
            updates update = (updates)e.ProgressPercentage;
            if (e.UserState != null) id = (int)e.UserState;

            switch (update)
            {

                case updates.NoPackages:
                    addPackages();
                    break;

                case updates.Charging:
                    if (ViewModel.Station.station != null && ViewModel.Station.station.ID == id)
                        ViewModel.Station.station = bl.DisplayStation(id);
                    updateStationsList();
                    break;

                case updates.FinishCharging:
                    if (ViewModel.Station.station != null && ViewModel.Station.station.ID == id)
                        ViewModel.Station.station = bl.DisplayStation(id);
                    updateStationsList();
                    break;

                case updates.Associate:
                    if (ViewModel.Package.package != null && ViewModel.Package.package.ID == id)
                        ViewModel.Package.package = bl.DisplayPackage(id);
                    updatePackagesList();
                    updateClientsList();
                    break;

                case updates.PickedUp:
                    if (ViewModel.Package.package != null && ViewModel.Package.package.ID == id)
                       ViewModel.Package.package = bl.DisplayPackage(id);
                    updatePackagesList();
                    updateClientsList();
                    break;

                case updates.Delivered:
                    if (ViewModel.Package.package != null && ViewModel.Package.package.ID == id)
                        ViewModel.Package.package = bl.DisplayPackage(id);
                    BO.Package package = bl.DisplayPackage(id);

                    if (ViewModel.Client.client != null && ViewModel.Client.client.ID == package.SenderClient.ID)
                        ViewModel.Client.client = bl.DisplayClient(package.SenderClient.ID);
                    else if (ViewModel.Client.client != null && ViewModel.Client.client.ID == package.TargetClient.ID)
                        ViewModel.Client.client = bl.DisplayClient(package.TargetClient.ID);

                    updatePackagesList();
                    updateClientsList();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// A function that adds packages randomly. The function is called if the packages ran out during the simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void addPackages()
        {
            IEnumerable<BO.ClientToList> clients = bl.DisplayClientList();

            for (int i = 0; i < random.Next(3, 7); i++)
            {
                BO.Package package = new BO.Package()
                {
                    SenderClient = new BO.ClientPackage() { ID = clients.ElementAt(random.Next(0, clients.Count())).Id },
                    TargetClient = new BO.ClientPackage() { ID = clients.ElementAt(random.Next(0, clients.Count())).Id },
                    Priority = (BO.Priorities)random.Next(0, 3),
                    Weight = (BO.WeightCategories)random.Next(0, 3)
                };

                int id = bl.AddPackage(package);
                BO.PackageToList packageToList = bl.GetPackageToList(id);
                PO.PackageToList poPackageTo = (PO.PackageToList)packageToList.CopyPropertiesToNew(typeof(PO.PackageToList));
                Model.ViewModel.packages.Add(poPackageTo);
            }
        }


    }


 
}
