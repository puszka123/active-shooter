﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyChat : MonoBehaviour {
    public GameObject CurrentSender;
    public List<ChatRoom> MyChatRooms;

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
    }

    public void ChatResponse(object[] args) //chatRoom, response, sender
    {
        ChatRoom chatRoom = (ChatRoom)args[0];
        ChatResponse response = (ChatResponse)args[1];
        GameObject sender = (GameObject)args[2];
    }

    public void HandleRequest(ChatRequest request, ChatRoom chatRoom, GameObject sender)
    {
        switch (request)
        {
            case global::ChatRequest.OPEN_DOOR:
                break;
            default:
                break;
        }
    }

    public void HandleResponse(ChatResponse response, ChatRoom chatRoom, GameObject sender)
    {
        switch (response)
        {
            case global::ChatResponse.OK:
                break;
            case global::ChatResponse.NO:
                break;
            default:
                break;
        }
    }
}
