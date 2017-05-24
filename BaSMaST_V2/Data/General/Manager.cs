using System.Collections.Generic;
using System.Linq;

namespace BaSMaST_V3
{
    public class Manager<T> where T : Base
    {
        private List<T> Items;

        public List<T> GetItems()
        {
            return Items;
        }

        public static Manager<T> Create(List<T> list = null)
        {
            var manager = new Manager<T>();
            manager.Items = list;

            return manager;
        }

        public void AddItem<V>(V item) where V : Base
        {
            if (Items == null)
                Items = new List<T>();
            Items.Add(item as T);
        }

        public void AddItems<V>(List<V> items) where V : Base
        {
            items.ForEach(i =>
            {
                AddItem(i);
            });
        }

        public void RemoveItem<V>(V item, TypeName type) where V : Base
        {
            if (Items == null)
                return;
            Items.Remove(item as T);
            DBDataManager.RemoveFromDatabase(AppSettings_Static.TypeInfos[ type ], item.ID);
            Helper.DeleteAllChildren(item);
            item.RemoveAllLinks(type);
        }

        public void RemoveItems<V>(List<V> items, TypeName type) where V : Base
        {
            items.ForEach(i =>
            {
                RemoveItem(i, type);
            });
        }

        public void RemoveAllItems(TypeName type)
        {
            Items.ForEach(i =>
            {
                DBDataManager.RemoveFromDatabase(AppSettings_Static.TypeInfos[ type ], i.ID);
                Helper.DeleteAllChildren(i);
                i.RemoveAllLinks(type);
            });
            Items.Clear();
        }
    }

    public class Manager<T,U> where T:Base where U : Base
    {
        private List<T> Items1;
        private List<U> Items2;

        public static Manager<T,U> Create(List<T> list1 = null, List<U> list2 = null)
        {
            var manager = new Manager<T, U>();
            manager.Items1 = list1;
            manager.Items2 = list2;

            return manager;
        }

        public List<V> GetItems<V>() where V:Base
        {
            if (Items1 != null && Items1.GetType() == typeof(List<V>))
            {
                var items = new List<V>();
                if (Items1.Any())
                {
                    Items1.ForEach(i =>
                    {
                        items.Add(i as V);
                    });
                    return items;
                }
                else return items;
            }
            else if (Items2 != null && Items2.GetType() == typeof(List<V>))
            {
                var items = new List<V>();
                if(Items2.Any())
                {
                    Items2.ForEach(i =>
                    {
                        items.Add(i as V);
                    });
                    return items;
                }
                else return items;
            }
            else return null;
        }

        public void AddItem<V>(V item) where V : Base
        {
            if (Items1 == null)
                Items1 = new List<T>();
            if (Items2 == null)
                Items2 = new List<U>();

            if (typeof(List<V>) == Items1.GetType())
            {
                Items1.Add(item as T);
            }
            else if (typeof(List<V>) == Items2.GetType())
            {
                Items2.Add(item as U);
            }
        }

        public void AddItems<V>(List<V> items) where V:Base
        {
            items.ForEach(i =>
            {
                AddItem(i);
            });
        }

        public void RemoveItem<V>(V item, TypeName type) where V : Base
        {
            if (typeof(List<V>) == Items1.GetType())
            {
                if (Items1 == null)
                    return;
                Items1.Remove(item as T);
                if(type != TypeName.Relationships)
                    DBDataManager.RemoveFromDatabase(AppSettings_Static.TypeInfos[ type ], item.ID);
                Helper.DeleteAllChildren(item);
                item.RemoveAllLinks(type);
            }
            else if (typeof(List<V>) == Items2.GetType())
            {
                if (Items2 == null)
                    return;
                Items2.Remove(item as U);
                if(type != TypeName.Relationships)
                    DBDataManager.RemoveFromDatabase(AppSettings_Static.TypeInfos[ type ], item.ID);
                Helper.DeleteAllChildren(item);
                item.RemoveAllLinks(type);
            }
        }

        public void RemoveItems<V>(List<V> items, TypeName type) where V:Base
        {
            items.ForEach(i =>
            {
                RemoveItem(i, type);
            });
        }

        public void RemoveAllItems<V>(TypeName type) where V:Base
        {
            if (typeof(List<V>) == Items1.GetType())
            {
                Items1.ForEach(i =>
                {
                    DBDataManager.RemoveFromDatabase(AppSettings_Static.TypeInfos[ type ], i.ID);
                    Helper.DeleteAllChildren(i);
                    i.RemoveAllLinks(type);
                });
                Items1.Clear();
            }
            else if (typeof(List<V>) == Items2.GetType())
            {
                Items2.ForEach(i =>
                {
                    DBDataManager.RemoveFromDatabase(AppSettings_Static.TypeInfos[ type ], i.ID);
                    Helper.DeleteAllChildren(i);
                });
                Items2.Clear();
            }
        }
    }

    public class LinkManager<T,U> where T : Base where U:Base
    {
        private List<Link<T,U>> Links;

        public List<Link<T, U>> GetLinks()
        {
            return Links;
        }

        public static LinkManager<T,U> Create(List<Link<T, U>> list = null)
        {
            var manager = new LinkManager<T,U>();
            manager.Links = list;

            return manager;
        }

        public void AddLink(T linkObj1, U linkObj2, TypeName table, bool fromDatabase = false)
        {
            if (Links == null)
                Links = new List<Link<T, U>>();

            var link = Link<T,U>.Create(linkObj1, linkObj2, table);
            Links.Add(link);

            if (fromDatabase)
                return;
            if (link.LinkObject1.GetType() == link.LinkObject2.GetType())
            {
                DBDataManager.InsertIntoDatabase(AppSettings_Static.TypeInfos[ link.Table ], link.LinkObject1, link.LinkObject2);
                DBDataManager.InsertIntoDatabase(AppSettings_Static.TypeInfos[ link.Table ], link.LinkObject2, link.LinkObject1);
            }
            else
            {
                if(typeof(T) == typeof(Plot))
                    DBDataManager.InsertIntoDatabase(AppSettings_Static.TypeInfos[ link.Table ], link.LinkObject1, link.LinkObject2, TypeName.Plot);
                if (typeof(T) == typeof(Lore))
                    DBDataManager.InsertIntoDatabase(AppSettings_Static.TypeInfos[ link.Table ], link.LinkObject1, link.LinkObject2, TypeName.Lore);
            }
        }

        public void RemoveAllLinksForItem<V>(V linkObj, TypeName type, TypeName table) where V:Base
        {
            if (Links == null)
                return;
            var links = new List<Link<T, U>>();
            if (typeof(V) == typeof(T))
            {
                links.AddRange(Links.FindAll(l => l.LinkObject1 == linkObj));
            }
            else if (typeof(V) == typeof(U))
            {
                links.AddRange(Links.FindAll(l => l.LinkObject2 == linkObj));
            }
            else return;
            if (links == null || !links.Any())
                return;

            links.ForEach(link =>
            {
                Links.Remove(link);
                if(link.LinkObject1.GetType() == link.LinkObject2.GetType())
                    DBDataManager.RemoveLinkOfSameTypeFromDatabase(AppSettings_Static.TypeInfos[link.Table],link.LinkObject1, link.LinkObject2);
                else DBDataManager.RemoveLinkFromDatabase(AppSettings_Static.TypeInfos[ link.Table ], type, link.LinkObject1);
            });
        }

        public void RemoveLink(T linkObj1, U linkObj2, TypeName type, TypeName table)
        {
            if (Links == null)
                return;
            var link = Links.Find(l => l.LinkObject1 == linkObj1 && l.LinkObject2 == linkObj2);
            if (link == null)
                return;

            Links.Remove(link);
            if (link.LinkObject1.GetType() == link.LinkObject2.GetType())
                DBDataManager.RemoveLinkOfSameTypeFromDatabase(AppSettings_Static.TypeInfos[ link.Table ], link.LinkObject1, link.LinkObject2);
            else
            {
                DBDataManager.RemoveLinkFromDatabase(AppSettings_Static.TypeInfos[ link.Table ], type, link.LinkObject1);
            }
        }

        public void RemoveAllLinks(TypeName type)
        {
            Links.ForEach(link =>
            {
                if (link.LinkObject1.GetType() == link.LinkObject2.GetType())
                    DBDataManager.RemoveLinkOfSameTypeFromDatabase(AppSettings_Static.TypeInfos[ link.Table ], link.LinkObject1, link.LinkObject2);
                else DBDataManager.RemoveLinkFromDatabase(AppSettings_Static.TypeInfos[ link.Table ], type, link.LinkObject1);
            });
            Links.Clear();
        }
    }

    public class Link<T,U> where T:Base where U : Base
    {
        public T LinkObject1 { get; private set; }
        public U LinkObject2 { get; private set; }
        public TypeName Table { get; private set; }

        public static Link<T,U> Create(T obj1, U obj2, TypeName table)
        {
            var link = new Link<T, U>();
            link.LinkObject1 = obj1;
            link.LinkObject2 = obj2;
            link.Table = table;

            return link;
        }
    }
}
