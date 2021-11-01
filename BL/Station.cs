﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IBL
{
    namespace BO
    {
        public class Station
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int ChargeSlots { get; set; }

            public IBL.BO.Position 


            public override string ToString()
            {
                string result = "";
                result += $"ID is {ID}, \n";
                result += $"Name is {Name},\n";
                result += $"ChargeSlots is {ChargeSlots}, \n";
                result += $"Latitude is {DAL.Coordinates.ConvertLatitude(Latitude)}, \n";
                result += $"Longitude is {DAL.Coordinates.ConvertLongitude(Longitude)}, \n";

                return result;
            }
        }
    }

    }



}