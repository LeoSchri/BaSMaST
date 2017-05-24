using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;

namespace BaSMaST_V3
{
    public partial class Popup
    {
        public static Popup PopupWindow { get; private set; }
        public static BrushConverter bc = new BrushConverter();

        public static PopupType Type { get; set; }
        public static object AffectedObject { get; set; }

        public void InitWindow()
        {
            PopupWindow.Background= ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            Screen.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);

            WindowTitle.BorderThickness = new Thickness(0);
            WindowTitle.Background= ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            WindowTitle.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            WindowTitle.FontSize = AppSettings_User.FontSize * 1.5;
            WindowTitle.FontFamily = new FontFamily(AppSettings_Static.Font2);
            WindowTitle.MouseDown += Drag;

            Message.BorderThickness = new Thickness(0);
            Message.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            Message.FontSize = AppSettings_User.FontSize;
            Message.FontFamily = new FontFamily(AppSettings_Static.Font2);

            OK.Content = "OK";
            OK.FontSize = 20;
            OK.FontFamily = new FontFamily(AppSettings_Static.Font2);
            OK.FontWeight = FontWeights.Bold;
            OK.PreviewMouseLeftButtonDown += Click;
            OK.Visibility = Visibility.Collapsed;

            Yes.Content = "Yes";
            Yes.FontSize = 18;
            Yes.FontFamily = new FontFamily(AppSettings_Static.Font2);
            Yes.HorizontalAlignment = HorizontalAlignment.Right;
            Yes.PreviewMouseLeftButtonDown += Click;
            Yes.Visibility = Visibility.Collapsed;

            No.Content = "No";
            No.FontSize = 20;
            No.FontFamily = new FontFamily(AppSettings_Static.Font2);
            No.FontWeight = FontWeights.Bold;
            No.HorizontalAlignment = HorizontalAlignment.Right;
            No.PreviewMouseLeftButtonDown += Click;
            No.Visibility = Visibility.Collapsed;

            var ApplyImage = new Image();
            ApplyImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Tick")));
            ApplyImage.MaxHeight = 25;
            ApplyImage.MaxWidth = 25;
            Apply.Content = ApplyImage;

            Apply.PreviewMouseLeftButtonDown += Click;
            Apply.Visibility = Visibility.Collapsed;

            var CancelImage = new Image();
            CancelImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Cancel")));
            CancelImage.MaxHeight = 25;
            CancelImage.MaxWidth = 25;
            Cancel.Content = CancelImage;

            Cancel.PreviewMouseLeftButtonDown += Click;
            Cancel.Visibility = Visibility.Collapsed;
        }

        private void Click(object sender, MouseButtonEventArgs e)
        {
            PopupWindow.Close();
            PopupWindow = null;

            if (Type== PopupType.Overwrite)
            {
                if (sender == Yes)
                {
                    var obj = AffectedObject;

                    if (obj is Project)
                    {
                        AppSettings_User.RemoveProject(obj as Project);
                        Config.Window.ProjectName.BorderBrush = null;
                        Config.Window.AddProject();
                        Config.Window.Close();
                        Config.Window = null;
                        MainWindow.Window.LoadMainScreen();
                    }
                }
            }
            else if(Type == PopupType.Delete)
            {
                if(sender == Yes)
                {
                    var obj = AffectedObject;

                    if(obj is Book)
                    {
                        var bookConfig = Config.BookConfigs[ AppSettings_User.CurrentProject.BookManager.GetItems().IndexOf(obj as Book) ];

                        if (Config.BookConfigs.Count == AppSettings_User.CurrentProject.BookManager.GetItems().Count)
                        {
                            AppSettings_User.CurrentProject.BookManager.RemoveItem( obj as Book, TypeName.Book);
                        }

                        Config.BookConfigs.Remove(bookConfig);
                        Config.Window.BookStack.Children.Remove(bookConfig.Container);
                    }
                    else if(obj is Project)
                    {
                        AppSettings_User.RemoveProject(obj as Project);
                        AppSettings_User.ChangeCurrentProject(null);
                        MainWindow.Window.LoadWelcomeScreen();
                        MainWindow.Window.Menu.Visibility = Visibility.Collapsed;

                        if(!AppSettings_User.Projects.Any())
                        {
                            MainWindow.Window.MenuStack2.Visibility = Visibility.Collapsed;
                            MainWindow.Window.MenuStack3.Visibility = Visibility.Collapsed;
                            MainWindow.Window.MenuStack4.Visibility = Visibility.Collapsed;
                        }
                    }
                    else if(obj is Task)
                    {
                        var taskEntry = Config.TaskList.Find(t=> t.AffectedTask == obj as Task);
                        Config.Window.TaskStack.Children.Remove(taskEntry.Container);
                        Config.TaskList.Remove(taskEntry);

                        AppSettings_User.CurrentProject.TaskManager.RemoveItem(obj as Task,TypeName.Task);
                    }
                    else if(obj is Attribute)
                    {
                        var costumAttribute = Config.Attributes.Find(a => a.AffectedAttribute == obj as Attribute);
                        Config.Window.CustomAttributesStack.Children.Remove(costumAttribute.Container);
                        Config.Attributes.Remove(costumAttribute);

                        if (Config.Window.AttributesFor == TypeName.Character)
                            Character.RemoveAttributes(new List<Attribute>() { obj as Attribute },TypeName.Character,AppSettings_User.CurrentProject.CharacterManager,true);
                    }
                    else if (obj is Note)
                    {
                        var noteEntry = NoteManager.Notes.Find(t => t.AffectedNote == obj as Note);
                        NoteManager.NoteStack.Children.Remove(noteEntry.Container);
                        NoteManager.Notes.Remove(noteEntry);

                        AppSettings_User.CurrentProject.NoteManager.RemoveItem( obj as Note, TypeName.Note);
                    }
                }
            }
            else if(Type== PopupType.Save)
            {
                if(sender == Yes)
                {
                    Events.SaveTableEntries(null, null);
                }
                if (AffectedObject != null)
                {
                    if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Plot || TableManager.CurrentTables.FirstOrDefault() == TypeName.Lore)
                    {
                        if (AffectedObject == TableManager.SwitchToFirstTable)
                            TableManager.LoadTables(TypeName.Plot, TypeName.Subplot, TypeName.Event);
                        else if (AffectedObject == TableManager.SwitchToSecondTable)
                            TableManager.LoadTables(TypeName.Lore);
                    }
                    else if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Character || TableManager.CurrentTables.FirstOrDefault() == TypeName.MainCharacter)
                    {
                        if (AffectedObject == TableManager.SwitchToFirstTable)
                            TableManager.LoadTables(TypeName.Character);
                        else if (AffectedObject == TableManager.SwitchToSecondTable)
                            TableManager.LoadTables(TypeName.MainCharacter);
                    }

                }
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

        public static void ShowWindow(string message, string caption, PopupButtons buttons, PopupType type, object affectedObject = null)
        {
            PopupWindow = new Popup();
            PopupWindow.WindowTitle.Content = caption;
            PopupWindow.Message.Content = message;

            Type = type;
            AffectedObject = affectedObject;

            if (buttons == PopupButtons.OK)
                PopupWindow.OK.Visibility = Visibility.Visible;
            if (buttons == PopupButtons.YesNo)
            {
                PopupWindow.Yes.Visibility = Visibility.Visible;
                PopupWindow.No.Visibility = Visibility.Visible;
            }
            if (buttons == PopupButtons.ApplyCancel)
            {
                PopupWindow.Apply.Visibility = Visibility.Visible;
                PopupWindow.Cancel.Visibility = Visibility.Visible;
            }

            PopupWindow.ShowDialog();
        }
    }
}
