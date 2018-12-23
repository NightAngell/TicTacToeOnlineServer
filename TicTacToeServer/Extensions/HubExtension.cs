using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.Extensions
{
    public static class HubExtension
    {
        public static void AddOrUpdateItemInContextItems<T>(this Hub hub, string key, T value)
        {
            if (hub.Context.Items.ContainsKey(key))
            {
                hub.Context.Items[key] = value;
            }
            else
            {
                hub.Context.Items.Add(key, value);
            }
        }
    }
}
