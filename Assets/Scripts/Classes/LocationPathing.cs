using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

// Containts the pathing of a character and a generator/constructor of this pathing.
// TODO -> Accept multiple locations to ban.
public static class LocationPathing 
{

    /*  Generate Pathing:
     *           Randomly generates a pathing between locations and time that makes logical sense.
     *  locationList: The available locations in the map.
     *  size: The amount of locations visited that night.
     *  mustIncludeLocation: a location they MUST visit.
     *  locationPosition: the order this location was visited. (first VS. last).
     *  newTimeRange: a time range that MUST be included.
     *  mustAvoidLocationIndex: if applicable, DO NOT visit this location.
     *  mustAvoidTimeRangeIndex: if applicable, DO NOT VISIT anywhere while in this hour.
     * */
    public static Dictionary<TimeRange, Location> GeneratePathing(List<Location> locationList, int size, Location mustIncludeLocation = null,
        TimeRange mustIncludeTimeRange = null, int mustAvoidLocationIndex = -1, int mustAvoidTimeRangeIndex = -1)
    {
        Dictionary<TimeRange, Location> P_LocationPath = new();
        List<int> possibleLocations = new(), possibleTimes = new();

        TimeRange newTimeRange = null;
        if (mustIncludeTimeRange != null)
        {
            newTimeRange = new(mustIncludeTimeRange.currentTime);
        }
        
        bool banLowerTimeRanges = false, banUpperTimeRanges = false;
        int locationPosition = -1;
        // Defining location position.
        if (newTimeRange != null)
        {
            int possibleLowerFloor = 0;
            int possibleUpperFloor = size;
            if ((size - 1) > (int)( TimeRange.AvailableTimes.FOUR_HALF - newTimeRange.currentTime))
            {
                possibleLowerFloor += (size - 1) - (int)(TimeRange.AvailableTimes.FOUR_HALF - newTimeRange.currentTime);
            }
            if (size > ((int) newTimeRange.currentTime) + 1)
            {
                possibleUpperFloor = (int) newTimeRange.currentTime + 1;
            }
            locationPosition = Random.Range(possibleLowerFloor, possibleUpperFloor);
        }
        // Available choices for Location.
        for (int i = 0; i < locationList.Count; i++)
        {
            if (i != mustAvoidLocationIndex)
            {
                possibleLocations.Add(i);
            }
        }
        //Available choices for TimeRange.
        if ((mustAvoidTimeRangeIndex - 1) < size - 1)
        {
            //Cannot allow time ranges lower than this.
            banLowerTimeRanges = true;
        }
        if (((int)TimeRange.AvailableTimes.FOUR_HALF - (mustAvoidTimeRangeIndex + 1)) < size - 1)
        {
            //Cannoit allow time ranges higher than this.
            banUpperTimeRanges = true;
        }
        if (banLowerTimeRanges && banUpperTimeRanges)
        {
            Debug.LogWarning("In Location pathing, avoiding the banned time range is impossible. Ignoring request.");
            banLowerTimeRanges = false;
            banUpperTimeRanges = false;
            mustAvoidTimeRangeIndex = -1;
        }
        for( int i = 0; i < ((int)TimeRange.AvailableTimes.FOUR_HALF); i++)
        {
            
            if (i != mustAvoidTimeRangeIndex && !banLowerTimeRanges && !banUpperTimeRanges)
            {
                possibleTimes.Add(i);
            }
            else if (i > mustAvoidTimeRangeIndex && banLowerTimeRanges)
            {
                possibleTimes.Add(i);
            }
            else if (i < mustAvoidTimeRangeIndex && banUpperTimeRanges)
            {
                possibleTimes.Add(i);
            }
        }
        if (mustIncludeLocation == null && newTimeRange != null)
        {
            int randomLocationIndex = possibleLocations[Random.Range(0, possibleLocations.Count)];
            P_LocationPath.Add(newTimeRange, locationList[randomLocationIndex]);
            if (mustAvoidLocationIndex == -1)
            {
                P_LocationPath = PopulateList(P_LocationPath, locationList[randomLocationIndex], newTimeRange, size, locationPosition
                                , null);
            }
            else 
            {
                P_LocationPath = PopulateList(P_LocationPath, locationList[randomLocationIndex], newTimeRange, size, locationPosition
                , locationList[mustAvoidLocationIndex]);
            }
        }
        else if (mustIncludeLocation != null && newTimeRange == null)
        {
            //Generates a valid time fringe.
            int randomTimeFringeIndex = possibleTimes[Random.Range(0, possibleTimes.Count)];
            TimeRange startingTime = new((TimeRange.AvailableTimes)randomTimeFringeIndex);
            P_LocationPath.Add(startingTime, mustIncludeLocation);
            Location toAvoid;
            if (mustAvoidLocationIndex == -1)
            {
                toAvoid = null;
            }
            else
            {
                toAvoid = locationList[mustAvoidLocationIndex];
            }
            if (randomTimeFringeIndex > mustAvoidTimeRangeIndex)
            {

                P_LocationPath = PopulateListRandomIndex(P_LocationPath, mustIncludeLocation, startingTime, size, toAvoid, 1);
                
            }
            if (randomTimeFringeIndex < mustAvoidTimeRangeIndex)
            {
                P_LocationPath = PopulateListRandomIndex(P_LocationPath, mustIncludeLocation, startingTime, size, toAvoid, - 1);
            }
        }
        else if (mustIncludeLocation != null && newTimeRange != null)
        {
            P_LocationPath.Add(newTimeRange, mustIncludeLocation);
            if (mustAvoidLocationIndex == -1)
            {
                P_LocationPath = PopulateList(P_LocationPath, mustIncludeLocation, newTimeRange, size, locationPosition , null);
            }
            else
            {
                P_LocationPath = PopulateList(P_LocationPath, mustIncludeLocation, newTimeRange, size, locationPosition, locationList[mustAvoidLocationIndex]);
            }
        }
        else
        {
            int randomLocationIndex = possibleLocations[Random.Range(0, possibleLocations.Count)];
            int randomTimeFringeIndex = possibleTimes[Random.Range(0, possibleTimes.Count)];
            TimeRange startingTime = new((TimeRange.AvailableTimes)randomTimeFringeIndex);
            Location toAvoid;
            P_LocationPath.Add(startingTime, locationList[randomLocationIndex]);
            if (mustAvoidLocationIndex == -1)
            {
                toAvoid = null;
            }
            else
            {
                toAvoid = locationList[mustAvoidLocationIndex];
            }
            if (randomTimeFringeIndex > mustAvoidTimeRangeIndex)
            {

                P_LocationPath = PopulateListRandomIndex(P_LocationPath, locationList[randomLocationIndex], startingTime, size, toAvoid, 1);

            }
            if (randomTimeFringeIndex < mustAvoidTimeRangeIndex)
            {
                P_LocationPath = PopulateListRandomIndex(P_LocationPath, locationList[randomLocationIndex], startingTime, size, toAvoid, -1);
            }
        }
        return P_LocationPath;
        
    }
    //Populates the rest of the list depending on the requiered visits. Already considers an initial reference.
    //      Follows only the position of the visited location as a starting index.
    private static Dictionary<TimeRange, Location> PopulateList(Dictionary<TimeRange, Location> P_LocationPath,
        Location locationReference, TimeRange timeReference, int size, int StartingIndex, Location mustAvoidLocation, int modifier = 1)
    {
        int currentIndex = StartingIndex + 1;
        TimeRange timeAux = timeReference;
        Location locationAux = locationReference;
        List<int> validNeighors = new();
        for (int i = 0; i < size - 1; i++)
        {
            if (currentIndex >= size)
            {
                currentIndex = StartingIndex - 1;
                modifier *= -1;
                timeAux = timeReference;
                locationAux = locationReference;
            }
            timeAux = new TimeRange(timeAux.currentTime + modifier);
            if (mustAvoidLocation == null)
            {
                locationAux = locationAux.GetNeighbor(Random.Range(0, locationReference.P_Locations.Count));
            }
            else
            {
                for(int j = 0; j < locationAux.P_Locations.Count; j++)
                {
                    if (!locationAux.P_Locations[j].P_Location_Name.Equals(mustAvoidLocation.P_Location_Name))
                    {
                        validNeighors.Add(j);
                    }
                }
                locationAux = locationAux.GetNeighbor(validNeighors[Random.Range(0, validNeighors.Count)]);
            }
            P_LocationPath.Add(timeAux, locationAux);
            currentIndex += modifier;
            validNeighors.Clear();
        }
        return P_LocationPath;
        
    }
    //Populates the rest of the list depending on the requiered visits. Already considers an initial reference.
    //      Doesn't care about positions, just about limits.
    private static Dictionary<TimeRange, Location> PopulateListRandomIndex(Dictionary<TimeRange, Location> P_LocationPath,
        Location locationReference, TimeRange timeReference, int size, Location mustAvoidLocation
        , int modifier = 1)
    {
        TimeRange timeAux = timeReference;
        Location locationAux = locationReference;
        List<int> validNeighors = new();
        for (int i = 0; i < size - 1; i++)
        {
            if ((timeAux.currentTime == TimeRange.AvailableTimes.FOUR_HALF && modifier == 1)
                || (timeAux.currentTime == TimeRange.AvailableTimes.TWELVE && modifier == -1))
            {
                modifier *= -1;
                timeAux = timeReference;
                locationAux = locationReference;
            }
            timeAux = new TimeRange(timeAux.currentTime + modifier);
            if (mustAvoidLocation == null)
            {
                locationAux = locationAux.GetNeighbor(Random.Range(0, locationReference.P_Locations.Count));
            }
            else
            {
                for (int j = 0; j < locationAux.P_Locations.Count; j++)
                {
                    if (!locationAux.P_Locations[j].P_Location_Name.Equals(mustAvoidLocation.P_Location_Name))
                    {
                        validNeighors.Add(j);
                    }
                }
                locationAux = locationAux.GetNeighbor(validNeighors[Random.Range(0, validNeighors.Count)]);
            }
            P_LocationPath.Add(timeAux, locationAux);
            validNeighors.Clear();
        }
        return P_LocationPath;
    }
}
