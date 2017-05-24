using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSMaST_V3
{
    public partial class DBDataManager
    {
        public static bool GetProjectData(Project p)
        {
            try
            {
                AppSettings_User.ChangeCurrentProject(p);

                Helper.SetProjectSpecificConfig(p);

                GetCharactersPerProject(p);
                GetMainCharactersPerProject(p);
                GetAllPlotsPerProject(p);
                GetAllLorePerProject(p);
                GetLinkedPlotsForAllPlots(p);
                GetLinkedPlotsForAllLore(p);
                GetLinkedLoreForAllLore(p);
                GetBooksPerProject(p);
                GetTasksPerProject(p);
                GetLocationsPerProject(p);
                //GetRepeatsPerProject(p);
                GetResourcesPerProject(p);
                GetOwnersForAllResources(p);
                GetNotesPerProject(p);
                GetItemTablesPerProject(p);
                GetOwnersForAllItems(p);
                GetAllLinksForAllEvents(p);
            }
            catch (Exception e)
            {
                Log.ErrorOut($"GetProjectData({p.Name})",e);
                return false;
            }
            return true;
        }

        private static bool GetAllPlotsPerProject(Project p)
        {
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Plot.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var plot = AddPlot(data.Rows[i]);
                    GetAllSubplotsPerPlot(plot, p);
                    if (p.Archive == null)
                        p.Archive = Manager<Plot, Lore>.Create();
                    p.Archive.AddItem(plot);
                }
            }
            catch (Exception e)
            {
                Log.Error(p, $"GetAllPlotsPerProject({p.Name})",e);
                return false;
            }
            return true;
        }

        public static Plot AddPlot(System.Data.DataRow row)
        {
            var plot = new Plot(row[ "Name" ].ToString(),
                                        row[ "Description" ].ToString(),
                                        row[ "IsSuperPlot" ].ToString() == "True" ? true : false,
                                        row[ $"id{TypeName.Plot}" ].ToString());
           
            return plot;
        }

        private static bool GetAllSubplotsPerPlot(Plot p, Project pro)
        {
            var subplots = new List<Subplot>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Subplot.ToString(), new Dictionary<string, string>() { { "Parent", p.ID } }, pro.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    subplots.Add(AddSubplot(p, data.Rows[i]));
                }
                subplots.ForEach(s => { GetAllEventsPerSubplot(s, pro); });
                p.SubplotManager.AddItems(subplots);
            }
            catch (Exception e)
            {
                Log.Error(pro,$"GetAllSubplotsPerPlot({p.Name},{pro.Name})",e);
                return false;
            }
            return true;
        }

        public static Subplot AddSubplot(Plot p, System.Data.DataRow row)
        {
            var subplot = new Subplot(row[ "Name" ].ToString(),
                                        row[ "Description" ].ToString(),
                                        p,
                                        row[ $"id{TypeName.Subplot}" ].ToString());

            return subplot;
        }

        private static bool GetAllEventsPerSubplot(Subplot s, Project p)
        {
            var events = new List<Event>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Event.ToString(), new Dictionary<string, string>() { { "Parent", s.ID } }, p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    events.Add(AddEvent(s, data.Rows[i]));
                }
                s.EventManager.AddItems(events);
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetAllEventsPerSubplot({s.Name},{p.Name})",e);
                return false;
            }
            return true;
        }

        public static Event AddEvent(Subplot s, System.Data.DataRow row)
        {
            DateTime? beginDate = null;
            DateTime? endDate = null;
            DateTime bDate;
            DateTime eDate;

            if (DateTime.TryParse(row[ "BeginDate" ].ToString(), out bDate))
                beginDate = bDate;
            if (DateTime.TryParse(row[ "EndDate" ].ToString(), out eDate))
                endDate = eDate;

            var ev = new Event(row[ "Name" ].ToString(),
                                        row[ "Description" ].ToString(),
                                        s,
                                        new PointInTime(beginDate, Helper.GetType<DayTime>(row[ "BeginDaytime" ].ToString())),
                                        new PointInTime(endDate, Helper.GetType<DayTime>(row[ "EndDaytime" ].ToString())),
                                        s.EventManager==null || s.EventManager.GetItems()==null || !s.EventManager.GetItems().Any() ? null:s.EventManager.GetItems().Find(e => e.ID == row[ "Source" ].ToString()),
                                        id: row[ $"id{TypeName.Event}" ].ToString());

            return ev;
        }

        private static bool GetAllLorePerProject(Project p)
        {
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Lore.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var lore = AddLore(data.Rows[i]);
                    GetAftermathePerLore(p, lore);
                    if (p.Archive == null)
                        p.Archive = Manager<Plot, Lore>.Create();
                    p.Archive.AddItem(lore);
                }
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetAllLorePerProject({p.Name})",e);
                return false;
            }
            return true;
        }

        public static Lore AddLore(System.Data.DataRow row)
        {
            DateTime? beginDate = null;
            DateTime? endDate = null;
            DateTime bDate;
            DateTime eDate;

            if (DateTime.TryParse(row[ "BeginDate" ].ToString(), out bDate))
                beginDate = bDate;
            if (DateTime.TryParse(row[ "EndDate" ].ToString(), out eDate))
                endDate = eDate;

            var lore = new Lore(row[ "Name" ].ToString(),
                                        row[ "Description" ].ToString(),
                                        Helper.GetType<Importance>(row[ "Importance" ].ToString()),
                                        beginDate,
                                        endDate,
                                        id: row[ $"id{TypeName.Lore}" ].ToString());
            return lore;
        }

        private static bool GetAftermathePerLore(Project p, Lore l)
        {
            var aftermath = new List<Aftermath>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Aftermath.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    aftermath.Add(AddAftermath(l, data.Rows[i]));
                }
                    l.AftermathManager.AddItems(aftermath);
            }
            catch (Exception e)
            {
                Log.Error(p, $"GetAllLorePerProject({p.Name})", e);
                return false;
            }
            return true;
        }

        public static Aftermath AddAftermath(Lore lore, System.Data.DataRow row)
        {
            DateTime? endDate = null;
            DateTime eDate;

            if (DateTime.TryParse(row[ "EndDate" ].ToString(), out eDate))
                endDate = eDate;

            var aftermath = new Aftermath(row[ "Name" ].ToString(),
                                        row[ "Description" ].ToString(),
                                        endDate,
                                        lore,
                                        row[ $"id{TypeName.Aftermath}" ].ToString());

            return aftermath;
        }

        private static bool GetBooksPerProject(Project p)
        {
            var books = new List<Book>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Book.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    books.Add(new Book(data.Rows[ i ][ "Name" ].ToString(),
                                        DateTime.Parse(data.Rows[ i ][ "EndDate" ].ToString()),
                                        DateTime.Parse(data.Rows[ i ][ "EndDate" ].ToString()),
                                        data.Rows[ i ][ $"id{TypeName.Book}" ].ToString()));
                }
                if (p.BookManager == null)
                    p.BookManager = Manager<Book>.Create();
                p.BookManager.AddItems(books);
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetBooksPerProject({p.Name})",e);
                return false;
            }
            return true;
        }

        private static bool GetTasksPerProject(Project p)
        {
            var tasks = new List<Task>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Task.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    tasks.Add(new Task(data.Rows[ i ][ "Name" ].ToString(),
                                        Helper.GetType<TaskState>(data.Rows[ i ][ "State" ].ToString()),
                                        Helper.GetType<Importance>(data.Rows[ i ][ "Importance" ].ToString()),
                                        DateTime.Parse(data.Rows[ i ][ "DueDate" ].ToString()),
                                        data.Rows[ i ][ $"id{TypeName.Task}" ].ToString()));
                }
                if (p.TaskManager == null)
                    p.TaskManager = Manager<Task>.Create();
                p.TaskManager.AddItems(tasks);
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetTasksPerProject({p.Name})",e);
                return false;
            }
            return true;
        }

        private static bool GetCharactersPerProject(Project p)
        {
            var characters = new List<Character>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Character.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var character = AddCharacter(data.Rows[ i ]);
                    GetEvolvementPhasesPerCharacter(character, p);
                    GetRelationshipsPerCharacter(character, p);
                    characters.Add(character);
                }
                if (p.CharacterManager == null)
                    p.CharacterManager = Manager<Character, MainCharacter>.Create();
                p.CharacterManager.AddItems(characters);
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetCharactersPerProject({p.Name})",e);
                return false;
            }
            return true;
        }

        public static Character AddCharacter(System.Data.DataRow row)
        {
            var character = new Character(row[ "Name" ].ToString(),
                                            row[ "IsMainCharacter" ].ToString() == "True" ? true : false,
                                            row[ $"id{TypeName.Character}" ].ToString());
            character.ApplyProps();

            if (character.Props != null && character.Props.Any())
            {
                character.Props.ForEach(p =>
                {
                    p.ChangeValue(character,TypeName.Character,row[ p.Attribute.Name ].ToString());
                });
            }

            return character;
        }

        private static bool GetMainCharactersPerProject(Project p)
        {
            var characters = new List<MainCharacter>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.MainCharacter.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    characters.Add(AddMainCharacter(data.Rows[i], p));
                }
                if (p.CharacterManager == null)
                    p.CharacterManager = Manager<Character, MainCharacter>.Create();
                p.CharacterManager.AddItems(characters);
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetMainCharactersPerProject({p.Name})",e);
                return false;
            }
            return true;
        }

        public static MainCharacter AddMainCharacter(System.Data.DataRow row, Project pro)
        {
            var character = new MainCharacter(row[ "Name" ].ToString(),
                                        pro.CharacterManager.GetItems<Character>().Find(c => c.ID == row[ "Character" ].ToString()),
                                        row[ $"id{TypeName.MainCharacter}" ].ToString());
            character.ApplyProps();

            if (character.Props != null && character.Props.Any())
            {
                character.Props.ForEach(p =>
                {
                    p.ChangeValue(character, TypeName.MainCharacter, row[ p.Attribute.Name ].ToString());
                });
            }

            return character;
        }

        private static bool GetRelationshipsPerCharacter(Character character,Project p)
        {
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Relationship.ToString(), new Dictionary<string, string>() { { "Owner", character.ID } }, p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var owner = p.CharacterManager.GetItems<Character>().Find(c => c.ID == data.Rows[ i ][ "Owner" ].ToString());
                    var otherParty = p.CharacterManager.GetItems<Character>().Find(c => c.ID == data.Rows[ i ][ "OtherParty" ].ToString());
                    var r = new Relationship(data.Rows[ i ][ "Name" ].ToString(),
                                           owner,
                                           otherParty,
                                           data.Rows[ i ][ $"id{TypeName.Relationship}" ].ToString());
                    if (owner.RelationshipManager == null)
                        owner.RelationshipManager = Manager<Relationship>.Create();
                    owner.RelationshipManager.AddItem(r);
                    GetRelationshipPhasesPerRelationship(r, p);
                }
            }
            catch (Exception e)
            {
                Log.Error(p, $"GetRelationshipsPerProject({p.Name})", e);
                return false;
            }
            return true;
        }

        private static bool GetRelationshipPhasesPerRelationship(Relationship r, Project p)
        {
            var phases = new List<RelationshipPhase>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.RelationshipPhase.ToString(), new Dictionary<string, string>() { { "Relationship", r.ID } }, p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    phases.Add(new RelationshipPhase(data.Rows[ i ][ "Name" ].ToString(),
                                                    r,
                                                    Helper.GetType<Relation>(data.Rows[ i ][ "Relation" ].ToString()),
                                                    Helper.GetType<Closeness>(data.Rows[ i ][ "Closeness" ].ToString()),
                                                    data.Rows[ i ][ "OpinionOfTheOther" ].ToString(),
                                                    data.Rows[ i ][ $"id{TypeName.RelationshipPhase}" ].ToString()));
                }
                r.PhaseManager.AddItems(phases);
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetRelationshipPhasesPerRelationship({r.Name}{p.Name})",e);
                return false;
            }
            return true;
        }

        //private static bool GetEvolvementsPerProject(Project p)
        //{
        //    try
        //    {
        //        var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Evolvement.ToString(), schema: p.Name);
        //        if (data.Rows.Count == 0)
        //            return false;
        //        for (int i = 0; i < data.Rows.Count; i++)
        //        {
        //            var owner = p.CharacterManager.GetItems<Character>().Find(c => c.ID == data.Rows[ i ][ "Owner" ].ToString());
        //            var e= new CharacterEvolvement(data.Rows[ i ][ "Name" ].ToString(),
        //                                   owner,
        //                                   id: data.Rows[ i ][ $"id{TypeName.Relationship}" ].ToString());
        //            GetEvolvementPhasesPerEvolvement(e,p);
        //            owner.SetEvolvement(e);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(p,$"GetEvolvementsPerProject({p.Name})",e);
        //        return false;
        //    }
        //    return true;
        //}

        private static bool GetEvolvementPhasesPerCharacter(Character character, Project p)
        {
            var phases = new List<EvolvementPhase>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.EvolvementPhase.ToString(), new Dictionary<string, string>() { { "Owner",character.ID } }, p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    phases.Add(new EvolvementPhase(data.Rows[ i ][ "Name" ].ToString(),
                                                    data.Rows[ i ][ "GoalsAndIntention" ].ToString(),
                                                    character,
                                                    data.Rows[ i ][ $"id{TypeName.EvolvementPhase}" ].ToString()));
                }
                character.EvolvementManager.AddItems(phases);
            }
            catch (Exception ex)
            {
                Log.Error(p,$"GetEvolvementPhasesPerCharacter({character.Name}{p.Name})",ex);
                return false;
            }
            return true;
        }

        private static bool GetLocationsPerProject(Project p)
        {
            var locations = new List<Location>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Location.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    locations.Add(new Location(data.Rows[ i ][ "Name" ].ToString(),
                                                data.Rows[ i ][ "Description" ].ToString(),
                                                p.LocationManager.GetItems().Find(l => l.ID == data.Rows[ i ][ "Parent" ].ToString()),
                                                Helper.GetType<Climate>(data.Rows[ i ][ "Climate" ].ToString()),
                                                Helper.GetType<Orientation>(data.Rows[ i ][ "Orientation" ].ToString()),
                                                id:data.Rows[ i ][ $"id{TypeName.Location}" ].ToString()));
                }
                if (p.LocationManager == null)
                    p.LocationManager = Manager<Location>.Create();
                p.LocationManager.AddItems(locations);
                p.LocationManager.GetItems().ForEach(l =>
                {
                    GetAllWeatherDataPerLocation(l, p);
                });
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetLocationsPerProject({p.Name})",e);
                return false;
            }
            return true;
        }

        private static bool GetAllWeatherDataPerLocation(Location l, Project p)
        {
            var weather = new List<WeatherData>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Weather.ToString(), new Dictionary<string, string>() { { "Location", l.ID } }, p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var beginDate = new DateTime();
                    var endDate = new DateTime();
                    DateTime.TryParse(data.Rows[ i ][ "BeginDate" ].ToString(), out beginDate);
                    DateTime.TryParse(data.Rows[ i ][ "EndDate" ].ToString(), out endDate);

                    weather.Add(new WeatherData(data.Rows[ i ][ "Name" ].ToString(),
                                                new PointInTime(beginDate, Helper.GetType<DayTime>(data.Rows[ i ][ "BeginDaytime" ].ToString())),
                                                new PointInTime(endDate, Helper.GetType<DayTime>(data.Rows[ i ][ "EndDaytime" ].ToString())),
                                                Helper.GetType<WeatherType>(data.Rows[ i ][ "WeatherType" ].ToString()),
                                                data.Rows[ i ][ $"id{TypeName.Weather}" ].ToString()));
                }
                l.WeatherManager.AddItems(weather);
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetAllWeatherDataPerLocation({l.Name}{p.Name})",e);
                return false;
            }
            return true;
        }

        //private static bool GetRepeatsPerProject(Project p)
        //{
        //    try
        //    {
        //        var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Repeat.ToString(), schema: p.Name);
        //        if (data.Rows.Count == 0)
        //            return false;
        //        for (int i = 0; i < data.Rows.Count; i++)
        //        {
        //            var source = Helper.FindEvent(p, data.Rows[ i ][ "Source" ].ToString());
        //            var r = new Repeat(data.Rows[ i ][ "Name" ].ToString(),
        //                                        source,
        //                                        source.Clones,
        //                                        Helper.GetType<Occurrence>(data.Rows[ i ][ "Occurrence" ].ToString()),
        //                                        DateTime.Parse(data.Rows[ i ][ "EndDate" ].ToString()),
        //                                        DateTime.Parse(data.Rows[ i ][ "EndDate" ].ToString()),
        //                                        id: data.Rows[ i ][ $"id{TypeName.Repeat}" ].ToString());
        //            if (p.RepeatManager == null)
        //                p.RepeatManager = Manager<Repeat>.Create();
        //            p.RepeatManager.AddItem(r);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(p,$"GetRepeatsPerProject({p.Name})",e);
        //        return false;
        //    }
        //    return true;
        //}

        private static bool GetResourcesPerProject(Project p)
        {
            var resources = new List<Resource>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Resource.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    resources.Add(new Resource(data.Rows[ i ][ "Name" ].ToString(),
                                                data.Rows[ i ][ "FilePath" ].ToString(),
                                                id: data.Rows[ i ][ $"id{TypeName.Repeat}" ].ToString()));
                }
                if (p.ResourceManager == null)
                    p.ResourceManager = Manager<Resource>.Create();
                p.ResourceManager.AddItems(resources);
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetResourcesPerProject({p.Name})",e);
                return false;
            }
            return true;
        }

        private static bool GetOwnersForAllResources(Project pro)
        {
            var owners = new List<Base>();
            try
            {
                if (pro.ResourceManager == null || pro.ResourceManager.GetItems() == null || !pro.ResourceManager.GetItems().Any())
                    return true;
                pro.ResourceManager.GetItems().ForEach(r =>
                {
                    var data = DBConnector.Select(new List<string>() { "*" }, TypeName.ResourceLink.ToString(), new Dictionary<string, string>() { { "Resource", r.ID } }, pro.Name);
                    if (data.Rows.Count != 0)
                    {
                        for (int i = 0; i < data.Rows.Count; i++)
                        {
                            owners.Add(Helper.GetOwner(pro,data.Rows[ i ][ "Owner" ].ToString()));
                        }
                        owners.ForEach(o =>
                        {
                            pro.ResourceLinkManager.AddLink(o, r, TypeName.ResourceLink);
                        });
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error(pro,$"GetOwnersForAllResources({pro.Name})",e);
                return false;
            }
            return true;
        }

        private static bool GetNotesPerProject(Project p)
        {
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Note.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;

                if (p.NoteManager == null)
                    p.NoteManager = Manager<Note>.Create();

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var owner = Helper.GetOwner(p, data.Rows[ i ][ "Owner" ].ToString());
                    var note = new Note(data.Rows[ i ][ "Name" ].ToString(),
                                                data.Rows[ i ][ "Content" ].ToString(),
                                                owner,
                                                data.Rows[ i ][ $"id{TypeName.Note}" ].ToString());
                    
                    p.NoteManager.AddItem(note);
                    owner.NoteManager.AddItem(note);
                }
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetNotesPerProject({p.Name})",e);
                return false;
            }
            return true;
        }

        public static bool GetItemTablesPerProject(Project p)
        {
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.ItemTable.ToString(), schema: p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var table = new ItemTable(data.Rows[ i ][ "Name" ].ToString(),
                                        data.Rows[ i ][ "TableLetter" ].ToString(),
                                        data.Rows[ i ][ "Icon" ].ToString(),
                                        data.Rows[ i ][ $"id{TypeName.ItemTable}" ].ToString());
                    GetItemsPerItemTable(table, p);
                }
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetItemTablesPerProject({p.Name})", e);
                return false;
            }
            return true;
        }

        private static bool GetItemsPerItemTable(ItemTable t,Project p)
        {
            var items = new List<Item>();
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, $"{t.Name}{TypeName.ItemTable}", schema:p.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    items.Add(new Item(t,
                                        data.Rows[ i ][ "Name" ].ToString(),
                                        data.Rows[ i ][ $"id{t.Name}" ].ToString()));
                }
                t.AddItems(items);
            }
            catch (Exception e)
            {
                Log.Error(p,$"GetBooksPerProject({p.Name})", e);
                return false;
            }
            return true;
        }

        private static bool GetOwnersForAllItems(Project pro)
        {
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.ItemLink.ToString(), schema: pro.Name);
                if (data.Rows.Count != 0)
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        var item = Helper.FindItem(pro,data.Rows[ i ][ "Item" ].ToString());
                        var owner = Helper.GetOwner(pro, data.Rows[ i ][ "Owner" ].ToString());

                        pro.ItemLinkManager.AddLink(owner,item,TypeName.ItemLink);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(pro,$"GetOwnersForAllItems({pro.Name})", e);
                return false;
            }
            return true;
        }

        private static bool GetLinkedPlotsForAllPlots(Project pro)
        {
            try
            {
                if (pro.Archive == null || pro.Archive.GetItems<Plot>() == null || !pro.Archive.GetItems<Plot>().Any())
                    return true;
                pro.Archive.GetItems<Plot>().ForEach(p =>
                {
                    var data = DBConnector.Select(new List<string>() { "*" }, TypeName.PlotLink.ToString(), new Dictionary<string, string>() { { "Plot", p.ID } }, pro.Name);
                    if (data.Rows.Count != 0)
                    {
                        for (int i = 0; i < data.Rows.Count; i++)
                        {
                            pro.PlotLinkManager.AddLink(p,pro.Archive.GetItems<Plot>().Find(plot => plot.ID == data.Rows[ i ][ $"OtherPlot" ].ToString()),TypeName.PlotLink,true);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error(pro,$"GetLinkedPlotsForAllPlots({pro.Name})",e);
                return false;
            }
            return true;
        }

        private static bool GetLinkedPlotsForAllLore(Project pro)
        {
            try
            {
                if (pro.Archive == null || pro.Archive.GetItems<Lore>() == null || !pro.Archive.GetItems<Lore>().Any())
                    return true;
                pro.Archive.GetItems<Lore>().ForEach(l =>
                {
                    var data = DBConnector.Select(new List<string>() { "*" }, TypeName.LorePlotLink.ToString(), new Dictionary<string, string>() { { "Lore", l.ID } }, pro.Name);
                    if (data.Rows.Count != 0)
                    {
                        for (int i = 0; i < data.Rows.Count; i++)
                        {
                            pro.PlotLoreLinkManager.AddLink(pro.Archive.GetItems<Plot>().Find(plot => plot.ID == data.Rows[ i ][ $"Plot" ].ToString()),l, TypeName.LorePlotLink, true);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error(pro, $"GetLinkedPlotsForAllLore({pro.Name})", e);
                return false;
            }
            return true;
        }

        private static bool GetLinkedLoreForAllLore(Project pro)
        {
            try
            {
                if (pro.Archive == null || pro.Archive.GetItems<Lore>() == null || !pro.Archive.GetItems<Lore>().Any())
                    return true;
                pro.Archive.GetItems<Lore>().ForEach(l =>
                {
                    var data = DBConnector.Select(new List<string>() { "*" }, TypeName.LoreLink.ToString(), new Dictionary<string, string>() { { "Lore", l.ID } }, pro.Name);
                    if (data.Rows.Count != 0)
                    {
                        for (int i = 0; i < data.Rows.Count; i++)
                        {
                            pro.LoreLinkManager.AddLink(pro.Archive.GetItems<Lore>().Find(lore => lore.ID == data.Rows[ i ][ $"OtherLore" ].ToString()), l, TypeName.LoreLink, true);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error(pro, $"GetLinkedLoreForAllLore({pro.Name})", e);
                return false;
            }
            return true;
        }

        private static void GetAllLinksForAllEvents(Project pro)
        {
            if (pro.Archive == null || pro.Archive.GetItems<Plot>()==null || !pro.Archive.GetItems<Plot>().Any())
                return;
            pro.Archive.GetItems<Plot>().ForEach(p =>
            {
                if(p.SubplotManager !=null && p.SubplotManager.GetItems() != null && p.SubplotManager.GetItems().Any())
                {
                    p.SubplotManager.GetItems().ForEach(s =>
                    {
                        if(s.EventManager!=null && s.EventManager.GetItems() != null && s.EventManager.GetItems().Any())
                        {
                            s.EventManager.GetItems().ForEach(e =>
                            {
                                //SetClonesForEvent(e);
                                GetAttachmentFiguresForEvent(e,pro);
                                GetCharactersPresentForEvent(e, pro);
                                GetLocationsForEvent(e, pro);
                            });
                        }
                    });
                }
            });
        }

        private static bool GetAttachmentFiguresForEvent(Event ev,Project pro)
        {
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.AttachmentFigure.ToString(), new Dictionary<string, string>() { { "Event", ev.ID } }, pro.Name);
                if (data.Rows.Count == 0)
                    return false;
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        pro.EventAttachmentCharacterLinkManager.AddLink(ev,pro.CharacterManager.GetItems<Character>().Find(c => c.ID == data.Rows[ i ][ $"Character" ].ToString()), TypeName.AttachmentFigure);
                    }
            }
            catch (Exception e)
            {
                Log.Error(pro,$"GetAttachmentFiguresForEvent({ev.Name}{pro.Name})",e);
                return false;
            }
            return true;
        }

        private static bool GetCharactersPresentForEvent(Event ev, Project pro)
        {
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.CharacterPresent.ToString(), new Dictionary<string, string>() { { "Event", ev.ID } }, pro.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    pro.EventCharacterPresentLinkManager.AddLink(ev,pro.CharacterManager.GetItems<Character>().Find(c => c.ID == data.Rows[ i ][ $"Character" ].ToString()), TypeName.CharacterPresent);
                }
            }
            catch (Exception e)
            {
                Log.Error(pro,$"GetCharactersPresentForEvent({ev.Name}{pro.Name})",e);
                return false;
            }
            return true;
        }

        private static bool GetLocationsForEvent(Event ev, Project pro)
        {
            try
            {
                var data = DBConnector.Select(new List<string>() { "*" }, TypeName.LocationLink.ToString(), new Dictionary<string, string>() { { "Event", ev.ID } }, pro.Name);
                if (data.Rows.Count == 0)
                    return false;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    pro.EventLocationLinkManager.AddLink(ev,pro.LocationManager.GetItems().Find(l => l.ID == data.Rows[ i ][ $"Location" ].ToString()),TypeName.LocationLink);
                }
            }
            catch (Exception e)
            {
                Log.Error(pro,$"GetLocationsForEvent({ev.Name}{pro.Name})",e);
                return false;
            }
            return true;
        }

        //private static void SetClonesForEvent(Event e)
        //{
        //    if (e.Source == null)
        //        e.SetClones();
        //}
    }
}
