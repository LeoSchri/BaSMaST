using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Windows.Data;

namespace BaSMaST_V3
{
    public static class Helper
    {
        public static long GetNumeric(string ID)
        {
            return Convert.ToInt64(Regex.Replace(ID, "[^0-9.]", ""));
        }

        public static string GetOnlyLetters(string text)
        {
            return Regex.Replace(text, @"[\d-]", string.Empty);
        }

        public static void CompressAndMoveDirectory(string source, string dest, string name)
        {
            try
            {
                ZipFile.CreateFromDirectory(source, $@"{dest}/{name}Backup{DateTime.Now}.zip", CompressionLevel.Optimal, false);
            }
            catch(Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject,$"CompressAndMoveDirectory({source},{dest},{name})",e);
            }
        }

        public static string FirstLetterUppercase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static Event FindEvent(Project p, string id)
        {
            var events = GetAllEvents(p);

            if (events.Any())
                return events.Find(e => e.ID == id);
            else return null;
        }

        public static List<Event> GetAllEvents(Project p)
        {
            var events = new List<Event>();
            p.Archive.GetItems<Plot>().ForEach(plot =>
            {
                if (plot.SubplotManager != null && plot.SubplotManager.GetItems() != null && plot.SubplotManager.GetItems().Any())
                {
                    plot.SubplotManager.GetItems().ForEach(s =>
                    {
                        if (s.EventManager != null && s.EventManager.GetItems() != null && s.EventManager.GetItems().Any())
                            events.AddRange(s.EventManager.GetItems());
                    });
                }
            });

            return events;
        }

        public static Subplot FindSubplot(Project p, string id)
        {
            var subplots = GetAllSubplots(p);

            if (subplots.Any())
                return subplots.Find(s => s.ID == id);
            else return null;
        }

        public static List<Subplot> GetAllSubplots(Project p)
        {
            var subplots = new List<Subplot>();
            if (p.Archive == null || p.Archive.GetItems<Plot>() == null || !p.Archive.GetItems<Plot>().Any())
                return null;
            p.Archive.GetItems<Plot>().ForEach(plot =>
            {
                if (plot.SubplotManager != null && plot.SubplotManager.GetItems() != null && plot.SubplotManager.GetItems().Any())
                    subplots.AddRange(plot.SubplotManager.GetItems());
            });

            return subplots;
        }

        public static Aftermath FindAftermath(Project p, string id)
        {
            var aftermath = GetAllAftermath(p);

            if (aftermath.Any())
                return aftermath.Find(a => a.ID == id);
            else return null;
        }

        public static List<Aftermath> GetAllAftermath(Project p)
        {
            var aftermath = new List<Aftermath>();
            if (p.Archive == null || p.Archive.GetItems<Lore>() == null || !p.Archive.GetItems<Lore>().Any())
                return null;
            p.Archive.GetItems<Lore>().ForEach(am =>
            {
                if (am.AftermathManager != null && am.AftermathManager.GetItems() != null && am.AftermathManager.GetItems().Any())
                    aftermath.AddRange(am.AftermathManager.GetItems());
            });

            return aftermath;
        }

        public static Item FindItem(Project p, string id)
        {
            var items = new List<Item>();
            p.ItemTables.ForEach(i =>
            {
                items.AddRange(i.Items);
            });

            return items.Find(i => i.ID == id);
        }

        public static Base GetOwner(Project p,string ID)
        {
            switch(GetOnlyLetters(ID))
            {
                case "B": return (p.BookManager.GetItems().Find(obj => obj.ID == ID) as Book);
                case "LR": return (p.Archive.GetItems<Lore>().Find(obj => obj.ID == ID) as Lore);
                case "P": return (p.Archive.GetItems<Plot>().Find(obj => obj.ID == ID) as Plot);
                case "S": return FindSubplot(p,ID);
                case "E": return FindEvent(p,ID) as Event;
                case "C": return (p.CharacterManager.GetItems<Character>().Find(obj => obj.ID == ID) as Character);
                case "L": return (p.LocationManager.GetItems().Find(obj => obj.ID == ID) as Location);
                case "I": return (FindItem(p,ID) as Item);
                case "AM": return FindAftermath(p, ID);
                default: return null;
            }
        }

        public static T GetType<T>(string type) where T : struct, IConvertible
        {
            var list = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            T result = list.Find(e => e.ToString() == "None");
            if (string.IsNullOrEmpty(type))
                return result;
            list.ForEach(e =>
            {
                if (e.ToString() == type)
                    result = e;
            });
            return result;
        }

        public static List<string> GetTypesAsList<T>() where T : struct, IConvertible
        {
            var list = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            var result = new List<string>();

            list.ForEach(item =>
            {
                result.Add(TextCatalog.GetName(item.ToString()));
            });

            return result;
        }

        public static List<string> GetTypesAsListWithoutNone<T>() where T : struct, IConvertible
        {
            var list = GetTypesAsList<T>();
            for(int i =0; i< list.Count; i++)
            {
                if (list[ i ] == TextCatalog.GetName("None"))
                    list.RemoveAt(i);
            }

            return list;
        }

        public static bool IsListOfEnum(Type type)
        {
            return ( type.IsInterface ? new[] { type } : type.GetInterfaces() )
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Any(i => i.GetGenericArguments().First().IsEnum);
        }

        public static bool ProgramIsRunning(string fullPath)
        {
            string FilePath = Path.GetDirectoryName(fullPath);
            string FileName = Path.GetFileNameWithoutExtension(fullPath).ToLower();
            bool isRunning = false;

            Process[] pList = Process.GetProcessesByName(FileName);
            foreach (Process p in pList)
                if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                    isRunning = true;

            return isRunning;
        }

        public static void StartProgram(string fullPath)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo();
            p.StartInfo.FileName = fullPath;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
        }

        private static string GetFromConfigFile(string info)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(GetConfigPath());

            return xml.SelectSingleNode($"//{info}").InnerText;
        }

        private static void ChangeConfigFile(string info, string value)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(GetConfigPath());
            xml.SelectSingleNode($"//{info}").InnerText = value;
            xml.Save(GetConfigPath());
        }

        private static string GetConfigPath()
        {
            var correctPath = Environment.CurrentDirectory.Split('\\').ToList();
            correctPath.RemoveRange(correctPath.Count-2,2);
            return $@"{string.Join(@"\",correctPath)}\Config.xml";
        }

        private static List<Attribute> GetAttributes(Project p, TypeName type, string name = null)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load($@"{p.Location}\ProjectConfig.xml");

            var attributes = new List<Attribute>();

            XmlNode root = xml.SelectSingleNode("//Attributes").SelectSingleNode($"//{type}");

            if (!string.IsNullOrEmpty(name))
            {
                root = root.SelectSingleNode($"//{name}");
            }
            XmlNodeList elemList = root.ChildNodes;

            for(int i =0; i< elemList.Count; i++)
            {
                var node = elemList[ i ];

                var attributeName = node.ChildNodes[ 0 ].InnerText;
                var attributeType = node.ChildNodes[ 1 ].InnerText;
                var allowsNull = node.ChildNodes[ 2 ].InnerText;

                var Type = typeof(string);

                switch(attributeType)
                {
                    case "Int": Type = typeof(int); break;
                    case "Double": Type = typeof(double); break;
                    case "Boolean": Type = typeof(bool); break;
                    case "Date": Type = typeof(DateTime); break;
                    case "ListOfString": Type = typeof(List<string>); break;
                    case "ListOfInt": Type = typeof(List<int>); break;
                    case "ListOfDouble": Type = typeof(List<Double>); break;
                }

                attributes.Add(new Attribute(attributeName, Type,p, type,allowsNull=="Yes"?true:false,true));
            }

            return attributes;
        }

        public static void AddAttribute(Project p, TypeName type, Attribute attribute, string name = null)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load($@"{p.Location}\ProjectConfig.xml");

            var attributes = new List<Attribute>();

            XmlNode root = xml.SelectSingleNode("//Attributes").SelectSingleNode($"//{type}");

            if (!string.IsNullOrEmpty(name))
            {
                root = root.SelectSingleNode($"//{name}");
            }

            var strNamespace = xml.DocumentElement.NamespaceURI;
            var node = xml.CreateNode(XmlNodeType.Element,"Attribute",strNamespace);

            root.AppendChild(node);

            var nodeName = xml.CreateNode(XmlNodeType.Element, "AttributeName", strNamespace);
            var nodeType = xml.CreateNode(XmlNodeType.Element, "AttributeType", strNamespace);
            var nodeAllowsNull = xml.CreateNode(XmlNodeType.Element, "AllowsNull", strNamespace);

            node.AppendChild(nodeName);
            node.AppendChild(nodeType);
            node.AppendChild(nodeAllowsNull);

            nodeName.InnerText = attribute.Name;
            var Type = "String";

            if(attribute.Type == typeof(int))
                Type = "Int";
            if (attribute.Type == typeof(double))
                Type = "Double";
            if (attribute.Type == typeof(bool))
                Type = "Boolean";
            if (attribute.Type == typeof(DateTime))
                Type = "Date";
            if (attribute.Type == typeof(List<string>))
                Type = "ListOfString";
            if (attribute.Type == typeof(List<int>))
                Type = "ListOfInt";
            if (attribute.Type == typeof(List<Double>))
                Type = "ListOfDouble";

            nodeType.InnerText = Type;
            nodeAllowsNull.InnerText = attribute.AllowsNull ? "Yes" : "No";

            xml.Save($@"{p.Location}\ProjectConfig.xml");
        }

        public static void RemoveAttribute (Project p, TypeName type, Attribute attribute,string name = null)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load($@"{p.Location}\ProjectConfig.xml");

            var attributes = new List<Attribute>();

            XmlNode root = xml.SelectSingleNode("//Attributes").SelectSingleNode($"//{type}");
            
            if (!string.IsNullOrEmpty(name))
            {
                root = root.SelectSingleNode($"//{name}");
            }
            XmlNodeList elemList = root.SelectNodes("//Attribute");

            foreach (XmlNode node in elemList)
            {
                if (node.ChildNodes[0].InnerText == attribute.Name)
                    root.RemoveChild(node);
            }

            xml.Save($@"{p.Location}\ProjectConfig.xml");
        }

        public static void SetConfig()
        {
            AppSettings_User.ChangeColorSchema( ColorSchema.GetColorSchemaByName(GetFromConfigFile("ColorSchema")));
            AppSettings_User.ChangeLanguage(GetType<Language>(GetFromConfigFile("Language")));
            AppSettings_User.ChangeFontSize(Convert.ToInt32(GetFromConfigFile("FontSize")));
        }

        public static void SetProjectSpecificConfig(Project p)
        {
            try
            {
                Character.RemoveAllAttributes();
                Character.AddAttributes(GetAttributes(p,TypeName.Character),TypeName.Character,AppSettings_User.CurrentProject.CharacterManager,true);

                MainCharacter.RemoveAllAttributes();
                MainCharacter.AddAttributes(GetAttributes(p, TypeName.MainCharacter), TypeName.MainCharacter, AppSettings_User.CurrentProject.CharacterManager, false);
                var attr = MainCharacter.Attributes;

                if (p.ItemTables != null && p.ItemTables.Any())
                {
                    p.ItemTables.ForEach(i =>
                    {
                        i.RemoveAllAttributes();
                        i.AddAttributes(GetAttributes(p,TypeName.ItemTable,i.Name));
                    });
                }
            }
            catch(Exception e)
            {
                Log.ErrorOut($"SetProjectSpecificConfig({p.Name})", e);
            }

        }

        public static void UpdateConfig(string info, string value)
        {
            ChangeConfigFile(info, value);
            Popup.ShowWindow(TextCatalog.GetName("RestartNecessary"), TextCatalog.GetName("Restart necessary"), PopupButtons.OK, PopupType.Restart);
        }

        public static string GetIcon(string name)
        {
            return Path.Combine(Environment.CurrentDirectory, "GUI", "Icons",$"{name}.png");
        }

        //public static void ChangePropertyValue<T>(T obj, string propName, string value)where T:Base
        //{
        //    if (typeof(T) == typeof(Character))
        //    {
        //        var item = obj as Character;
        //        var match = item.Props.Find(p => p.Attribute.Name == propName);
        //        match.ChangeValue(obj,TypeName.Character, value);
        //    }
        //    if (typeof(T) == typeof(MainCharacter))
        //    {
        //        var item = obj as MainCharacter;
        //        var match = item.Props.Find(p => p.Attribute.Name == propName);
        //        match.ChangeValue(obj,TypeName.MainCharacter, value);
        //    }
        //    if (typeof(T) == typeof(Item))
        //    {
        //        var item = obj as Item;
        //        var match = item.Props.Find(p => p.Attribute.Name == propName);
        //        match.ChangeValue(obj,TypeName.ItemTable, value);
        //    }
        //}

        public static IEnumerable<T> FindWindowChildren<T>(DependencyObject dObj) where T : DependencyObject
        {
            if (dObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dObj); i++)
                {
                    DependencyObject ch = VisualTreeHelper.GetChild(dObj, i);
                    if (ch != null && ch is T)
                    {
                        yield return ( T ) ch;
                    }

                    foreach (T nestedChild in FindWindowChildren<T>(ch))
                    {
                        yield return nestedChild;
                    }
                }
            }
        }

        public static string MakeDeletionText(string objType, string objName)
        {
            var result = $"{TextCatalog.GetName("DeleteI")} {TextCatalog.GetName(objType)} {objName} {TextCatalog.GetName("DeleteII")}";
            return result;
        }

        public static string MakeParentNeccessaryText(string parentType,string objType, string objName)
        {
            var result = $"{TextCatalog.GetName("There is no corresponding")} {TextCatalog.GetName(parentType)} {TextCatalog.GetName("for")} {TextCatalog.GetName(objType)} {objName}.";
            return result;
        }

        public static bool IsDate(string date)
        {
            if (Regex.IsMatch(date, @"[0-9]{2}.[0-9]{2}.[0-9]{4}"))
                return true;
            return false;
        }

        public static void DeleteAllChildren<T>(T item) where T:Base
        {
            if(typeof(T) == typeof(Plot))
            {
                var plot = item as Plot;
                plot.SubplotManager.GetItems().ForEach(s =>
                {
                    s.EventManager.RemoveAllItems(TypeName.Event);
                });
                plot.SubplotManager.RemoveAllItems(TypeName.Subplot);
            }
            else if (typeof(T) == typeof(Lore))
            {
                var lore = item as Lore;
                lore.AftermathManager.RemoveAllItems(TypeName.Aftermath);
            }
            else if (typeof(T) == typeof(Character))
            {
                var character = item as Character;
                var mainCharacter = AppSettings_User.CurrentProject.CharacterManager.GetItems<MainCharacter>().Find(mC => mC.Character == character);
                if(mainCharacter !=null)
                    AppSettings_User.CurrentProject.CharacterManager.RemoveItem(mainCharacter, TypeName.MainCharacter);
            }
            else if (typeof(T) == typeof(Location))
            {
                var location = item as Location;
                location.WeatherManager.RemoveAllItems(TypeName.Weather);
            }
        }

        public class ParentConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var owner = GetOwner(AppSettings_User.CurrentProject, value.ToString());
                if (owner != null)
                    return $"{owner.Name} ({owner.ID})";
                else return string.Empty;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var ownerID = value.ToString().Split('(')[ 1 ].Replace(")", "");
                var owner = GetOwner(AppSettings_User.CurrentProject, ownerID);
                if (owner != null)
                    return ownerID;
                else return string.Empty;
            }
        }

        public class EnumConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return TextCatalog.GetName(value.ToString());
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return TextCatalog.GetSpecifier(value.ToString());
            }
        }

        public class DateConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (string.IsNullOrEmpty(value.ToString()))
                    return string.Empty;
                else
                {
                    var date = new DateTime();
                    DateTime.TryParse(value.ToString(), out date);
                    return date.ToString("dd.MM.yyyy");
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (string.IsNullOrEmpty(value.ToString()))
                    return string.Empty;
                else
                {
                    var date = new DateTime();
                    DateTime.TryParse(value.ToString(), out date);
                    return date.ToString("dd.MM.yyyy");
                }
            }
        }
    }
}
