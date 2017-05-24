using System.Collections.Generic;

namespace BaSMaST_V3
{
    public class ItemTable : Base
    {
        private static long _itemTableNextID;

        public string TableLetter { get; private set;}
        public string Icon { get; private set; }
        public List<Item> Items { get; private set; }
        public List<Attribute> Attributes { get; private set; }

        public ItemTable(string name, string tableLetter, string icon = null, string id=null) : base($"{tableLetter}{_itemTableNextID++}", name)
        {
            TableLetter = tableLetter;
            Icon = icon;
            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.ItemTable.ToString());
            else SetID(id);
            _itemTableNextID = Helper.GetNumeric(ID)+2;
            AppSettings_User.CurrentProject.ItemTables.Add(this);
        }

        public static bool LetterExists (List<ItemTable> list, string letter)
        {
            var match = list.Find(i => i.TableLetter == letter);
            if(match == null)
            {
                return false;
            }

            return true;
        }

        public void AddItems(List<Item> items)
        {

        }

        public void AddAttributes(List<Attribute> attr)
        {
            if (Attributes == null)
                Attributes = new List<Attribute>();
            Attributes.AddRange(attr);
            attr.ForEach(a =>
            {
                DBDataManager.AddColumnToTable(TypeName.ItemTable.ToString(), a.Name, a.Type, a.AllowsNull);
                AppSettings_User.CurrentProject.ItemTables.ForEach(t =>
                {
                    t.Items.ForEach(i =>
                    {
                        i.Props.Add(new Property(a, null));
                    });
                });

            });
        }

        public void RemoveAttributes(List<Attribute> attr)
        {
            attr.ForEach(a =>
            {
                if(Attributes != null && Attributes.Contains(a))
                {
                    Attributes.Remove(a);
                    AppSettings_User.CurrentProject.ItemTables.ForEach(t =>
                    {
                        t.Items.ForEach(i =>
                        {
                            i.Props.Remove(i.Props.Find(p => p.Attribute == a));
                        });
                    });
                    Helper.RemoveAttribute(AppSettings_User.CurrentProject, TypeName.Task, a, Name);
                }
            });
        }

        public void RemoveAllAttributes()
        {
            if (Attributes != null)
                Attributes.Clear();
        }

        public void ChangeIcon(string path)
        {
            Icon = path;
        }
    }
}
