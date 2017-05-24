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
        public static List<TaskEntry> TaskList { get; set; }
        public TextBox AddTaskName { get; set; }
        public List<MainWindow.ComboBox> ComboBoxes { get; set; }
        public TextBox DateYear { get; set; }
        public Button AcceptTask { get; set; }
        public Button CancelTask { get; set; }
        public Label ShowTasksLabel { get; set; }
        public bool ShowDoneTasks { get; set; }

        public Dictionary<string, int> Months { get; private set; }

        public void ApplyTaskSetter()
        {
            //Apply.Visibility = Visibility.Collapsed;
            ShowDoneTasks = false;

            TaskDock.Width = ContentPanel.ActualWidth - 10;

            AddTaskBtn.PreviewMouseLeftButtonDown += AddTask;

            var AddTaskImage = new Image();
            AddTaskImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Add")));
            AddTaskImage.Height = AddTaskBtn.ActualHeight * 0.6;
            AddTaskImage.MaxWidth = AddTaskBtn.ActualWidth * 0.6;
            AddTaskBtn.Content = AddTaskImage;

            SortTasksBtn.PreviewMouseLeftButtonDown += SortTasks;

            var SortTaskImage = new Image();
            SortTaskImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Sort")));
            SortTaskImage.Height = SortTasksBtn.ActualHeight * 0.6;
            SortTaskImage.MaxWidth = SortTasksBtn.ActualWidth * 0.6;
            SortTasksBtn.Content = SortTaskImage;

            ReloadBtn.PreviewMouseLeftButtonDown += ReloadList;

            var ReloadImage = new Image();
            ReloadImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Reload")));
            ReloadImage.Height = ReloadBtn.ActualHeight * 0.6;
            ReloadImage.MaxWidth = ReloadBtn.ActualWidth * 0.6;
            ReloadBtn.Content = ReloadImage;

            var box = new Viewbox();
            ShowTasksLabel = new Label();
            ShowTasksLabel.Content = TextCatalog.GetName("ShowDoneTasks");
            ShowTasksLabel.FontFamily = new FontFamily(AppSettings_Static.Font2);
            ShowTasksLabel.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            box.Child = ShowTasksLabel;
            ShowTasks.Content = box;

            ShowTasks.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            ShowTasks.FontSize = AppSettings_User.FontSize;
            ShowTasks.FontFamily = new FontFamily(AppSettings_Static.Font2);
            ShowTasks.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            ShowTasks.BorderThickness = new Thickness(1);
            ShowTasks.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            ShowTasks.Style = ( Style ) ShowTasks.FindResource($"FieldSmall{AppSettings_User.ColorSchema.Name}");
            ShowTasks.PreviewMouseLeftButtonDown += AlterTaskList;

            var TaskLabel = new Label();
            TaskLabel.Content = TextCatalog.GetName("New Task");
            TaskLabel.Margin = new Thickness(10, 0, 2, 5);
            TaskLabel.Height = AppSettings_User.FontSize * 2.25;
            TaskLabel.Background = null;
            TaskLabel.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            TaskLabel.FontSize = AppSettings_User.FontSize;
            TaskLabel.FontFamily = new FontFamily(AppSettings_Static.Font1);
            TaskLabel.VerticalContentAlignment = VerticalAlignment.Center;
            TaskLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
            TaskLabel.HorizontalAlignment = HorizontalAlignment.Left;
            TaskLabel.VerticalAlignment = VerticalAlignment.Top;
            TaskLabel.Padding = new Thickness(2, 0, 0, 0);
            TaskLabel.BorderThickness = new Thickness(0);
            TaskLabel.BorderBrush = null;
            TaskLabel.Height = AppSettings_User.FontSize * 1.75;
            TaskLabel.SetValue(DockPanel.DockProperty, Dock.Top);
            AddTaskDock.Children.Add(TaskLabel);

            AddTaskName = new TextBox();
            AddTaskName.Margin = new Thickness(10, 0, 2, 5);
            AddTaskName.Height = AppSettings_User.FontSize * 1.75;
            AddTaskName.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            AddTaskName.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            AddTaskName.FontSize = AppSettings_User.FontSize;
            AddTaskName.FontFamily = new FontFamily(AppSettings_Static.Font2);
            AddTaskName.VerticalContentAlignment = VerticalAlignment.Top;
            AddTaskName.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            AddTaskName.HorizontalAlignment = HorizontalAlignment.Stretch;
            AddTaskName.VerticalAlignment = VerticalAlignment.Top;
            AddTaskName.Padding = new Thickness(2, 0, 0, 0);
            AddTaskName.BorderThickness = new Thickness(1);
            AddTaskName.BorderBrush = null;
            AddTaskName.Style = ( Style ) AddTaskName.FindResource("ComboBoxTextBox");
            AddTaskName.Height = AppSettings_User.FontSize * 1.75;

            CancelTask = new Button();
            CancelTask.Margin = new Thickness(2,0,2,5);
            CancelTask.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
            CancelTask.HorizontalAlignment = HorizontalAlignment.Right;
            CancelTask.VerticalAlignment = VerticalAlignment.Top;
            CancelTask.Height = AppSettings_User.FontSize * 1.75;
            CancelTask.Width = CancelTask.Height;
            CancelTask.BorderThickness = new Thickness(0);
            CancelTask.Style = ( Style ) CancelTask.FindResource("PopupButtonRed");
            CancelTask.SetValue(DockPanel.DockProperty, Dock.Right);
            CancelTask.PreviewMouseLeftButtonDown += RemoveTask;
            AddTaskDock.Children.Add(CancelTask);

            var CancelTaskImage = new Image();
            CancelTaskImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Cancel")));
            CancelTaskImage.Height = CancelTask.Height * 0.6;
            CancelTaskImage.MaxWidth = CancelTask.Width * 0.6;
            CancelTask.Content = CancelTaskImage;

            AcceptTask = new Button();
            AcceptTask.Margin = new Thickness(2,0,2,5);
            AcceptTask.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
            AcceptTask.HorizontalAlignment = HorizontalAlignment.Right;
            AcceptTask.VerticalAlignment = VerticalAlignment.Top;
            AcceptTask.Height = AppSettings_User.FontSize * 1.75;
            AcceptTask.Width = AcceptTask.Height;
            AcceptTask.BorderThickness = new Thickness(0);
            AcceptTask.Style = ( Style ) AcceptTask.FindResource("PopupButtonGreen");
            AcceptTask.SetValue(DockPanel.DockProperty, Dock.Right);
            AcceptTask.PreviewMouseLeftButtonDown += NewTask;
            AddTaskDock.Children.Add(AcceptTask);

            var AcceptTaskImage = new Image();
            AcceptTaskImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Tick")));
            AcceptTaskImage.Height = AcceptTask.Height * 0.6;
            AcceptTaskImage.MaxWidth = AcceptTask.Width * 0.6;
            AcceptTask.Content = AcceptTaskImage;

            DateYear = new TextBox();
            DateYear.Margin = new Thickness(0, 0, 2, 5);
            DateYear.Height = AppSettings_User.FontSize * 1.75;
            DateYear.MinWidth = 50;
            DateYear.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            DateYear.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            DateYear.FontSize = AppSettings_User.FontSize;
            DateYear.FontFamily = new FontFamily(AppSettings_Static.Font2);
            DateYear.Style = ( Style ) DateYear.FindResource("ComboBoxTextBox");
            DateYear.VerticalContentAlignment = VerticalAlignment.Center;
            DateYear.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            DateYear.HorizontalAlignment = HorizontalAlignment.Right;
            DateYear.VerticalAlignment = VerticalAlignment.Top;
            DateYear.Padding = new Thickness(2, 0, 0, 0);
            DateYear.BorderThickness = new Thickness(1);
            DateYear.BorderBrush = null;
            DateYear.Height = AppSettings_User.FontSize * 1.75;
            DateYear.Text = DateTime.Now.Year.ToString();
            DateYear.TextChanged += GeneralEvents.ResetMonthDayCounts;
            DateYear.SetValue(DockPanel.DockProperty, Dock.Right);
            AddTaskDock.Children.Add(DateYear);

            Months = new Dictionary<string, int>();
            var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.ToList();
            var monthNames = new List<string>();
            months.ForEach(m =>
            {
                monthNames.Add(TextCatalog.GetName(m));
                Months.Add(m, months.IndexOf(m));
            });

            ComboBoxes = new List<MainWindow.ComboBox>();

            ComboBoxes.Add(new MainWindow.ComboBox("DueMonth", AddTaskDock, 90, 35, VerticalAlignment.Top, HorizontalAlignment.Stretch, Dock.Right, new Thickness(0), monthNames, monthNames[ DateTime.Now.Month - 1 ], false, false, GeneralEvents.ResetMonthDayCounts));
            ComboBoxes.Add(new MainWindow.ComboBox("DueDay", AddTaskDock, 50, 35, VerticalAlignment.Top, HorizontalAlignment.Stretch, Dock.Right, new Thickness(0), GeneralEvents.SetMonthDayCount(DateTime.Now.Year, DateTime.Now.Month), GeneralEvents.SetMonthDayCount(DateTime.Now.Year, DateTime.Now.Month)[ 0 ], false, false));

            ComboBoxes.Add(new MainWindow.ComboBox("Importance", AddTaskDock, 90, 35, VerticalAlignment.Top, HorizontalAlignment.Stretch, Dock.Right, new Thickness(0), Helper.GetTypesAsList<Importance>(), Helper.GetTypesAsList<Importance>()[ 0 ], false, false));

            AddTaskDock.Children.Add(AddTaskName);

            LoadTasks();
        }

        private void LoadTasks(bool WithSorting = true)
        {
            TaskList = new List<TaskEntry>();
            TaskStack.Children.Clear();
            if(WithSorting)
                SortTasks();

            if (AppSettings_User.CurrentProject.TaskManager != null && AppSettings_User.CurrentProject.TaskManager.GetItems()!=null && AppSettings_User.CurrentProject.TaskManager.GetItems().Any())
            {
                AppSettings_User.CurrentProject.TaskManager.GetItems().ForEach(t =>
                {
                    new TaskEntry(t, TaskStack);
                });
            }

            if (!ShowDoneTasks)
                RemoveDoneTasks();
        }

        private void ReloadList(object sender, RoutedEventArgs e)
        {
            LoadTasks(false);
        }

        private void AlterTaskList(object sender, RoutedEventArgs e)
        {
            if (!ShowDoneTasks)
            {
                ShowDoneTasks = true;
            }
            else
            {
                ShowDoneTasks = false;
            }
            LoadTasks(false);
            ShowTasksLabel.Content = !ShowDoneTasks?TextCatalog.GetName("ShowDoneTasks"): TextCatalog.GetName("HideDoneTasks");
        }

        private void RemoveDoneTasks()
        {
            var tasksToRemove = new List<TaskEntry>();

            for (int i = 0; i < TaskList.Count; i++)
            {
                if (TaskList[ i ].AffectedTask.State == TaskState.Done)
                {
                    tasksToRemove.Add(TaskList[ i ]);
                }
            }

            tasksToRemove.ForEach(t =>
            {
                TaskList.Remove(t);
                TaskStack.Children.Remove(t.Container);
            });
        }

        private static void ChangeState(object sender, MouseButtonEventArgs e)
        {
            var taskEntry = TaskList.Find(t => t.State == sender);
            var task = taskEntry.AffectedTask;

            var StateImage = new Image();
            StateImage.Height = ( sender as Button ).Height * 0.5;
            ( sender as Button ).Content = StateImage;

            if (task.State == TaskState.InProgress)
            {
                StateImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Tick")));
                ( sender as Button ).Background = ( Brush ) bc.ConvertFrom(ColorSchema.Green.Color1);
                task.State = TaskState.Done;
            }
            else if (task.State == TaskState.New)
            {
                StateImage.Source = new BitmapImage(new Uri(Helper.GetIcon("InProgress")));
                ( sender as Button ).Background = ( Brush ) bc.ConvertFrom(ColorSchema.Sun.Color1);
                task.State = TaskState.InProgress;
            }
            else if (task.State == TaskState.Done)
            {
                StateImage.Source = new BitmapImage(new Uri(Helper.GetIcon("InProgress")));
                ( sender as Button ).Background = ( Brush ) bc.ConvertFrom(ColorSchema.Sun.Color1);
                task.State = TaskState.InProgress;
            }
        }

        private void AddTask(object sender, MouseButtonEventArgs e)
        {
            AddTaskDock.Visibility = Visibility.Visible;
            AddTaskBtn.Visibility = Visibility.Collapsed;
        }

        private void RemoveTask(object sender, MouseButtonEventArgs e)
        {
            AddTaskName.Text = string.Empty;
            AddTaskDock.Visibility = Visibility.Collapsed;
            AddTaskBtn.Visibility = Visibility.Visible;
        }

        private void NewTask(object sender, MouseButtonEventArgs e)
        {
            if (!ValidateTask())
                return;

            var Year = Convert.ToInt32(DateYear.Text);
            var Month = ComboBoxes.Find(c => c.Name.Contains("Month")).ComboBoxTextBox.Text;
            var Day = ComboBoxes.Find(c => c.Name.Contains("Day")).ComboBoxTextBox.Text;
            var Date = new DateTime(Year, Months[ Month ]+1, Convert.ToInt32(Day));

            var task = new Task(AddTaskName.Text, TaskState.New,Helper.GetType<Importance>( ComboBoxes.Find(c=> c.Name == "Importance").ComboBoxTextBox.Text),Date);

            new TaskEntry(task,TaskStack);

            if (AppSettings_User.CurrentProject.TaskManager == null)
                AppSettings_User.CurrentProject.TaskManager = Manager<Task>.Create();
            AppSettings_User.CurrentProject.TaskManager.AddItem(task);

            AddTaskName.Text = string.Empty;
            AddTaskDock.Visibility = Visibility.Collapsed;
            AddTaskBtn.Visibility = Visibility.Visible;
        }

        private void SortTasks(object sender, MouseButtonEventArgs e)
        {
            LoadTasks();
        }

        private void SortTasks()
        {
            if (AppSettings_User.CurrentProject.TaskManager != null && AppSettings_User.CurrentProject.TaskManager.GetItems()!=null && AppSettings_User.CurrentProject.TaskManager.GetItems().Any())
            {
                AppSettings_User.CurrentProject.TaskManager.GetItems().Sort(delegate (Task x, Task y)
                    {
                        int a = x.Importance.CompareTo(y.Importance);
                        if (a == 0)
                            a = x.DueDate.CompareTo(y.DueDate);
                        if (a == 0)
                            a = y.State.CompareTo(x.State);

                        return a;
                    });
            }
        }

        private static void RemoveAffectedTask(object sender, MouseButtonEventArgs e)
        {
            var taskEntry = TaskList.Find(t=> t.Remove == sender);
            Popup.ShowWindow(Helper.MakeDeletionText("Task", taskEntry.AffectedTask.Name), TextCatalog.GetName("Confirm deletion"), PopupButtons.YesNo, PopupType.Delete, taskEntry.AffectedTask);
        }

        private bool ValidateTask()
        {
            var _dataIsValid = true;

            foreach (TextBox c in Helper.FindWindowChildren<TextBox>(AddTaskDock))
            {
                if (string.IsNullOrEmpty(c.Text) && c!= DateYear)
                {
                    c.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Red.Color1);
                    _dataIsValid = false;
                }
                else
                {
                    c.BorderBrush = null;
                }

                if (c == DateYear && !string.IsNullOrEmpty(DateYear.Text))
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

        public class TaskEntry
        {
            public string Name { get; private set; }
            public DockPanel Container { get; private set; }
            public Label TaskName { get; private set; }
            public Label TaskDueDate { get; private set; }
            public Button State { get; set; }
            public Button ImportanceBtn { get; set; }
            public Button OverDue { get; set; }
            public Button Remove { get; set; }

            public Task AffectedTask { get; private set; }

            public TaskEntry(Task task, StackPanel parent)
            {
                CreateTaskEntry(task, parent);
                TaskList.Add(this);
            }

            public void CreateTaskEntry(Task task, StackPanel parent)
            {
                Container = new DockPanel();
                Container.Margin = new Thickness(5, 0, 0, 0);
                AffectedTask = task;

                ImportanceBtn = new Button();
                ImportanceBtn.Style = ( Style ) ImportanceBtn.FindResource("FieldSmall");
                ImportanceBtn.Margin = new Thickness(0, 0, 2, 5);
                ImportanceBtn.BorderBrush = null;
                ImportanceBtn.Height = AppSettings_User.FontSize * 1.75;
                ImportanceBtn.Width = ImportanceBtn.Height;
                ImportanceBtn.HorizontalAlignment = HorizontalAlignment.Left;
                ImportanceBtn.VerticalAlignment = VerticalAlignment.Top;
                ImportanceBtn.SetValue(DockPanel.DockProperty, Dock.Left);
                ImportanceBtn.BorderThickness = new Thickness(0);
                switch(AffectedTask.Importance)
                {
                    case Importance.High: ImportanceBtn.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Red.Color1); break;
                    case Importance.Medium: ImportanceBtn.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Sun.Color1); break;
                    case Importance.Low: ImportanceBtn.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Green.Color1); break;
                }
                Container.Children.Add(ImportanceBtn);

                TaskName = new Label();
                TaskName.Content = AffectedTask.Name;
                TaskName.Margin = new Thickness(2, 0, 2, 5);
                TaskName.Height = AppSettings_User.FontSize * 1.75;
                TaskName.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                TaskName.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                TaskName.FontSize = AppSettings_User.FontSize;
                TaskName.FontFamily = new FontFamily(AppSettings_Static.Font2);
                TaskName.VerticalContentAlignment = VerticalAlignment.Center;
                TaskName.HorizontalContentAlignment = HorizontalAlignment.Center;
                TaskName.HorizontalAlignment = HorizontalAlignment.Stretch;
                TaskName.VerticalAlignment = VerticalAlignment.Top;
                TaskName.Padding = new Thickness(2, 0, 0, 0);
                TaskName.BorderThickness = new Thickness(0);
                TaskName.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                TaskName.Style = ( Style ) TaskName.FindResource("RoundedLabel");
                TaskName.Height = AppSettings_User.FontSize * 1.75;

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
                Remove.PreviewMouseLeftButtonDown += RemoveAffectedTask;

                var RemoveImage = new Image();
                RemoveImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Remove")));
                RemoveImage.Width = Remove.Width * 0.6;
                Remove.Content = RemoveImage;

                Container.Children.Add(Remove);

                OverDue = new Button();
                OverDue.Style = ( Style ) OverDue.FindResource("PopupButtonRed");
                OverDue.Margin = new Thickness(2, 0, 0, 5);
                OverDue.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Red.Color1); ;
                OverDue.BorderBrush = null;
                OverDue.Height = AppSettings_User.FontSize * 1.75;
                OverDue.Width = OverDue.Height;
                OverDue.HorizontalAlignment = HorizontalAlignment.Right;
                OverDue.VerticalAlignment = VerticalAlignment.Top;
                OverDue.HorizontalContentAlignment = HorizontalAlignment.Center;
                OverDue.VerticalContentAlignment = VerticalAlignment.Center;
                OverDue.SetValue(DockPanel.DockProperty, Dock.Right);
                OverDue.BorderThickness = new Thickness(0);
                OverDue.Visibility = Visibility.Hidden;

                var OverDueImage = new Image();
                OverDueImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Attention")));
                OverDueImage.Height = OverDue.Height * 0.6;
                OverDueImage.MaxWidth = OverDue.Width * 0.6;
                OverDue.Content = OverDueImage;
                Container.Children.Add(OverDue);

                if (AffectedTask.DueDate < DateTime.Now && AffectedTask.State != TaskState.Done)
                {
                    OverDue.Visibility = Visibility.Visible;

                    var tooltip = new ToolTip { Content = TextCatalog.GetName("Overdue") };
                    tooltip.FontSize = AppSettings_User.FontSize * 0.75;
                    tooltip.FontFamily = new FontFamily(AppSettings_Static.Font2);
                    OverDue.ToolTip = tooltip;
                }

                State = new Button();
                State.Style = ( Style ) State.FindResource("FieldSmall");
                State.Margin = new Thickness(2, 0, 2, 5);
                State.Background = null;
                State.BorderBrush = null;
                State.Height = AppSettings_User.FontSize * 1.75;
                State.Width = State.Height;
                State.HorizontalAlignment = HorizontalAlignment.Right;
                State.VerticalAlignment = VerticalAlignment.Top;
                State.HorizontalContentAlignment = HorizontalAlignment.Center;
                State.VerticalContentAlignment = VerticalAlignment.Center;
                State.SetValue(DockPanel.DockProperty, Dock.Right);
                State.BorderThickness = new Thickness(0);
                State.PreviewMouseLeftButtonDown += ChangeState;

                var StateImage = new Image();
                StateImage.Height = State.Height * 0.5;
                switch (AffectedTask.State)
                {
                    case TaskState.New:
                        State.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
                        State.Content = null;
                        break;
                    case TaskState.InProgress:
                        State.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Sun.Color1);
                        StateImage.Source = new BitmapImage(new Uri(Helper.GetIcon("InProgress")));
                        State.Content = StateImage;
                        break;
                    case TaskState.Done:
                        State.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Green.Color1);
                        StateImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Tick")));
                        State.Content = StateImage;
                        break;
                }
                Container.Children.Add(State);

                TaskDueDate = new Label();
                TaskDueDate.Content = string.Format("{0:dd/MM/yyyy}", AffectedTask.DueDate);
                TaskDueDate.Margin = new Thickness(2, 0, 2, 5);
                TaskDueDate.Height = AppSettings_User.FontSize * 1.75;
                TaskDueDate.MinWidth = 70;
                TaskDueDate.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                TaskDueDate.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                TaskDueDate.FontSize = AppSettings_User.FontSize;
                TaskDueDate.FontFamily = new FontFamily(AppSettings_Static.Font2);
                TaskDueDate.VerticalContentAlignment = VerticalAlignment.Center;
                TaskDueDate.HorizontalContentAlignment = HorizontalAlignment.Center;
                TaskDueDate.HorizontalAlignment = HorizontalAlignment.Stretch;
                TaskDueDate.VerticalAlignment = VerticalAlignment.Top;
                TaskDueDate.Padding = new Thickness(2, 0, 0, 0);
                TaskDueDate.BorderThickness = new Thickness(0);
                TaskDueDate.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                TaskDueDate.Style = ( Style ) TaskDueDate.FindResource("RoundedLabel");
                TaskDueDate.SetValue(DockPanel.DockProperty, Dock.Right);
                Container.Children.Add(TaskDueDate);

                Container.Children.Add(TaskName);
                parent.Children.Add(Container);
            }
        }
    }
}
