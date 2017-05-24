
namespace BaSMaST_V3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            Helper.SetConfig();
            DBDataManager.ConnectToDatabase();
            DBDataManager.SynchronizeProjects();
            InitializeComponent();

            Window = this;
            ButtonWidth = 185;

            //SizeChanged += WindowSizeChanged;
            Screen.Loaded += Events.Load;
        }
    }
}
