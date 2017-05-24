using System;

namespace BaSMaST_V3
{
    public class Book : Base
    {
        private static long _bookNextID;
        private DateTime _begin;
        private DateTime _end;

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

        public Book(string name, DateTime begin, DateTime end, string id = null) : base($"{AppSettings_Static.TypeInfos[ TypeName.Book ].IDLetter}{_bookNextID++}", name)
        {
            _begin = begin;
            _end = end;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Book.ToString());
            else SetID(id);
            _bookNextID = Helper.GetNumeric(ID)+2;
        }
    }
}
