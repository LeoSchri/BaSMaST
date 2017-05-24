
namespace BaSMaST_V3
{
    public class CharacterEvolvement:Base
    {
        private static long _evolvementNextID;

        public Character Owner { get; private set; }
        public Manager<EvolvementPhase> PhaseManager { get; private set; }

        public CharacterEvolvement(string name, Character owner, string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.Evolvement ].IDLetter}{_evolvementNextID++}",name)
        {
            Owner = owner;
            PhaseManager = Manager<EvolvementPhase>.Create();

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Evolvement.ToString());
            else SetID(id);
            _evolvementNextID = Helper.GetNumeric(ID)+2;
        }
    }
}
