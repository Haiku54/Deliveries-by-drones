﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.Threading;
using static BL.BL;

namespace BL
{
    class Simulator
    {
        BlApi.IBL BL;
        int droneID;
        Action action;
        Func<bool> stop;

        Simulator(BlApi.IBL bl, int id, Action action, Func<bool> stop)
        {
            BL = bl;
            droneID = id;
            this.action = action;
            this.stop = stop;
        }
    }
}
