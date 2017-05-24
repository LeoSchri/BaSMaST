
namespace BaSMaST_V3
{
    public class Subplot : Base
    {
        private static long _subplotNextID;
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                DBDataManager.UpdateDatabase(this, TypeName.Subplot.ToString(), "Description");
            }
        }
        public Plot Parent { get; private set; }
        public Manager<Event> EventManager { get; set; }

        public Subplot(string name, string desc, Plot parent, string id =null) : base($"{AppSettings_Static.TypeInfos[ TypeName.Subplot ].IDLetter}{_subplotNextID++}", name)
        {
            _description = desc;
            Parent = parent;

            EventManager = Manager<Event>.Create();

            if(string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Subplot.ToString());
            else
            {
                SetID(id);
            }
            _subplotNextID = Helper.GetNumeric(ID)+2;
        }

        public void ChangeParent(Plot plot)
        {
            Parent.SubplotManager.RemoveItem( this, TypeName.Subplot);
            Parent = plot;
            plot.SubplotManager.AddItem( this);
        }

        public void RemoveAllLinks()
        {
            RemoveAllLinks(TypeName.Subplot);
        }
    }
}
