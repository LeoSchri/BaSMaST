
namespace BaSMaST_V3
{
    public class Location:Base
    {
        private static long _locationNextID;
        private Climate _climate;
        private Orientation _orientation;
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                DBDataManager.UpdateDatabase(this, TypeName.Location.ToString(), "Description");
            }
        }
        public Location Parent { get; private set; }
        public Climate Climate
        {
            get { return _climate; }
            set
            {
                _climate = value;
                DBDataManager.UpdateDatabase(this, TypeName.Event.ToString(), "Climate");
            }
        }
        public Manager<WeatherData> WeatherManager { get; private set; }
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                DBDataManager.UpdateDatabase(this,TypeName.Event.ToString(), "Orientation");
            }
        }

        public Location (string name, string desc, Location parent, Climate climate, Orientation ori,string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.Location ].IDLetter}{_locationNextID++}",name)
        {
            _description = desc;
            Parent = parent;
            Climate = climate;
            Orientation = ori;
            WeatherManager = Manager<WeatherData>.Create();

            if (string.IsNullOrEmpty(id))
            {
                DBDataManager.InsertIntoDatabase(this, TypeName.Location.ToString());
            }
            else
            {
                SetID(id);
            }
            _locationNextID = Helper.GetNumeric(ID) + 1;
        }

        public void ChangeParent(Location location)
        {
            Parent = location;
        }

        public void RemoveAllLinks()
        {
            AppSettings_User.CurrentProject.EventLocationLinkManager.RemoveAllLinksForItem(this,TypeName.Location,TypeName.LocationLink);
            RemoveAllLinks(TypeName.Location);
        }
    }
}
