using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : Evidence
{
    public string P_Location_Name { get; private set; }
    public string P_Description { get; private set;  }

    public List<Location> P_Locations;   

    public Location(string location_Name, string description)
    {
        P_Location_Name = location_Name;
        P_Description = description;
        P_Locations = new();
    }
    public void AddNeighborLocation(Location neighbor)
    {
        P_Locations.Add(neighbor);
    }
    public Location GetNeighbor(int index)
    {
        if (index >= P_Locations.Count)
        {
            Debug.LogWarning("Neighbour Locations index is out of range!");
            return null;
        }
        return P_Locations[index];
    }
    public override string EvidenceToString()
    {
        return P_Location_Name;
    }
} 
