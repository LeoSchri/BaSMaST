using System.Collections.Generic;

namespace BaSMaST_V3
{
    public class Base
    {
        public string ID { get; private set; }
        public string Name { get; set; }
        public Manager<Note> NoteManager { get; set;}

        public Base(string id, string name, List<Resource> resources = null)
        {
            ID = id;
            Name = name;
            NoteManager = Manager<Note>.Create() ;
        }

        public void ChangeName(string name, TypeName type)
        {
            Name = name;
            DBDataManager.UpdateDatabase(this, type.ToString(), "Name");
        }

        public void SetID(string id)
        {
            ID = id;
        }

        public void RemoveAllLinks(TypeName type)
        {
            AppSettings_User.CurrentProject.ResourceLinkManager.RemoveAllLinksForItem(this, type, TypeName.ResourceLink);
            AppSettings_User.CurrentProject.ItemLinkManager.RemoveAllLinksForItem(this, type, TypeName.ItemLink);
        }
    }
}
