using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyChat : MonoBehaviour {
    public GameObject CurrentSender;
    public List<ChatRoom> MyChatRooms;
    public ChatRequest RequestToProcess;

	// Update is called once per frame
	void Update () {
		
	}

    public void JoinChatRoom(ChatRoom chatRoom)
    {
        AddChatRoom(chatRoom);
        chatRoom.AddMember(gameObject);
    }

    public void AddChatRoom(ChatRoom chatRoom)
    {
        if(MyChatRooms == null)
        {
            MyChatRooms = new List<ChatRoom>() { chatRoom };
        }
        else
        {
            MyChatRooms.Add(chatRoom);
        }
    }

    public void ChatRequest(object[] args) //chatRoom, request, sender
    {
        ChatRoom chatRoom = (ChatRoom)args[0];
        ChatRequest request = (ChatRequest)args[1];
        GameObject sender = (GameObject)args[2];
        object param = args[3];

        HandleRequest(request, chatRoom, sender, param);
    }

    public void ChatResponse(object[] args) //chatRoom, response, sender
    {
        ChatRoom chatRoom = (ChatRoom)args[0];
        ChatResponse response = (ChatResponse)args[1];
        GameObject sender = (GameObject)args[2];
        ChatRequest request = (ChatRequest)args[3];
        HandleResponse(response, chatRoom, sender, request);
    }

    public void HandleRequest(ChatRequest request, ChatRoom chatRoom, GameObject sender, object param)
    {
        switch (request)
        {
            case global::ChatRequest.OPEN_DOOR:
                chatRoom.SelectMemberToOpenDoor(sender, (GameObject)param);
                break;
            case global::ChatRequest.CLOSE_DOOR:
                chatRoom.SelectMemberToCloseDoor(sender, (GameObject)param);
                break;
            case global::ChatRequest.INFO_ABOUT_SHOOTER:
                ShooterInfo shooterInfo = (ShooterInfo)param;
                Debug.Log(shooterInfo.Name + " " + gameObject);
                break;
            default:
                break;
        }
    }

    public void HandleResponse(ChatResponse response, ChatRoom chatRoom, GameObject sender, ChatRequest request)
    {
        switch (response)
        {
            case global::ChatResponse.OK:
                HandleOK(request);
                break;
            case global::ChatResponse.NO:
                break;
            default:
                break;
        }
    }

    public void HandleOK(ChatRequest request)
    {
        switch (request)
        {
            case global::ChatRequest.OPEN_DOOR:
                GetComponent<Person>().doorExecutor.DoorWillOpen();
                break;
            default:
                break;
        }
    }

    public void InformAboutShooter(ShooterInfo shooterInfo)
    {
        PersonMemory memory = GetComponent<Person>().PersonMemory;
        if (memory.UpdateActiveShooterInfo(shooterInfo)) //the employee has currently known about shooter 
        {
            Person person = transform.GetComponent<Person>();
            person.FirstDecision = true;
            //memory.UpdateNodesBlockedByShooter();
            //person.SelectBehaviour();
        }
    }
}
