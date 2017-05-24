using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSMaST_V3
{
    public static class AppSettings_Static
    {
        public class TypeInfo
        {
            public TypeName Name { get; private set; }
            public string IDLetter { get; private set; }
            public List<Link> Links { get; private set; }

            public TypeInfo(TypeName name, string idLetter, params Link[] links)
            {
                Name = name;
                IDLetter = idLetter;
                Links = links.ToList();
            }
        }

        public class Link
        {
            public TypeName Name { get; private set; }
            public string PropName { get; private set; }

            public Link(TypeName name, string prop= null)
            {
                Name = name;
                if (string.IsNullOrEmpty(prop))
                    PropName = name.ToString();
                else PropName = prop;
            }
        }


        public static Dictionary<TypeName,TypeInfo> TypeInfos { get; private set; } =  new Dictionary<TypeName, TypeInfo>()
        {
                {TypeName.Project, new TypeInfo(TypeName.Project, "PJ")},
                {TypeName.Book, new TypeInfo(TypeName.Book, "B")},
                {TypeName.Lore, new TypeInfo(TypeName.Lore, "LR")},
                {TypeName.Plot, new TypeInfo(TypeName.Plot, "P")},
                {TypeName.Subplot, new TypeInfo(TypeName.Subplot, "S", new Link(TypeName.Plot,"Parent"))},
                {TypeName.Event, new TypeInfo(TypeName.Event, "E",new Link(TypeName.Subplot,"Parent"),new Link(TypeName.Event,"Source"))},
                {TypeName.Character, new TypeInfo(TypeName.Character, "C")},
                {TypeName.MainCharacter, new TypeInfo(TypeName.MainCharacter, "MC",new Link(TypeName.Character))},
                {TypeName.AttachmentFigure, new TypeInfo(TypeName.AttachmentFigure, "AF",new Link(TypeName.Event), new Link(TypeName.Character))},
                {TypeName.CharacterPresent, new TypeInfo(TypeName.CharacterPresent, "CP", new Link(TypeName.Event), new Link(TypeName.Character))},
                {TypeName.Repeat, new TypeInfo(TypeName.Repeat, "RT",new Link(TypeName.Event,"Source"))},
                //{TypeName.Relationships, new TypeInfo(TypeName.Relationships, "RL")},
                {TypeName.Relationship, new TypeInfo(TypeName.Relationship, "R",new Link(TypeName.Character,"Owner"),new Link(TypeName.Character,"OtherParty"))},
                {TypeName.RelationshipPhase, new TypeInfo(TypeName.RelationshipPhase, "RP",new Link(TypeName.Relationship))},
                {TypeName.RelationshipPhaseLink, new TypeInfo(TypeName.RelationshipPhaseLink, "RL",new Link(TypeName.Event), new Link(TypeName.RelationshipPhase))},
                //{TypeName.Evolvement, new TypeInfo(TypeName.Evolvement, "V",new Link(TypeName.Character,"Owner"))},
                {TypeName.EvolvementPhase, new TypeInfo(TypeName.EvolvementPhase, "VP")},
                {TypeName.EvolvementPhaseLink, new TypeInfo(TypeName.EvolvementPhaseLink, "VL",new Link(TypeName.Event),new Link(TypeName.EvolvementPhase))},
                {TypeName.Location, new TypeInfo(TypeName.Location, "L",new Link(TypeName.Location,"Parent"))},
                {TypeName.LocationLink, new TypeInfo(TypeName.LocationLink, "LL", new Link(TypeName.Event), new Link(TypeName.Location))},
                {TypeName.Weather, new TypeInfo(TypeName.Weather, "W")},
                {TypeName.Resource, new TypeInfo(TypeName.Resource, "RS")},
                {TypeName.ResourceLink, new TypeInfo(TypeName.ResourceLink, "RK",new Link(TypeName.Resource), new Link(TypeName.ResourceOwner, "Owner"))},
                {TypeName.Task, new TypeInfo(TypeName.Task, "T")},
                {TypeName.Item, new TypeInfo(TypeName.Item, "I")},
                {TypeName.ItemTable, new TypeInfo(TypeName.ItemTable, "IT")},
                {TypeName.ItemLink, new TypeInfo(TypeName.ItemLink, "IL",new Link(TypeName.Item),new Link(TypeName.ItemOwner,"Owner"))},
                {TypeName.Aftermath, new TypeInfo(TypeName.Aftermath, "AM",new Link(TypeName.Lore,"Parent"))},
                {TypeName.PlotLink, new TypeInfo(TypeName.PlotLink, "PL",new Link(TypeName.Plot), new Link(TypeName.Plot,"OtherPlot"))},
                {TypeName.LoreLink, new TypeInfo(TypeName.LoreLink, "LL",new Link(TypeName.Lore), new Link(TypeName.Lore,"OtherLore"))},
                {TypeName.LorePlotLink, new TypeInfo(TypeName.LorePlotLink, "LP",new Link(TypeName.Lore), new Link(TypeName.Plot))},
                {TypeName.EventLink, new TypeInfo(TypeName.EventLink, "E",new Link(TypeName.Event),new Link(TypeName.Event,"Clone"))},
                {TypeName.Note, new TypeInfo(TypeName.Note, "N")},
        };

        public static string Server { get; private set; } = "localhost";
        public static string User { get; private set; } = "root";
        public static string Password { get; private set; } = "";

        public static string Font1 { get; private set; } = "Lucida Calligraphy";
        public static string Font2 { get; private set; } = "Arial";
    }
}
