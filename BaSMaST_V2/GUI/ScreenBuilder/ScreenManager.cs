using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaSMaST_V3
{
    public partial class MainWindow
    {
        public static MainWindow Window { get; set; }

        public TextBox SearchText { get; set; }
        public static List<ComboBox> ComboBoxes { get; set; }

        public static Pages CurrentContent { get; set; }

        public int ButtonWidth { get; private set; }

        public static BrushConverter bc = new BrushConverter();

        public void InitWindow()
        {
            ApplyColorSchema();
            FormatControls();
            AddImages();
            AddEventHandler();

            if (AppSettings_User.Projects != null && AppSettings_User.Projects.Any())
                EnableProjectLoad();
            else DisableProjectLoad();
        }

        private void ApplyColorSchema()
        {
            MenuStrip.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            Menu.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color3);
            Settings.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color3);

            WindowTitle.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            ScreenTitle.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            NewProjectLabel.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            OpenProjectLabel.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            ConfigureProjectLabel.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            DeleteProjectLabel.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            CloseLabel.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);

            MenuBtn.Style = ( Style ) MenuBtn.FindResource($"MenuStripButton{AppSettings_User.ColorSchema.Name}");
            HomeBtn.Style = ( Style ) HomeBtn.FindResource($"MenuStripButton{AppSettings_User.ColorSchema.Name}");
            TaskBtn.Style = ( Style ) TaskBtn.FindResource($"MenuStripButton{AppSettings_User.ColorSchema.Name}");
            HelpBtn.Style = ( Style ) HelpBtn.FindResource($"MenuStripButton{AppSettings_User.ColorSchema.Name}");
            SettingsBtn.Style = ( Style ) SettingsBtn.FindResource($"MenuStripButton{AppSettings_User.ColorSchema.Name}");

            NewProjectBtn.Style= ( Style ) NewProjectBtn.FindResource($"Field");
            OpenProjectBtn.Style = ( Style ) OpenProjectBtn.FindResource($"Field");
            ConfigureProjectBtn.Style = ( Style ) ConfigureProjectBtn.FindResource($"Field");
            DeleteProjectBtn.Style = ( Style ) DeleteProjectBtn.FindResource($"Field");
            CloseApplicationBtn.Style = ( Style ) CloseApplicationBtn.FindResource($"Field");

            MenuStack1.Style = ( Style ) MenuStack1.FindResource($"MenuStackPanel{AppSettings_User.ColorSchema.Name}");
            MenuStack2.Style = ( Style ) MenuStack1.FindResource($"MenuStackPanel{AppSettings_User.ColorSchema.Name}");
            MenuStack3.Style = ( Style ) MenuStack1.FindResource($"MenuStackPanel{AppSettings_User.ColorSchema.Name}");
            MenuStack4.Style = ( Style ) MenuStack1.FindResource($"MenuStackPanel{AppSettings_User.ColorSchema.Name}");
            MenuStack5.Style = ( Style ) MenuStack1.FindResource($"MenuStackPanel{AppSettings_User.ColorSchema.Name}");

            SearchBtn.Style = ( Style ) SearchBtn.FindResource($"SearchButton{AppSettings_User.ColorSchema.Name}");

            Seperator.Style = ( Style ) Seperator.FindResource($"MenuStripButton");
            Seperator1.Style = ( Style ) Seperator1.FindResource($"MenuStripButton");
        }

        private void FormatControls()
        {
            WindowTitle.Content = "BASMAST";
            WindowTitle.FontSize = AppSettings_User.FontSize * 1.5;
            WindowTitle.FontFamily = new FontFamily(AppSettings_Static.Font2);

            ScreenTitle.HorizontalContentAlignment = HorizontalAlignment.Center;
            ScreenTitle.VerticalContentAlignment = VerticalAlignment.Center;
            ScreenTitle.FontSize = AppSettings_User.FontSize * 1.25;
            ScreenTitle.FontFamily = new FontFamily(AppSettings_Static.Font1);

            NewProjectLabel.Content = TextCatalog.GetName("New Project");
            NewProjectLabel.Margin = new Thickness(5);
            NewProjectLabel.FontSize = AppSettings_User.FontSize*1.15;
            NewProjectLabel.FontFamily = new FontFamily(AppSettings_Static.Font1);

            OpenProjectLabel.Content = TextCatalog.GetName("Open Project");
            OpenProjectLabel.Margin = new Thickness(5);
            OpenProjectLabel.FontSize = AppSettings_User.FontSize * 1.15;
            OpenProjectLabel.FontFamily = new FontFamily(AppSettings_Static.Font1);

            ConfigureProjectLabel.Content = TextCatalog.GetName("Edit Project");
            ConfigureProjectLabel.Margin = new Thickness(5);
            ConfigureProjectLabel.FontSize = AppSettings_User.FontSize * 1.15;
            ConfigureProjectLabel.FontFamily = new FontFamily(AppSettings_Static.Font1);

            DeleteProjectLabel.Content = TextCatalog.GetName("Delete Project");
            DeleteProjectLabel.Margin = new Thickness(5);
            DeleteProjectLabel.FontSize = AppSettings_User.FontSize * 1.15;
            DeleteProjectLabel.FontFamily = new FontFamily(AppSettings_Static.Font1);

            CloseLabel.Content = TextCatalog.GetName("Exit");
            CloseLabel.Margin = new Thickness(5);
            CloseLabel.FontSize = AppSettings_User.FontSize * 1.15;
            CloseLabel.FontFamily = new FontFamily(AppSettings_Static.Font1);

            MenuStack3.Visibility = Visibility.Hidden;
            MenuStack4.Visibility = Visibility.Hidden;
            if (AppSettings_User.Projects == null || !AppSettings_User.Projects.Any())
                MenuStack2.Visibility = Visibility.Hidden;

            Menu.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;

            CreateSearchButtonContent(SearchBtn);

            ComboBoxes= new List<ComboBox>();
            new ComboBox("Language", Settings, 170, 30, VerticalAlignment.Top, HorizontalAlignment.Center, Dock.Top, new Thickness(10), Helper.GetTypesAsList<Language>(),AppSettings_User.Language.ToString(),false,true);
            new ComboBox("FontSize", Settings,170,30,VerticalAlignment.Top, HorizontalAlignment.Center, Dock.Top, new Thickness(10), new List<string>() {"11","12","14","16"},AppSettings_User.FontSize.ToString(),false,true);
            CreateColorPicker();

            BuildField();
        }

        private void AddEventHandler()
        {
            CloseBtn.PreviewMouseLeftButtonDown += Events.CloseApplication;
            //ResizeBtn.PreviewMouseLeftButtonDown += Resize;
            MinimizeBtn.PreviewMouseLeftButtonDown += Events.Minimize;
            TitleBar.MouseDown += Events.Drag;
            SearchBtn.PreviewMouseLeftButtonDown += Events.SearchButtonClick;
            SearchBtn.PreviewKeyDown += Events.SearchEnter;
            MenuBtn.PreviewMouseLeftButtonDown += Events.MenuClick;
            SettingsBtn.PreviewMouseLeftButtonDown += Events.SettingsClick;
            HomeBtn.PreviewMouseLeftButtonDown += Events.LoadMainScreen;
            TaskBtn.PreviewMouseLeftButtonDown += Events.LoadTasks;
            MenuStack1.PreviewMouseLeftButtonDown += Events.LoadProjectConfig;
            MenuStack2.PreviewMouseLeftButtonDown += Events.LoadProjects;
            MenuStack3.PreviewMouseLeftButtonDown += Events.LoadProjectConfig;
            MenuStack4.PreviewMouseLeftButtonDown += Events.DeleteProject;
            MenuStack5.PreviewMouseLeftButtonDown += Events.CloseApplication;
        }

        private void AddImages()
        {
            AddImage(CloseBtn, "Cancel");
            //AddImage(ResizeBtn, "Normalize");
            AddImage(MinimizeBtn, "Minimize");
            AddImage(NewProjectBtn, "NewProject",0.9);
            AddImage(OpenProjectBtn, "OpenProject", 0.9);
            AddImage(ConfigureProjectBtn, "EditProject", 0.9);
            AddImage(DeleteProjectBtn, "DeleteProject", 0.9);
            AddImage(CloseApplicationBtn, "Exit", 0.9);

            AddImage(MenuBtn, $"Menu{AppSettings_User.ColorSchema.Name}");
            AddImage(HomeBtn, $"Home{AppSettings_User.ColorSchema.Name}");
            AddImage(TaskBtn, $"Tasks{AppSettings_User.ColorSchema.Name}",0.8);
            AddImage(HelpBtn, $"Help{AppSettings_User.ColorSchema.Name}");
            AddImage(SettingsBtn, $"Settings{AppSettings_User.ColorSchema.Name}");
            AddImage(Seperator, $"Seperator{AppSettings_User.ColorSchema.Name}");
            AddImage(Seperator1, $"Seperator{AppSettings_User.ColorSchema.Name}");
        }

        public void AddImage(Button btn, string imgName, double imgProcent = 0.6)
        {
            var Img = new Image();
            Img.Source = new BitmapImage(new Uri(Helper.GetIcon(imgName)));
            Img.Width = btn.Width* imgProcent;
            Img.Height = btn.Height* imgProcent;

            btn.Content = Img;
            btn.HorizontalContentAlignment = HorizontalAlignment.Center;
            btn.VerticalContentAlignment = VerticalAlignment.Center;
        }

        private void CreateColorPicker()
        {
            Blue.PreviewMouseLeftButtonDown += Events.PickSchema;
            Blue.Style = ( Style ) MenuStack1.FindResource("SettingsStackPanel");
            AddColorSchema(ColorSchema.Blue, Blue);

            Red.PreviewMouseLeftButtonDown += Events.PickSchema;
            Red.Style = ( Style ) MenuStack1.FindResource("SettingsStackPanel");
            AddColorSchema(ColorSchema.Red, Red);

            Green.PreviewMouseLeftButtonDown += Events.PickSchema;
            Green.Style = ( Style ) MenuStack1.FindResource("SettingsStackPanel");
            AddColorSchema(ColorSchema.Green, Green);

            Sun.PreviewMouseLeftButtonDown += Events.PickSchema;
            Sun.Style = ( Style ) MenuStack1.FindResource("SettingsStackPanel");
            AddColorSchema(ColorSchema.Sun, Sun);

            Gray.PreviewMouseLeftButtonDown += Events.PickSchema;
            Gray.Style = ( Style ) MenuStack1.FindResource("SettingsStackPanel");
            AddColorSchema(ColorSchema.Gray, Gray);
        }

        private static void AddColorSchema(ColorSchema color, StackPanel parent)
        {
            var btn = new Button();
            btn.Style = ( Style ) btn.FindResource("ColorButton");
            btn.BorderBrush = null;
            btn.BorderThickness = new Thickness(0);
            btn.Margin = new Thickness(5,2,5,2);
            btn.Background = ( Brush ) bc.ConvertFrom(color.Color1);
            btn.Width = 170;
            btn.Height =15;

            var btn2 = new Button();
            btn2.Style = ( Style ) btn2.FindResource("ColorButton");
            btn2.BorderBrush = null;
            btn2.BorderThickness = new Thickness(5, 2, 5, 2);
            btn2.Margin = new Thickness(2);
            btn2.Background = ( Brush ) bc.ConvertFrom(color.Color2);
            btn2.Width = 170;
            btn2.Height = 15;

            var btn3 = new Button();
            btn3.Style = ( Style ) btn3.FindResource("ColorButton");
            btn3.BorderBrush = null;
            btn3.BorderThickness = new Thickness(5, 2, 5, 5);
            btn3.Margin = new Thickness(2);
            btn3.Background = ( Brush ) bc.ConvertFrom(color.Color3);
            btn3.Width = 170;
            btn3.Height = 15;

            parent.Children.Add(btn);
            parent.Children.Add(btn2);
            parent.Children.Add(btn3);
        }

        private void CreateSearchButtonContent(Button parent)
        {
            var littleDock = new DockPanel();
            littleDock.LastChildFill = true;

            var SearchBtnImage = new Image();
            SearchBtnImage.Source = new BitmapImage(new Uri(Helper.GetIcon($"Search{AppSettings_User.ColorSchema.Name}")));
            SearchBtnImage.Width = SearchBtn.MaxWidth * 0.1;
            SearchBtnImage.Height = SearchBtnImage.Width;
            SearchBtnImage.Margin = new Thickness(0, 0, 5, 0);
            SearchBtnImage.SetValue(DockPanel.DockProperty, Dock.Right);

            SearchText = new TextBox();
            SearchText.Text = $"{TextCatalog.GetName("Search")} . . .";
            SearchText.Width = SearchBtn.Width * 0.85;
            SearchText.Height = SearchBtn.Height * 0.8;
            SearchText.Margin = new Thickness(2.5, 5, 0, 0);
            SearchText.VerticalAlignment = VerticalAlignment.Center;
            SearchText.HorizontalAlignment = HorizontalAlignment.Left;
            SearchText.BorderBrush = null;
            SearchText.BorderThickness = new Thickness(0);
            SearchText.FontSize = AppSettings_User.FontSize;
            SearchText.FontFamily = new FontFamily(AppSettings_Static.Font2);
            SearchText.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color3);
            SearchText.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            SearchText.SetValue(DockPanel.DockProperty, Dock.Left);

            littleDock.Children.Add(SearchBtnImage);
            littleDock.Children.Add(SearchText);
            parent.Content = littleDock;
        }

        public class ComboBox
        {
            public string Name { get; private set; }
            public Label Title { get; private set; }
            public Button ComboBoxToggleButton { get; set; }
            public TextBox ComboBoxTextBox { get; set; }
            public Image ComboBoxImage { get; set; }
            public Border ComboBoxItemStackBorder { get; set; }
            public StackPanel ComboBoxItemStack { get; set; }
            public List<Button> ComboBoxItems { get; set; }

            public ComboBox(string name, DockPanel parent, double minWidth, double minHeight, VerticalAlignment vAlignment, HorizontalAlignment hAlignment, Dock dock, Thickness margin, List<string> items, string selectedItem, bool editable, bool makeTitle, TextChangedEventHandler textChanged = null)
            {
                Name = name;
                CreateComboBox(parent, minWidth, minHeight, vAlignment, hAlignment, dock, margin, items, selectedItem, editable, makeTitle, textChanged);
                ComboBoxes.Add(this);
            }

            public void ChangeItems(List<string> items, string selectedItem)
            {
                var minHeight = (ComboBoxItemStack.Children[ 0 ] as Button).MinHeight;
                AddItems(minHeight, items);
                ComboBoxTextBox.Text = selectedItem;
            }

            public void CreateComboBox(DockPanel parent, double minWidth, double minHeight, VerticalAlignment vAlignment, HorizontalAlignment hAlignment, Dock dock, Thickness margin, List<string> items, string selectedItem, bool editable, bool makeTitle, TextChangedEventHandler textChanged)
            {
                var dockPanel = new DockPanel();
                dockPanel.MinWidth = minWidth;
                dockPanel.SetValue(DockPanel.DockProperty, dock);
                dockPanel.HorizontalAlignment = hAlignment;
                dockPanel.VerticalAlignment = vAlignment;
                dockPanel.LastChildFill = true;

                if (makeTitle)
                {
                    Title = new Label();
                    Title.Content = TextCatalog.GetName(Name);
                    Title.MinWidth = minWidth;
                    Title.Height = AppSettings_User.FontSize * 2.5;
                    Title.Margin = new Thickness(margin.Left, margin.Top, 2.5, 0);
                    Title.VerticalAlignment = VerticalAlignment.Stretch;
                    Title.HorizontalAlignment = HorizontalAlignment.Stretch;
                    Title.HorizontalContentAlignment = HorizontalAlignment.Left;
                    Title.VerticalContentAlignment = VerticalAlignment.Center;
                    Title.BorderBrush = null;
                    Title.BorderThickness = new Thickness(0);
                    Title.FontSize = AppSettings_User.FontSize;
                    Title.FontFamily = new FontFamily(AppSettings_Static.Font1);
                    Title.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
                    Title.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color3);
                    Title.SetValue(DockPanel.DockProperty, Dock.Top);

                    dockPanel.Children.Add(Title);
                }

                var comboBox = new DockPanel();
                comboBox.LastChildFill = false;
                comboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                comboBox.VerticalAlignment = VerticalAlignment.Top;
                comboBox.Margin = new Thickness(margin.Left, makeTitle ? 0 : margin.Top * 1.3, margin.Right, 0);
                comboBox.MinHeight = minHeight;
                comboBox.MinWidth = minWidth;
                comboBox.SetValue(DockPanel.DockProperty, Dock.Top);

                ComboBoxToggleButton = new Button();
                ComboBoxToggleButton.Name = $"ToggleButton{Name}";
                ComboBoxToggleButton.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color2);
                ComboBoxToggleButton.Style = ( Style ) ComboBoxToggleButton.FindResource("ToggleButton");
                ComboBoxToggleButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                ComboBoxToggleButton.VerticalAlignment = VerticalAlignment.Top;
                ComboBoxToggleButton.Width = AppSettings_User.FontSize * 1.75;
                ComboBoxToggleButton.Height = ComboBoxToggleButton.Width;
                ComboBoxToggleButton.BorderThickness = new Thickness(1);
                ComboBoxToggleButton.BorderBrush = null;
                ComboBoxToggleButton.Margin = new Thickness(1);
                ComboBoxToggleButton.MouseEnter += Events.ToggleButtonMouseEnter;
                ComboBoxToggleButton.MouseLeave += Events.ToggleButtonMouseLeave;
                ComboBoxToggleButton.PreviewMouseLeftButtonDown += Events.ToggleButtonClick;
                ComboBoxToggleButton.Height = ComboBoxToggleButton.Width;
                ComboBoxToggleButton.SetValue(DockPanel.DockProperty, Dock.Right);

                ComboBoxImage = new Image();
                ComboBoxImage.Name = $"ToggleImage{Name}";
                ComboBoxImage.Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowDown{AppSettings_User.ColorSchema.Name}")));
                ComboBoxImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                ComboBoxImage.VerticalAlignment = VerticalAlignment.Stretch;
                ComboBoxImage.Width = ComboBoxToggleButton.Width * 0.6;
                ComboBoxImage.Height = ComboBoxImage.Width;

                ComboBoxToggleButton.Content = ComboBoxImage;

                ComboBoxTextBox = new TextBox();
                ComboBoxTextBox.Name = $"ComboBoxText{Name}";
                ComboBoxTextBox.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                ComboBoxTextBox.BorderBrush = null;
                ComboBoxTextBox.BorderThickness = new Thickness(1);
                ComboBoxTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                ComboBoxTextBox.VerticalAlignment = VerticalAlignment.Top;
                ComboBoxTextBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                ComboBoxTextBox.VerticalContentAlignment = VerticalAlignment.Center;
                ComboBoxTextBox.MinWidth = minWidth - ComboBoxToggleButton.Width;
                ComboBoxTextBox.Height = ComboBoxToggleButton.Height;
                ComboBoxTextBox.Margin = new Thickness(1);
                ComboBoxTextBox.Text = selectedItem;
                ComboBoxTextBox.IsEnabled = editable;
                ComboBoxTextBox.FontSize = AppSettings_User.FontSize;
                ComboBoxTextBox.FontFamily = new FontFamily(AppSettings_Static.Font2);
                ComboBoxTextBox.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                ComboBoxTextBox.Style = ( Style ) ComboBoxToggleButton.FindResource("ComboBoxTextBox");
                if (textChanged != null)
                    ComboBoxTextBox.TextChanged += textChanged;

                comboBox.Children.Add(ComboBoxTextBox);
                comboBox.Children.Add(ComboBoxToggleButton);

                ComboBoxItemStackBorder = new Border();
                ComboBoxItemStackBorder.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                ComboBoxItemStackBorder.BorderThickness = new Thickness(0);
                ComboBoxItemStackBorder.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                ComboBoxItemStackBorder.CornerRadius = new CornerRadius(5);
                ComboBoxItemStackBorder.VerticalAlignment = VerticalAlignment.Top;
                ComboBoxItemStackBorder.HorizontalAlignment = HorizontalAlignment.Left;
                ComboBoxItemStackBorder.Width = minWidth + 4;
                ComboBoxItemStackBorder.MaxHeight = 100;
                ComboBoxItemStackBorder.Margin = new Thickness(margin.Left, margin.Top / 2, margin.Right, margin.Bottom);
                ComboBoxItemStackBorder.Padding = new Thickness(5);
                ComboBoxItemStackBorder.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
                ComboBoxItemStackBorder.SetValue(DockPanel.DockProperty, Dock.Bottom);
                ComboBoxItemStackBorder.Visibility = Visibility.Collapsed;

                ComboBoxItemStack = new StackPanel();
                ComboBoxItemStack.Orientation = System.Windows.Controls.Orientation.Vertical;
                ComboBoxItemStack.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                ComboBoxItemStack.HorizontalAlignment = HorizontalAlignment.Stretch;
                ComboBoxItemStack.VerticalAlignment = VerticalAlignment.Stretch;

                var scroll = new ScrollViewer();
                scroll.Style = ( Style ) scroll.FindResource("ScrollViewer");
                scroll.MaxHeight = 100;
                scroll.Content = ComboBoxItemStack;

                AddItems(minHeight,items);

                ComboBoxItemStackBorder.Child = scroll;
                dockPanel.Children.Add(comboBox);
                dockPanel.Children.Add(ComboBoxItemStackBorder);
                parent.Children.Add(dockPanel);
            }

            private void AddItems(double minHeight, List<string> items)
            {
                ComboBoxItems = new List<Button>();
                ComboBoxItemStack.Children.Clear();

                items.ForEach(i =>
                {
                    var btn = new Button();
                    btn.Padding = new Thickness(5, 0, 0, 0);
                    btn.MinHeight = minHeight * 0.85;
                    btn.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
                    btn.BorderThickness = new Thickness(0);
                    btn.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
                    btn.Content = i;
                    btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                    btn.VerticalAlignment = VerticalAlignment.Stretch;
                    btn.HorizontalContentAlignment = HorizontalAlignment.Left;
                    btn.VerticalContentAlignment = VerticalAlignment.Center;
                    btn.FontSize = AppSettings_User.FontSize;
                    btn.FontFamily = new FontFamily(AppSettings_Static.Font2);
                    btn.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                    btn.Style = ( Style ) ComboBoxToggleButton.FindResource($"ComboBoxItem{AppSettings_User.ColorSchema.Name}");
                    btn.PreviewMouseLeftButtonDown += Events.ChangeSelectedItem;
                    ComboBoxItems.Add(btn);
                    ComboBoxItemStack.Children.Add(btn);
                });
            }
        }

        public static void DisableProjectLoad()
        {
            Window.MenuStack2.Visibility = Visibility.Collapsed;
            Window.OpenProject.IsEnabled = false;
        }

        public static void EnableProjectLoad()
        {
            Window.MenuStack2.Visibility = Visibility.Visible;
            Window.OpenProject.IsEnabled = true;
        }

        public void EnableProjectEdit()
        {
            MenuStack3.Visibility = Visibility.Visible;
            MenuStack4.Visibility = Visibility.Visible;
        }
    }
}
