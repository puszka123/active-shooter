using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Keys
{
    LEVEL, ROOM
}

public class Limit
{
    //public bool Not;
    //public int? NumberLimit;
    //public bool? BoolLimit;
    //public string StrLimit;

    public string LocationId;
    public Room FoundRoom;
    public GameObject DoorToOpen;
    public ChatRoom chatRoom;
}

//public class Condition {
//    public Dictionary<Keys, Limit> Limits;

//    public Condition(List<DictionaryEntry> limits)
//    {
//        Limits = new Dictionary<Keys, Limit>();
//        try
//        {
//            foreach (var item in limits)
//            {
//                Limits.Add((Keys)item.Key, (Limit)item.Value);
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.Log("Adding to dictionary failed");
//            throw e;
//        }
//    }

//    public bool ConditionMet(PersonMemory memory)
//    {
//        foreach (var item in Limits)
//        {
//            bool[] limits = new bool[3];
//            limits[0] = CheckNumber(item.Key, item.Value, memory);
//            limits[1] = CheckBoolean(item.Key, item.Value, memory);
//            limits[2] = CheckString(item.Key, item.Value, memory);

//            foreach (var l in limits)
//            {
//                if (!l) return false;
//            }
//        }
//        return true;
//    }

//    bool CheckNumber(Keys key, Limit limit, PersonMemory memory)
//    {
//        if (!limit.NumberLimit.HasValue) return true;

//        switch (key)
//        {
//            case Keys.LEVEL:
//                return limit.Not ? !(memory.CurrentFloor == limit.NumberLimit) : (memory.CurrentFloor == limit.NumberLimit);
//            case Keys.ROOM:
//                break;
//            default:
//                break;
//        }
//        return true;
//    }

//    bool CheckString(Keys key, Limit limit, PersonMemory memory)
//    {
//        if (limit.StrLimit == null) return true;

//        switch (key)
//        {
//            case Keys.LEVEL:
//                break;
//            case Keys.ROOM:
//                return limit.Not ? !(memory.CurrentRoom.Id == limit.StrLimit) : (memory.CurrentRoom.Id == limit.StrLimit);
//            default:
//                break;
//        }
//        return true;
//    }

//    bool CheckBoolean(Keys key, Limit limit, PersonMemory memory)
//    {
//        if (!limit.BoolLimit.HasValue) return true;

//        switch (key)
//        {
//            case Keys.LEVEL:
//                break;
//            case Keys.ROOM:
//                return (memory.CurrentRoom != null) == limit.BoolLimit;
//            default:
//                break;
//        }
//        return true;
//    }
//}
