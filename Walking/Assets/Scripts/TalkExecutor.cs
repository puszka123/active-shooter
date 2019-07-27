using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TalkExecutor {
    public Person Person;
    ChatRoom chatRoom;
    ChatRoomManager chatRoomManager;
    public bool Executing;
    public Task TaskToExecute;

    public float talkingTime = 10f;
    public float time = 0f;
    public float waitForOthers = 1f;

    public TalkExecutor(Person person)
    {
        Person = person;
        chatRoomManager = GameObject.FindGameObjectWithTag("ChatRoomManager").GetComponent<ChatRoomManager>();
    }

    public void ExecuteTask(Task task)
    {
        if (IsTaskExecuting(task)) return; //don't do that again!
        if (task.IsDone) return;
        Executing = true;
        TaskToExecute = task;
        switch (task.Command)
        {
            case Command.TELL_ABOUT_SHOOTER:
                InitChat();
                break;
            case Command.SAY_ACTIVE_SHOOTER:
                if(Person.PersonMemory.ShooterInfo == null)
                {
                    FinishTalk();
                    return;
                }
                InformNeighbours();
                FinishTalk();
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

    public void CanTalk()
    {
        switch (TaskToExecute.Command)
        {
            case Command.TELL_ABOUT_SHOOTER:
                TellAboutShooter();
                break;
        }
    }

    public void TellAboutShooter()
    {
        Person.PersonMemory.AddInformedPeople(chatRoom.GetOtherMembers(Person.gameObject));
        SendInfoAboutShooter();
        PretendTalking();
    }

    public void FinishTalk()
    {
        time = 0f;
        Executing = false;
        TaskToExecute.IsDone = true;
    }

    public bool IsTaskExecuting(Task task)
    {
        return (TaskToExecute != null && task.Command == TaskToExecute.Command && Executing);
    }

    public void InitChat()
    {
        GameObject[] members = GetMembers();
        if (members == null)
        {
            FinishTalk();
            return;
        }
        if(TaskToExecute.Command == Command.TELL_ABOUT_SHOOTER && AlreadyInformed(members, Person.PersonMemory))
        {
            FinishTalk();
            return;
        }
        members = FilterMembers(members);
        chatRoom = chatRoomManager.CreateChatRoom(this);
        chatRoom.InviteMembers(members);
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

    public GameObject[] GetMembers()
    {
        GameObject[] members = null;
        Room room = Utils.GetRoom(TaskToExecute);
        if (room == null)
        {
            members = Utils.GetEmployees(TaskToExecute);
        }
        else
        {
            members = room.Employees;
        }
        return members;
    }

    public GameObject[] FilterMembers(GameObject[] members)
    {
        List<GameObject> filtered = new List<GameObject>();
        foreach (var member in members)
        {
            if(CanHear(Person.gameObject, member))
            {
                filtered.Add(member);
            }
        }
        return filtered.ToArray();
    }

    //Currently they can talk if no doors or wall is between them!
    public bool CanHear(GameObject person1, GameObject person2)
    {
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door");
        bool blocked = Physics.Linecast(person1.transform.position, person2.transform.position, layerMask);

        return !blocked;
    }

    public bool AlreadyInformed(GameObject[] members, PersonMemory memory)
    {
        return memory.AreInformed(members);
    }

    public void InformNeighbours()
    {
        float radius = 15f;
        List<MyChat> neighbours = Utils.GetNeighbours(Person, radius);
        foreach (var neighbour in neighbours)
        {
            neighbour.InformAboutShooter(Person.PersonMemory.ShooterInfo);
        }
    }
}
