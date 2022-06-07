using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Linq;
//using Npgsql;

public class DatabaseUtil : MonoBehaviour
{
    //public SubjectInfo SInfo;
    public Group currentGroup;
    List<Group> GroupsInDropdown;
    public Subject Current;
    List<Subject> SubjectsInDropdown;
    WebRequest webrequest = new WebRequest();
    public GameObject dropdownGroups;
    public GameObject dropdownGroupsCreateSubjectPage;
    public GameObject dropdownSubjects;
    System.Action<string> _createGroupsCallback;
    System.Action<string> _createSubjectsCallback;
    System.Action<string> _createNewGroupCallback;
    System.Action<string> _createNewSubjectCallback;

    private void Start()
    {
        _createGroupsCallback = (jsonArray) => {
            List<Group> groups = GroupsFromJson(jsonArray);
            List<string> namelist = new List<string>();
            currentGroup = groups.First();
            foreach (Group g in groups)
            {
                Debug.Log(g.name);
                namelist.Add(g.name);
            }
            dropdownGroups.transform.GetComponent<Dropdown>().options.Clear();
            GroupsInDropdown = groups;
            dropdownGroups.transform.GetComponent<Dropdown>().AddOptions(namelist);
            dropdownGroupsCreateSubjectPage.transform.GetComponent<Dropdown>().options.Clear();
            dropdownGroupsCreateSubjectPage.transform.GetComponent<Dropdown>().AddOptions(namelist);
            CreateSubjects(currentGroup.id);
        };
        _createSubjectsCallback = (jsonArray) => {
            List<Subject> subjects = SubjectsFromJson(jsonArray);
            List<string> namelist = new List<string>();
            Current = subjects.First();
            foreach (Subject s in subjects)
            {
                namelist.Add(s.name);
            }
            dropdownSubjects.transform.GetComponent<Dropdown>().options.Clear();
            SubjectsInDropdown = subjects;
            dropdownSubjects.transform.GetComponent<Dropdown>().AddOptions(namelist);
        };

        _createNewGroupCallback = (jsonArray) =>
        {
            List<Group> g = JsonConvert.DeserializeObject<List<Group>>(jsonArray);
            Debug.Log(g);
            foreach(Group gh in g)
            {
                GroupsInDropdown.Add(gh);
                Dropdown.OptionData od = new Dropdown.OptionData();
                od.text = gh.name;
                dropdownGroups.transform.GetComponent<Dropdown>().options.Add(od);
                dropdownGroupsCreateSubjectPage.transform.GetComponent<Dropdown>().options.Add(od);
            }
        };

        _createNewSubjectCallback = (jsonArray) =>
        {
            List<Subject> s = JsonConvert.DeserializeObject<List<Subject>>(jsonArray);
            Debug.Log(s);
            foreach (Subject sh in s)
            {
                Current = sh;
            }
        };
        CreateGroups();

        dropdownGroups.transform.GetComponent<Dropdown>().onValueChanged.AddListener(delegate {
            DropdownGroupsChanged();
        });
        dropdownSubjects.transform.GetComponent<Dropdown>().onValueChanged.AddListener(delegate {
            DropdownSubjectsChanged();
        });

        dropdownGroupsCreateSubjectPage.transform.GetComponent<Dropdown>().onValueChanged.AddListener(delegate {
            DropdownGroupsChanged();
        });
    }

    private void Update()
    {
        
    }

    public void DropdownGroupsChanged()
    {
        //THIS DOES NOT SUPPORT IDENTICAL NAMES FOR THE FUTURE, NEEDS TO BE FIXED
        foreach(Group g in GroupsInDropdown)
        {
            if(dropdownGroups.transform.GetComponent<Dropdown>().captionText.text == g.name)
            {
                currentGroup = g;
            }
        }
        CreateSubjects(currentGroup.id);
    }

    public void DropdownSubjectsChanged()
    {
        foreach(Subject s in SubjectsInDropdown)
        {
            if (s.name == dropdownSubjects.transform.GetComponent<Dropdown>().captionText.text)
            {
                Current = s;
            }
        }
    }

    public void CreateGroups()
    {
        StartCoroutine(webrequest.GetGroups("http://localhost/server.php", _createGroupsCallback));
    }

    public void CreateSubjects(int groupid)
    {
        //http://158.37.193.176/getsubjectsbygroup.php
        StartCoroutine(webrequest.GetSubjectsFromGroup("http://localhost/getsubjectsbygroup.php", groupid, _createSubjectsCallback));
    }

    public void CreateNewGroup(string groupname)
    {
        StartCoroutine(webrequest.InsertNewGroupAndReturnId("http://localhost/SaveNewGroup.php", groupname, _createNewGroupCallback));
    }

    public void CreateNewSubject(string name, int groupid)
    {
        StartCoroutine(webrequest.InsertNewSubjectAndReturnId("http://localhost/SaveNewSubject.php", name, groupid, _createNewSubjectCallback));
    }

    public List<Group> GroupsFromJson(string json)
    {
        Debug.Log(json);
        List<Group> grouplist = JsonConvert.DeserializeObject<List<Group>>(json);
        return grouplist;
    }
    public List<Subject> SubjectsFromJson(string json)
    {
        //Debug.Log(json);
        List<Subject> subjectlist = JsonConvert.DeserializeObject<List<Subject>>(json);
        return subjectlist;
    }
    /*
    public NpgsqlConnection createAndReturnConnection()
    {
        string connectionString =
            "Server=localhost;" +
            "Database=candlook;" +
            "User ID=edugamelab;" +
            "Password=edugame;";
        // IDbConnection dbcon; ## CHANGE THIS TO
        NpgsqlConnection dbcon;

        dbcon = new NpgsqlConnection(connectionString);
        return dbcon;

    }

    public List<Group> returnAllGroups(NpgsqlConnection dbcon)
    {
        NpgsqlCommand dbcmd = dbcon.CreateCommand();

        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbcmd);

        string sql = "SELECT id, name " + "FROM group_entity";

        dbcmd.CommandText = sql;

        NpgsqlDataReader reader = dbcmd.ExecuteReader();

        List<Group> grouplist = new List<Group>();

        while (reader.Read())
        {
            int ID = (int) reader["id"];
            string name = (string)reader["name"];

            Group groupfromdb = new Group(ID, name);
            grouplist.Add(groupfromdb);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;

        return grouplist;
    }

    public List<Subject> getAllSubjectsFromGroup(NpgsqlConnection dbcon, int groupid)
    {
        NpgsqlCommand dbcmd = dbcon.CreateCommand();

        string sql = "SELECT * " + "FROM subject_entity " + "WHERE group_id=" + groupid;

        dbcmd.CommandText = sql;

        NpgsqlDataReader reader = dbcmd.ExecuteReader();

        List<Subject> subjectList = new List<Subject>();

        while (reader.Read())
        {
            int ID = (int)reader["subject_id"];
            string Name = (string)reader["name"];
            int Age = (int)reader["age"];
            int Group_id = (int)reader["group_id"];
            string Note = (string)reader["note"];
            string birthdate = (string)reader["birthdate"];

            Subject SubjectfromDB = new Subject(ID, Name, Age, Group_id, Note, birthdate);
            subjectList.Add(SubjectfromDB);

        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;

        return subjectList;

    }


    public void closeConnection(NpgsqlConnection dbcon)
    {
        dbcon.Close();
        dbcon = null;
    }
    //dbcon.Open();
    //IDbCommand dbcmd = dbcon.CreateCommand();## CHANGE THIS TO
}

public class Group{

    public int id { get; }
    public string name { get; }

    public Group(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
}

public class Subject
{
    public int id { get; }
    public string name { get; }
    public int age { get; }
    public int group_id { get; }
    public string note { get; }

    public string birthdate { get; }

    public Subject(int id, string name, int age, int group_id, string note, string birthday) 
    {
        this.id = id;
        this.name = name;
        this.age = age;
        this.group_id = group_id;
        this.note = note;
        this.birthdate = birthday;
    }
    */
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
