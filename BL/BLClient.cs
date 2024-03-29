﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.Runtime.CompilerServices;
using BlApi;


namespace BL
{
    internal partial class BL : IBL          // Partial Client BL Class that contains Clients Functions
    {

        /// <summary>
        /// The function get a object client from user input and adds it to the clients list in Datasource 
        /// </summary>
        /// <param name="client"> Client object from ConsoleUi </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddClient(Client client)
        {

            if (client.ID < 0)                                                                         // Id input exceptions
                throw new BO.Exceptions.NegativeException("Client ID cannot be negative", client.ID);
            if (client.ID < 100000000 || client.ID > 1000000000)
                throw new Exceptions.IDException("Id not valid", client.ID);

            DO.Client dalClient = new DO.Client();        //creating new datasource client then assigning its attributes then adding it to client list

            correctPhone(client.Phone);          //send to check if phone input is correct, else exception

            dalClient.ID = client.ID;
            dalClient.Name = client.Name;
            dalClient.Phone = client.Phone;
            if (((client.ClientLocation.Latitude < 31.73) || (client.ClientLocation.Latitude > 31.83)) ||
                ((client.ClientLocation.Longitude < 35.16) || (client.ClientLocation.Longitude > 35.26)))      //location exception 
            {
                throw new Exceptions.LocationOutOfRange("Client Location entered is out of shipping range", client.ID);
            }
            dalClient.Latitude = client.ClientLocation.Latitude;
            dalClient.Longitude = client.ClientLocation.Longitude;

            try    // adding it to client list in datasource
            {
                lock (dal)
                {
                    dal.AddClient(dalClient);
                }
            }
            catch (DO.Exceptions.IDException ex)
            {
                throw new Exceptions.IDException("Client with this ID already exists", ex, dalClient.ID);  //new ibl exception
            }
        }

        /// <summary>
        /// The function update an existing client and changes it name or phone number according to user
        /// </summary>
        /// <param name="id"> id to find the client to update info </param>
        /// <param name="name"> new name to give to that </param>
        /// <param name="phone"> new phone to give to that client </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateClient(int id, string name, string phone)
        {
            DO.Client dalClient;

            lock (dal)
            {
                if (!dal.ClientsList().Any(x => x.ID == id))
                    throw new BO.Exceptions.IDException("Client ID not found", id);

                correctPhone(phone);        //check if phone number is correct

                dalClient = dal.ClientById(id);

                DO.Client clientTemp = dalClient;

                if (name != "")         //changing name or phone or both, if no input (only enter), no changes
                    clientTemp.Name = name;
                if (phone != "")
                    clientTemp.Phone = phone;

                dal.DeleteClient(dalClient.ID);
                dal.AddClient(clientTemp);
            }
        }

        /// <summary>
        /// the function find the client according to id input, assign its attributes to clientBl object then returns it and so display its client information
        /// </summary>
        /// <param name="id"> client id from console </param>
        /// <returns> Client object type </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Client DisplayClient(int id)
        {
            lock (dal)
            {
                if (!dal.ClientsList().Any(x => x.ID == id))
                    throw new BO.Exceptions.IDException("Client ID not found", id);      //else client with id exist in dal

                DO.Client dalClient = dal.ClientsList().First(x => x.ID == id);

                Client client = new Client();           //create new object and getting and assign the client info from datasource

                client.ID = dalClient.ID;
                client.Name = dalClient.Name;
                client.Phone = dalClient.Phone;

                Location location = new Location();
                location.Longitude = dalClient.Longitude;
                location.Latitude = dalClient.Latitude;

                client.ClientLocation = location;

                List<PackageAtClient> senderPackage = new List<PackageAtClient>();

                foreach (var item in dal.PackageList())     //finding packages list that the client sent
                {
                    if (item.SenderId == dalClient.ID)
                    {
                        PackageAtClient packageAtClient = new PackageAtClient();  //new object to assign to package at client attribute

                        packageAtClient.Id = item.ID;
                        packageAtClient.Weight = (WeightCategories)item.Weight;
                        packageAtClient.Priority = (Priorities)item.Priority;

                        if (item.Associated == null)           //assigning package status according to its status
                            packageAtClient.Status = PackageStatus.Created;
                        else if (item.PickedUp == null)
                        {
                            packageAtClient.Status = PackageStatus.Associated;
                        }
                        else if (item.Delivered == null)
                        {
                            packageAtClient.Status = PackageStatus.PickedUp;
                        }
                        else
                            packageAtClient.Status = PackageStatus.Delivered;

                        ClientPackage clientPackage = new ClientPackage();        //new object of target client that the sender sent the package
                        clientPackage.ID = item.TargetId;
                        clientPackage.Name = dal.ClientById(item.TargetId).Name;

                        packageAtClient.Source_Destination = clientPackage;

                        senderPackage.Add(packageAtClient);         // adding it to the list
                    }
                }

                List<PackageAtClient> receiverPackage = new List<PackageAtClient>();

                foreach (var item in dal.PackageList())         //finding packages list that the client received and its informations
                {

                    if (item.TargetId == dalClient.ID)
                    {
                        PackageAtClient packageAtClient = new PackageAtClient();

                        packageAtClient.Id = item.ID;
                        packageAtClient.Weight = (WeightCategories)item.Weight;
                        packageAtClient.Priority = (Priorities)item.Priority;


                        if (item.Associated == null)               //assigning package status according to its status
                            packageAtClient.Status = PackageStatus.Created;
                        else if (item.PickedUp == null)
                        {
                            packageAtClient.Status = PackageStatus.Associated;
                        }
                        else if (item.Delivered == null)
                        {
                            packageAtClient.Status = PackageStatus.PickedUp;
                        }
                        else
                            packageAtClient.Status = PackageStatus.Delivered;

                        ClientPackage clientPackage = new ClientPackage();

                        clientPackage.ID = item.SenderId;
                        clientPackage.Name = dal.ClientById(item.SenderId).Name;

                        packageAtClient.Source_Destination = clientPackage;

                        receiverPackage.Add(packageAtClient);       //adding to list
                    }
                }

                client.ClientsSender = senderPackage;    //assign two lists to client attribute then return the client
                client.ClientsReceiver = receiverPackage;

                return client;  //return the client object to display it using Tostring
            }
        }


        /// <summary>
        /// The function Display every Client attributes with a list of package info
        /// </summary>
        /// <returns> Client List </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ClientToList> DisplayClientList()
        {
            lock (dal)
            {
                List<ClientToList> clients = new List<ClientToList>();   //creating new list to return after assign

                foreach (var dalClient in dal.ClientsList())
                {
                    ClientToList clientToList = new ClientToList();  //new list , getting its values then returns all of it to display

                    clientToList.Id = dalClient.ID;
                    clientToList.Name = dalClient.Name;
                    clientToList.Phone = dalClient.Phone;

                    IEnumerable<DO.Package> sentAndDelivered = dal.PackageList().Where(x => x.SenderId == dalClient.ID && x.Delivered != null);  //new list of sender package that have been delevered

                    clientToList.sentAndDeliveredPackage = sentAndDelivered.Count();                 //getting number of element we have and assigning to attribute "number of sent and delivered sender client" same for all below

                    IEnumerable<DO.Package> sentAndUndelivered = dal.PackagesFilter(x => x.SenderId == dalClient.ID && x.Delivered == null);

                    clientToList.sentAndUndeliveredPackage = sentAndUndelivered.Count();

                    IEnumerable<DO.Package> ReceivedAndDelivered = dal.PackagesFilter(x => x.TargetId == dalClient.ID && x.Delivered != null);

                    clientToList.ReceivedAndDeliveredPackage = ReceivedAndDelivered.Count();

                    IEnumerable<DO.Package> ReceivedAndUnDelivered = dal.PackagesFilter(x => x.TargetId == dalClient.ID && x.Delivered == null);

                    clientToList.ReceivedAndUnDeliveredPackage = ReceivedAndUnDelivered.Count();

                    clients.Add(clientToList);      //adding it to the list
                }
                return clients;
            }
        }

        /// <summary>
        /// The fonction finds the nearest station from the client that has Available charge slots To charge a shipping drone
        /// </summary>
        /// <param name="ClientID">  Client id to to find the right location </param>
        /// <returns> station with closest location </returns>
        private DO.Station NearestStationToClient(int ClientID)
        {
            lock (dal)
            {
                DO.Station tempStation = new DO.Station();
                double distance = int.MaxValue;
                if ((dal.StationsFilter(s => s.ChargeSlots > 0)).Count() == 0)
                    throw new BO.Exceptions.SendingDroneToCharging("There are no charging slots available at any station", 0); //if there are no station with available charge slots

                foreach (var station in dal.StationsFilter(s => s.ChargeSlots > 0)) //calculating the min of station distance with client
                {
                    double tempDistance = DalObject.Coordinates.Distance(dal.ClientById(ClientID).Latitude, dal.ClientById(ClientID).Longitude, station.Latitude, station.Longitude);
                    if (tempDistance < distance)
                    {
                        distance = tempDistance;
                        tempStation.Latitude = station.Latitude;
                        tempStation.Longitude = station.Longitude;
                    }
                }
                return tempStation;
            }
        }


        /// <summary>
        /// private function that check if phone number entered is correct according to israel standard
        /// </summary>
        /// <param name="phone"> phone inputed by user </param>
        private void correctPhone(string phone)
        {
            string[] nums = { "052", "053", "054", "055", "056", "057", "058" };

            if (phone == "")
                return;

            if (phone.Length != 10) throw new BO.Exceptions.PhoneExceptional("The cell phone number is incorrect", phone);

            phone = phone.Substring(0, 3);
            if (!nums.Any(x => x == phone)) throw new BO.Exceptions.PhoneExceptional("The cell phone number is incorrect", phone);
        }


        /// <summary>
        /// Get ClientToList
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ClientToList GetClientToList(int id)
        {
            return DisplayClientList().First(c => c.Id == id);
        }
    }
}
