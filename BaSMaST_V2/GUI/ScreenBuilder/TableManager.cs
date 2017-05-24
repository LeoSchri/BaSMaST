using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System;

namespace BaSMaST_V3
{
    public class TableManager
    {
        public static BrushConverter bc = new BrushConverter();

        public static List<TypeName> CurrentTables { get; set; }

        public static List<DataGrid> Tables { get; set; }
        public static List<Label> TableTitles { get; set; }
        public static List<DockPanel> AdditionalTables { get; set; }
        public static Button ApplyChanges { get; set; }
        public static Button RevertChanges { get; set; }
        public static Button Add_RemoveAttributes { get; set; }
        public static Button SwitchToFirstTable { get; set; }
        public static Button SwitchToSecondTable { get; set; }

        public static DockPanel TableDock { get; set; }
        public static StackPanel TableStack { get; set; }
        public static List<Image> OpenCloseTableImgs { get; set; }

        public static void LoadTables(params TypeName[] type)
        {
            var tables = type.ToList();

            MainWindow.Window.ContentPanel.Children.Clear();

            CurrentTables = tables;
            MainWindow.CurrentContent = Pages.TableView;

            TableDock = new DockPanel();

            var border = new Border();
            border.Margin = new Thickness(10);
            border.MaxWidth = MainWindow.Window.ContentPanel.ActualWidth - 20;
            border.MaxHeight = MainWindow.Window.ContentPanel.ActualHeight - 20;
            border.BorderBrush = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            border.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            border.BorderThickness = new Thickness(0);
            border.CornerRadius = new CornerRadius(10);

            var scroll = new ScrollViewer();
            scroll.Padding = new Thickness(5);
            scroll.Margin = new Thickness(10);
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            border.Child = scroll;

            TableStack = new StackPanel();
            TableStack.Orientation = System.Windows.Controls.Orientation.Horizontal;
            TableStack.MaxHeight = MainWindow.Window.ContentPanel.ActualHeight - 20;

            scroll.Content = TableStack;

            NoteManager.BuildNoteManager();
            MainWindow.Window.ContentPanel.Children.Add(TableDock);
            AddTableOptions(tables.FirstOrDefault());
            TableDock.Children.Add(border);

            Tables = new List<DataGrid>();
            TableTitles = new List<Label>();
            AdditionalTables = new List<DockPanel>();
            OpenCloseTableImgs = new List<Image>();

            MainWindow.Window.ScreenTitle.Content = TextCatalog.GetName($"{tables.FirstOrDefault()}s");

            bool enableOpen_Close = false;
            if (tables.Count > 1)
            {
                enableOpen_Close = true;
            }

            tables.ForEach(t =>
            {
                LoadTable(t, enableOpen_Close);
            });
        }

        public static void LoadTable(TypeName type, bool enableOpen_Close = false)
        {
            var TableSet = new DockPanel();

            var TablesStack = new DockPanel();
            TablesStack.Margin = new Thickness(5, 0, 0, 10);
            TablesStack.SetValue(DockPanel.DockProperty, Dock.Right);
            TablesStack.LastChildFill = false;

            AdditionalTables.Add(TablesStack);

            if (type == TypeName.Plot)
            {
                AddTable(TablesStack, TypeName.PlotLink, type);
                AddTable(TablesStack, TypeName.LorePlotLink, type);
            }
            else if(type == TypeName.Lore)
            {
                AddTable(TablesStack, TypeName.Aftermath, TypeName.Lore, Dock.Right, generateColumns:true);
                AddTable(TablesStack, TypeName.LoreLink, type);
                AddTable(TablesStack, TypeName.LorePlotLink, type);
            }
            else if (type == TypeName.Event)
            {
                AddTable(TablesStack, TypeName.AttachmentFigure, type);
                AddTable(TablesStack, TypeName.CharacterPresent, type);
                AddTable(TablesStack, TypeName.LocationLink, type);
            }

            TableSet.Children.Add(TablesStack);
            AddTable(TableSet, type, enableOpen_Close);

            TableStack.Children.Add(TableSet);

            //Events.SetTablesForOwner();
        }

        private static void AddTable(DockPanel parent, TypeName type, bool enableOpen_Close = false)
        {
            var datatable = DBDataManager.GetTableFromDatabase(type);
            var TableContainer = new DockPanel();
            TableContainer.Margin = new Thickness(5, 5, 0, 10);

            var border = new Border();
            border.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            border.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(5);
            border.Height = AppSettings_User.FontSize * 2.5;
            border.Margin = new Thickness(0, 0, 0, 5);
            border.SetValue(DockPanel.DockProperty, Dock.Top);

            var TableHeader = new DockPanel();
            TableHeader.Margin = new Thickness(2);
            TableHeader.Name = $"{type}Table";
            TableHeader.MouseEnter += Events.ToggleButtonMouseEnter;
            TableHeader.MouseLeave += Events.ToggleButtonMouseLeave;
            TableHeader.PreviewMouseLeftButtonDown += Events.Open_CloseTable;
            TableHeader.IsEnabled = enableOpen_Close;
            border.Child = TableHeader;

            var TableTitle = new Label();
            TableTitle.HorizontalAlignment = HorizontalAlignment.Center;
            TableTitle.VerticalAlignment = VerticalAlignment.Center;
            TableTitle.Content = type.ToString();
            TableTitle.FontFamily = new FontFamily(AppSettings_Static.Font2);
            TableTitle.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            TableTitle.FontSize = AppSettings_User.FontSize;

            TableTitles.Add(TableTitle);

            var OpenCloseTable = new Button();
            OpenCloseTable.Margin = new Thickness(0, 0, 5, 0);
            OpenCloseTable.Width = 20;
            OpenCloseTable.Height = OpenCloseTable.Width;
            OpenCloseTable.SetValue(DockPanel.DockProperty, Dock.Right);
            if (enableOpen_Close)
                OpenCloseTable.Visibility = Visibility.Visible;
            else OpenCloseTable.Visibility = Visibility.Collapsed;
            OpenCloseTable.Style = ( Style ) OpenCloseTable.FindResource($"FieldSmall");

            var OpenCloseTableImg = new Image();
            OpenCloseTableImg.Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowLeft{ AppSettings_User.ColorSchema.Name }")));
            OpenCloseTableImg.Width = OpenCloseTable.Width * 0.5;
            OpenCloseTableImgs.Add(OpenCloseTableImg);

            OpenCloseTable.Content = OpenCloseTableImg;
            OpenCloseTable.Background = null;
            OpenCloseTable.BorderBrush = null;
            OpenCloseTable.BorderThickness = new Thickness(1);

            TableHeader.Children.Add(OpenCloseTable);
            TableHeader.Children.Add(TableTitle);

            var Table = new DataGrid();
            Table.Name = $"{type}Table";
            Table.AutoGenerateColumns = true;
            Table.CanUserDeleteRows = true;
            Table.CanUserResizeColumns = false;
            Table.CanUserResizeRows = false;
            Table.ItemsSource = datatable.DefaultView;
            Table.FontFamily = new FontFamily(AppSettings_Static.Font2);
            Table.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            Table.FontSize = AppSettings_User.FontSize;
            Table.VerticalAlignment = VerticalAlignment.Top;
            Table.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            Table.VerticalContentAlignment = VerticalAlignment.Stretch;
            Table.AutoGeneratingColumn += Events.AlterColumns;
            Table.CellEditEnding += Events.CellValueChanged;
            Table.PreviewKeyDown += Events.RowDeleted;
            Table.SelectionChanged += Events.SetOwner;

            Tables.Add(Table);
            TableContainer.Children.Add(border);
            TableContainer.Children.Add(Table);

            parent.Children.Add(TableContainer);
        }

        private static void AddTable(DockPanel parent, TypeName type, TypeName typeOfOwner, Dock dock = Dock.Left, bool enableOpen_Close = false, bool generateColumns = false)
        {
            var TableContainer = new DockPanel();
            TableContainer.SetValue(DockPanel.DockProperty, dock);
            TableContainer.Margin = new Thickness(5, 5, 0, 0);

            var border = new Border();
            border.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            border.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(5);
            border.Height = AppSettings_User.FontSize * 2.5;
            border.Margin = new Thickness(0, 0, 0, 5);
            border.SetValue(DockPanel.DockProperty, Dock.Top);

            var TableHeader = new DockPanel();
            TableHeader.Name = $"{type}Table";
            TableHeader.MouseEnter += Events.ToggleButtonMouseEnter;
            TableHeader.MouseLeave += Events.ToggleButtonMouseLeave;
            TableHeader.PreviewMouseLeftButtonDown += Events.Open_CloseTable;
            TableHeader.IsEnabled = enableOpen_Close;
            border.Child = TableHeader;

            var TableTitle = new Label();
            TableTitle.HorizontalAlignment = HorizontalAlignment.Center;
            TableTitle.VerticalAlignment = VerticalAlignment.Center;
            TableTitle.Content = type.ToString();
            TableTitle.FontFamily = new FontFamily(AppSettings_Static.Font2);
            TableTitle.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            TableTitle.FontSize = AppSettings_User.FontSize;

            TableTitles.Add(TableTitle);

            var OpenCloseTable = new Button();
            OpenCloseTable.Margin = new Thickness(0, 0, 5, 0);
            OpenCloseTable.Width = 20;
            OpenCloseTable.Height = OpenCloseTable.Width;
            OpenCloseTable.SetValue(DockPanel.DockProperty, Dock.Right);
            if (enableOpen_Close)
                OpenCloseTable.Visibility = Visibility.Visible;
            else OpenCloseTable.Visibility = Visibility.Collapsed;
            OpenCloseTable.Style = ( Style ) OpenCloseTable.FindResource($"FieldSmall");

            var OpenCloseTableImg = new Image();
            OpenCloseTableImg.Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowLeft{ AppSettings_User.ColorSchema.Name }")));
            OpenCloseTableImg.Width = OpenCloseTable.Width * 0.5;
            OpenCloseTableImgs.Add(OpenCloseTableImg);

            OpenCloseTable.Content = OpenCloseTableImg;
            OpenCloseTable.Background = null;
            OpenCloseTable.BorderBrush = null;
            OpenCloseTable.BorderThickness = new Thickness(1);

            TableHeader.Children.Add(OpenCloseTable);
            TableHeader.Children.Add(TableTitle);

            var Table = new DataGrid();

            if (NoteManager.NoteOwner == null)
            {
                Table.Visibility = Visibility.Collapsed;
            }

            Table.Name = $"{type}Table";
            Table.CanUserDeleteRows = true;
            Table.CanUserResizeColumns = false;
            Table.CanUserResizeRows = false;
            Table.FontFamily = new FontFamily(AppSettings_Static.Font2);
            Table.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            Table.FontSize = AppSettings_User.FontSize;
            Table.VerticalAlignment = VerticalAlignment.Top;
            Table.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            Table.VerticalContentAlignment = VerticalAlignment.Stretch;
            Table.CellEditEnding += Events.CellValueChanged;
            Table.PreviewKeyDown += Events.RowDeleted;
            if(generateColumns)
            {
                Table.AutoGeneratingColumn += Events.AlterColumns;
                Table.SelectionChanged += Events.SetOwner;
                Table.AutoGenerateColumns = true;
            }
            else Table.AutoGenerateColumns = false;

            Tables.Add(Table);
            TableContainer.Children.Add(border);
            TableContainer.Children.Add(Table);

            parent.Children.Add(TableContainer);
        }

        private static void AddTableOptions(TypeName type)
        {
            var buttonDock = new DockPanel();
            buttonDock.SetValue(DockPanel.DockProperty, Dock.Bottom);

            var buttonStack = new StackPanel();
            buttonStack.Orientation = System.Windows.Controls.Orientation.Horizontal;
            buttonStack.HorizontalAlignment = HorizontalAlignment.Right;
            buttonStack.VerticalAlignment = VerticalAlignment.Stretch;
            buttonDock.SetValue(DockPanel.DockProperty, Dock.Bottom);

            SwitchToFirstTable = new Button();
            switch (type)
            {
                case TypeName.Character:
                case TypeName.MainCharacter: SwitchToFirstTable.Content = TextCatalog.GetName("SwitchToCharacters"); break;
                case TypeName.Plot:
                case TypeName.Lore: SwitchToFirstTable.Content = TextCatalog.GetName("SwitchToPlot"); break;
            }
            SwitchToFirstTable.Margin = new Thickness(10, 10, 10, 0);
            SwitchToFirstTable.Height = AppSettings_User.FontSize * 2;
            SwitchToFirstTable.MinWidth = 250;
            SwitchToFirstTable.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color2);
            SwitchToFirstTable.BorderBrush = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color2);
            SwitchToFirstTable.BorderThickness = new Thickness(1);
            SwitchToFirstTable.FontFamily = new FontFamily(AppSettings_Static.Font2);
            SwitchToFirstTable.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            SwitchToFirstTable.FontSize = AppSettings_User.FontSize;
            SwitchToFirstTable.Style = ( Style ) SwitchToFirstTable.FindResource($"PopupButton{AppSettings_User.ColorSchema.Name}");
            SwitchToFirstTable.HorizontalContentAlignment = HorizontalAlignment.Center;
            SwitchToFirstTable.VerticalContentAlignment = VerticalAlignment.Center;
            SwitchToFirstTable.PreviewMouseLeftButtonDown += Events.SwitchView;
            SwitchToFirstTable.SetValue(DockPanel.DockProperty, Dock.Top);

            SwitchToSecondTable = new Button();
            switch (type)
            {
                case TypeName.Character:
                case TypeName.MainCharacter: SwitchToSecondTable.Content = TextCatalog.GetName("SwitchToMainCharacters"); break;
                case TypeName.Plot:
                case TypeName.Lore: SwitchToSecondTable.Content = TextCatalog.GetName("SwitchToLore"); break;
            }
            SwitchToSecondTable.Margin = new Thickness(10, 10, 10, 0);
            SwitchToSecondTable.Height = AppSettings_User.FontSize * 2;
            SwitchToSecondTable.MinWidth = 250;
            SwitchToSecondTable.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color2);
            SwitchToSecondTable.BorderBrush = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color2);
            SwitchToSecondTable.BorderThickness = new Thickness(1);
            SwitchToSecondTable.FontFamily = new FontFamily(AppSettings_Static.Font2);
            SwitchToSecondTable.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            SwitchToSecondTable.FontSize = AppSettings_User.FontSize;
            SwitchToSecondTable.Style = ( Style ) SwitchToSecondTable.FindResource($"PopupButton{AppSettings_User.ColorSchema.Name}");
            SwitchToSecondTable.HorizontalContentAlignment = HorizontalAlignment.Center;
            SwitchToSecondTable.VerticalContentAlignment = VerticalAlignment.Center;
            SwitchToSecondTable.PreviewMouseLeftButtonDown += Events.SwitchView;
            SwitchToSecondTable.SetValue(DockPanel.DockProperty, Dock.Top);

            switch (type)
            {
                case TypeName.Character:
                case TypeName.Plot: SwitchToFirstTable.Visibility = Visibility.Collapsed; SwitchToSecondTable.Visibility = Visibility.Visible; break;
                case TypeName.MainCharacter:
                case TypeName.Lore: SwitchToFirstTable.Visibility = Visibility.Visible; SwitchToSecondTable.Visibility = Visibility.Collapsed; break;
            }

            ApplyChanges = new Button();
            ApplyChanges.Content = TextCatalog.GetName("Apply changes");
            ApplyChanges.Margin = new Thickness(10, 0, 10, 10);
            ApplyChanges.Height = AppSettings_User.FontSize * 2;
            ApplyChanges.MinWidth = 100;
            ApplyChanges.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            ApplyChanges.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            ApplyChanges.BorderThickness = new Thickness(1);
            ApplyChanges.FontFamily = new FontFamily(AppSettings_Static.Font2);
            ApplyChanges.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            ApplyChanges.FontSize = AppSettings_User.FontSize;
            ApplyChanges.Style = ( Style ) ApplyChanges.FindResource($"FieldSmall{AppSettings_User.ColorSchema.Name}");
            ApplyChanges.HorizontalAlignment = HorizontalAlignment.Right;
            ApplyChanges.VerticalAlignment = VerticalAlignment.Stretch;
            ApplyChanges.HorizontalContentAlignment = HorizontalAlignment.Center;
            ApplyChanges.VerticalContentAlignment = VerticalAlignment.Center;
            ApplyChanges.Visibility = Visibility.Collapsed;
            ApplyChanges.PreviewMouseLeftButtonDown += Events.SaveTableEntries;

            RevertChanges = new Button();
            RevertChanges.Content = TextCatalog.GetName("Revert changes");
            RevertChanges.Margin = new Thickness(5, 0, 10, 10);
            RevertChanges.Height = AppSettings_User.FontSize * 2;
            RevertChanges.MinWidth = 100;
            RevertChanges.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            RevertChanges.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            RevertChanges.BorderThickness = new Thickness(1);
            RevertChanges.FontFamily = new FontFamily(AppSettings_Static.Font2);
            RevertChanges.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            RevertChanges.FontSize = AppSettings_User.FontSize;
            RevertChanges.Style = ( Style ) RevertChanges.FindResource($"FieldSmall{AppSettings_User.ColorSchema.Name}");
            RevertChanges.HorizontalAlignment = HorizontalAlignment.Right;
            RevertChanges.VerticalAlignment = VerticalAlignment.Stretch;
            RevertChanges.HorizontalContentAlignment = HorizontalAlignment.Center;
            RevertChanges.VerticalContentAlignment = VerticalAlignment.Center;
            RevertChanges.Visibility = Visibility.Collapsed;
            RevertChanges.PreviewMouseLeftButtonDown += Events.ReloadTable;

            Add_RemoveAttributes = new Button();
            Add_RemoveAttributes.Content = TextCatalog.GetName("Add/Remove Columns");
            Add_RemoveAttributes.Margin = new Thickness(10, 0, 10, 10);
            Add_RemoveAttributes.Height = AppSettings_User.FontSize * 2;
            Add_RemoveAttributes.MinWidth = 150;
            Add_RemoveAttributes.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            Add_RemoveAttributes.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            Add_RemoveAttributes.BorderThickness = new Thickness(1);
            Add_RemoveAttributes.FontFamily = new FontFamily(AppSettings_Static.Font2);
            Add_RemoveAttributes.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            Add_RemoveAttributes.FontSize = AppSettings_User.FontSize;
            Add_RemoveAttributes.Style = ( Style ) Add_RemoveAttributes.FindResource($"FieldSmall{AppSettings_User.ColorSchema.Name}");
            Add_RemoveAttributes.HorizontalAlignment = HorizontalAlignment.Left;
            Add_RemoveAttributes.VerticalAlignment = VerticalAlignment.Stretch;
            Add_RemoveAttributes.HorizontalContentAlignment = HorizontalAlignment.Center;
            Add_RemoveAttributes.VerticalContentAlignment = VerticalAlignment.Center;
            Add_RemoveAttributes.PreviewMouseLeftButtonDown += Events.LoadAttributeConfig;


            buttonStack.Children.Add(ApplyChanges);
            buttonStack.Children.Add(RevertChanges);

            if (type == TypeName.Character || type == TypeName.MainCharacter)
                buttonDock.Children.Add(Add_RemoveAttributes);
            buttonDock.Children.Add(buttonStack);

            TableDock.Children.Add(SwitchToFirstTable);
            TableDock.Children.Add(SwitchToSecondTable);
            TableDock.Children.Add(buttonDock);
        }
    }
}
