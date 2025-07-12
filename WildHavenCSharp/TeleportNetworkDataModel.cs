using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildHaven;

public class TeleportNetworkDataModel
{
    public string Id = string.Empty;
    public Dictionary<string, TeleportLocation> Locations = [];
}

public class TeleportLocation
{
    public string Id = string.Empty;
    public int Precedence = 0;
    public string DisplayName = string.Empty;
    public string Condition = string.Empty;
    public string Destination = string.Empty;
}
