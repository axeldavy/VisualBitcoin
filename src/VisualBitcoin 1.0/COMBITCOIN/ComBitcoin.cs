#region License, Terms and Conditions
//
// Jayrock - A JSON-RPC implementation for the Microsoft .NET Framework
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using Bitnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace COMBITCOIN
{
    public class ComBitcoin
    {
        public void RetrieveTransactions(String transactions)
        {
            BitnetClient bc = new BitnetClient("http://127.0.0.1:8332");
            bc.Credentials = new NetworkCredential("user", "pass");
            var l = bc.InvokeMethod("listtransactions")["result"] as JArray;
            Console.WriteLine(l);
        }
    }
}
