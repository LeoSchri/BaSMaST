using System.Linq;

namespace BaSMaST_V3
{
    public class Character:AttributeHolder
    {
        private static long _characterNextID;
        private bool _isMainCharacter;

        public bool IsMainCharacter
        {
            get { return _isMainCharacter; }
            set
            {
                _isMainCharacter = value;

                if (AppSettings_User.CurrentProject.CharacterManager.GetItems<MainCharacter>() != null && AppSettings_User.CurrentProject.CharacterManager.GetItems<MainCharacter>().Any())
                {
                    if (_isMainCharacter && AppSettings_User.CurrentProject.CharacterManager.GetItems<MainCharacter>().Find(mC => mC.Character == this) == null)
                        AppSettings_User.CurrentProject.CharacterManager.AddItem(new MainCharacter(Name,this));
                }
                else if(AppSettings_User.CurrentProject.CharacterManager.GetItems<MainCharacter>() == null || !AppSettings_User.CurrentProject.CharacterManager.GetItems<MainCharacter>().Any())
                    AppSettings_User.CurrentProject.CharacterManager.AddItem(new MainCharacter(Name, this));

                DBDataManager.UpdateDatabase(this, TypeName.Character.ToString(), "IsMainCharacter");
            }
        }
        public Manager<Relationship> RelationshipManager { get; set; }
        public Manager<EvolvementPhase> EvolvementManager { get; set; }

        public Character(string name, bool isMain = false, string id = null, params Property[] properties) :base($"{AppSettings_Static.TypeInfos[ TypeName.Character ].IDLetter}{_characterNextID++}", name)
        {
            _isMainCharacter = isMain;
            RelationshipManager = Manager<Relationship>.Create();
            EvolvementManager = Manager<EvolvementPhase>.Create();

            Props = properties.ToList();

            if (string.IsNullOrEmpty(id))
            {
                DBDataManager.InsertIntoDatabase(this, TypeName.Character.ToString());
                if(isMain)
                    AppSettings_User.CurrentProject.CharacterManager.AddItem(new MainCharacter(Name, this));
            }
            else SetID(id);
        }
    }
}
