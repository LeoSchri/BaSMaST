using System.Collections.Generic;

namespace BaSMaST_V3
{
    public class Event : Base
    {
        private static long _eventNextID;
        private PointInTime _begin;
        private PointInTime _end;
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                DBDataManager.UpdateDatabase(this, TypeName.Event.ToString(), "Description");
            }
        }
        public Subplot Parent { get; private set; }
        public PointInTime Begin
        {
            get { return _begin; }
            set
            {
                _begin = value;
                DBDataManager.UpdateDatabase(this,TypeName.Event.ToString(), "Begin");
            }
        }
        public PointInTime End
        {
            get { return _end; }
            set
            {
                _end = value;
                DBDataManager.UpdateDatabase(this, TypeName.Event.ToString(), "End");
            }
        }
        public Mood Mood { get; private set; }
        public Event Source { get; private set; }

        public Event(string name, string desc, Subplot parent, PointInTime begin = null, PointInTime end = null, Event source = null, Mood mood = Mood.None,string id = null) : base($"{AppSettings_Static.TypeInfos[ TypeName.Event ].IDLetter}{_eventNextID++}", name)
        {
            _description = desc;
            Parent = parent;
            Source = source;
            Mood = mood;
            _begin = begin;
            _end = end;

            if (begin != null && end != null && begin.Date != null && end.Date != null && begin.DayTime != DayTime.None && end.DayTime != DayTime.None)
            {
                AddToSchedule(begin, end);
            }

            if (string.IsNullOrEmpty(id))
            {
                _eventNextID = Helper.GetNumeric(ID)+2;
                DBDataManager.InsertIntoDatabase(this, TypeName.Event.ToString());
            }
            else
            {
                SetID(id);
                _eventNextID = Helper.GetNumeric(ID)+2;
            }
        }

        public void ChangeParent(Subplot subplot)
        {
            Parent.EventManager.RemoveItem( this ,TypeName.Event);
            Parent = subplot;
            subplot.EventManager.AddItem( this );
        }

        public void ChangeMood(Mood mood)
        {
            Mood = mood;
            DBDataManager.UpdateDatabase(this,TypeName.Event.ToString(),"Mood");
        }

        public void AddToSchedule(PointInTime begin, PointInTime end)
        {
            var schedule = AppSettings_User.CurrentProject.Schedule;

            schedule.AddItem(this);
        }

        public void RemoveFromSchedule()
        {
            var schedule = AppSettings_User.CurrentProject.Schedule;

            Begin = null;
            End = null;

            schedule.RemoveItems(new List<Event>() { this },TypeName.Event);
        }

        public static void RemoveEventsFromSchedule(List<Event> evs)
        {
            var schedule = AppSettings_User.CurrentProject.Schedule;

            evs.ForEach(e =>
            {
                e.Begin = null;
                e.End = null;
            });

            schedule.RemoveItems(evs,TypeName.Event);
        }

        public void RemoveAllLinks()
        {
            AppSettings_User.CurrentProject.EventCharacterPresentLinkManager.RemoveAllLinksForItem(this,TypeName.Event, TypeName.CharacterPresent);
            AppSettings_User.CurrentProject.EventAttachmentCharacterLinkManager.RemoveAllLinksForItem(this,TypeName.Event, TypeName.AttachmentFigure);
            AppSettings_User.CurrentProject.EventLocationLinkManager.RemoveAllLinksForItem(this,TypeName.Event, TypeName.LocationLink);
            AppSettings_User.CurrentProject.EventSourceLinkManager.RemoveAllLinksForItem(this,TypeName.Event, TypeName.EventLink);
            AppSettings_User.CurrentProject.EventCharacterPresentLinkManager.RemoveAllLinksForItem(this,TypeName.Event, TypeName.CharacterPresent);
            AppSettings_User.CurrentProject.EventCharacterPresentLinkManager.RemoveAllLinksForItem(this,TypeName.Event, TypeName.CharacterPresent);
            RemoveAllLinks(TypeName.Event);
        }

        //public void AddRepeat(Occurrence occ, DateTime begin, DateTime end)
        //{
        //    if (Clones != null)
        //    {
        //        var answer = MessageBox.Show(null, "This will delete the former repeat. Are you sure you want to continue?", "Creating a new repeat.", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //        if (answer == DialogResult.Yes)
        //        {
        //            RemoveAllClones();
        //        }
        //        else return;
        //    }
        //    //Each event gets this as it's source

        //}

        //public void RemoveAllClones()
        //{
        //    Event.RemoveEventsFromSchedule(Clones);
        //    Parent.EventManager.RemoveItems(Clones,TypeName.Event);
        //    Clones.Clear();
        //}

        //public void RemoveClone(Event clone)
        //{
        //    clone.RemoveFromSchedule();
        //    Parent.EventManager.RemoveItem(clone,TypeName.Event);
        //    Clones.Remove(clone);
        //}

        //public void MakeIndependent()
        //{
        //    Source.Clones.Remove(this);
        //    Source = null;
        //}

        //public void SetClones()
        //{
        //    if (Parent.EventManager == null)
        //        Parent.EventManager = Manager<Event>.Create();
        //    if (Parent.EventManager.GetItems() == null || !Parent.EventManager.GetItems().Any())
        //        return;
        //    Parent.EventManager.GetItems().ForEach(e=>
        //    {
        //        if (e.Source == this)
        //        {
        //            if (Clones == null)
        //                Clones = new List<Event>();
        //            if(!Clones.Contains(e))
        //                Clones.Add(e);
        //        }
        //    });
        //}
    }
}
