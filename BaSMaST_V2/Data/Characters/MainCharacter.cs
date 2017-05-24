
namespace BaSMaST_V3
{
    public class MainCharacter:AttributeHolder
    {
        private static long _mainCharacterNextID;

        public Character Character { get; private set; }

        public MainCharacter(string name, Character character, string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.MainCharacter ].IDLetter}{_mainCharacterNextID++}", name)
        {
            Character = character;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.MainCharacter.ToString());
            else SetID(id);
            _mainCharacterNextID = Helper.GetNumeric(ID)+2;
        }

        public void RemoveAllLinks()
        {
            AppSettings_User.CurrentProject.ResourceLinkManager.RemoveAllLinksForItem(this, TypeName.MainCharacter, TypeName.ResourceLink);
            RemoveAllLinks(TypeName.MainCharacter);
        }
    }
}
