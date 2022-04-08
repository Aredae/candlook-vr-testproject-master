using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Util
{
    [Serializable]
    public class Group
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    [Serializable]
    public class Subject
    {
        public int subject_id { get; set; }
        public string name { get; set; }
    }

    public class SubjectInfo : MonoBehaviour
    {
        public static SubjectInfo instance;
        
        Subject s;
        string notes;

        private void Awake()
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        public void SetSubjectInInfo(Subject s)
        {
            this.s = s;
        }

        public void SetNotes(string n)
        {
            this.notes = n;
        }
    }
    /*
    public class Group
    {

        public int Id; // { get; set; }
        public string Name; // { get; set; }

        public Group(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        
    }
    */
}

