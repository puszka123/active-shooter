using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatRoomManager : MonoBehaviour {
    Dictionary<int, ChatRoom> ChatRooms;

    public ChatRoom CreateChatRoom()
    {
        ChatRoom chatRoom = new ChatRoom();
        AddChatRoom(chatRoom);
        return chatRoom;
    }

    public void AddChatRoom(ChatRoom chatRoom)
    {
        if (ChatRooms == null)
        {
            ChatRooms = new Dictionary<int, ChatRoom>();
        }
        ChatRooms.Add(chatRoom.Id, chatRoom);
    } 

}
