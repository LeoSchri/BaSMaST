using System;
using System.Collections.Generic;
using System.IO;

namespace BaSMaST_V3
{
    public class Project : Base
    {
        private static long _projectNextID;
        private string _resourceLocation;
        private string _backupLocation;
        private string _logLocation;
        private DateTime _lastModified;

        public string Location { get; private set; }
        public string ResourceLocation
        {
            get { return _resourceLocation; }
            set
            {
                _resourceLocation = value;
                DBDataManager.UpdateDatabase(this, TypeName.Project.ToString(), "ResourceLocation");
            }
        }
        public string BackupLocation
        {
            get { return _backupLocation; }
            set
            {
                _backupLocation = value;
                DBDataManager.UpdateDatabase(this, TypeName.Project.ToString(), "BackupLocation");
            }
        }
        public string LogLocation
        {
            get { return _logLocation; }
            set
            {
                _logLocation = value;
                DBDataManager.UpdateDatabase(this, TypeName.Project.ToString(), "LogLocation");
            }
        }
        public DateTime Creation { get; private set; }
        public DateTime LastModified
        {
            get { return _lastModified; }
            set
            {
                _lastModified = value;
                DBDataManager.UpdateDatabase(this, TypeName.Project.ToString(), "LastModified");
            }
        }

        public Manager<Book> BookManager { get; set;}
        public Manager<Task> TaskManager { get; set; }
        public Manager<Event,Lore> Schedule { get; set; }
        public Manager<Plot, Lore> Archive { get; set; }
        public Manager<Character, MainCharacter> CharacterManager { get; set; }
        public List<ItemTable> ItemTables { get; set; }
        public Manager<Location> LocationManager { get; set; }
        public Manager<Repeat> RepeatManager { get; set; }
        public Manager<Resource> ResourceManager { get; set; }

        public LinkManager<Plot, Plot> PlotLinkManager;
        public LinkManager<Plot, Lore> PlotLoreLinkManager;
        public LinkManager<Lore, Lore> LoreLinkManager;
        public LinkManager<Event, Character> EventCharacterPresentLinkManager;
        public LinkManager<Event, Character> EventAttachmentCharacterLinkManager;
        public LinkManager<Event, Location> EventLocationLinkManager;
        public LinkManager<Event, Event> EventSourceLinkManager;
        public LinkManager<Base, Resource> ResourceLinkManager;
        public LinkManager<Base, Item> ItemLinkManager;
        public LinkManager<Event,EvolvementPhase> EvolvementLinkManager { get; set; }
        public LinkManager<Event,RelationshipPhase> RelationshipLinkManager { get; set; }

        public Project(string name, string location, string backupLocation, string id = null) : base($"{AppSettings_Static.TypeInfos[ TypeName.Project ].IDLetter}{_projectNextID++}", name)
        {
            if (string.IsNullOrEmpty(id))
            {
                Location = $@"{location}\{name}";
            }
            else
            {
                Location = location;
            }
            _resourceLocation = $@"{Location}\Resources";
            _backupLocation = backupLocation;
            _logLocation = $@"{Location}\Logs";
            Creation = DateTime.Now;
            _lastModified = DateTime.Now;

            AppSettings_User.AddProject(this);
            AppSettings_User.ChangeCurrentProject(this);

            BookManager = Manager<Book>.Create();
            TaskManager = Manager<Task>.Create();
            Schedule = Manager<Event,Lore>.Create();
            Archive = Manager<Plot,Lore>.Create();
            CharacterManager = Manager<Character, MainCharacter>.Create();
            LocationManager = Manager<Location>.Create();
            RepeatManager = Manager<Repeat>.Create();
            ResourceManager = Manager<Resource>.Create();

            PlotLinkManager = LinkManager<Plot, Plot>.Create();
            PlotLoreLinkManager = LinkManager<Plot, Lore>.Create();
            LoreLinkManager = LinkManager<Lore, Lore>.Create();
            EventCharacterPresentLinkManager = LinkManager<Event, Character>.Create();
            EventAttachmentCharacterLinkManager = LinkManager<Event, Character>.Create();
            EventLocationLinkManager = LinkManager<Event, Location>.Create();
            EventSourceLinkManager = LinkManager<Event, Event>.Create();
            ResourceLinkManager = LinkManager<Base, Resource>.Create();
            ItemLinkManager = LinkManager<Base, Item>.Create();
            EvolvementLinkManager = LinkManager<Event, EvolvementPhase>.Create();
            RelationshipLinkManager = LinkManager<Event, RelationshipPhase>.Create();

            if (string.IsNullOrEmpty(id))
            {
                Directory.CreateDirectory(Location);
                Directory.CreateDirectory(LogLocation);
                Directory.CreateDirectory(ResourceLocation);
                DBTableManager.CreateTables();

                DBDataManager.InsertIntoDatabase(this, TypeName.Project.ToString());
            }
            else
            {
                 SetID(id);
            }
            _projectNextID = Helper.GetNumeric(ID) + 1;

            if(!File.Exists($@"{Location}\ProjectConfig.xml"))
                File.Move($@"{Environment.CurrentDirectory}\ProjectConfigTemplate.xml",$@"{Location}\ProjectConfig.xml");
        }

        public void ChangeLocation(string loc)
        {
            Directory.Move(Location,loc);
            Location = loc;
            ResourceLocation= $@"{Location}\Resources";
            LogLocation = $@"{Location}\Logs";
            DBDataManager.UpdateDatabase(this, TypeName.Project.ToString(), "Location");
        }

        public void Backup()
        {
            Helper.CompressAndMoveDirectory(Location, BackupLocation, Name);
            DBConnector.ExecuteQuery($@"mysqldump --opt {Name} > {BackupLocation}\{Name}Backup{DateTime.Now}.dump");
        }

        public void AddItemTable(ItemTable table, List<Attribute> attributes)
        {
            ItemTables.Add(table);
            if (ItemTables.Count == 1)
                DBConnector.ExecuteQuery(DBDataManager.SqlQueryBuilder(TypeName.ItemLink));
            DBConnector.ExecuteQuery(DBDataManager.SqlQueryBuilder<ItemTable>(TypeName.ItemTable, table.Name));
            if (attributes != null)
                table.AddAttributes(attributes);
        }

        public void RemoveItemTables(List<ItemTable> tables)
        {
            tables.ForEach(t =>
            {
                ItemTables.Remove(t);
                DBConnector.ExecuteQuery($"DROP TABLE `{Name}`.`{t.Name}`;");
            });
        }
    }
}
