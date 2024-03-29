﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;
using DalApi;
using System.Runtime.CompilerServices;


namespace DalObject
{
    internal class DataSource
    {
        internal static List<Client> ClientList = new List<Client>();//Creating List of Clients
        internal static List<Drone> DroneList = new List<Drone>(); // Creating List of Drone
        internal static List<Station> StationList = new List<Station>(); // Creating List of Station
        internal static List<Package> PackageList = new List<Package>(); // Creating List of Package
        internal static List<DroneCharge> droneCharge = new List<DroneCharge>(); // Creating List of droneCharge

        internal static List<string> Names = new List<string>() { "Smith", "Johnson", "Williams", "Brown", "Lee", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", }; // Random list of names

        static Random rand = new Random();

        /// <summary>
        /// The function gets a range of coordinates and adds to them up to one point, so that the new random position will be in the range of the range that the function has received.
        ///The function returns the new position
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double GetRandCoordinate(double num) //return Coordinate
        {
            double res = num + rand.NextDouble() / 10;
            return Math.Round(res, 6);
        }


        internal class Config
        {
            public static int PackageId = 1000;

            internal static double PowerAvailableDrone = 1;
            internal static double PowerLightDrone = 2;
            internal static double PowerMediumDrone = 3;
            internal static double PowerHeavyDrone = 4;
            public static double ChargeRate = 5;

        }

        public static void Initialize()
        {
            InitializeClient();
            InitializeStation();
            InitializeDrone();
            InitializePackage();
        }

        /// <summary>
        /// Initialize 10 clients
        /// </summary>
        static void InitializeClient()
        {
            for (int i = 0; i < 10; i++)
            {
                ClientList.Add(new Client()
                {
                    ID = rand.Next(100000000, (1000000000 - 10)) + i,
                    Name = $"{Names[i]}",
                    Phone = $"0{rand.Next(52, 59)}{rand.Next(1000000, 10000000)}",
                    Latitude = GetRandCoordinate(31.73),
                    Longitude = GetRandCoordinate(35.16)
                });
            }
        }


        /// <summary>
        /// Initialize 3 stations
        /// </summary>
        static void InitializeStation()
        {

            StationList.Add(new Station()
            {
                ID = 10,
                Name = "Malcha Mall",
                ChargeSlots = rand.Next(4, 10),
                Latitude = 31.7515163,
                Longitude = 35.1872451
            });

            StationList.Add(new Station()
            {
                ID = 20,
                Name = "Central Station",
                ChargeSlots = rand.Next(3, 10),
                Latitude = 31.7888727,
                Longitude = 35.2031491
            });

            StationList.Add(new Station()
            {
                ID = 30,
                Name = "Mount Scopus",
                ChargeSlots = rand.Next(3, 10),
                Latitude = 31.7930604,
                Longitude = 35.2449342
            });
        }


        /// <summary>
        /// Initialize 10 drones
        /// </summary>
        static void InitializeDrone()
        {


            for (int i = 0; i < 4; i++)
            {
                DroneList.Add(new Drone()
                {
                    ID = rand.Next(1000000, 10000000) + i,
                    Model = "Dji_Mavic_2_Pro",
                    MaxWeight = WeightCategories.Heavy,

                });
            }

            for (int i = 0; i < 3; i++)
            {
                DroneList.Add(new Drone()
                {
                    ID = rand.Next(1000000, 10000000) + i * 4,
                    Model = "Dji_Mavic_2_Air",
                    MaxWeight = WeightCategories.Medium,

                });
            }

            for (int i = 0; i < 3; i++)
            {
                DroneList.Add(new Drone()
                {
                    ID = rand.Next(1000000, 10000000) + i * 12,
                    Model = "Dji_Mavic_2_Zoom",
                    MaxWeight = WeightCategories.Light,

                });
            }
        }


        /// <summary>
        /// Initialize 10 Packages
        /// </summary>
        public static void InitializePackage()
        {
            for (int i = 0; i < 3; i++)
            {
                PackageList.Add(new Package()
                {
                    ID = Config.PackageId++,
                    SenderId = ClientList[rand.Next(0, 10)].ID,
                    TargetId = ClientList[rand.Next(0, 10)].ID,
                    Weight = (WeightCategories)rand.Next(3),
                    Priority = (Priorities)rand.Next(3),
                    DroneId = 0,
                    Created = DateTime.Now,
                    Associated = null,
                    PickedUp = null,
                    Delivered = null,

                });
            }

            PackageList.Add(new Package()
            {
                ID = Config.PackageId++,
                SenderId = ClientList[rand.Next(0, 10)].ID,
                TargetId = ClientList[rand.Next(0, 10)].ID,
                Weight = WeightCategories.Heavy,
                Priority = (Priorities)rand.Next(3),
                DroneId = DroneList[0].ID,
                Created = DateTime.Now.AddMinutes(-30),
                Associated = DateTime.Now.AddMinutes(-20),
                PickedUp = null,
                Delivered = null,

            }); ;

            PackageList.Add(new Package()
            {
                ID = Config.PackageId++,
                SenderId = ClientList[rand.Next(0, 10)].ID,
                TargetId = ClientList[rand.Next(0, 10)].ID,
                Weight = WeightCategories.Heavy,
                Priority = (Priorities)rand.Next(3),
                DroneId = DroneList[1].ID,
                Created = DateTime.Now.AddMinutes(-30),
                Associated = DateTime.Now.AddMinutes(-20),
                PickedUp = null,
                Delivered = null,
            });

            PackageList.Add(new Package()
            {
                ID = Config.PackageId++,
                SenderId = ClientList[rand.Next(0, 10)].ID,
                TargetId = ClientList[rand.Next(0, 10)].ID,
                Weight = WeightCategories.Medium,
                Priority = (Priorities)rand.Next(3),
                DroneId = DroneList[4].ID,
                Created = DateTime.Now.AddMinutes(-50),
                Associated = DateTime.Now.AddMinutes(-40),
                PickedUp = DateTime.Now.AddMinutes(-10),
                Delivered = null,
            });

            PackageList.Add(new Package()
            {
                ID = Config.PackageId++,
                SenderId = ClientList[rand.Next(0, 10)].ID,
                TargetId = ClientList[rand.Next(0, 10)].ID,
                Weight = WeightCategories.Medium,
                Priority = (Priorities)rand.Next(3),
                DroneId = DroneList[5].ID,
                Created = DateTime.Now.AddMinutes(-45),
                Associated = DateTime.Now.AddMinutes(-43),
                PickedUp = DateTime.Now.AddMinutes(-8),
                Delivered = null,
            });

            PackageList.Add(new Package()
            {
                ID = Config.PackageId++,
                SenderId = ClientList[rand.Next(0, 10)].ID,
                TargetId = ClientList[rand.Next(0, 10)].ID,
                Weight = WeightCategories.Light,
                Priority = (Priorities)rand.Next(3),
                DroneId = DroneList[7].ID,
                Created = DateTime.Now.AddMinutes(-60),
                Associated = DateTime.Now.AddMinutes(-50),
                PickedUp = DateTime.Now.AddMinutes(-40),
                Delivered = DateTime.Now.AddMinutes(-30),
            });

            PackageList.Add(new Package()
            {
                ID = Config.PackageId++,
                SenderId = ClientList[rand.Next(0, 10)].ID,
                TargetId = ClientList[rand.Next(0, 10)].ID,
                Weight = WeightCategories.Light,
                Priority = (Priorities)rand.Next(3),
                DroneId = DroneList[8].ID,
                Created = DateTime.Now.AddMinutes(-30),
                Associated = DateTime.Now.AddMinutes(-20),
                PickedUp = DateTime.Now.AddMinutes(-15),
                Delivered = DateTime.Now.AddMinutes(-5),
            });

            PackageList.Add(new Package()
            {
                ID = Config.PackageId++,
                SenderId = ClientList[rand.Next(0, 10)].ID,
                TargetId = ClientList[rand.Next(0, 10)].ID,
                Weight = WeightCategories.Light,
                Priority = (Priorities)rand.Next(3),
                DroneId = DroneList[2].ID,
                Created = DateTime.Now.AddMinutes(-120),
                Associated = DateTime.Now.AddMinutes(-100),
                PickedUp = DateTime.Now.AddMinutes(-80),
                Delivered = DateTime.Now.AddMinutes(-60),
            });

        }

    }
    internal class DalObject : DalApi.IDAL
    {
        static readonly IDAL instance = new DalObject();
        internal static IDAL Instance { get { return instance; } }
        static DalObject() { }
        DalObject() { DataSource.Initialize(); }




        /// <summary>
        /// The function receives an object Station and adds it to the list
        /// </summary>
        /// <param name="station"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(Station station)
        {
            if (DataSource.StationList.FindIndex(x => x.ID == station.ID) != -1)
                throw new DO.Exceptions.IDException("A Station ID already exists", station.ID);
            DataSource.StationList.Add(station);
        }


        /// <summary>
        /// The function gets a drone object and adds it to the list
        /// </summary>
        /// <param name="drone"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone drone)
        {
            if (DataSource.DroneList.FindIndex(x => x.ID == drone.ID) != -1)
                throw new DO.Exceptions.IDException("A Drone with ID already exists", drone.ID);
            DataSource.DroneList.Add(drone);
        }


        /// <summary>
        /// The function accepts a client-type object and adds it to the list of clients
        /// </summary>
        /// <param name="client"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddClient(Client client)
        {
            if (DataSource.ClientList.FindIndex(x => x.ID == client.ID) != -1)
                throw new DO.Exceptions.IDException("A Client with ID already exists", client.ID);
            DataSource.ClientList.Add(client);
        }


        /// <summary>
        /// The function receives a package type object and adds it to the list of packages
        /// </summary>
        /// <param name="package"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AddPackage(Package package)
        {
            package.ID = DataSource.Config.PackageId++;
            DataSource.PackageList.Add(package);
            return DataSource.Config.PackageId - 1;
        }


        /// <summary>
        /// The function accepts a client's id and returns the object that is its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Client ClientById(int id)
        {
            foreach (Client item in DataSource.ClientList)
            {
                if (item.ID == id) return item;
            }
            throw new DO.Exceptions.IDException("Client ID not found", id);
        }


        /// <summary>
        /// The function receives a packet ID and returns the object whose ID it is
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Package PackageById(int id)
        {
            foreach (var item in DataSource.PackageList)
            {
                if (item.ID == id) return item;
            }
            throw new DO.Exceptions.IDException("Package ID not found", id);
        }


        /// <summary>
        /// The function receives a drone ID and returns the object that is its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone DroneById(int id)
        {
            foreach (var item in DataSource.DroneList)
            {
                if (item.ID == id) return item;
            }
            throw new DO.Exceptions.IDException("Drone id not found", id);
        }


        /// <summary>
        /// The function receives a station ID and returns the object whose ID it is
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station StationById(int id)
        {
            foreach (var item in DataSource.StationList)
            {
                if (item.ID == id) return item;
            }
            throw new DO.Exceptions.IDException("Station ID not found", id);
        }


        /// <summary>
        /// The function gets a DroneCharge ID and returns the object that is its DroneID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DroneCharge DroneChargeByIdDrone(int id)
        {
            foreach (var item in DataSource.droneCharge)
            {
                if (item.DroneId == id) return item;
            }
            throw new DO.Exceptions.IDException("DroneCharge ID not found", id);
        }


        /// <summary>
        /// The function receives a Drone object and a Package object and assigns the packet to the drone
        /// </summary>
        /// <param name="package"></param>
        /// <param name="DroneID"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void packageToDrone(Package package, int DroneID)
        {
            Package packageTemp = package;
            packageTemp.DroneId = DroneID; //  Updates in temp of  package
            packageTemp.Associated = DateTime.Now;//  package
            DataSource.PackageList.Add(packageTemp); // Add temp to list and delete old
            DataSource.PackageList.Remove(package);
        }

        /// <summary>
        /// The function receives a Package object and updates the pick-up time by the drone
        /// </summary>
        /// <param name="package"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickedUpByDrone(Package package)// Package collection by drone
        {
            Package packageTemp = package;
            packageTemp.PickedUp = DateTime.Now; // Updates in temp of drone
            DataSource.PackageList.Add(packageTemp); // Add temp to list and delete old
            DataSource.PackageList.Remove(package);
        }


        /// <summary>
        /// The function receives a package object and updates that the package has been delivered to the customer.
        ///Updates the delivery time and the drone is available
        /// </summary>
        /// <param name="package"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliveredToClient(Package package) // The package was delivered to the client
        {

            Drone drone = DroneById(package.DroneId);
            Drone droneTemp = drone;
            
            DataSource.DroneList.Add(droneTemp); // Add temp to list and delete old
            DataSource.DroneList.Remove(drone);

            Package packageTemp = package;
            packageTemp.Delivered = DateTime.Now; // Updates in temp of  package
            DataSource.PackageList.Add(packageTemp); //Add temp to list and delete old
            DataSource.PackageList.Remove(package);

        }


        /// <summary>
        /// The function receives a station ID, updates on one occupied charging station. And returns the object
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station ChargingStation(int stationID) // The function handles the station
        {
            Station station = StationById(stationID);
            Station stationTemp = station;
            stationTemp.ChargeSlots--;
            DataSource.StationList.Add(stationTemp);
            DataSource.StationList.Remove(station);
            return stationTemp;
        }


        /// <summary>
        /// The function receives a station ID, sends it to the ChargingStation function and receives the requested station.
        ///The function receives a drone, updates it that it is charging and creates a new DroneCharg instance with the details of the charging station and the loaded drone
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="stationID"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DroneCharge(Drone drone, int stationID)
        {
            Station station = ChargingStation(stationID); // The station that the user choose
            Drone droneTemp = drone;

            DroneCharge droneCharg = new DroneCharge() // Initialization of a new instance for DroneCharge
            {
                DroneId = droneTemp.ID,
                StationId = station.ID,
                ChargingStartTime = DateTime.Now
            };
            DataSource.droneCharge.Add(droneCharg); // Add the instance to the list

            DataSource.DroneList.Add(droneTemp); // Add temp to list and delete old
            DataSource.DroneList.Remove(drone);
        }


        /// <summary>
        /// The function receives a DroneCharge instance and updates the station and drone upon completion of charging
        /// </summary>
        /// <param name="droneCharge"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void FinishCharging(DroneCharge droneCharge) // Finish drone Chargeing, update drone status and update station
        {
            Drone drone = DroneById(droneCharge.DroneId);
            Drone droneTemp = drone; //   Updates in temp of drone
            //droneTemp.Status = DroneStatus.Available; //    Updates in temp of drone
            //droneTemp.Battery = 100; //    Updates in temp of drone
            DataSource.DroneList.Add(droneTemp); // Add temp to list and delete old
            DataSource.DroneList.Remove(drone);


            Station station = StationById(droneCharge.StationId); // The station that Charged the drone
            Station stationTemp = station;
            stationTemp.ChargeSlots++; // Updates in temp of station 
            DataSource.StationList.Add(stationTemp); // Add temp to list and delete old
            DataSource.StationList.Remove(station);

            DataSource.droneCharge.Remove(droneCharge); // Deleting the instance from the list
        }

        /// <summary>
        /// returns dronecharge list type
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> droneChargesList()
        {
            List<DroneCharge> droneChargeTemp = new List<DroneCharge>(DataSource.droneCharge);
            return droneChargeTemp;
        }


        /// <summary>
        /// The function returns the list of stations
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> StationsList()
        {
            List<Station> temp = new List<Station>(DataSource.StationList);
            return temp;
        }


        /// <summary>
        /// The function returns the list of drones
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> DroneList()
        {
            List<Drone> temp = new List<Drone>(DataSource.DroneList);
            return temp;
        }


        /// <summary>
        /// The function returns the list of clients
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Client> ClientsList()
        {
            List<Client> temp = new List<Client>(DataSource.ClientList);
            return temp;
        }


        /// <summary>
        /// The function returns the list of packages
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Package> PackageList()
        {
           IEnumerable<Package> temp = new List<Package>(DataSource.PackageList).OrderBy(p=>p.ID);
            return temp;
        }


        /// <summary>
        /// The function creates and returns a list of packages that have not yet been assigned to the glider
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Package> PackagesFilter(Predicate<Package> match)
        {
            List<Package> packagesFilter = new List<Package>(DataSource.PackageList).FindAll(match);
            return packagesFilter;
        }


        /// <summary>
        /// The function creates and returns a list of stations with available charging slots
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> StationsFilter(Predicate<Station> match)
        {
            List<Station> stationsFilter = new List<Station>(DataSource.StationList).FindAll(match);
            return stationsFilter;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> DronedFilter(Predicate<Drone> match)
        {
            List<Drone> dronedFilter = new List<Drone>(DataSource.DroneList).FindAll(match);
            return dronedFilter;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Client> ClientsFilter(Predicate<Client> match)
        {
            List<Client> clientsFilter = new List<Client>(DataSource.ClientList).FindAll(match);
            return clientsFilter;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> DroneChargeFilter(Predicate<DroneCharge> match)
        {
            List<DroneCharge> droneChargeFilter = new List<DroneCharge>(DataSource.droneCharge).FindAll(match);
            return droneChargeFilter;
        }

        /// <summary>
        /// remove item drone from the list of drones
        /// </summary>
        /// <param name="drone"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDrone(int id)
        {
            if (!DataSource.DroneList.Any(x => x.ID == id)) { throw new DO.Exceptions.IDException("id to remove not found", id); }
            DataSource.DroneList.Remove(DroneById(id));
        }

        /// <summary>
        /// remove item station from the list of station
        /// </summary>
        /// <param name="station"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteStation(int id )
        {
            if (!DataSource.StationList.Any(x => x.ID == id)) { throw new DO.Exceptions.IDException("id to remove not found", id); }
            DataSource.StationList.Remove(StationById(id));
        }

        /// <summary>
        /// remove item package from the list of package
        /// </summary>
        /// <param name="package"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeletePackage(int id)
        {
            if (!DataSource.PackageList.Any(x => x.ID == id)) { throw new DO.Exceptions.IDException("id to remove not found", id); }
            DataSource.PackageList.Remove(PackageById(id));

        }

        /// <summary>
        /// remove item client from the list of client
        /// </summary>
        /// <param name="client"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteClient(int id)
        {
            if (!DataSource.ClientList.Any(x => x.ID == id)) { throw new DO.Exceptions.IDException("id to remove not found", id); }
            DataSource.ClientList.Remove(ClientById(id));
        }

        /// <summary>
        /// remove item droneCharge from the list of droneCharge
        /// </summary>
        /// <param name="droneCharge"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDroneCharge(DroneCharge droneCharge)
        {
            if (!DataSource.droneCharge.Any(x => x.DroneId == droneCharge.DroneId)) { throw new DO.Exceptions.IDException("id to remove not found", droneCharge.DroneId); }
            DataSource.droneCharge.Remove(droneCharge);
        }

        /// <summary>
        /// double array to store attributes of powerneed of different type of drone
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] PowerConsumptionByDrone()
        {
            double[] arr = new double[5];
            arr[0] = DataSource.Config.PowerAvailableDrone;
            arr[1] = DataSource.Config.PowerLightDrone;
            arr[2] = DataSource.Config.PowerMediumDrone;
            arr[3] = DataSource.Config.PowerHeavyDrone;
            arr[4] = DataSource.Config.ChargeRate;
            return arr;
        }
    }
}