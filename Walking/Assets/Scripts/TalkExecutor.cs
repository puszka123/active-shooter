using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TalkExecutor {
    public Person Person;
    ChatRoom chatRoom;
    ChatRoomManager chatRoomManager;
    public bool Executing;
    public Action ActionToExecute;

    public float talkingTime = 10f;
    public float time = 0f;
    public float waitForOthers = 1f;

    public TalkExecutor(Person person)
    {
        Person = person;
        chatRoomManager = GameObject.FindGameObjectWithTag("ChatRoomManager").GetComponent<ChatRoomManager>();
    }

    public void ExecuteAction(Action action)
    {
        if (IsActionExecuting(action)) return; //don't do that again!
        if (action.IsDone) return;
        Executing = true;
        ActionToExecute = action;
        switch (action.Command)
        {
            case Command.TELL_ABOUT_SHOOTER:
                InitChat();
                SendInfoAboutShooter();
                PretendTalking();
                break;
        }
    }

    public void UpdateTalkingTimer(float deltaTime)
    {
        if (Executing)
        {
            time += deltaTime;
        }
    }

    public void CheckTalking()
    {
        if(time >= talkingTime)
        {
            FinishTalk();
        }
    }

    public void FinishTalk()
    {
        Executing = false;
        ActionToExecute.IsDone = true;
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

    public void InitChat()
    {
        Room room = GetRoom(ActionToExecute);
        if (room == null) return;
        chatRoom = chatRoomManager.CreateChatRoom();
        chatRoom.InviteMembers(room.Employees);
        chatRoom.InviteMember(Person.gameObject);
    }

    public void SendInfoAboutShooter()
    {
        chatRoom.SendRequest(ChatRequest.INFO_ABOUT_SHOOTER, Person.gameObject, Person.PersonMemory.ShooterInfo);
    }

    public void PretendTalking()
    {
        time = 0f;
    }
}
