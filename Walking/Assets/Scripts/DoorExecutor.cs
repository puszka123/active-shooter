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
    public bool WaitForOpenDoor;
    public float timer = 0.0f;
    public float MaxTimeWait = 3f;

    public DoorExecutor(PersonDoor personDoor, MyChat myChat, GameObject me)
    {
        PersonDoor = personDoor;
        MyChat = myChat;
        Me = me;
    }

    public void ExecuteAction(Action action, Transform transform)
    {
        if (IsActionExecuting(action)) return; //don't do that again!
        if (action.IsDone) return;
        Executing = true;
        ActionToExecute = action;
        switch (action.Command)
        {
            case Command.KNOCK:
                timer = 0.0f;
                WaitForOpenDoor = true;
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
                    Where(r => r != null).ToArray()?[0];
        return room;
    }

    public void DoorWillOpen()
    {
        timer = 0.0f;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (WaitForOpenDoor)
        {
            timer += deltaTime;
        }
    }

    public void CheckDoor(GameObject door)
    {
        if (door == null) return;
        if(WaitForOpenDoor 
            && !door.GetComponent<DoorController>().IsOpen 
            && timer >= MaxTimeWait)
        {
            FinishKnockAction();
        }
        if(door.GetComponent<DoorController>().IsOpen)
        {
            FinishKnockAction();
        }

        if (timer >= MaxTimeWait)
        {
            FinishKnockAction();
        }
    }

    public void FinishKnockAction()
    {
        //Debug.Log(timer);
        Me.GetComponent<Person>().PersonMemory.AddInformedRoom(GetRoom(ActionToExecute));
        timer = 0;
        WaitForOpenDoor = false;
        Executing = false;
        Me.GetComponent<Person>().PersonMemory.ClearFoundRoom();
        //close chat room related to this action
        ActionToExecute.IsDone = true;
    }
}
