using System.Collections.Generic;

namespace BaSMaST_V3
{
    public class EvolvementPhase:Base
    {
        private static long _evolvementPhaseNextID;
        private string _goalsAndIntention;

        public Character Owner { get; private set; }
        public string GoalsAndIntention
        {
            get { return _goalsAndIntention; }
            set
            {
                _goalsAndIntention = value;
                DBDataManager.UpdateDatabase(this, TypeName.EvolvementPhase.ToString(), "GoalsAndIntention");
            }
        }
        public List<Trait> Traits { get; private set; }
        public PointInTime Begin { get; private set; }
        public PointInTime End { get;private set; }

        public EvolvementPhase(string name, string goalsAndIntention, Character owner, string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.EvolvementPhase ].IDLetter}{_evolvementPhaseNextID++}", name)
        {
            _goalsAndIntention = goalsAndIntention;
            Owner = owner;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.EvolvementPhase.ToString());
            else SetID(id);
            _evolvementPhaseNextID = Helper.GetNumeric(ID)+2;
        }

        public void AddTraits(List<Trait> traits)
        {
            Traits.AddRange(traits);
        }

        public void RemoveTraits(List<Trait> traits)
        {
            traits.ForEach(t =>
            {
                Traits.Remove(t);
            });
        }

        public void RemoveAllLinks()
        {
            AppSettings_User.CurrentProject.EvolvementLinkManager.RemoveAllLinksForItem(this, TypeName.EvolvementPhase, TypeName.EvolvementPhaseLink);
            RemoveAllLinks(TypeName.EvolvementPhase);
        }
    }
}
