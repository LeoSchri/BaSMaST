
namespace BaSMaST_V3
{
    public class Relationship:Base
    {
        private static long _relationshipNextID;

        public Character OtherParty { get; private set; }
        public Character Owner { get; private set; }
        public Manager<RelationshipPhase> PhaseManager { get; private set; }

        public Relationship(string name, Character owner, Character otherParty, string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.Relationship ].IDLetter}{_relationshipNextID++}",name)
        {
            Owner = owner;
            OtherParty = otherParty;
            PhaseManager = Manager<RelationshipPhase>.Create();

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Relationship.ToString());
            else SetID(id);
            _relationshipNextID = Helper.GetNumeric(ID) + 1;
        }
    }
}
