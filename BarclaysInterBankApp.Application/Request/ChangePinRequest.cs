﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Request
{
    public class ChangePinRequest
    {
        public string AccountNumber { get; set; }
        public string PinHash { get; set; }
    }
}