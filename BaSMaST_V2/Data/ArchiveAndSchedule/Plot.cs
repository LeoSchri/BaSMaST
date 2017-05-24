
namespace BaSMaST_V3
{
    public class Plot : Base
    {
        private static long _plotNextID;
        private bool _isSuperplot;
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                DBDataManager.UpdateDatabase(this, TypeName.Plot.ToString(), "Description");
            }
        }
        public bool IsSuperPlot
        { get { return _isSuperplot; }
            set {
                    _isSuperplot = value;
                    DBDataManager.UpdateDatabase(this, TypeName.Plot.ToString(), "IsSuperPlot");
                }
        }

        public Manager<Subplot> SubplotManager { get; set; }

        public Plot(string name, string desc, bool isSuper, string id = null) : base($"{AppSettings_Static.TypeInfos[ TypeName.Plot ].IDLetter}{_plotNextID}", name)
        {
            _isSuperplot = isSuper;
            _description = desc;

            SubplotManager = Manager<Subplot>.Create();

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Plot.ToString());
            else
            {
                SetID(id);
            }
            _plotNextID = Helper.GetNumeric(ID)+2;
        }

        public void RemoveAllLinks()
        {
            AppSettings_User.CurrentProject.PlotLinkManager.RemoveAllLinksForItem(this, TypeName.Plot, TypeName.PlotLink);
            AppSettings_User.CurrentProject.PlotLoreLinkManager.RemoveAllLinksForItem(this, TypeName.Plot, TypeName.LorePlotLink);
            RemoveAllLinks(TypeName.Plot);
        }
    }
}
