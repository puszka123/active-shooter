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

    public List<GameObject> Members
    {
        get
        {
            return new List<GameObject>(Members);
        }
    }


    public void InitChatRoom()
    {
        Id = ++IdGenerator;
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

    public void SendRequest(ChatRequest request, GameObject sender)
    {
        foreach (var member in GetOtherMembers(sender))
        {
            object[] requestParams = new object[] { this, request, sender };
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
}
