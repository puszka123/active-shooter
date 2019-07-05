using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ChatRequest
{
    OPEN_DOOR
}

public enum ChatResponse
{
    OK, NO
}

public class ChatRoom {
    public static int IdGenerator = 0;
    public int Id;

    List<GameObject> members;

    Negotiation doorOpenNegotiation;

    public List<GameObject> Members
    {
        get
        {
            return new List<GameObject>(Members);
        }
    }


    public ChatRoom()
    {
        Id = ++IdGenerator;
        members = new List<GameObject>();
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

    public void SendResponse(ChatResponse response, GameObject sender, GameObject receiver)
    {
        object[] responseParams = new object[] { this, response, sender };
        receiver.SendMessage("ChatResponse", responseParams);
    }

    public void SelectMemberToOpenDoor(GameObject sender, GameObject door)
    {
        if(doorOpenNegotiation == null)
        {
            doorOpenNegotiation = new Negotiation();
            GameObject memberToOpenDoor = doorOpenNegotiation.PersonToOpenDoor(door, members.Where(m => m.name != sender.name).ToArray());
            memberToOpenDoor.SendMessage("OpenDoor", door);
        }
    }
}
