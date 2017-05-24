using System;

namespace BaSMaST_V3
{

    public enum Occurrence
    {
        Daily,
        Weekly,
        EveryTwoWeeks,
        EveryThreeWeeks,
        Monthly,
        EveryHalfAYear,
        Yearly,
        Custom,
        None
    }

    public enum Mood
    {
        Happy,
        Sad,
        Worried,
        Sceptical,
        None
    }

    public enum Climate
    {
        TropicalZone,
        Subtropics,
        TemperateZone,
        ColdZone,
        None
    }

    public enum WeatherType
    {
        Sunny,
        PartialCloudy,
        Cloudy,
        Fog,
        Rain,
        Sleet,
        Snow,
        Hail,
        Storm,
        Thunderstorm,
        None
    }

    public enum Orientation
    {
        North,
        Northeast,
        Northwest,
        East,
        South,
        Southeast,
        Southwest,
        West,
        None
    }

    public enum LocationType
    {
        Castle,
        Palace,
        Tower,
        City,
        Town,
        Village,
        Farm,
        District,
        Marketplace,
        Dwelling,
        School,
        Stable,
        AmusementFacility,
        MilitaryFacility,
        AdministrativeFacility,
        HealthFacility,
        SupplyFacility,
        ServiceProvider,
        ReligiousFacility,
        Graveyard,
        Ornament,
        Garden,
        Park,
        Well,
        Lake,
        Pond,
        Spring,
        Waterfall,
        Stream,
        River,
        Sea,
        Mountain,
        Hill,
        Valley,
        Canyon,
        Swamp,
        Meadow,
        Forest,
        Field,
        Plantation,
        Desert,
    }

    public enum Relation
    {
        Romantic,
        Friendship,
        Family,
        Casual,
        Professional,
        None
    }

    public enum Closeness
    {
        Inseparable,
        VeryClose,
        Close,
        Distant,
        VeryDistant,
        Foreign,
        None
    }

    public enum TaskState
    {
        New,
        InProgress,
        Done,
        None
    }

    public enum FileType
    {
        TextDocument,
        Image,
        Other,
        None
    }

    public enum Importance
    {
        High,
        Medium,
        Low,
        None
    }

    public enum TraitType
    {
        Positive,
        Negative,
        Ambivalent
    }

    public enum DayTime
    {
        EarlyMorning,
        Morning,
        LateMorning,
        EarlyNoon,
        Noon,
        LateNoon,
        EarlyEvening,
        Evening,
        LateEvening,
        EarlyNight,
        MidNight,
        LateNight,
        None
    }

    public enum TypeName
    {
        Project,
        Book,
        Lore,
        LoreLink,
        LorePlotLink,
        Plot,
        Subplot,
        Event,
        EventLink,
        Character,
        MainCharacter,
        AttachmentFigure,
        CharacterPresent,
        Repeat,
        Relationships,
        Relationship,
        RelationshipPhase,
        RelationshipPhaseLink,
        Evolvement,
        EvolvementPhase,
        EvolvementPhaseLink,
        Location,
        LocationLink,
        Weather,
        Resource,
        ResourceLink,
        Task,
        Item,
        ItemTable,
        Aftermath,
        PlotLink,
        ItemLink,
        ItemOwner,
        ResourceOwner,
        Note,
        NoteLink,
        NoteOwner,
        //Additional Types
        Attributes,
        None
    }

    public enum Pages
    {
        None,
        Welcome,
        Projects,
        MainScreen,
        TableView,
        Locations
    }

    public enum Language
    {
        English,
        Deutsch
    }

    public enum PopupButtons
    {
        OK,
        YesNo,
        ApplyCancel
    }

    public enum PopupType
    {
        Overwrite,
        Restart,
        Delete,
        Save,
        Message
    }

    public class PointInTime
    {
        public DateTime? Date { get; set; }
        public DayTime DayTime { get; set; }

        public PointInTime(DateTime? date, DayTime dayTime)
        {
            Date = date;
            DayTime = dayTime;
        }
    }

    public class ColorSchema
    {
        public string Name { get; private set; }
        public string Color1 { get; private set; }
        public string Color2 { get; private set; }
        public string Color3 { get; private set; }

        public static string Primary1 { get; private set; } = "#FFF8F5EC";
        public static string Primary2 { get; private set; } = "#FFFFFDF7"; 

        private ColorSchema(string name, string c1, string c2, string c3)
        {
            Name = name;
            Color1 = c1;
            Color2 = c2;
            Color3 = c3;
        }

        public static ColorSchema Blue = new ColorSchema("Blue","#FF2C2A82", "#FF686797", "#FF9A9AB2");
        public static ColorSchema Red = new ColorSchema("Red", "#FFA32247","#FFBE768F","#FFDFBCC8");
        public static ColorSchema Green = new ColorSchema("Green", "#FF66A723", "#FF9FC379", "#FFD3E6C1");
        public static ColorSchema Gray = new ColorSchema("Gray", "#FF636363","#FF999999","#FFCECECE");
        public static ColorSchema Sun = new ColorSchema("Sun", "#FFFDC62D", "#FFFDDB7D", "#FFFEEAB3");

        public static ColorSchema GetColorSchemaByName(string name)
        {
            switch(name)
            {
                case "Blue": return Blue;
                case "Red": return Red;
                case "Green": return Green;
                case "Gray": return Gray;
                default: return null;
            }
        }
    }

    public class Attribute
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public bool AllowsNull { get; private set; }

        public Attribute(string name, Type type, Project p,TypeName tabletype, bool allowsNull = true, bool FromConfig = false)
        {
            Name = name;
            Type = type;
            AllowsNull = allowsNull;

            if(!FromConfig)
            {
                Helper.AddAttribute(p, tabletype, this);
                DBDataManager.AddColumnToTable(tabletype.ToString(), Name, Type, AllowsNull);
            }
        }
    }

    public class Property
    {
        public Attribute Attribute { get; private set; }
        public string Value { get; private set; }

        public Property(Attribute attribute, string value)
        {
            Attribute = attribute;
            Value = value;
        }

        public void ChangeValue(Base obj, TypeName type, string value)
        {
            Value = value;
            DBDataManager.UpdateDatabaseAttribute(obj, type.ToString(), Attribute, this);
        }
    }
}

