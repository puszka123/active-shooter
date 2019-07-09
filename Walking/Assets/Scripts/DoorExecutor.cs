using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorExecutor {
    public PersonDoor PersonDoor;
    public bool Executing;
    public Task TaskToExecute;
    public MyChat MyChat;
    public GameObject Me;
    public bool WaitForOpenDoor;
    public float timer = 0.0f;
    public float MaxTimeWait = 5f;

    ChatRoom chatRoom;
    ChatRoomManager chatRoomManager;

    public DoorExecutor(PersonDoor personDoor, MyChat myChat, GameObject me)
    {
        PersonDoor = personDoor;
        MyChat = myChat;
        Me = me;
        chatRoomManager = GameObject.FindGameObjectWithTag("ChatRoomManager").GetComponent<ChatRoomManager>();
    }

    public void ExecuteTask(Task task, Transform transform)
    {
        if (IsTaskExecuting(task)) return; //don't do that again!
        if (task.IsDone) return;
        Executing = true;
        TaskToExecute = task;
        switch (task.Command)
        {
            case Command.KNOCK:
                timer = 0.0f;
                WaitForOpenDoor = true;
                Room room = Utils.GetRoom(task);
                if (room == null || room.Door == null || Utils.ToFar(room.Door, Me, 2)) //null or to far from door
                {
                    FinishKnockTask();
                    return;
                }
                chatRoom = chatRoomManager.CreateChatRoom(this);
                foreach (var employee in room.Employees)
                {
                    chatRoom.InviteMember(employee);
                }
                chatRoom.InviteMember(Me);
                if (Utils.DoorIsLocked(room.Door))
                {
                    chatRoom.SendRequest(ChatRequest.OPEN_DOOR, Me, room.Door);
                }
                else
                {
                    FinishKnockTask();
                }
                break;
            case Command.OPEN_DOOR:
                GameObject door2 = Utils.GetDoor(task);
                if (door2 == null || Utils.ToFar(door2, Me, 2)) //null or to far from door
                {
                    FinishDoorOpenTask();
                    return;
                }
                PersonDoor.TryToOpenDoor(door2);
                break;
            case Command.CLOSE_DOOR:
                GameObject door1 = Utils.GetDoor(task);
                if (door1 == null || Utils.ToFar(door1, Me, 2)) //null or to far from door
                {
                    FinishDoorCloseTask();
                    return;
                }
                PersonDoor.TryToCloseDoor(door1);
                break;
            case Command.ASK_CLOSE_DOOR:
                chatRoom.SendRequest(ChatRequest.CLOSE_DOOR, Me, Utils.GetRoom(task).Door);
                FinishAskCloseDoor();
                break;
            case Command.LOCK_DOOR:
                GameObject door3 = Utils.GetDoor(task);
                if (door3 == null || Utils.ToFar(door3, Me, 2)) //null or to far from door
                {
                    FinishDoorLockTask();
                    return;
                }
                PersonDoor.TryToLockDoor(door3);
                break;

        }
    }

    public bool IsTaskExecuting(Task task)
    {
        return (TaskToExecute != null && task.Command == TaskToExecute.Command && Executing);
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

    public void CheckDoor(Task task)
    {
        if (TaskToExecute == null) return;
        GameObject door = null;
        switch (TaskToExecute.Command)
        {
            case Command.KNOCK:
                door = Utils.GetRoom(task)?.Door;
                if (door == null) return;
                KnockCheck(door);
                break;
            case Command.OPEN_DOOR:
                door = Utils.GetDoor(task);
                if (door == null) return;
                OpenDoorCheck(door);
                break;
            case Command.CLOSE_DOOR:
                door = Utils.GetDoor(task);
                if (door == null) return;
                CloseDoorCheck(door);
                break;
            case Command.LOCK_DOOR:
                door = Utils.GetDoor(task);
                if (door == null) return;
                LockDoorCheck(door);
                break;
        }
    }

    private void KnockCheck(GameObject door)
    {
        if (WaitForOpenDoor
            && Utils.DoorIsLocked(door)
            && timer >= MaxTimeWait)
        {
            FinishKnockTask();
        }
        if (!Utils.DoorIsLocked(door))
        {
            FinishKnockTask();
        }

        if (timer >= MaxTimeWait)
        {
            FinishKnockTask();
        }
    }

    private void OpenDoorCheck(GameObject door)
    {
        if(door.GetComponent<DoorController>().IsOpen)
        {
            FinishDoorOpenTask();
        }
    }

    private void CloseDoorCheck(GameObject door)
    {
        if (!door.GetComponent<DoorController>().IsOpen)
        {
            FinishDoorCloseTask();
        }
    }

    private void LockDoorCheck(GameObject door)
    {
        if (door.GetComponent<DoorController>().IsLocked)
        {
            FinishDoorLockTask();
        }
    }

    public void FinishKnockTask()
    {
        Me.GetComponent<Person>().PersonMemory.AddInformedRoom(Utils.GetRoom(TaskToExecute));
        timer = 0;
        WaitForOpenDoor = false;
        Executing = false;
        //close chat room related to this task
        TaskToExecute.IsDone = true;
    }

    public void FinishDoorOpenTask()
    {
        timer = 0;
        Executing = false;
        //close chat room related to this task
        TaskToExecute.IsDone = true;
    }

    public void FinishDoorCloseTask()
    {
        timer = 0;
        Executing = false;
        //close chat room related to this task
        TaskToExecute.IsDone = true;
    }

    public void FinishDoorLockTask()
    {
        timer = 0;
        Executing = false;
        //close chat room related to this task
        TaskToExecute.IsDone = true;
    }

    public void FinishAskCloseDoor()
    {
        Executing = false;
        //close chat room related to this task
        TaskToExecute.IsDone = true;
    }
}
