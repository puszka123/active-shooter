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

    ChatRoom chatRoom;
    ChatRoomManager chatRoomManager;

    public DoorExecutor(PersonDoor personDoor, MyChat myChat, GameObject me)
    {
        PersonDoor = personDoor;
        MyChat = myChat;
        Me = me;
        chatRoomManager = GameObject.FindGameObjectWithTag("ChatRoomManager").GetComponent<ChatRoomManager>();
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
                Room room = Utils.GetRoom(action);
                if (room == null || Utils.ToFar(room.Door, Me, 2)) //null or to far from door
                {
                    FinishKnockAction();
                    return;
                }
                chatRoom = chatRoomManager.CreateChatRoom(this);
                foreach (var employee in room.Employees)
                {
                    chatRoom.InviteMember(employee);
                }
                chatRoom.InviteMember(Me);
                chatRoom.SendRequest(ChatRequest.OPEN_DOOR, Me, room.Door);
                break;
            case Command.OPEN_DOOR:
                GameObject door2 = Utils.GetDoor(action);
                if (door2 == null || Utils.ToFar(door2, Me, 2)) //null or to far from door
                {
                    FinishKnockAction();
                    return;
                }
                PersonDoor.TryToOpenDoor(door2);
                break;
            case Command.CLOSE_DOOR:
                GameObject door1 = Utils.GetDoor(action);
                if (door1 == null || Utils.ToFar(door1, Me, 2)) //null or to far from door
                {
                    FinishKnockAction();
                    return;
                }
                PersonDoor.TryToCloseDoor(door1);
                break;
            case Command.ASK_CLOSE_DOOR:
                chatRoom.SendRequest(ChatRequest.CLOSE_DOOR, Me, Utils.GetRoom(action).Door);
                FinishAskCloseDoor();
                break;
        }
    }

    public bool IsActionExecuting(Action action)
    {
        return (ActionToExecute != null && action.Command == ActionToExecute.Command && Executing);
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

    public void CheckDoor(Action action)
    {
        if (ActionToExecute == null) return;
        GameObject door = null;
        switch (ActionToExecute.Command)
        {
            case Command.KNOCK:
                door = Utils.GetRoom(action)?.Door;
                if (door == null) return;
                KnockCheck(door);
                break;
            case Command.OPEN_DOOR:
                door = Utils.GetDoor(action);
                if (door == null) return;
                OpenDoorCheck(door);
                break;
        }
    }

    private void KnockCheck(GameObject door)
    {
        if (WaitForOpenDoor
            && !door.GetComponent<DoorController>().IsOpen
            && timer >= MaxTimeWait)
        {
            FinishKnockAction();
        }
        if (door.GetComponent<DoorController>().IsOpen)
        {
            FinishKnockAction();
        }

        if (timer >= MaxTimeWait)
        {
            FinishKnockAction();
        }
    }

    private void OpenDoorCheck(GameObject door)
    {
        if(door.GetComponent<DoorController>().IsOpen)
        {
            FinishDoorOpenAction();
        }
    }

    public void FinishKnockAction()
    {
        Me.GetComponent<Person>().PersonMemory.AddInformedRoom(Utils.GetRoom(ActionToExecute));
        timer = 0;
        WaitForOpenDoor = false;
        Executing = false;
        //close chat room related to this action
        ActionToExecute.IsDone = true;
    }

    public void FinishDoorOpenAction()
    {
        timer = 0;
        Executing = false;
        //close chat room related to this action
        ActionToExecute.IsDone = true;
    }

    public void FinishAskCloseDoor()
    {
        Executing = false;
        //close chat room related to this action
        ActionToExecute.IsDone = true;
    }
}
