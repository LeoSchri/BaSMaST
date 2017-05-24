using System;

namespace BaSMaST_V3
{
    public class Aftermath : Base
    {
        private static long _aftermathNextID;
        private DateTime? _end;
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                DBDataManager.UpdateDatabase(this, TypeName.Aftermath.ToString(), "Description");
            }
        }
        public DateTime? End
        {
            get { return _end; }
            set
            {
                _end = value;
                DBDataManager.UpdateDatabase(this,TypeName.Aftermath.ToString(), "End");
            }
        }
        public Lore Parent { get; private set; }

        public Aftermath(string name, string desc, DateTime? end, Lore parent, string id=null) : base($"{AppSettings_Static.TypeInfos[TypeName.Aftermath].IDLetter}{_aftermathNextID++}", name)
        {
            _end = end;
            _description = desc;
            Parent = parent;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Aftermath.ToString());
            else SetID(id);
            _aftermathNextID = Helper.GetNumeric(ID) + 1;
        }

        public void ChangeParent(Lore lore)
        {
            Parent.AftermathManager.RemoveItem( this , TypeName.Aftermath);
            Parent = lore;
            lore.AftermathManager.AddItem( this );
        }
    }
}
