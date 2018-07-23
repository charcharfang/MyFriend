using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFriend.Shop
{
    public interface ICharge
    {
        string GetStations();
        string GetStationDetail(string staid);
    }    
}
