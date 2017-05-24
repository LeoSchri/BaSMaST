using System.Collections.Generic;
using System.Linq;

namespace BaSMaST_V3
{
    public abstract class AttributeHolder:Base
    {
        public static List<Attribute> Attributes { get; set; }
        public List<Property> Props { get; set; }

        public AttributeHolder(string id, string name) : base(id, name) { }
       
        public static void AddAttributes<T,U>(List<Attribute> attr, TypeName type, Manager<T,U> manager, bool UseFirstList) where T:AttributeHolder where U:AttributeHolder
        {
            if (Attributes == null)
                Attributes = new List<Attribute>();
            Attributes.AddRange(attr);
            attr.ForEach(a =>
            {
                if(UseFirstList)
                {
                    if (manager.GetItems<T>() == null || !manager.GetItems<T>().Any())
                        return;
                    manager.GetItems<T>().ForEach(c =>
                    {
                        if (c.Props == null)
                            c.Props = new List<Property>();
                        c.Props.Add(new Property(a, null));
                    });
                }
                else
                {
                    if (manager == null || manager.GetItems<U>() == null || !manager.GetItems<U>().Any())
                        return;
                    manager.GetItems<U>().ForEach(c =>
                    {
                        if (c.Props == null)
                            c.Props = new List<Property>();
                        c.Props.Add(new Property(a, null));
                    });
                }
            });
        }

        public static void RemoveAttributes<T,U>(List<Attribute> attr, TypeName type, Manager<T, U> manager, bool UseFirstList) where T : AttributeHolder where U : AttributeHolder
        {
            attr.ForEach(a =>
            {
                if (Attributes != null && Attributes.Contains(a))
                {
                    Attributes.Remove(a);
                    DBDataManager.RemoveColumnFromTable(type.ToString(), a.Name);
                    if(UseFirstList)
                    {
                        if (manager.GetItems<T>() == null || !manager.GetItems<T>().Any())
                            return;
                        manager.GetItems<T>().ForEach(c =>
                        {
                            if (c.Props != null)
                            {
                                var match = c.Props.Find(p => p.Attribute == a);
                                if (match != null)
                                {
                                    c.Props.Remove(match);
                                }
                            }
                        });
                    }
                    else
                    {
                        if (manager == null || manager.GetItems<U>() == null || !manager.GetItems<U>().Any())
                            return;
                        manager.GetItems<U>().ForEach(c =>
                        {
                            if (c.Props != null)
                            {
                                var match = c.Props.Find(p => p.Attribute == a);
                                if (match != null)
                                {
                                    c.Props.Remove(match);
                                }
                            }
                        });
                    }

                    Helper.RemoveAttribute(AppSettings_User.CurrentProject, TypeName.Character, a);
                }
            });
        }

        public static void RemoveAllAttributes()
        {
            if (Attributes != null)
                Attributes.Clear();
        }

        public void ApplyProps()
        {
            if (Attributes == null || !Attributes.Any())
                return;

            if (Props == null)
                Props = new List<Property>();

            Attributes.ForEach(a =>
            {
                Props.Add(new Property(a, null));
            });
        }
    }
}
