
namespace BaSMaST_V3
{
    public class CharacterRelationships:Base
    {
        private static long _relationshipNextID;

        public Manager<Relationship> RelationshipManager { get; private set; }
        public Character Owner { get; private set; }

        public CharacterRelationships(string name, Character owner, string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.Relationships ].IDLetter}{_relationshipNextID++}",name)
        {
            RelationshipManager = Manager<Relationship>.Create();
            Owner = owner;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Relationship.ToString());
            else SetID(id);
            _relationshipNextID = Helper.GetNumeric(ID)+2;
        }
    }
}
