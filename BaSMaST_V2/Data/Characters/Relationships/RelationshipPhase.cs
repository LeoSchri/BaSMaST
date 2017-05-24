using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSMaST_V3
{
    public class RelationshipPhase:Base
    {
        private static long _relationshipPhaseNextID;
        private Relation _relation;
        private Closeness _closeness;
        private string _opinion;

        public Relationship Relationship { get; private set; }
        public Relation Relation
        {
            get { return _relation; }
            set
            {
                _relation = value;
                DBDataManager.UpdateDatabase(this, TypeName.RelationshipPhase.ToString(), "Relation");
            }
        }
        public Closeness Closeness
        {
            get { return _closeness; }
            set
            {
                _closeness = value;
                DBDataManager.UpdateDatabase(this, TypeName.RelationshipPhase.ToString(), "Closeness");
            }
        }
        public string OpinionOfTheOther
        {
            get { return _opinion; }
            set
            {
                _opinion = value;
                DBDataManager.UpdateDatabase(this, TypeName.RelationshipPhase.ToString(), "OpinionOfTheOther");
            }
        }
        public PointInTime Begin { get; private set; }
        public PointInTime End { get; private set; }

        public RelationshipPhase(string name, Relationship relationship, Relation relation, Closeness closeness, string opinion,string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.RelationshipPhase ].IDLetter}{_relationshipPhaseNextID++}", name)
        {
            Relationship = relationship;
            _relation = relation;
            _closeness = closeness;
            _opinion = opinion;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.RelationshipPhase.ToString());
            else SetID(id);
            _relationshipPhaseNextID = Helper.GetNumeric(ID) + 1;
        }

        public void RemoveAllLinks()
        {
            AppSettings_User.CurrentProject.EvolvementLinkManager.RemoveAllLinksForItem(this, TypeName.RelationshipPhase, TypeName.RelationshipPhaseLink);
            RemoveAllLinks(TypeName.RelationshipPhase);
        }
    }
}
