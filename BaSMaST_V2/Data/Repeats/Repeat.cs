using System;
using System.Collections.Generic;

namespace BaSMaST_V3
{
    public class Repeat:Base
    {
        private static long _repeatNextID;
        private DateTime _begin;
        private DateTime _end;

        public Event Source { get; private set; }
        public Occurrence Occurrence { get; private set; }
        public DateTime Begin
        {
            get { return _begin; }
            set
            {
                _begin = value;
                DBDataManager.UpdateDatabase(this, TypeName.Event.ToString(), "Begin");
            }
        }
        public DateTime End
        {
            get { return _end; }
            set
            {
                _end = value;
                DBDataManager.UpdateDatabase(this, TypeName.Book.ToString(), "End");
            }
        }

        public Repeat(string name, Event source, List<Event> clones, Occurrence occ, DateTime begin, DateTime end, string id=null):base($"{AppSettings_Static.TypeInfos[ TypeName.Repeat ].IDLetter}{_repeatNextID++}",name)
        {
            Source = source;
            Occurrence = occ;
            _begin = begin;
            _end = end;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Repeat.ToString());
            else SetID(id);
            _repeatNextID = Helper.GetNumeric(ID)+2;
        }
    }
}
