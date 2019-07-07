using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ChatRequest
{
    OPEN_DOOR, CLOSE_DOOR, INFO_ABOUT_SHOOTER
}

public enum ChatResponse
{
    OK, NO
}

public class ChatRoom {
    public static int IdGenerator = 0;
    public int Id;
    public float Timer = 0f;
    public float TimeThreshold = 0f;
    public object Initializer;
    public bool IsInitiated;

    List<GameObject> members;

    Negotiation doorOpenNegotiation;
    Negotiation doorCloseNegotiation;

    public List<GameObject> Members
    {
        get
        {
            return new List<GameObject>(Members);
        }
    }


    public ChatRoom(object initializer, float timerThreshold = 1.0f)
    {
        Id = ++IdGenerator;
        members = new List<GameObject>();
        TimeThreshold = timerThreshold;
        Timer = 0f;
        Initializer = initializer;
        IsInitiated = false;
    }

    public void AddMember(GameObject member)
    {
        members.Add(member);
    }

    public void ClearMembers()
    {
        members = new List<GameObject>();
    }

    public void InviteMember(GameObject member)
    {
        member.SendMessage("JoinChatRoom", this);
    }

    public void InviteMembers(IEnumerable<GameObject> members)
    {
        foreach (var member in members)
        {
            InviteMember(member);
        }
    }

    public void SendRequest(ChatRequest request, GameObject sender, object param = null)
    {
        //Debug.Log(request + " sender: " + sender);
        foreach (var member in GetOtherMembers(sender))
        {
            object[] requestParams = new object[] { this, request, sender, param };
            member.SendMessage("ChatRequest", requestParams);
        }
    }

    public GameObject[] GetOtherMembers(GameObject sender)
    {
        return members.Where(member => member.name != sender.name).ToArray();
    }

    public void SendResponse(ChatResponse response, GameObject sender, GameObject receiver, ChatRequest request)
    {
        object[] responseParams = new object[] { this, response, sender, request };
        receiver.SendMessage("ChatResponse", responseParams);
    }

    public void SelectMemberToOpenDoor(GameObject sender, GameObject door)
    {
        if(doorOpenNegotiation == null)
        {
            doorOpenNegotiation = new Negotiation();
            GameObject memberToOpenDoor = doorOpenNegotiation.PersonNearestDoor(door, members.Where(m => m.name != sender.name).ToArray());
            memberToOpenDoor.SendMessage("OpenDoor", door);
            SendResponse(ChatResponse.OK, memberToOpenDoor, sender, ChatRequest.OPEN_DOOR);
        }
    }

    public void SelectMemberToCloseDoor(GameObject sender, GameObject door)
    {
        if (doorCloseNegotiation == null)
        {
            doorCloseNegotiation = new Negotiation();
            GameObject memberToCloseDoor = doorCloseNegotiation.PersonNearestDoor(door, members.Where(m => m.name != sender.name).ToArray());
            memberToCloseDoor.SendMessage("CloseDoor", door);
            SendResponse(ChatResponse.OK, memberToCloseDoor, sender, ChatRequest.CLOSE_DOOR);
        }
    }

    public void UpdateTimer(float time)
    {
        Timer += time;
        CheckThreshold();
    }

    public void CheckThreshold()
    {
        if (Timer >= TimeThreshold && !IsInitiated)
        {
            IsInitiated = true;
            SendCanTalk();
        }
    }

    public void SendCanTalk()
    {
        if (Initializer.GetType() == typeof(TalkExecutor))
        {
            TalkExecutor talkExecutor = (TalkExecutor)Initializer;
            talkExecutor.CanTalk();
        }
    }

    public bool CanSendRequests()
    {
        return Timer >= TimeThreshold;
    }
}
