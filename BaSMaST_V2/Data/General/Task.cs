using System;

namespace BaSMaST_V3
{
    public class Task:Base
    {
        private static long _taskNextID;
        private TaskState _state;
        private DateTime _dueDate;
        private Importance _importance;

        public TaskState State
        {
            get { return _state; }
            set
            {
                _state = value;
                DBDataManager.UpdateDatabase(this, TypeName.Task.ToString(), "State");
            }
        }
        public DateTime DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                DBDataManager.UpdateDatabase(this, TypeName.Task.ToString(), "DueDate");
            }
        }
        public Importance Importance
        {
            get { return _importance; }
            set
            {
                _importance = value;
                DBDataManager.UpdateDatabase(this, TypeName.Task.ToString(), "Importance");
            }
        }

        public Task(string name, TaskState state, Importance importance, DateTime due, string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.Task ].IDLetter}{_taskNextID++}",name)
        {
            State = state;
            DueDate = due;

            Importance = importance;

            if (string.IsNullOrEmpty(id))
            {
                DBDataManager.InsertIntoDatabase(this, TypeName.Task.ToString());
            }
            else
            {
                SetID(id);
            }
            _taskNextID = Helper.GetNumeric(ID) + 1;
        }
    }
}
