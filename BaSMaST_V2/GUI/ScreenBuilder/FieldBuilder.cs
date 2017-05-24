using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System;

namespace BaSMaST_V3
{
    public partial class MainWindow
    {
        public List<StackPanel> ContentPages { get; set; }
        public StackPanel CurrentPanel { get; set; }
        public Dictionary<string,List<StackPanel>> Stacks { get; private set; }
        public Button NewProject { get; set; }
        public Button OpenProject { get; set; }
        public Button PageUp { get; set; }
        public Button PageDown { get; set; }

        

        

        public TreeView LocationTree { get; set; }

        public void BuildField()
        {
            LoadWelcomeScreen();
        }

        public void InitEmptyField(int producedPanelsCount = 1)
        {
            ContentPages = new List<StackPanel>();
            Stacks = new Dictionary<string, List<StackPanel>>();
            ContentPanel.Children.Clear();

            PageUp = new Button();
            PageUp.Height = 30;
            PageUp.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color2);
            PageUp.Style = ( Style ) PageUp.FindResource("ToggleButton");
            PageUp.PreviewMouseLeftButtonDown += Events.ChangeContent;
            PageUp.MouseEnter += Events.UpDownMouseEnter;
            PageUp.MouseLeave += Events.UpDownMouseLeave;
            PageUp.HorizontalAlignment = HorizontalAlignment.Stretch;
            PageUp.VerticalAlignment = VerticalAlignment.Top;
            PageUp.Margin = new Thickness(5,10,5,5);
            PageUp.Visibility = Visibility.Collapsed;
            AddImage(PageUp, $"ArrowUp{AppSettings_User.ColorSchema.Name}");
            PageUp.SetValue(DockPanel.DockProperty, Dock.Top);

            PageDown = new Button();
            PageDown.Height = 30;
            PageDown.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color2);
            PageDown.Style = ( Style ) PageDown.FindResource("ToggleButton");
            PageDown.PreviewMouseLeftButtonDown += Events.ChangeContent;
            PageDown.MouseEnter += Events.UpDownMouseEnter;
            PageDown.MouseLeave += Events.UpDownMouseLeave;
            PageDown.HorizontalAlignment = HorizontalAlignment.Stretch;
            PageDown.VerticalAlignment = VerticalAlignment.Top;
            PageDown.Margin = new Thickness(5,5,5,10);
            if (producedPanelsCount == 1)
                PageDown.Visibility = Visibility.Collapsed;
            AddImage(PageDown, $"ArrowDown{AppSettings_User.ColorSchema.Name}");
            PageDown.SetValue(DockPanel.DockProperty, Dock.Bottom);

            ContentPanel.Children.Add(PageUp);
            ContentPanel.Children.Add(PageDown);

            for (int c=0; c< producedPanelsCount;c++)
            {
                var ContentPage = new StackPanel();
                ContentPage.Name = $"ContentPage{c}";
                ContentPage.Orientation = System.Windows.Controls.Orientation.Vertical;
                ContentPage.HorizontalAlignment = HorizontalAlignment.Center;
                ContentPage.VerticalAlignment = VerticalAlignment.Center;
                ContentPage.Margin = new Thickness(0,10,0,10);
                ContentPage.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary1);
                ContentPage.Visibility = Visibility.Collapsed;

                ContentPages.Add(ContentPage);

                ContentPanel.SizeChanged += Events.Reload;
                ContentPanel.Children.Add(ContentPage);

                var ColumnCount = ( int ) ContentPanel.ActualWidth / ButtonWidth;
                var RowCount = ( int ) ContentPanel.ActualHeight / ButtonWidth;

                var stackList = new List<StackPanel>();

                for (int i=0; i<RowCount; i++)
                {
                    var stack = new StackPanel();
                    stack.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    stack.HorizontalAlignment = HorizontalAlignment.Center;
                    stack.VerticalAlignment = VerticalAlignment.Center;

                    for (int j = 0; j < ColumnCount; j++)
                    {
                        stack.Children.Add(AddButton(i,j,ButtonWidth));
                    }

                    stackList.Add(stack);
                    ContentPage.Children.Add(stack);
                }

                Stacks.Add(ContentPage.Name, stackList);
                ContentPages[0].Visibility = Visibility.Visible;
                CurrentPanel = ContentPages[ 0 ];
            }
        }

        private static Button AddButton(int row, int col, double buttonWidth, double buttonHeight = 0)
        {
            Button newBtn = new Button();

            newBtn.Margin = new Thickness(2.5);
            newBtn.Height = buttonHeight==0?buttonWidth:buttonHeight;
            newBtn.Width = buttonWidth;
            newBtn.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            newBtn.BorderBrush = null;

            newBtn.Style = ( Style ) newBtn.FindResource("Field");

            newBtn.Name = $"Button{row}_{col}";

            return newBtn;
        }

        public void LoadWelcomeScreen()
        {
            InitEmptyField();

            var count = Stacks[ContentPages[0].Name][0].Children.Count;
            var titleRow = ContentPages[0].Children.Count/2-1;
            if (titleRow < 0)
                titleRow = 0;
            var length = count - 2;
            if (length < 1)
            {
                ButtonWidth = Convert.ToInt32(ButtonWidth * 0.7);
                InitEmptyField();
                LoadWelcomeScreen();
                ButtonWidth = 195;
                return;
            }

            var length2 = length - 2;
            if (length == 2 || length == 1)
                length2 = length;
            var rest = ( count - length2 ) / 2;
            
            var width = ButtonWidth;
            var btn = AddButton(titleRow, 1, width * length + length * 2, width);
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            var box = new Viewbox();
            var label = new Label();
            label.Content = TextCatalog.GetName("Welcome!");
            label.FontFamily = new FontFamily(AppSettings_Static.Font1);
            label.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            box.Child = label;
            btn.Content = box;
            btn.VerticalContentAlignment = VerticalAlignment.Center;
            btn.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            Stacks[ ContentPages[ 0 ].Name ][ titleRow ].Children.Clear();

            Stacks[ ContentPages[ 0 ].Name ][ titleRow ].Children.Add(AddButton(titleRow, 0, width));
            Stacks[ ContentPages[ 0 ].Name ][ titleRow ].Children.Add(btn);
            Stacks[ ContentPages[ 0 ].Name ][ titleRow ].Children.Add(AddButton(titleRow, 2, width));

            Stacks[ ContentPages[ 0 ].Name ][ titleRow +1].Children.Clear();

            var stack = new StackPanel();
            stack.Orientation = System.Windows.Controls.Orientation.Vertical;

            NewProject = AddButton(titleRow+1, rest + 1, width * length2 + length2 * 2, width * 0.5);
            NewProject.Style = ( Style ) NewProject.FindResource($"Field{AppSettings_User.ColorSchema.Name}");
            NewProject.PreviewMouseLeftButtonDown += Events.LoadProjectConfig;
            var box2 = new Viewbox();
            var label2 = new Label();
            label2.Content = TextCatalog.GetName("New Project");
            label2.FontFamily = new FontFamily(AppSettings_Static.Font1);
            label2.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            box2.Child = label2;
            NewProject.Content = box2;

            OpenProject = AddButton(titleRow+1, rest + 2, width * length2 + length2 * 2, width * 0.5);
            OpenProject.Style = ( Style ) NewProject.FindResource($"Field{AppSettings_User.ColorSchema.Name}");
            var box3 = new Viewbox();
            var label3 = new Label();
            label3.Content = TextCatalog.GetName("Open Project");
            label3.FontFamily = new FontFamily(AppSettings_Static.Font1);
            label3.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            box3.Child = label3;
            OpenProject.PreviewMouseLeftButtonDown += Events.LoadProjects;
            OpenProject.Content = box3;

            if (AppSettings_User.Projects == null || !AppSettings_User.Projects.Any())
                OpenProject.IsEnabled = false;
            else OpenProject.IsEnabled = true;

            stack.Children.Add(NewProject);
            stack.Children.Add(OpenProject);

            for (int i = 0; i < rest; i++)
            {
                Stacks[ ContentPages[ 0 ].Name ][ titleRow + 1 ].Children.Add(AddButton(titleRow+1, i, width));
            }
            Stacks[ ContentPages[ 0 ].Name ][ titleRow + 1 ].Children.Add(stack);
            for (int i = rest + 3; i < rest + 3 + rest; i++)
            {
                Stacks[ ContentPages[ 0 ].Name ][ titleRow + 1 ].Children.Add(AddButton(titleRow+1, i, width));
            }
            CurrentContent = Pages.Welcome;
        }

        public void LoadProjectsScreen()
        {
            if (AppSettings_User.Projects == null)
                return;

            ScreenTitle.Content = TextCatalog.GetName("Projects");

            var projectCount = AppSettings_User.Projects.Count;
            InitEmptyFieldAcordingToNumberOfItems(projectCount);

            var projectNames = new List<string>();
            AppSettings_User.Projects.ForEach(p => projectNames.Add(p.Name));

            FillButtons(projectNames, GetButtonsFromField());
            CurrentContent = Pages.Projects;
        }

        private List<Button> GetButtonsFromField()
        {
            var buttonList = new List<Button>();

            ContentPages.ForEach(c =>
            {
                Stacks[ c.Name ].ForEach(s =>
                {
                    foreach (Button button in s.Children)
                    {
                        buttonList.Add(button);
                    }
                });
            });

            return buttonList;
        }

        public void LoadProjectConfiguration(Project project = null)
        {
            Config.ShowWindow(TypeName.Project,project);
        }

        public void LoadMainScreen()
        {
            ScreenTitle.Content = AppSettings_User.CurrentProject.Name;

            var itemTableCount = 6 + (AppSettings_User.CurrentProject.ItemTables != null ? AppSettings_User.CurrentProject.ItemTables.Count : 0);
            InitEmptyFieldAcordingToNumberOfItems(itemTableCount);

            var iconNames = new List<string>()
            {
                "ScheduleIcon",
                "ArchiveIcon",
                "CharactersIcon",
                //"RelationshipsIcon",
                //"CharacterEvolvementIcon",
                "LocationsIcon"
            };

            //if(itemTableCount != 0 && AppSettings_User.CurrentProject.ItemTables!= null)
            //{
            //    AppSettings_User.CurrentProject.ItemTables.ForEach(i =>
            //    {
            //        if (!string.IsNullOrEmpty(i.Icon))
            //            iconNames.Add(i.Icon);
            //    });
            //}

            var items = new List<string>();

            for(int i = 0; i< itemTableCount; i++)
            {
                items.Add(i.ToString());
            }

            FillButtons(items, GetButtonsFromField(), iconNames);
            
            CurrentContent = Pages.MainScreen;
        }

        private void InitEmptyFieldAcordingToNumberOfItems(int numberOfItems)
        {
            var ColumnCount = ( int ) ContentPanel.ActualWidth / ButtonWidth;
            var RowCount = ( int ) ContentPanel.ActualHeight / ButtonWidth;
            var product = ColumnCount * RowCount;

            if (numberOfItems > 0 && numberOfItems > product)
            {
                if (numberOfItems % product == 0)
                    InitEmptyField(numberOfItems / product);
                else InitEmptyField(numberOfItems / product + 1);
            }
            else InitEmptyField();
        }

        private void FillButtons(List<string> Items, List<Button> buttonList, List<string> iconNames = null)
        {
            var index = 0;

            Items.ForEach(i =>
            {
                var button = buttonList[ Items.IndexOf(i) ];

                if(iconNames == null)
                {
                    var box = new Viewbox();
                    var label = new Label();
                    label.Content = i;
                    box.Child = label;
                    button.Content = box;
                    button.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                    button.FontFamily = new FontFamily(AppSettings_Static.Font1);
                    if(i.Length < 15)
                        button.FontSize = AppSettings_User.FontSize * 2;
                    else button.FontSize = AppSettings_User.FontSize;
                    button.Content = i;

                    var project = AppSettings_User.Projects.Find(p => p.Name == i);
                    if (project != null)
                        button.PreviewMouseLeftButtonDown += Events.LoadProject;
                }
                else
                {
                    if(index <iconNames.Count)
                    {
                        AddImage(button, iconNames[index],0.95);
                        button.Name = iconNames[ index ].Replace("Icon","");
                    }
                    //else
                    //{
                    //    switch(index%4)
                    //    {
                    //        case 0: AddImage(button, "ItemTableIconBlue",0.95);break;
                    //        case 1: AddImage(button, "ItemTableIconRed",0.95);break;
                    //        case 2: AddImage(button, "ItemTableIconGreen",0.95);break;
                    //        case 3: AddImage(button, "ItemTableIconGray",0.95);break;
                    //    }
                    //    button.Name = $"ItemTable{index-4}";
                    //}
                    index++;
                    button.PreviewMouseLeftButtonDown += Events.MainScreenElementClick;
                }

                button.Style = ( Style ) button.FindResource($"Field{AppSettings_User.ColorSchema.Name}");
            });
        }

        public void LoadLocations()
        {
            ContentPanel.Children.Clear();

            var border = new Border();
            border.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            border.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(31);
            border.Margin = new Thickness(20);
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.VerticalAlignment = VerticalAlignment.Stretch;

            LocationTree = new TreeView();
            LocationTree.Margin = new Thickness(10);
            LocationTree.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            LocationTree.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            LocationTree.BorderThickness = new Thickness(1);
            LocationTree.HorizontalAlignment = HorizontalAlignment.Stretch;
            LocationTree.VerticalAlignment = VerticalAlignment.Stretch;
            LocationTree.HorizontalContentAlignment = HorizontalAlignment.Center;
            LocationTree.VerticalContentAlignment = VerticalAlignment.Center;

            AddLocations(LocationTree);

            border.Child = LocationTree;
            ContentPanel.Children.Add(border);

            CurrentContent = Pages.Locations;
        }

        private void AddLocations(TreeView treeView)
        {
            if (AppSettings_User.CurrentProject.LocationManager == null || !AppSettings_User.CurrentProject.LocationManager.GetItems().Any())
                return;
            var locations = AppSettings_User.CurrentProject.LocationManager.GetItems();

            AddTo(null, locations.FindAll(l => l.Parent == null));
        }

        private void AddTo(Location parent, List<Location> locations)
        {
            var treeItem = new TreeViewItem();
            if (parent != null)
            {
                treeItem = ( LocationTree.FindName(parent.Name) as TreeViewItem );
            }

            locations.ForEach(location =>
            {
                var newItem = new TreeViewItem();
                newItem.Header = location.Name;
                newItem.Tag = location;

                if (parent == null)
                    LocationTree.Items.Add(newItem);
                else treeItem.Items.Add(newItem);

                if (locations.Any(loc=> loc.Parent == location))
                {
                    AddTo(location, locations.FindAll(l => l.Parent == location));
                }
            });
        }
    }
}
