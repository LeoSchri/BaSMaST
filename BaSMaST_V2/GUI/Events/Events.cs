using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Linq;

namespace BaSMaST_V3
{
    public partial class Events
    {
        public static BrushConverter bc = new BrushConverter();

        public static void Load(object sender, RoutedEventArgs e)
        {
            MainWindow.Window.InitWindow();
        }

        public static void CloseApplication(object sender, MouseButtonEventArgs e)
        {
            //if(AppSettings_User.Projects != null && AppSettings_User.Projects.Any())
            //{
            //    AppSettings_User.Projects.ForEach(p =>
            //    {
            //        p.Backup();
            //    });
            //}

            DBDataManager.DisconnectFromDatabase();
            Application.Current.Shutdown();
        }

        //public static void Resize(object sender, MouseButtonEventArgs e)
        //{
        //    if (WindowState == WindowState.Maximized)
        //    {
        //        WindowState = WindowState.Normal;
        //    }
        //    else if (WindowState == WindowState.Normal)
        //    {
        //        WindowState = WindowState.Maximized;
        //    }
        //}

        //public static void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (WindowState == WindowState.Maximized)
        //    {
        //        AddImage(ResizeBtn, "Normalize");
        //    }
        //    else if (WindowState == WindowState.Normal)
        //    {
        //        AddImage(ResizeBtn, "FullScreen");
        //    }
        //}

        public static void Minimize(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Window.WindowState = WindowState.Minimized;
        }

        public static void Drag(object sender, MouseButtonEventArgs e)
        {
            try
            {
                while (e.ButtonState == MouseButtonState.Pressed && MainWindow.Window.WindowState == WindowState.Normal)
                    MainWindow.Window.DragMove();
            }
            catch (Exception)
            {

            }
        }

        public static void Reload(object sender, SizeChangedEventArgs e)
        {
            if (MainWindow.CurrentContent == Pages.Welcome)
                MainWindow.Window.LoadWelcomeScreen();
            if (MainWindow.CurrentContent == Pages.Projects)
                MainWindow.Window.LoadProjectsScreen();
            if (MainWindow.CurrentContent == Pages.MainScreen)
                MainWindow.Window.LoadMainScreen();
        }

        public static void SearchButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.Window.SearchText.Text == "Search . . .")
            {
                MainWindow.Window.SearchText.Text = string.Empty;
            }
        }

        public static void SearchEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //SearchFor(Search.Text);
                MainWindow.Window.SearchText.Text = "Search . . .";
            }
        }

        public static void MenuClick(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.Window.Menu.IsVisible)
            {
                MainWindow.Window.Menu.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindow.Window.Menu.Visibility = Visibility.Visible;
            }
        }

        public static void SettingsClick(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.Window.Settings.IsVisible)
            {
                MainWindow.Window.Settings.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindow.Window.Settings.Visibility = Visibility.Visible;
            }
        }

        public static void LoadProjects(object sender, MouseButtonEventArgs e)
        {
            if (sender == MainWindow.Window.MenuStack2)
                MainWindow.Window.Menu.Visibility = Visibility.Collapsed;
            MainWindow.Window.LoadProjectsScreen();
        }

        public static void LoadProject(object sender, MouseButtonEventArgs e)
        {
            AppSettings_User.ChangeCurrentProject(AppSettings_User.Projects.Find(p => p.Name == ( sender as Button ).Content.ToString()));
            MainWindow.Window.EnableProjectEdit();
            MainWindow.Window.LoadMainScreen();
            DBDataManager.GetProjectData(AppSettings_User.CurrentProject);
            MainWindow.Window.HomeBtn.IsEnabled = true;
            MainWindow.Window.TaskBtn.IsEnabled = true;
        }

        public static void LoadProjectConfig(object sender, MouseButtonEventArgs e)
        {
            if(sender== MainWindow.Window.MenuStack1 || sender == MainWindow.Window.NewProject)
                MainWindow.Window.LoadProjectConfiguration();
            if(sender == MainWindow.Window.MenuStack3)
                MainWindow.Window.LoadProjectConfiguration(AppSettings_User.CurrentProject);
            MainWindow.Window.Menu.Visibility = Visibility.Collapsed;
        }

        public static void DeleteProject(object sender, MouseButtonEventArgs e)
        {
            Popup.ShowWindow(Helper.MakeDeletionText("Project",AppSettings_User.CurrentProject.Name), TextCatalog.GetName("Confirm deletion"), PopupButtons.YesNo, PopupType.Delete, AppSettings_User.CurrentProject);
        }

        public static void LoadTasks(object sender, MouseButtonEventArgs e)
        {
            Config.ShowWindow(TypeName.Task);
        }

        public static void LoadMainScreen(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Window.LoadMainScreen();
        }

        public static void LoadAttributeConfig(object sender, MouseButtonEventArgs e)
        {
            if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Character)
            {
                if (AppSettings_User.CurrentProject.CharacterManager != null && AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>() != null && AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>().Any())
                    Config.ShowWindow(TypeName.Attributes, AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>().FirstOrDefault());
                else
                {
                    var dummy = new Character("DummyCharacter", false, "C7");
                    Config.ShowWindow(TypeName.Attributes, dummy);
                }
            }
            else if (TableManager.CurrentTables.FirstOrDefault() == TypeName.MainCharacter)
            {
                if (AppSettings_User.CurrentProject.CharacterManager != null && AppSettings_User.CurrentProject.CharacterManager.GetItems<MainCharacter>() != null && AppSettings_User.CurrentProject.CharacterManager.GetItems<MainCharacter>().Any())
                    Config.ShowWindow(TypeName.Attributes, AppSettings_User.CurrentProject.CharacterManager.GetItems<MainCharacter>().FirstOrDefault());
                else
                {
                    var dummy = new MainCharacter("DummyCharacter", new Character("DummyCharacter2",true, "C7"), "MC7");
                    Config.ShowWindow(TypeName.Attributes, dummy);
                }
            }
        }

        public static void MainScreenElementClick(object sender, MouseButtonEventArgs e)
        {
            switch((sender as Button).Name)
            {
                case "Schedule": break;
                case "Archive": TableManager.LoadTables(TypeName.Plot, TypeName.Subplot, TypeName.Event); break;
                case "Characters": TableManager.LoadTables(TypeName.Character); break;
                case "Relationships": break;
                case "CharacterEvolvement": break;
                case "Locations": MainWindow.Window.LoadLocations(); break;
            }
        }

        public static void ToggleButtonMouseEnter(object sender, MouseEventArgs e)
        {
            if(sender is Button)
                ( sender as Button ).Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            if (sender is DockPanel)
                ( sender as DockPanel ).Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
        }

        public static void ToggleButtonMouseLeave(object sender, MouseEventArgs e)
        {
            if(sender is Button)
                ( sender as Button ).Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color2);
            if (sender is DockPanel)
                ( sender as DockPanel ).Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
        }

        public static void UpDownMouseEnter(object sender, MouseEventArgs e)
        {
            ( sender as Button ).Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
        }

        public static void UpDownMouseLeave(object sender, MouseEventArgs e)
        {
            ( sender as Button ).Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color2);
        }

        public static void ToggleButtonClick(object sender, MouseButtonEventArgs e)
        {
            var comboBox = MainWindow.ComboBoxes.Find(c => c.ComboBoxToggleButton == ( sender as Button ));
            if (comboBox.ComboBoxItemStackBorder.IsVisible)
            {
                comboBox.ComboBoxItemStackBorder.Visibility = Visibility.Collapsed;
                comboBox.ComboBoxImage.Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowDown{AppSettings_User.ColorSchema.Name}")));
            }
            else
            {
                comboBox.ComboBoxItemStackBorder.Visibility = Visibility.Visible;
                comboBox.ComboBoxImage.Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowUp{AppSettings_User.ColorSchema.Name}")));
            }
        }

        public static void ChangeSelectedItem(object sender, MouseButtonEventArgs e)
        {
            var comboBox = MainWindow.ComboBoxes.Find(c => c.ComboBoxItems.Find(i => i == ( sender as Button )) != null);
            comboBox.ComboBoxTextBox.Text = ( sender as Button ).Content.ToString();
            comboBox.ComboBoxItemStackBorder.Visibility = Visibility.Collapsed;
            switch (comboBox.Name)
            {
                case "Language":
                    if (Helper.GetType<Language>(( sender as Button ).Content.ToString()) != AppSettings_User.Language)
                        Helper.UpdateConfig("Language", ( sender as Button ).Content.ToString());
                    break;
                case "FontSize":
                    if (Convert.ToInt32(( sender as Button ).Content.ToString()) != AppSettings_User.FontSize)
                        Helper.UpdateConfig("FontSize", ( sender as Button ).Content.ToString());
                    break;
            }
        }

        public static void PickSchema(object sender, MouseButtonEventArgs e)
        {
            if (sender == MainWindow.Window.Blue)
                if (AppSettings_User.ColorSchema != ColorSchema.Blue)
                    Helper.UpdateConfig("ColorSchema", "Blue");
            if (sender == MainWindow.Window.Red)
                if (AppSettings_User.ColorSchema != ColorSchema.Red)
                    Helper.UpdateConfig("ColorSchema", "Red");
            if (sender == MainWindow.Window.Green)
                if (AppSettings_User.ColorSchema != ColorSchema.Green)
                    Helper.UpdateConfig("ColorSchema", "Green");
            if (sender == MainWindow.Window.Sun)
                if (AppSettings_User.ColorSchema != ColorSchema.Sun)
                    Helper.UpdateConfig("ColorSchema", "Sun");
            if (sender == MainWindow.Window.Gray)
                if (AppSettings_User.ColorSchema != ColorSchema.Gray)
                    Helper.UpdateConfig("ColorSchema", "Gray");
        }

        public static void ChangeContent(object sender, MouseButtonEventArgs e)
        {
            var index = MainWindow.Window.ContentPages.IndexOf(MainWindow.Window.ContentPages.Find(p => p.Name == MainWindow.Window.CurrentPanel.Name));

            if (sender == MainWindow.Window.PageDown)
            {
                MainWindow.Window.CurrentPanel.Visibility = Visibility.Collapsed;
                MainWindow.Window.PageUp.Visibility = Visibility.Visible;

                MainWindow.Window.CurrentPanel = MainWindow.Window.ContentPages[ index + 1 ];
                MainWindow.Window.CurrentPanel.Visibility = Visibility.Visible;
                if (MainWindow.Window.ContentPages.IndexOf(MainWindow.Window.CurrentPanel) == MainWindow.Window.ContentPages.Count - 1)
                    MainWindow.Window.PageDown.Visibility = Visibility.Collapsed;
            }

            if (sender == MainWindow.Window.PageUp)
            {
                MainWindow.Window.CurrentPanel.Visibility = Visibility.Collapsed;
                MainWindow.Window.PageDown.Visibility = Visibility.Visible;

                MainWindow.Window.CurrentPanel = MainWindow.Window.ContentPages[ index -1 ];
                MainWindow.Window.CurrentPanel.Visibility = Visibility.Visible;
                if (MainWindow.Window.ContentPages.IndexOf(MainWindow.Window.CurrentPanel) == 0)
                    MainWindow.Window.PageUp.Visibility = Visibility.Collapsed;
            }
        }
    }
}
