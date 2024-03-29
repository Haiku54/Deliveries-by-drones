﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DO
{
    public struct DroneCharge
    {
        public int DroneId { get; set; }
        public int StationId { get; set; }
        public DateTime ChargingStartTime { get; set; }



        public override string ToString()
        {
            string result = "";
            result += $"DroneId is {DroneId},\n";
            result += $"StationId is {StationId}, \n";
            result += $"ChargingStartTime is {ChargingStartTime}, \n";

            return result;
        }
    }


}
