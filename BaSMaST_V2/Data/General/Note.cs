
namespace BaSMaST_V3
{
    public class Note:Base
    {
        private static long _noteNextID;
        private string _content;

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                DBDataManager.UpdateDatabase(this, TypeName.Note.ToString(), "Content");
            }
        }
        public Base Owner { get; private set; }

        public Note(string name, string content, Base owner,string id = null) : base($"{AppSettings_Static.TypeInfos[ TypeName.Note ].IDLetter}{_noteNextID++}", name)
        {
            _content = content;
            Owner = owner;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Note.ToString());
            else SetID(id);
        }

        public void ChangeOwner(Base owner)
        {
            Owner = owner;
        }
    }
}
