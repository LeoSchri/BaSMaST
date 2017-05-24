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
using System.Globalization;

namespace BaSMaST_V3
{
    public partial class Config
    {
        private Project Project { get; set; }
        public static List<BookConfig> BookConfigs { get; private set; }

        private void ApplyProjectSetter()
        {
            CreationDate.FontSize = AppSettings_User.FontSize;
            CreationDate.FontFamily = new FontFamily(AppSettings_Static.Font2);
            CreationDate.Height = AppSettings_User.FontSize;

            LastModified.FontSize = AppSettings_User.FontSize;
            LastModified.FontFamily = new FontFamily(AppSettings_Static.Font2);
            LastModified.Height = AppSettings_User.FontSize;

            LabelProjectName.Content = TextCatalog.GetName("Project name");
            LabelProjectName.FontSize = AppSettings_User.FontSize;
            LabelProjectName.FontFamily = new FontFamily(AppSettings_Static.Font1);
            LabelProjectName.Height = AppSettings_User.FontSize * 2.25;

            ProjectName.Height = AppSettings_User.FontSize * 1.75;
            ProjectName.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);

            LabelProjectLocation.Content = TextCatalog.GetName("Project location");
            LabelProjectLocation.FontSize = AppSettings_User.FontSize;
            LabelProjectLocation.FontFamily = new FontFamily(AppSettings_Static.Font1);
            LabelProjectLocation.Height = AppSettings_User.FontSize * 2.25;

            LocationBrowser.Content = TextCatalog.GetName("Browse");
            LocationBrowser.FontSize = AppSettings_User.FontSize;
            LocationBrowser.FontFamily = new FontFamily(AppSettings_Static.Font2);
            LocationBrowser.Height = AppSettings_User.FontSize * 1.75;
            LocationBrowser.MouseEnter += ButtonMouseEnter;
            LocationBrowser.MouseLeave += ButtonMouseLeave;
            LocationBrowser.PreviewMouseLeftButtonDown += BrowserClick;

            ProjectLocation.Height = AppSettings_User.FontSize * 1.75;
            ProjectLocation.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);

            LabelProjectBackup.Content = TextCatalog.GetName("Backup location");
            LabelProjectBackup.FontSize = AppSettings_User.FontSize;
            LabelProjectBackup.FontFamily = new FontFamily(AppSettings_Static.Font1);
            LabelProjectBackup.Height = AppSettings_User.FontSize * 2.25;

            BackupBrowser.Content = TextCatalog.GetName("Browse");
            BackupBrowser.FontSize = AppSettings_User.FontSize;
            BackupBrowser.FontFamily = new FontFamily(AppSettings_Static.Font2);
            BackupBrowser.Height = AppSettings_User.FontSize * 1.75;
            BackupBrowser.MouseEnter += ButtonMouseEnter;
            BackupBrowser.MouseLeave += ButtonMouseLeave;
            BackupBrowser.PreviewMouseLeftButtonDown += BrowserClick;

            ProjectBackup.Height = AppSettings_User.FontSize * 1.75;
            ProjectBackup.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);

            BookLabel.Content = TextCatalog.GetName("Books");
            BookLabel.FontSize = AppSettings_User.FontSize;
            BookLabel.FontFamily = new FontFamily(AppSettings_Static.Font1);
            BookLabel.Height = AppSettings_User.FontSize * 2.25;

            BookConfigs = new List<BookConfig>();
            new BookConfig("Book0", BookStack);

            if (AppSettings_User.CurrentProject != null && AppSettings_User.CurrentProject.BookManager != null && AppSettings_User.CurrentProject.BookManager.GetItems() != null && AppSettings_User.CurrentProject.BookManager.GetItems().Count > 1)
            {
                for (int i = 1; i < AppSettings_User.CurrentProject.BookManager.GetItems().Count; i++)
                {
                    new BookConfig($"Book{i}", BookStack);
                }
            }

            AddBook.Height = AppSettings_User.FontSize * 1.75;
            AddBook.Width = AddBook.Height;
            AddBook.PreviewMouseLeftButtonDown += AddAnotherBook;

            var AddBookImage = new Image();
            AddBookImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Add")));
            AddBookImage.Height = AddBook.Height * 0.6;
            AddBookImage.MaxWidth = AddBook.Width * 0.6;
            AddBook.Content = AddBookImage;

            if (Project != null)
                FillProjectFields();
        }

        private void FillProjectFields()
        {
            var p = AppSettings_User.CurrentProject;
            ProjectName.Text = p.Name;
            ProjectLocation.Text = p.Location;
            ProjectBackup.Text = p.BackupLocation;
            
            if(p.BookManager != null && p.BookManager.GetItems()!=null && p.BookManager.GetItems().Any())
            {
                p.BookManager.GetItems().ForEach(book =>
                {
                    var bookConfig = BookConfigs[ p.BookManager.GetItems().IndexOf(book) ];
                    bookConfig.BookName.Text = book.Name;
                    bookConfig.BookDateFromYear.Text = book.Begin.Year.ToString();
                    bookConfig.BookDateToYear.Text = book.End.Year.ToString();
                    bookConfig.ComboBoxes.Find(c => c.Name.Contains("FromMonth")).ComboBoxTextBox.Text = TextCatalog.GetName( CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(book.Begin.Month));
                    bookConfig.ComboBoxes.Find(c => c.Name.Contains("FromDay")).ComboBoxTextBox.Text = book.Begin.Day.ToString();
                    bookConfig.ComboBoxes.Find(c => c.Name.Contains("ToMonth")).ComboBoxTextBox.Text = TextCatalog.GetName(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(book.End.Month));
                    bookConfig.ComboBoxes.Find(c => c.Name.Contains("ToDay")).ComboBoxTextBox.Text = book.End.Day.ToString();
                });
            }
        }

        private bool ValidateProject()
        {
            var _dataIsValid = true;

            foreach (TextBox c in Helper.FindWindowChildren<TextBox>(Window))
            {
                if (string.IsNullOrEmpty(c.Text))
                {
                    c.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Red.Color1);
                    _dataIsValid = false;
                }
                else
                {
                    c.BorderBrush = null;
                }

                if (c == ProjectBackup || c == ProjectLocation)
                {
                    if (!Directory.Exists(c.Text))
                    {
                        c.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Red.Color1);
                        _dataIsValid = false;
                    }
                    else
                    {
                        c.BorderBrush = null;
                    }
                }

                if(c== ProjectName)
                {
                    if(Project == null)
                    {
                        if (AppSettings_User.Projects != null)
                        {
                            var match = AppSettings_User.Projects.Find(p => p.Name == c.Text);
                            if(match != null)
                            {
                                c.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Red.Color1);
                                Popup.ShowWindow($"{TextCatalog.GetName("Project")} {TextCatalog.GetName("Overwrite?")}", TextCatalog.GetName("Overwriting"), PopupButtons.YesNo, PopupType.Overwrite, match);
                                _dataIsValid = false;
                            }
                        }
                    }
                }

                if (c.Name.Contains("Year"))
                {
                    int res;
                    if (!Int32.TryParse(c.Text, out res))
                    {
                        c.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Red.Color1);
                        _dataIsValid = false;
                    }
                    else
                    {
                        c.BorderBrush = null;
                    }
                }
            }

            return _dataIsValid;
        }


        private void SaveProjectChanges()
        {
            var p = AppSettings_User.CurrentProject;
            p.Name= ProjectName.Text;
            if(p.Location != ProjectLocation.Text)
                p.ChangeLocation(ProjectLocation.Text);
            if(p.BackupLocation != ProjectBackup.Text)
                p.BackupLocation = ProjectBackup.Text;

            var projectBookCount = p.BookManager!=null && p.BookManager.GetItems()!=null && p.BookManager.GetItems().Any()?p.BookManager.GetItems().Count:0;

            if(projectBookCount > BookConfigs.Count)
            {
                var books = new List<Book>();
                for(int i = BookConfigs.Count; i< projectBookCount; i++)
                {
                    books.Add(p.BookManager.GetItems()[i]);
                }
                p.BookManager.RemoveItems(books, TypeName.Book);
            }

            if (projectBookCount < BookConfigs.Count)
            {
                AddBooks(p, projectBookCount);
            }

            if (p.BookManager != null && p.BookManager.GetItems().Any())
            {
                BookConfigs.ForEach(bookConfig =>
                {
                    var book = p.BookManager.GetItems()[ BookConfigs.IndexOf(bookConfig) ];
                    if (book.Name != bookConfig.BookName.Text)
                        book.Name = bookConfig.BookName.Text;

                    var fromYear = Convert.ToInt32(bookConfig.BookDateFromYear.Text);
                    var fromMonth = bookConfig.ComboBoxes.Find(c => c.Name.Contains("FromMonth")).ComboBoxTextBox.Text;
                    var fromDay = bookConfig.ComboBoxes.Find(c => c.Name.Contains("FromDay")).ComboBoxTextBox.Text;
                    var fromDate = new DateTime(fromYear, bookConfig.Months[ fromMonth ], Convert.ToInt32(fromDay));
                    if (book.Begin != fromDate)
                        book.Begin = fromDate;

                    var toYear = Convert.ToInt32(bookConfig.BookDateToYear.Text);
                    var toMonth = bookConfig.ComboBoxes.Find(c => c.Name.Contains("ToMonth")).ComboBoxTextBox.Text;
                    var toDay = bookConfig.ComboBoxes.Find(c => c.Name.Contains("ToDay")).ComboBoxTextBox.Text;
                    var toDate = new DateTime(toYear, bookConfig.Months[ toMonth ], Convert.ToInt32(toDay));
                    if (book.Begin != toDate)
                        book.Begin = toDate;
                });
            }
        }

        private void AddBooks(Project p, int projectBookCount)
        {
            var books = new List<Book>();

            if (projectBookCount < 1)
                projectBookCount = 0;
            else projectBookCount = projectBookCount - 1;

            for (int i = projectBookCount; i < BookConfigs.Count; i++)
            {
                var fromYear = Convert.ToInt32(BookConfigs[ i ].BookDateFromYear.Text);
                var fromMonth = BookConfigs[ i ].ComboBoxes.Find(c => c.Name.Contains("FromMonth")).ComboBoxTextBox.Text;
                var fromDay = BookConfigs[ i ].ComboBoxes.Find(c => c.Name.Contains("FromDay")).ComboBoxTextBox.Text;
                var fromDate = new DateTime(fromYear, BookConfigs[ i ].Months[ fromMonth ], Convert.ToInt32(fromDay));

                var toYear = Convert.ToInt32(BookConfigs[ i ].BookDateToYear.Text);
                var toMonth = BookConfigs[ i ].ComboBoxes.Find(c => c.Name.Contains("ToMonth")).ComboBoxTextBox.Text;
                var toDay = BookConfigs[ i ].ComboBoxes.Find(c => c.Name.Contains("ToDay")).ComboBoxTextBox.Text;
                var toDate = new DateTime(toYear, BookConfigs[ i ].Months[ toMonth ], Convert.ToInt32(toDay));

                books.Add(new Book(BookConfigs[ i ].BookName.Text, fromDate, toDate));
            }
            if (p.BookManager == null)
                p.BookManager = Manager<Book>.Create();
            if(books != null && books.Any())
                p.BookManager.AddItems(books);
        }

        public void AddProject()
        {
            var name = ProjectName.Text;
            var location = ProjectLocation.Text;
            var backup = ProjectBackup.Text;

            var p = new Project(name, location, backup);
            
            AppSettings_User.AddProject(p);

            AddBooks(p,0);
            MainWindow.Window.LoadMainScreen();
            MainWindow.Window.HomeBtn.IsEnabled = true;
            MainWindow.Window.TaskBtn.IsEnabled = true;
        }

        private static void RemoveBook(object sender, MouseButtonEventArgs e)
        {
            var bookConfig = BookConfigs.Find(bC => bC.Remove == sender);
            if (BookConfigs.IndexOf(bookConfig) != 0)
            {
                if(AppSettings_User.CurrentProject.BookManager.GetItems().Count >= BookConfigs.Count)
                {
                    var book = AppSettings_User.CurrentProject.BookManager.GetItems()[ BookConfigs.IndexOf(bookConfig) ];
                    Popup.ShowWindow(Helper.MakeDeletionText("Book",book.Name), TextCatalog.GetName("Confirm deletion"), PopupButtons.YesNo, PopupType.Delete, book);
                }
                else
                {
                    Config.BookConfigs.Remove(bookConfig);
                    Config.Window.BookStack.Children.Remove(bookConfig.Container);
                }
            }
        }

        private static void AddAnotherBook(object sender, MouseButtonEventArgs e)
        {
            var index = BookConfigs.Count;
            new BookConfig($"Book{index}", Window.BookStack);
        }

        public class BookConfig
        {
            public string Name { get; private set; }
            public DockPanel Container { get; private set; }
            public TextBox BookName { get; private set; }
            public TextBox BookDateFromYear { get; private set; }
            public TextBox BookDateToYear { get; private set; }
            public List<MainWindow.ComboBox> ComboBoxes { get; private set; }
            public Button Remove { get; private set; }

            public Dictionary<string, int> Months { get; private set; }

            public BookConfig(string name, DockPanel parent)
            {
                CreateBookConfig(name, parent);
                BookConfigs.Add(this);
            }

            public void CreateBookConfig(string name, DockPanel parent)
            {
                ComboBoxes = new List<MainWindow.ComboBox>();
                Months = new Dictionary<string, int>();

                Container = new DockPanel();
                Container.HorizontalAlignment = HorizontalAlignment.Stretch;
                Container.VerticalAlignment = VerticalAlignment.Top;
                Container.SetValue(DockPanel.DockProperty, Dock.Top);
                Container.LastChildFill = false;

                var dockTop = new DockPanel();
                dockTop.HorizontalAlignment = HorizontalAlignment.Stretch;
                dockTop.VerticalAlignment = VerticalAlignment.Top;
                dockTop.LastChildFill = true;
                dockTop.SetValue(DockPanel.DockProperty, Dock.Top);

                var dockBottom = new DockPanel();
                dockBottom.HorizontalAlignment = HorizontalAlignment.Stretch;
                dockBottom.VerticalAlignment = VerticalAlignment.Top;
                dockBottom.Margin = new Thickness(10, 0, 0, 0);
                dockBottom.LastChildFill = false;
                dockBottom.SetValue(DockPanel.DockProperty, Dock.Bottom);

                BookName = new TextBox();
                BookName.Margin = new Thickness(10, 0, 10, 5);
                BookName.Height = AppSettings_User.FontSize * 1.75;
                BookName.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                BookName.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                BookName.Style = ( Style ) BookName.FindResource("ComboBoxTextBox");
                BookName.VerticalContentAlignment = VerticalAlignment.Top;
                BookName.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                BookName.HorizontalAlignment = HorizontalAlignment.Stretch;
                BookName.VerticalAlignment = VerticalAlignment.Top;
                BookName.Padding = new Thickness(2, 0, 0, 0);
                BookName.BorderThickness = new Thickness(1);
                BookName.BorderBrush = null;
                BookName.Height = AppSettings_User.FontSize * 1.75;

                Remove = new Button();
                Remove.Style = ( Style ) Remove.FindResource("PopupButtonRed");
                Remove.Margin = new Thickness(5, 0, 10, 5);
                Remove.Background = null;
                Remove.BorderBrush = null;
                Remove.Height = AppSettings_User.FontSize * 1.75;
                Remove.Width = Remove.Height;
                Remove.HorizontalAlignment = HorizontalAlignment.Right;
                Remove.VerticalAlignment = VerticalAlignment.Top;
                Remove.HorizontalContentAlignment = HorizontalAlignment.Center;
                Remove.VerticalContentAlignment = VerticalAlignment.Center;
                Remove.SetValue(DockPanel.DockProperty, Dock.Right);
                Remove.BorderThickness = new Thickness(0);
                Remove.PreviewMouseLeftButtonDown += RemoveBook;

                var RemoveImage = new Image();
                RemoveImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Remove")));
                RemoveImage.Width = Remove.Width * 0.6;
                Remove.Content = RemoveImage;

                dockTop.Children.Add(Remove);

                var months = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.ToList();
                var monthNames = new List<string>();
                months.ForEach(m =>
                {
                    monthNames.Add(TextCatalog.GetName(m));
                    Months.Add(m, months.IndexOf(m));
                });

                ComboBoxes.Add(new MainWindow.ComboBox("BookDateFromDay", dockBottom, 50, 35, VerticalAlignment.Top, HorizontalAlignment.Stretch, Dock.Left, new Thickness(0), GeneralEvents.SetMonthDayCount(DateTime.Now.Year, DateTime.Now.Month), GeneralEvents.SetMonthDayCount(DateTime.Now.Year, DateTime.Now.Month)[ 0 ], false, false));
                ComboBoxes.Add(new MainWindow.ComboBox("BookDateFromMonth", dockBottom, 90, 35, VerticalAlignment.Top, HorizontalAlignment.Stretch, Dock.Left, new Thickness(0), monthNames, monthNames[ DateTime.Now.Month - 1 ], false, false, GeneralEvents.ResetMonthDayCounts));

                BookDateFromYear = new TextBox();
                BookDateFromYear.Name = "BookDateFromYear";
                BookDateFromYear.Margin = new Thickness(10, 0, 10, 5);
                BookDateFromYear.Height = AppSettings_User.FontSize * 1.75;
                BookDateFromYear.MinWidth = 50;
                BookDateFromYear.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                BookDateFromYear.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                BookDateFromYear.Style = ( Style ) BookDateFromYear.FindResource("ComboBoxTextBox");
                BookDateFromYear.VerticalContentAlignment = VerticalAlignment.Center;
                BookDateFromYear.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                BookDateFromYear.HorizontalAlignment = HorizontalAlignment.Right;
                BookDateFromYear.VerticalAlignment = VerticalAlignment.Top;
                BookDateFromYear.Padding = new Thickness(2, 0, 0, 0);
                BookDateFromYear.BorderThickness = new Thickness(1);
                BookDateFromYear.BorderBrush = null;
                BookDateFromYear.Height = AppSettings_User.FontSize * 1.75;
                BookDateFromYear.Text = DateTime.Now.Year.ToString();
                BookDateFromYear.TextChanged += GeneralEvents.ResetMonthDayCounts;
                BookDateFromYear.SetValue(DockPanel.DockProperty, Dock.Left);
                dockBottom.Children.Add(BookDateFromYear);

                BookDateToYear = new TextBox();
                BookDateToYear.Name = "BookDateToYear";
                BookDateToYear.Margin = new Thickness(10, 0, 10, 5);
                BookDateToYear.Height = AppSettings_User.FontSize * 1.75;
                BookDateToYear.MinWidth = 50;
                BookDateToYear.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                BookDateToYear.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                BookDateToYear.Style = ( Style ) BookDateToYear.FindResource("ComboBoxTextBox");
                BookDateToYear.VerticalContentAlignment = VerticalAlignment.Center;
                BookDateToYear.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                BookDateToYear.HorizontalAlignment = HorizontalAlignment.Right;
                BookDateToYear.VerticalAlignment = VerticalAlignment.Top;
                BookDateToYear.Padding = new Thickness(2, 0, 0, 0);
                BookDateToYear.BorderThickness = new Thickness(1);
                BookDateToYear.BorderBrush = null;
                BookDateToYear.Height = AppSettings_User.FontSize * 1.75;
                BookDateToYear.Text = DateTime.Now.Year.ToString();
                BookDateToYear.TextChanged += GeneralEvents.ResetMonthDayCounts;
                BookDateToYear.SetValue(DockPanel.DockProperty, Dock.Right);
                dockBottom.Children.Add(BookDateToYear);

                ComboBoxes.Add(new MainWindow.ComboBox("BookDateToMonth", dockBottom, 90, 35, VerticalAlignment.Top, HorizontalAlignment.Stretch, Dock.Right, new Thickness(0), monthNames, monthNames[ DateTime.Now.Month - 1 ], false, false, GeneralEvents.ResetMonthDayCounts));
                ComboBoxes.Add(new MainWindow.ComboBox("BookDateToDay", dockBottom, 50, 35, VerticalAlignment.Top, HorizontalAlignment.Stretch, Dock.Right, new Thickness(0), GeneralEvents.SetMonthDayCount(DateTime.Now.Year, DateTime.Now.Month), GeneralEvents.SetMonthDayCount(DateTime.Now.Year, DateTime.Now.Month)[ 0 ], false, false));

                dockTop.Children.Add(BookName);
                Container.Children.Add(dockTop);
                Container.Children.Add(dockBottom);
                parent.Children.Add(Container);
            }
        }
    }
}
