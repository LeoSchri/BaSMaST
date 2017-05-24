using System.Collections.Generic;

namespace BaSMaST_V3
{
    public class Item :Base
    {
        private static long _itemNextID;

        public ItemTable Parent { get; private set; }
        public List<Property> Props { get; private set; }

        public Item(ItemTable parent, string name, string id = null) : base($"{parent.TableLetter}.{AppSettings_Static.TypeInfos[ TypeName.Item ].IDLetter}{_itemNextID++}", name)
        {
            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Item.ToString());
            else SetID(id);
            _itemNextID = Helper.GetNumeric(ID)+2;
        }

        public void ChangePropertyValue(string propName, string value)
        {
            var match = Props.Find(p => p.Attribute.Name == propName);
            match.ChangeValue(this, TypeName.ItemTable,value);
        }

        public void RemoveAllLinks()
        {
            RemoveAllLinks(TypeName.Item);
        }
    }
}
