using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRange : Evidence
{
    public enum AvailableTimes { TWELVE, TWELVE_HALF, ONE, ONE_HALF, TWO, TWO_HALF, THREE, THREE_HALF, FOUR, FOUR_HALF };
    public AvailableTimes currentTime;
    private int minutes;
    public TimeRange(AvailableTimes currentTime, int bannedTime = -1)
    {
        this.currentTime = currentTime;
        GenerateMinutes(bannedTime);
    }
    private void GenerateMinutes(int bannedTime)
    {
        switch (currentTime)
        {
            case (AvailableTimes.TWELVE):
            case (AvailableTimes.ONE):
            case (AvailableTimes.TWO):
            case (AvailableTimes.THREE):
            case (AvailableTimes.FOUR):
                minutes = Random.Range(10, 21);
                if (bannedTime != -1 && (minutes >= bannedTime - 5 && minutes < bannedTime + 5))
                {
                    int toChange = 5;
                    if (minutes <= bannedTime)
                    {
                        toChange *= -1;
                    }
                    minutes += toChange;
                    if (minutes < 0)
                    {
                        minutes += 30;
                    }
                    if (minutes >= 30)
                    {
                        minutes -= 30;
                    }
                }
                break;
            case (AvailableTimes.TWELVE_HALF):
            case (AvailableTimes.ONE_HALF):
            case (AvailableTimes.TWO_HALF):
            case (AvailableTimes.THREE_HALF):
            case (AvailableTimes.FOUR_HALF):
                minutes = Random.Range(40, 51);
                if (bannedTime != -1 && (minutes >= bannedTime - 5 && minutes < bannedTime + 5))
                {
                    int toChange = 5;
                    if (minutes <= bannedTime)
                    {
                        toChange *= -1;
                    }
                    minutes += toChange;
                    if (minutes < 30)
                    {
                        minutes += 30;
                    }
                    if (minutes >= 60)
                    {
                        minutes -= 30;
                    }
                }
                break;
            default:
                Debug.LogWarning("Time Range out of Range when assigning minutes");
                break;

        }
    }
    public int GetHour()
    {
        switch (currentTime)
        {
            case (AvailableTimes.TWELVE):
            case (AvailableTimes.TWELVE_HALF):
                return 12;
            case (AvailableTimes.ONE_HALF):
            case (AvailableTimes.ONE):
                return 1;
            case (AvailableTimes.TWO_HALF):
            case (AvailableTimes.TWO):
                return 2;
            case (AvailableTimes.THREE_HALF):
            case (AvailableTimes.THREE):
                return 3;
            case (AvailableTimes.FOUR_HALF):
            case (AvailableTimes.FOUR):
                return 4;
            default:
                Debug.LogWarning("Time Range out of Range when getting hours");
                return -1;

        }
    }
    public static int GetHourStatic(AvailableTimes times)
    {
        switch (times)
        {
            case (AvailableTimes.TWELVE):
            case (AvailableTimes.TWELVE_HALF):
                return 12;
            case (AvailableTimes.ONE_HALF):
            case (AvailableTimes.ONE):
                return 1;
            case (AvailableTimes.TWO_HALF):
            case (AvailableTimes.TWO):
                return 2;
            case (AvailableTimes.THREE_HALF):
            case (AvailableTimes.THREE):
                return 3;
            case (AvailableTimes.FOUR_HALF):
            case (AvailableTimes.FOUR):
                return 4;
            default:
                Debug.LogWarning("Time Range out of Range when getting hours");
                return -1;

        }
    }
    public int GetMinutes()
    {
        return minutes;
    }
    public override string EvidenceToString()
    {
        return GetHour() + ":" + GetMinutes();
    }
    public string EvidenceToStringRounded()
    {
        int minutes = GetMinutes();
        int hour = GetHour();
        if (minutes >= 55)
        {
            hour++;
        }
        minutes = ((int)Mathf.Round(minutes / 10)) * 10;
        if (minutes < 10)
        {
            return hour + ":" + minutes + "0";
        }
        return hour + ":" + minutes;
    }
    public static string TimeRangeToString(AvailableTimes times)
    {
        bool isSecondHalf = false;
        int currHour = GetHourStatic(times);
        switch (times)
        {
            case (AvailableTimes.TWELVE_HALF):
            case (AvailableTimes.ONE_HALF):
            case (AvailableTimes.TWO_HALF):
            case (AvailableTimes.THREE_HALF):
            case (AvailableTimes.FOUR_HALF):
                isSecondHalf = true;
                break;
        }
        if (isSecondHalf)
        {
            return currHour + ":30" + " - " + (currHour + 1) + ":00"; 
        }
        else
        {
            return currHour + ":00" + " - " + currHour + ":30";
        }
    }

}
