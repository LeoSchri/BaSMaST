using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace BaSMaST_V3
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Window
    {
        public static Config Window { get; set; }
        public static BrushConverter bc = new BrushConverter();
        public static TypeName Type { get; private set; }

        public Config()
        {
            InitializeComponent();
            Loaded += Load;
        }

        public static void ShowWindow(TypeName type, Base obj = null)
        {
            Window = new Config();

            Type = type;

            switch (type)
            {
                case TypeName.Project: Window.ProjectStack.Visibility = Visibility.Visible; Window.Project = obj as Project; break;
                case TypeName.Task: Window.TaskDock.Visibility = Visibility.Visible; break;
                case TypeName.Attributes: Window.AttributesDock.Visibility = Visibility.Visible;
                                            Window.AttributesFor = Helper.GetType<TypeName>(obj.GetType().ToString().Split('.')[ 1 ]);
                                            if(TableManager.RevertChanges.Visibility == Visibility.Visible)
                                                Popup.ShowWindow(TextCatalog.GetName("SaveChanges?"),TextCatalog.GetName("SaveChanges"),PopupButtons.YesNo,PopupType.Save);
                                            break;
            }

            Window.ShowDialog();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            ApplySetter();
        }

        private void ApplySetter()
        {
            WindowTitle.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            WindowTitle.FontSize = AppSettings_User.FontSize * 1.5;
            WindowTitle.FontFamily = new FontFamily(AppSettings_Static.Font2);
            WindowTitle.MouseDown += Drag;

            var ApplyImage = new Image();
            ApplyImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Tick")));
            ApplyImage.Height = Apply.ActualHeight * 0.6;
            ApplyImage.MaxWidth = Apply.ActualWidth * 0.6;
            Apply.Content = ApplyImage;
            Apply.BorderThickness = new Thickness(0);
            Apply.PreviewMouseLeftButtonDown += Click;

            var CancelImage = new Image();
            CancelImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Cancel")));
            CancelImage.Height = Cancel.ActualHeight * 0.6;
            CancelImage.MaxWidth = Cancel.ActualWidth * 0.6;
            Cancel.Content = CancelImage;
            Cancel.BorderThickness = new Thickness(0);
            Cancel.PreviewMouseLeftButtonDown += Click;

            if (Type == TypeName.Project)
            {
                WindowTitle.Content = TextCatalog.GetName("Project configuration");
                ApplyProjectSetter();
            }
            if (Type == TypeName.Task)
            {
                WindowTitle.Content = TextCatalog.GetName("Tasks");
                ApplyTaskSetter();
            }
            if (Type == TypeName.Attributes)
            {
                WindowTitle.Content = TextCatalog.GetName("Attributes");
                ApplyAttributeSetter();
            }
        }

        public void Drag(object sender, MouseButtonEventArgs e)
        {
            try
            {
                while (e.ButtonState == MouseButtonState.Pressed)
                    DragMove();
            }
            catch (Exception)
            {

            }
        }

        private static void ButtonMouseEnter(object sender, MouseEventArgs e)
        {
            ( sender as Button ).Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
        }

        private static void ButtonMouseLeave(object sender, MouseEventArgs e)
        {
            ( sender as Button ).Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color2);
        }

        private static void Click(object sender, MouseButtonEventArgs e)
        {
            if (sender == Window.Apply && !Window.ValidateData())
                return;
            if (Window.Project != null)
            {
                if (sender == Window.Apply)
                    if(Type == TypeName.Project)
                        Window.SaveProjectChanges();
            }
            else
            {
                if (sender == Window.Apply)
                {
                    if (Type == TypeName.Project)
                    {
                        Window.AddProject();
                        MainWindow.Window.LoadMainScreen();
                        MainWindow.Window.EnableProjectEdit();
                    }
                }
            }

            if(sender == Window.Cancel && MainWindow.CurrentContent == Pages.TableView)
            {
                Events.ReloadTable(null, null);
            }

            Window.Close();
            Window = null;
        }

        private bool ValidateData()
        {
            if (Type == TypeName.Project)
                return ValidateProject();
            if (Type == TypeName.Task)
                return ValidateTask();
            if (Type == TypeName.Attributes)
                return ValidateAttribute();
            return true;
        }

        private void BrowserClick(object sender, MouseButtonEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.RootFolder = Environment.SpecialFolder.MyDocuments;
                
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    if(sender == LocationBrowser)
                        ProjectLocation.Text = fbd.SelectedPath;
                    if (sender == BackupBrowser)
                        ProjectBackup.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
