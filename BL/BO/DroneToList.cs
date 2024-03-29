﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    public class DroneToList
    {
        public int ID { get; set; }
        public string Model { get; set; }
        public WeightCategories MaxWeight { get; set; }
        public double Battery { get; set; }
        public DroneStatus Status { get; set; }
        public Location DroneLocation { get; set; }
        public int PackageID { get; set; }

        public override string ToString()
        {
            string result = "";
            result += $"Drone ID is {ID}, ";
            result += $"Drone Model is {Model}, ";
            result += $"Drone Max Weight Capacity is {MaxWeight}, ";
            result += $"Drone Battery is {Battery}, ";
            result += $"Drone Status is {Status}, ";
            result += $"Drone Location is: {DroneLocation}\n";
            result += $"Drone PackageID Id is {PackageID}.";

            return result;
        }
    }
}

