using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatRoomManager : MonoBehaviour {
    Dictionary<int, ChatRoom> ChatRooms;

    public ChatRoom CreateChatRoom(object initializer)
    {
        ChatRoom chatRoom = new ChatRoom(initializer);
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

    private void FixedUpdate()
    {
        if (ChatRooms != null)
        {
            UpdateChatRoomsTimers(Time.deltaTime);
        }
    }

    public void UpdateChatRoomsTimers(float time)
    {
        foreach (var item in ChatRooms)
        {
            item.Value.UpdateTimer(time);
        }
    }

}
