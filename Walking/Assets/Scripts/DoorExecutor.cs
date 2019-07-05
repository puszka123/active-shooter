using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorExecutor {
    public PersonDoor PersonDoor;
    public bool Executing;
    public Action ActionToExecute;
    public MyChat MyChat;
    public GameObject Me;

    public DoorExecutor(PersonDoor personDoor, MyChat myChat, GameObject me)
    {
        PersonDoor = personDoor;
        MyChat = myChat;
        Me = me;
    }

    public void ExecuteAction(Action action, Transform transform)
    {
        if (IsActionExecuting(action)) return; //don't do that again!
        Executing = true;
        ActionToExecute = action;
        switch (action.Command)
        {
            case Command.KNOCK:
                Room room = GetRoom(action);
                if (room == null) return;
                ChatRoomManager chatRoomManager = GameObject.FindGameObjectWithTag("ChatRoomManager").GetComponent<ChatRoomManager>();
                ChatRoom chatRoom = chatRoomManager.CreateChatRoom();
                foreach (var employee in room.Employees)
                {
                    chatRoom.InviteMember(employee);
                }
                chatRoom.InviteMember(Me);
                chatRoom.SendRequest(ChatRequest.OPEN_DOOR, Me, room.Door);
                //find people who have access to a room ok
                //create chat room with them ok
                //select a member to open the door
                //done
                break;
        }
    }

    public bool IsActionExecuting(Action action)
    {
        return (ActionToExecute != null && action.Command == ActionToExecute.Command && Executing);
    }

    public Room GetRoom(Action action)
    {
        Room room = action.Limits.
                    Select(limit => limit.FoundRoom).
                    Where(r => r != null).ToArray()[0];
        return room;
    }
}
