﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRoleCommandProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var bookingProcessor = new BookingProcessor(false);
        }
    }
}
