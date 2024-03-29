﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.Runtime.CompilerServices;

namespace BlApi
{
    public interface IBL
    {
        //-----------ADD Functions------------

        /// <summary>
        /// The function receives a drone and station number for charging and adds the drone to the list and location of the station
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="stationNumToCharge"></param>
        void AddDrone(Drone drone, int stationNumToCharge);
        


        /// <summary>
        /// The function get a object station from user input and adds it to the station list in Datasource 
        /// </summary>
        /// <param name="station"> Station object from ConsoleUi </param>
        void AddStation(Station station);


        /// <summary>
        /// The function Add a package in the list of packages in Datasource 
        /// And returns the ID number of the package (running number)
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        int AddPackage(Package package);


        /// <summary>
        /// The function get a object client from user input and adds it to the clients list in Datasource 
        /// </summary>
        /// <param name="client"> Client object from ConsoleUi </param>
        void AddClient(Client client);



        //-----------Update Functions------------//

        /// <summary>
        /// A function that gets a new name for the station and updates the station name if the input is not empty
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        void UpdateDroneName(int id, string name);
        


        /// <summary>
        /// The function receives a skimmer number and sends it for charging
        /// </summary>
        /// <param name="id"></param>
        void ChargeDrone(int id);



        /// <summary>
        /// The function gets the drone number and the time it was charging and takes the drone out of charge
        /// </summary>
        /// <param name="droneID"></param>
        /// <param name="minutesCharging"></param>
        void FinishCharging(int droneID);


        /// <summary>
        /// The function receives a drone number
        /// and assigns it a package that can belong to it according to urgency, weight, and battery
        /// </summary>
        /// <param name="droneID"></param>
        void packageToDrone(int droneID);


        /// <summary>
        /// the function get a drone to pick up a package
        /// </summary>
        /// <param name="droneID"></param>
        void PickedUpByDrone(int droneID);


        /// <summary>
        /// The function receives a drone number and delivers the package to the destination
        /// </summary>
        /// <param name="droneID"></param>
        void DeliveredToClient(int droneID);


        /// <summary>
        /// The function update an existing client and changes it name or phone number according to user
        /// </summary>
        /// <param name="id"> id to find the client to update info </param>
        /// <param name="name"> new name to give to that </param>
        /// <param name="phone"> new phone to give to that client </param>
        void UpdateClient(int id, string name, string phone);
  


        /// <summary>
        /// The function update the number of charge slot of the station that matches the id user inputed
        /// </summary>
        /// <param name="id">id to find which station user wants to change the name </param>
        /// <param name="numCharge">new number of charge slot to give to the given station </param>
        void UpdateStationNumCharge(int id, int numCharge);


        /// <summary>
        /// The function update the name of the station that matches the id user inputed
        /// </summary>
        /// <param name="id"> id to find which station user wants to change the name</param>
        /// <param name="name"> new name to give to the station</param>
        void UpdateStationName(int id, string name);



        //-----------Display Item------------//

        /// <summary>
        /// The function receives a drone number and returns its display
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Drone DisplayDrone(int id);


        /// <summary>
        /// the function find the station according to id, create new BL station to return it to display the station information 
        /// </summary>
        /// <param name="id"> id inputed in ConsoleUi to find which station to display </param>
        /// <returns> returns StationBl object </returns>
        Station DisplayStation(int id);


        /// <summary>
        /// the function find and display package information according to user id input
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        Package DisplayPackage(int packageID);


        /// <summary>
        /// the function find the client according to id input, assign its attributes to clientBl object then returns it and so display its client information
        /// </summary>
        /// <param name="id"> client id from console </param>
        /// <returns> Client object type </returns>
        Client DisplayClient(int id);



        //----------Display Lists------------//

        /// <summary>
        /// A function that returns the list of drones
        /// </summary>
        /// <returns></returns>
        IEnumerable<DroneToList> DisplayDroneList();


        /// <summary>
        /// The function Display list of all station information
        /// </summary>
        /// <returns> Returns List of stations </returns>
        IEnumerable<StationToList> DisplayStationList();


        /// <summary>
        /// function that return list of station with available chargeSlot
        /// </summary>
        /// <returns> Returns List of stations </returns>
        IEnumerable<StationToList> DisplayStationListWitAvailableChargingSlots();


        /// <summary>
        /// The function Display list of all packages information
        /// </summary>
        /// <returns></returns>
        IEnumerable<PackageToList> DisplayPackageList();


        /// <summary>
        /// function that return list of package that havent been associated
        /// </summary>
        /// <returns></returns>
        IEnumerable<PackageToList> DisplayPackageListWithoutDrone();


        /// <summary>
        /// The function Display every Client attributes with a list of package info
        /// </summary>
        /// <returns> Client List </returns>
        IEnumerable<ClientToList> DisplayClientList();

        /// <summary>
        /// List of packages grouped by date
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IEnumerable<PackageToList> GetPackageFilterByDate(DateTime from, DateTime to);

        /// <summary>
        /// Returns a filtered list of drones
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        IEnumerable<DroneToList> DisplayDroneListFilter(Predicate<DroneToList> match);


        /// <summary>
        /// A function that returns a grouped list of packages of a receiving customer
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGrouping<string, PackageToList>> PackagesGroupingReceiver();


        /// <summary>
        /// A function that returns a grouped list of packages of a sender customer
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGrouping<string, PackageToList>> PackagesGroupingSender();

        /// <summary>
        /// Returns a grouped list of stations with free slots
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGrouping<int, StationToList>> GroupStationByNumSlots();

        /// <summary>
        /// grouping function to group by status
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGrouping<DroneStatus, DroneToList>> DroneGroupbyStatus();


        /// <summary>
        /// List of packages grouped by sending customer
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        IEnumerable<PackageToList> GetPackagesSentBySpecificClient(int ClientId);


        /// <summary>
        /// List of packages grouped by receiving customer
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        IEnumerable<PackageToList> GetPackagesSentToSpecificClient(int ClientId);


        /// <summary>
        /// Delete Package
        /// </summary>
        /// <param name="ID"></param>
        void DeletePackage(int ID);

        /// <summary>
        /// Simulator operation function
        /// </summary>
        /// <param name="id">id of drone</param>
        /// <param name="action"></param>
        /// <param name="stop"></param>
        void StartSimulator(int id, Action<string,int> action, Func<bool> stop);


        /// <summary>
        /// Get PackageToList
        /// </summary>
        /// <param name="id">id of package</param>
        /// <returns></returns>
        PackageToList GetPackageToList(int id);


        /// <summary>
        /// Get ClientToList
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ClientToList GetClientToList(int id);


        /// <summary>
        /// Returns the station where the drone is loaded
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BO.Station GetStationWithDrones(int id);

       










    }

}
