﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using BlApi;
using System.Runtime.CompilerServices;

namespace BL
{
    internal partial class BL : BlApi.IBL
    {
        static Random rand = new Random();
        List<DroneToList> DroneList = new List<DroneToList>();
        internal DalApi.IDAL dal;
        double PowerVacantDrone;
        double PowerLightDrone;
        double PowerMediumDrone;
        double PowerHeavyDrone;
        double ChargeRate;


        static readonly IBL instance = new BL();
        internal static IBL Instance { get => instance; }

        static BL() { }
        
        BL()
        {
            dal = DalApi.DalFactory.GetDal("XML");

            lock (dal)
            {

                //Battery consumption fields by weight, and charge rate
                var arr = (dal.PowerConsumptionByDrone());
                PowerVacantDrone = arr[0];
                PowerLightDrone = arr[1];
                PowerMediumDrone = arr[2];
                PowerHeavyDrone = arr[3];
                ChargeRate = arr[4];

                //List or first time in XML - List
                //In XML mode not for the first time - XML
                initializeDrone("XML");// XML or List
            }

        }


        /// <summary>
        /// A function that initializes the list of drones in BL by calling the list from DAL
        /// </summary>
        private void initializeDrone(string mode)
        {

            foreach (var drone in dal.DroneList()) //Check on each drone in DAL what data it will receive
            {
                DroneToList droneToList = new DroneToList();
                droneToList.DroneLocation = new Location();
                droneToList.ID = drone.ID;
                droneToList.Model = drone.Model;
                droneToList.MaxWeight = (WeightCategories)drone.MaxWeight;

                bool flag = false;
                foreach (var package in dal.PackageList()) // Each drone must check if there is a package to which it is associated
                {

                    if ((package.DroneId == drone.ID) && (package.Delivered == null)) // If the drone is associated with a package and the package has not yet been delivered
                    {
                        flag = true;
                        droneToList.Status = DroneStatus.Shipping;
                        droneToList.PackageID = package.ID;

                        // There are 2 options: either the package has not been collected yet or it has already been collected
                        if (package.PickedUp == null) // Location of the drone if the package has not yet been collected
                        {
                            DO.Station location = NearestStationToClient(package.SenderId); // The location of the drone will be at the station closest to the sender
                            droneToList.DroneLocation.Latitude = location.Latitude;
                            droneToList.DroneLocation.Longitude = location.Longitude;
                        }
                        else // If the package is collected, the location of the drone will be at the location of the sender
                        {
                            droneToList.DroneLocation.Latitude = dal.ClientById(package.SenderId).Latitude;
                            droneToList.DroneLocation.Longitude = dal.ClientById(package.SenderId).Longitude;
                        }

                        // If the drone is associated but not yet delivered then the battery will at least be able to reach the package to deliver it and reach the station
                        Location targetLocation = new Location(); // Target location
                        targetLocation.Latitude = dal.ClientById(dal.PackageById(droneToList.PackageID).TargetId).Latitude;
                        targetLocation.Longitude = dal.ClientById(dal.PackageById(droneToList.PackageID).TargetId).Longitude;

                        Location senderLocation = new Location(); // Sender location
                        senderLocation.Latitude = dal.ClientById(dal.PackageById(droneToList.PackageID).SenderId).Latitude;
                        senderLocation.Longitude = dal.ClientById(dal.PackageById(droneToList.PackageID).SenderId).Longitude;

                        double minBattery;
                        minBattery = batteryConsumption(droneToList.DroneLocation.Latitude, droneToList.DroneLocation.Longitude, senderLocation.Latitude, senderLocation.Longitude, 3); // From the position of the drone to the position of the sender at empty weight (if it is not different from the position of the sender then nothing will be added)
                        minBattery += batteryConsumption(droneToList.DroneLocation.Latitude, droneToList.DroneLocation.Longitude, targetLocation.Latitude, targetLocation.Longitude, (int)package.Weight); // From the location of the sender to the destination location in the weight of the package

                        DO.Station stationLocation = NearestStationToClient(package.TargetId); //The station closest to the target
                        minBattery += batteryConsumption(targetLocation.Latitude, targetLocation.Longitude, stationLocation.Latitude, stationLocation.Longitude, 3); //From the destination location to the location of the nearest station at empty weight

                        if (minBattery > 100) throw new Exceptions.UnableToItinitDrone("Battery over 100", droneToList.ID);
                        droneToList.Battery = rand.Next((int)minBattery + 1, 101); // Between the minimum battery consumption and 100

                        break;
                    }
                }

                if(mode == "List")
                {
                    if (!flag) // If the drone is not associated with a package that has not yet been delivered
                    {
                        droneToList.Status = (DroneStatus)(rand.Next(0, 2)); // Status between available and maintained
                        if (droneToList.Status == DroneStatus.Maintenance) // If it is in maintenance
                        {
                            DO.Station station = dal.StationsFilter(s => s.ChargeSlots > 0).ElementAt(rand.Next(0, dal.StationsFilter(s => s.ChargeSlots > 0).Count())); // Lottery location between stations with charging stations available
                            droneToList.DroneLocation.Latitude = station.Latitude;
                            droneToList.DroneLocation.Longitude = station.Longitude;
                            droneToList.Battery = rand.Next(0, 21);

                            dal.DroneCharge(drone, station.ID); // Adding a drone for charging
                        }
                        else // If the status is available
                        {
                            int index = rand.Next(0, 10);
                            while (dal.PackageList().ElementAt(index).Delivered == null) //The location will be in the customer who has a package delivered to him. (There is one that we created at boot)
                            {
                                index = rand.Next(0, 10);
                            }
                            int clientID = dal.PackageList().ElementAt(index).TargetId; //The customer selected - having a package delivered to him                                                                

                            droneToList.DroneLocation.Latitude = dal.ClientById(clientID).Latitude;
                            droneToList.DroneLocation.Longitude = dal.ClientById(clientID).Longitude;

                            double minBattery;
                            DO.Station stationLocation = NearestStationToClient(dal.ClientById(clientID).ID);
                            minBattery = batteryConsumption(droneToList.DroneLocation.Latitude, droneToList.DroneLocation.Longitude, stationLocation.Latitude, stationLocation.Longitude, 3); //
                                                                                                                                                                                              //minBattery = BatteryByKM(3, KM);
                            droneToList.Battery = rand.Next((int)minBattery + 1, 101); //

                        }
                    }
                }
                else // xml mode
                {
                    if (!flag) // If the drone is not associated with a package that has not yet been delivered
                    {
                        try
                        {
                            DO.DroneCharge droneCharge = dal.DroneChargeByIdDrone(drone.ID);

                            droneToList.Status = DroneStatus.Maintenance;
                            DO.Station station = dal.StationById(droneCharge.StationId);
                            droneToList.DroneLocation.Latitude = station.Latitude;
                            droneToList.DroneLocation.Longitude = station.Longitude;
                            droneToList.Battery = rand.Next(0, 21);
                        }
                        catch (Exception ex)
                        {
                            droneToList.Status = DroneStatus.Available;
                            if (ex.Message != "Drone not found in Chargeing") throw new Exceptions.StationException("In BL constructor in XML mode - no drone station was found to charge, even though the drone is charging at this station", ex);
                            int index = rand.Next(0, 10);
                            while (dal.PackageList().ElementAt(index).Delivered == null) //The location will be in the customer who has a package delivered to him. (There is one that we created at boot)
                            {
                                index = rand.Next(0, 10);
                            }
                            int clientID = dal.PackageList().ElementAt(index).TargetId; //The customer selected - having a package delivered to him                                                                

                            droneToList.DroneLocation.Latitude = dal.ClientById(clientID).Latitude;
                            droneToList.DroneLocation.Longitude = dal.ClientById(clientID).Longitude;

                            double minBattery;
                            DO.Station stationLocation = NearestStationToClient(dal.ClientById(clientID).ID);
                            minBattery = batteryConsumption(droneToList.DroneLocation.Latitude, droneToList.DroneLocation.Longitude, stationLocation.Latitude, stationLocation.Longitude, 3); //
                                                                                                                                                                                              //minBattery = BatteryByKM(3, KM);
                            droneToList.Battery = rand.Next((int)minBattery + 1, 101); //
                        }
                    }
                }
                DroneList.Add(droneToList);
            }
        }


        /// <summary>
        /// Function for updating the battery when the drone is charging - for simulator 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="indexDroneToList"></param>
        internal void updateDroneBattery(int id, DateTime? start ,int indexDroneToList = -1)
        {
            if (start == null) start = dal.droneChargesList().First(x => x.DroneId == id).ChargingStartTime;
            if (indexDroneToList == -1) indexDroneToList = DroneList.FindIndex(d => d.ID == id);

            int secondsCharging = (int)((DateTime.Now - start).Value.TotalSeconds); // המרה מspentime
            int battary = (int)((secondsCharging) * ChargeRate); // Calculate the new battery
            DroneList[indexDroneToList].Battery += battary;
            if (DroneList[indexDroneToList].Battery > 100) DroneList[indexDroneToList].Battery = 100;
        }



        /// <summary>
        /// The function receives a drone and station number for charging and adds the drone to the list and location of the station
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="stationNumToCharge"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone drone, int stationNumToCharge)
        {

            lock (dal)
            {
                if (drone.ID < 0) throw new BO.Exceptions.NegativeException("Drone ID can not be negative", drone.ID);
                if (!dal.StationsList().Any(x => x.ID == stationNumToCharge)) throw new BO.Exceptions.IDException("Station ID not found", stationNumToCharge);
                if (dal.StationById(stationNumToCharge).ChargeSlots <= 0) throw new BO.Exceptions.SendingDroneToCharging("There are no charging slots available at the station", stationNumToCharge);


                DO.Drone droneDAL = new DO.Drone(); // For addition to the list in DAL
                droneDAL.ID = drone.ID;
                droneDAL.Model = drone.Model;
                droneDAL.MaxWeight = (DO.WeightCategories)(int)(drone.MaxWeight);
                try // Exceeding the data layer
                {
                    dal.AddDrone(droneDAL); // addition to the list in DAL
                }
                catch (DO.Exceptions.IDException ex)
                {
                    throw new Exceptions.IDException("A Drone ID already exists", ex, droneDAL.ID);
                }
                dal.DroneCharge(droneDAL, stationNumToCharge); // add drone to charge


                DroneToList droneToList = new DroneToList();
                droneToList.DroneLocation = new Location();
                droneToList.ID = drone.ID;
                droneToList.Model = drone.Model;
                droneToList.MaxWeight = drone.MaxWeight;
                droneToList.Battery = rand.Next(20, 41);
                droneToList.Status = DroneStatus.Maintenance;
                droneToList.DroneLocation.Latitude = dal.StationById(stationNumToCharge).Latitude;
                droneToList.DroneLocation.Longitude = dal.StationById(stationNumToCharge).Longitude;
                droneToList.PackageID = 0;
                DroneList.Add(droneToList); // Add to list in BL
            }
        }



        /// <summary>
        /// A function that gets a new name for the station and updates the station name if the input is not empty
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDroneName(int id, string name)
        {
            lock (dal)
            {
                DO.Drone droneDAL;
                if (!DroneList.Any(x => x.ID == id)) throw new BO.Exceptions.IDException("Drone ID not found", id);
                droneDAL = dal.DroneById(id);

                foreach (var droneBL in DroneList) // Update the name on the list in BL
                {
                    if (droneBL.ID == id)
                    {
                        droneBL.Model = name;
                        break;
                    }
                }
                DO.Drone droneDalTemp = droneDAL; // Update the name in the list in DAL
                droneDalTemp.Model = name;
                try
                {
                    dal.DeleteDrone(droneDAL.ID);
                    dal.AddDrone(droneDalTemp);
                }
                catch (DO.Exceptions.IDException ex) { throw new BO.Exceptions.IDException("Fault in drone update. Was not supposed to be an exception because we have already checked before", ex, id); }
            }
        }



        /// <summary>
        /// The function receives a drone id and sends it for charging
        /// </summary>
        /// <param name="id"> id inputed by user </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ChargeDrone(int id)
        {
            DroneToList drone = DroneList.Find(drone => drone.ID == id);
            if (drone == null) { throw new BO.Exceptions.IDException("Drone ID not found", id); }

            if (drone.Status == DroneStatus.Available) //Will ship for loading only if available
            {
                DO.Station station = NearestStationToDrone(drone.ID);
                double minBattery = batteryConsumption(drone.DroneLocation.Latitude, drone.DroneLocation.Longitude, station.Latitude, station.Longitude, 3); // The amount of battery required for the drone to fly from its location to the station

                if (drone.Battery < minBattery) throw new BO.Exceptions.SendingDroneToCharging("The drone can not reach the station, Not enough battery", drone.ID);

                // Can be sent for loading
                DroneToList tempDrone = drone;

                tempDrone.Battery -= (int)Math.Round(minBattery); // The drone flew to the station so it was necessary to reduce the battery from where it was to the station
                tempDrone.DroneLocation.Latitude = station.Latitude;
                tempDrone.DroneLocation.Longitude = station.Longitude;
                tempDrone.Status = DroneStatus.Maintenance;

                lock (dal)
                {
                    dal.DroneCharge(dal.DroneById(drone.ID), station.ID); // Creating a DroneCharge and reducing the free slots in the station
                }

            }
            else
            {
                throw new BO.Exceptions.SendingDroneToCharging("Drone status is not Available", drone.ID);
            }
        }



        /// <summary>
        /// The function gets the drone number and the time it was charging and takes the drone out of charge
        /// </summary>
        /// <param name="droneID"></param>
        /// <param name="minutesCharging"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void FinishCharging(int droneID)
        {

            lock (dal)
            {
                if (!DroneList.Any(drone => drone.ID == droneID)) throw new BO.Exceptions.IDException("Drone ID not found", droneID);
                if (DroneList.Find(d => d.ID == droneID).Status != DroneStatus.Maintenance) throw new BO.Exceptions.EndDroneCharging("The status of the drone is not Maintenance", droneID);
                if (!dal.droneChargesList().Any(d => d.DroneId == droneID)) throw new BO.Exceptions.EndDroneCharging("The status of the drone is charging but it is not in the droneCharges list", droneID);
                int indexDroneToList = DroneList.FindIndex(d => d.ID == droneID);
                if (DroneList.Find(drone => drone.ID == droneID).Status != DroneStatus.Maintenance) throw new BO.Exceptions.EndDroneCharging("Drone status is not Maintenance", droneID);

                updateDroneBattery(droneID, dal.droneChargesList().First(x => x.DroneId == droneID).ChargingStartTime, indexDroneToList);
                //int secondsCharging = (int)((DateTime.Now - dal.droneChargesList().First(x => x.DroneId == droneID).ChargingStartTime).TotalSeconds); // המרה מspentime

                //int battary = (int)((secondsCharging ) * ChargeRate); // Calculate the new battery
                //DroneList[indexDroneToList].Battery += battary;
                //if (DroneList[indexDroneToList].Battery > 100) DroneList[indexDroneToList].Battery = 100;
                DroneList[indexDroneToList].Status = DroneStatus.Available;

                DO.DroneCharge droneCharge = dal.droneChargesList().First(d => d.DroneId == droneID);

                dal.FinishCharging(droneCharge); // Send to a function that will increase load slots and delete the droneCharge
            }

        }

      


        /// <summary>
        /// The function receives a drone number and returns its display
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Drone object </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone DisplayDrone(int id)
        {
            if (!DroneList.Any(d => d.ID == id))
                throw new BO.Exceptions.IDException("Drone ID not found", id);
            DroneToList droneToList = DroneList.Find(d => d.ID == id);
            if (droneToList == null) throw new BO.Exceptions.IDException("Drone ID not found", id);

            Drone drone = new Drone();
            drone.DronePackageProcess = new PackageProcess();

            drone.ID = droneToList.ID;
            drone.Battery = droneToList.Battery;
            drone.DroneLocation = droneToList.DroneLocation;
            drone.MaxWeight = droneToList.MaxWeight;
            drone.Model = droneToList.Model;
            drone.Status = droneToList.Status;

            if (drone.Status == DroneStatus.Shipping) // Only if the drone delivers a package will the DronePackageProcess be initialized
            {
                lock (dal)
                {
                    DO.Package package = dal.PackageList().First(x => x.DroneId == drone.ID && x.Delivered == null);
                    drone.DronePackageProcess.Id = package.ID;
                    drone.DronePackageProcess.Priority = (Priorities)(package.Priority);
                    drone.DronePackageProcess.Weight = (WeightCategories)(package.Weight);
                    if (package.PickedUp == null) drone.DronePackageProcess.PackageShipmentStatus = ShipmentStatus.Waiting;
                    else drone.DronePackageProcess.PackageShipmentStatus = ShipmentStatus.OnGoing;

                    ClientPackage sender = new ClientPackage();
                    ClientPackage receiver = new ClientPackage();
                    Location collectLocation = new Location();
                    Location destinationLocation = new Location();

                    // Initials of customers in the package
                    sender.ID = package.SenderId;
                    sender.Name = dal.ClientById(sender.ID).Name;
                    receiver.ID = package.TargetId;
                    receiver.Name = dal.ClientById(receiver.ID).Name;
                    drone.DronePackageProcess.Sender = sender;
                    drone.DronePackageProcess.Receiver = receiver;

                    //Initials of customers' locations in the package
                    collectLocation.Latitude = dal.ClientById(sender.ID).Latitude;
                    collectLocation.Longitude = dal.ClientById(sender.ID).Longitude;
                    destinationLocation.Latitude = dal.ClientById(receiver.ID).Latitude;
                    destinationLocation.Longitude = dal.ClientById(receiver.ID).Longitude;
                    drone.DronePackageProcess.CollectLocation = collectLocation;
                    drone.DronePackageProcess.DestinationLocation = destinationLocation;

                    if(drone.DronePackageProcess.PackageShipmentStatus == ShipmentStatus.Waiting)
                    {
                        drone.DronePackageProcess.Distance = DalObject.Coordinates.Distance(drone.DroneLocation.Latitude, drone.DroneLocation.Longitude, drone.DronePackageProcess.CollectLocation.Latitude, drone.DronePackageProcess.CollectLocation.Longitude);
                    }
                    else
                    {
                        drone.DronePackageProcess.Distance = DalObject.Coordinates.Distance(drone.DroneLocation.Latitude, drone.DroneLocation.Longitude,
                    drone.DronePackageProcess.DestinationLocation.Latitude, drone.DronePackageProcess.DestinationLocation.Longitude); //The distance between the sender and the destination
                    }
                    
                }
            }
            else drone.DronePackageProcess = null;

            return drone;
        }



        /// <summary>
        /// A function that returns the list of drones
        /// </summary>
        /// <returns> Drone list </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> DisplayDroneList()
        {
            List<DroneToList> drones = new List<DroneToList>(DroneList); // Copy of list without reference !!
            return drones;
        }

        /// <summary>
        /// Returns a filtered list of drones
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> DisplayDroneListFilter(Predicate<DroneToList> match)
        {
            List<DroneToList> drones = new List<DroneToList>(DroneList).FindAll(match); // Copy of list without reference !!
            return drones;
        }

        /// <summary>
        /// Get PackageToList
        /// </summary>
        /// <param name="id">id of package</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public PackageToList GetPackageToList(int id)
        {
           return DisplayPackageList().First(p => p.Id == id);
        }


        /// <summary>
        /// Calculating the station closest to the drone and returning it
        /// </summary>
        /// <param name="DroneID"></param>
        /// <returns> closest dal station object </returns>
        private DO.Station NearestStationToDrone(int DroneID)
        {
            DroneToList drone = DroneList.Find(x => x.ID == DroneID);
            DO.Station neareStatuon = new DO.Station();
            double distance = int.MaxValue;

            lock (dal)
            {
                if (dal.StationsFilter(s => s.ChargeSlots > 0).Count() == 0) throw new BO.Exceptions.SendingDroneToCharging("There are no charging slots available at any station", DroneID); // If no charging slots are available at any station
                foreach (var station in dal.StationsFilter(s => s.ChargeSlots > 0))
                {

                    double tempDistance = DalObject.Coordinates.Distance(drone.DroneLocation.Latitude, drone.DroneLocation.Longitude, station.Latitude, station.Longitude);
                    if (tempDistance < distance) //If it's closer
                    {
                        distance = tempDistance;
                        neareStatuon = station;
                    }
                }
            }
            return neareStatuon;
        }


        /// <summary>
        /// A function that receives 2 points and calculates the battery consumption
        /// for the drone to fly between the points and according to the weight of the package And returns it
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="long1"></param>
        /// <param name="lat2"></param>
        /// <param name="long2"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        private double batteryConsumption(double lat1, double long1, double lat2, double long2, int weight)
        {
            double KM = DalObject.Coordinates.Distance(lat1, long1, lat2, long2);
            double battery = BatteryByKM(weight, KM);

            return battery;
        }


        /// <summary>
        /// A function that receives a package weight and several kilometers 
        /// and calculates the battery consumption by package weight in the same number of kilometers And returns it
        /// </summary>
        /// <param name="weight">weight option </param>
        /// <param name="KM"> kilometer </param>
        /// <returns> Battery value </returns>
        internal double BatteryByKM(int weight, double KM)
        {
            double power;
            if (weight == 0) power = PowerLightDrone;
            else if (weight == 1) power = PowerMediumDrone;
            else if (weight == 2) power = PowerHeavyDrone;
            else power = PowerVacantDrone;
            double temp = (KM * power);
            return temp;
        }


        /// <summary>
        /// grouping function to group by status
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<IGrouping<DroneStatus, DroneToList>> DroneGroupbyStatus()
        {
            return DisplayDroneList().GroupBy(x => x.Status);
        }


        /// <summary>
        /// Simulator operation function
        /// </summary>
        /// <param name="id">id of drone</param>
        /// <param name="action"></param>
        /// <param name="stop"></param>
        public void StartSimulator(int id, Action<string,int> action, Func<bool> stop)
        {
            Simulator simulator = new Simulator(this, id, action, stop);
        }


        /// <summary>
        /// Function for updating location from the drone - for the simulator
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lonPlus"></param>
        /// <param name="latPlus"></param>
        internal void UpdateDroneLocation(int id, double lonPlus, double latPlus)
        {

            DroneToList droneToList = DroneList.Find(d => d.ID == id);

            droneToList.DroneLocation.Latitude += latPlus;
            droneToList.DroneLocation.Longitude += lonPlus;
        }

        /// <summary>
        /// Function for reducing the battery from the drone - for the simulator
        /// </summary>
        /// <param name="id"></param>
        /// <param name="LessBattery"></param>
        internal void UpdateLessBattery(int id, double LessBattery)
        {

            DroneToList droneToList = DroneList.Find(d => d.ID == id);

            droneToList.Battery -= LessBattery;
        }



    }


   


}