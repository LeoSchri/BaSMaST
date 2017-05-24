using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace BaSMaST_V3
{
    public class NoteManager
    {
        public static Border NoteBorder { get; set; }
        public static StackPanel NoteStack { get; set; }
        public static Image OpenCloseNoteImg { get; set; }
        public static Button AddNoteBtn { get; set; }
        public static DockPanel AddNoteStack { get; set; }
        public static TextBox AddNoteName { get; set; }
        public static TextBox AddNoteContent { get; set; }
        public static Button AcceptNote { get; set; }
        public static Button CancelNote { get; set; }
        public static List<NoteEntry> Notes { get; set; }
        public static Base NoteOwner { get; set; }

        public static BrushConverter bc = new BrushConverter();

        public static void BuildNoteManager()
        {
            var NoteDock = new DockPanel();
            NoteDock.Margin = new Thickness(10);
            NoteDock.SetValue(DockPanel.DockProperty, Dock.Right);

            NoteBorder = new Border();
            NoteBorder.Margin = new Thickness(5, 0, 0, 0);
            NoteBorder.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
            NoteBorder.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
            NoteBorder.BorderThickness = new Thickness(1);
            NoteBorder.CornerRadius = new CornerRadius(15);
            NoteBorder.MinWidth = 200;
            NoteBorder.MaxWidth = 300;
            NoteBorder.Visibility = Visibility.Collapsed;

            var NotePanel = new DockPanel();
            NotePanel.MaxWidth = 350;
            NoteBorder.Child = NotePanel;

            var OpenCloseNotes = new Button();
            OpenCloseNotes.MaxWidth = 20;
            OpenCloseNotes.SetValue(DockPanel.DockProperty, Dock.Left);
            OpenCloseNotes.Style = ( Style ) MainWindow.Window.PageDown.FindResource($"PopupButton{AppSettings_User.ColorSchema.Name}");

            OpenCloseNoteImg = new Image();
            OpenCloseNoteImg.Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowLeft{ AppSettings_User.ColorSchema.Name }")));
            OpenCloseNoteImg.Width = OpenCloseNotes.Width * 0.7;

            OpenCloseNotes.Content = OpenCloseNoteImg;
            OpenCloseNotes.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color2);
            OpenCloseNotes.BorderBrush = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color2);
            OpenCloseNotes.BorderThickness = new Thickness(1);
            OpenCloseNotes.PreviewMouseLeftButtonDown += Events.OpenCloseNotesPage;

            NoteDock.Children.Add(OpenCloseNotes);
            NoteDock.Children.Add(NoteBorder);

            var NoteHeader = new DockPanel();
            NoteHeader.SetValue(DockPanel.DockProperty, Dock.Top);

            var NoteTitle = new Label();
            NoteTitle.HorizontalAlignment = HorizontalAlignment.Center;
            NoteTitle.Margin = new Thickness(5, 5, 0, 0);
            NoteTitle.Content = TextCatalog.GetName("Notes");
            NoteTitle.FontFamily = new FontFamily(AppSettings_Static.Font1);
            NoteTitle.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            NoteTitle.FontSize = AppSettings_User.FontSize * 1.75;

            AddNoteBtn = new Button();
            AddNoteBtn.Height = AppSettings_User.FontSize * 2;
            AddNoteBtn.Width = AddNoteBtn.Height;
            AddNoteBtn.Style = ( Style ) AddNoteBtn.FindResource("PopupButtonGreen");
            AddNoteBtn.Margin = new Thickness(5, 5, 10, 5);
            AddNoteBtn.Background = null;
            AddNoteBtn.BorderBrush = null;
            AddNoteBtn.BorderThickness = new Thickness(0);
            AddNoteBtn.PreviewMouseLeftButtonDown += Events.AddNodeBlock;
            MainWindow.Window.AddImage(AddNoteBtn, "Add");
            AddNoteBtn.SetValue(DockPanel.DockProperty, Dock.Right);

            NoteHeader.Children.Add(AddNoteBtn);
            NoteHeader.Children.Add(NoteTitle);

            var scroll = new ScrollViewer();
            scroll.MaxHeight = MainWindow.Window.ContentPanel.ActualHeight * 0.65;
            scroll.VerticalAlignment = VerticalAlignment.Top;

            NoteStack = new StackPanel();

            scroll.Content = NoteStack;

            Notes = new List<NoteEntry>();
            LoadNotes();

            AddNoteStack = new DockPanel();
            AddNoteStack.Visibility = Visibility.Collapsed;
            AddNoteStack.Margin = new Thickness(0, 0, 10, 10);
            AddNoteStack.SetValue(DockPanel.DockProperty, Dock.Bottom);

            var Dock1 = new DockPanel();

            var Dock2 = new DockPanel();
            Dock2.HorizontalAlignment = HorizontalAlignment.Right;
            Dock2.VerticalAlignment = VerticalAlignment.Bottom;
            Dock2.SetValue(DockPanel.DockProperty, Dock.Bottom);

            var NoteLabel = new Label();
            NoteLabel.Content = TextCatalog.GetName("New Note");
            NoteLabel.Margin = new Thickness(10, 0, 2, 5);
            NoteLabel.Height = AppSettings_User.FontSize * 2.25;
            NoteLabel.Background = null;
            NoteLabel.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            NoteLabel.FontSize = AppSettings_User.FontSize;
            NoteLabel.FontFamily = new FontFamily(AppSettings_Static.Font1);
            NoteLabel.VerticalContentAlignment = VerticalAlignment.Center;
            NoteLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
            NoteLabel.HorizontalAlignment = HorizontalAlignment.Left;
            NoteLabel.VerticalAlignment = VerticalAlignment.Top;
            NoteLabel.Padding = new Thickness(2, 0, 0, 0);
            NoteLabel.BorderThickness = new Thickness(0);
            NoteLabel.BorderBrush = null;
            NoteLabel.SetValue(DockPanel.DockProperty, Dock.Top);

            //var NoteNameLabel = new Label();
            //NoteNameLabel.Content = "Name:";
            //NoteNameLabel.Margin = new Thickness(10, 0, 2, 5);
            //NoteNameLabel.Height = AppSettings_User.FontSize * 1.75;
            //NoteNameLabel.Background = null;
            //NoteNameLabel.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            //NoteNameLabel.FontSize = AppSettings_User.FontSize;
            //NoteNameLabel.FontFamily = new FontFamily(AppSettings_Static.Font2);
            //NoteNameLabel.VerticalContentAlignment = VerticalAlignment.Center;
            //NoteNameLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
            //NoteNameLabel.HorizontalAlignment = HorizontalAlignment.Left;
            //NoteNameLabel.VerticalAlignment = VerticalAlignment.Stretch;
            //NoteNameLabel.Padding = new Thickness(2, 0, 0, 0);
            //NoteNameLabel.BorderThickness = new Thickness(0);
            //NoteNameLabel.BorderBrush = null;
            //NoteNameLabel.SetValue(DockPanel.DockProperty, Dock.Left);

            //AddNoteName = new TextBox();
            //AddNoteName.Margin = new Thickness(5, 0, 2, 5);
            //AddNoteName.Height = AppSettings_User.FontSize * 1.75;
            //AddNoteName.MinWidth = 50;
            //AddNoteName.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            //AddNoteName.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            //AddNoteName.FontSize = AppSettings_User.FontSize;
            //AddNoteName.FontFamily = new FontFamily(AppSettings_Static.Font2);
            //AddNoteName.VerticalContentAlignment = VerticalAlignment.Top;
            //AddNoteName.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            //AddNoteName.HorizontalAlignment = HorizontalAlignment.Stretch;
            //AddNoteName.VerticalAlignment = VerticalAlignment.Top;
            //AddNoteName.Padding = new Thickness(2, 0, 0, 0);
            //AddNoteName.BorderThickness = new Thickness(1);
            //AddNoteName.BorderBrush = null;
            //AddNoteName.SetValue(DockPanel.DockProperty, Dock.Left);
            //AddNoteName.Style = ( Style ) AddNoteName.FindResource("ComboBoxTextBox");

            var NoteContentLabel = new Label();
            NoteContentLabel.Content = $"{TextCatalog.GetName("Content")}:";
            NoteContentLabel.Margin = new Thickness(10, 0, 2, 5);
            NoteContentLabel.Height = AppSettings_User.FontSize * 1.75;
            NoteContentLabel.Background = null;
            NoteContentLabel.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            NoteContentLabel.FontSize = AppSettings_User.FontSize;
            NoteContentLabel.FontFamily = new FontFamily(AppSettings_Static.Font2);
            NoteContentLabel.VerticalContentAlignment = VerticalAlignment.Center;
            NoteContentLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
            NoteContentLabel.HorizontalAlignment = HorizontalAlignment.Left;
            NoteContentLabel.VerticalAlignment = VerticalAlignment.Stretch;
            NoteContentLabel.Padding = new Thickness(2, 0, 0, 0);
            NoteContentLabel.BorderThickness = new Thickness(0);
            NoteContentLabel.BorderBrush = null;
            NoteContentLabel.SetValue(DockPanel.DockProperty, Dock.Left);

            AddNoteContent = new TextBox();
            AddNoteContent.Margin = new Thickness(10, 0, 2, 5);
            AddNoteContent.MinHeight = AppSettings_User.FontSize * 1.75;
            AddNoteContent.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            AddNoteContent.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            AddNoteContent.FontSize = AppSettings_User.FontSize;
            AddNoteContent.FontFamily = new FontFamily(AppSettings_Static.Font2);
            AddNoteContent.VerticalContentAlignment = VerticalAlignment.Top;
            AddNoteContent.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            AddNoteContent.HorizontalAlignment = HorizontalAlignment.Stretch;
            AddNoteContent.VerticalAlignment = VerticalAlignment.Stretch;
            AddNoteContent.Padding = new Thickness(2);
            AddNoteContent.BorderThickness = new Thickness(1);
            AddNoteContent.BorderBrush = null;
            AddNoteContent.TextWrapping = TextWrapping.Wrap;
            AddNoteContent.AcceptsReturn = true;
            if (AppSettings_User.Language == BaSMaST_V3.Language.English)
                AddNoteContent.Language = System.Windows.Markup.XmlLanguage.GetLanguage("en-GB");
            else if (AppSettings_User.Language == BaSMaST_V3.Language.Deutsch)
                AddNoteContent.Language = System.Windows.Markup.XmlLanguage.GetLanguage("de-DE");
            AddNoteContent.SpellCheck.IsEnabled = true;
            AddNoteContent.Style = ( Style ) AddNoteContent.FindResource("ComboBoxTextBox");

            CancelNote = new Button();
            CancelNote = new Button();
            CancelNote.Margin = new Thickness(2, 0, 2, 5);
            CancelNote.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
            CancelNote.HorizontalAlignment = HorizontalAlignment.Right;
            CancelNote.VerticalAlignment = VerticalAlignment.Top;
            CancelNote.Height = AppSettings_User.FontSize * 1.75;
            CancelNote.Width = CancelNote.Height;
            CancelNote.BorderThickness = new Thickness(0);
            CancelNote.Style = ( Style ) CancelNote.FindResource("PopupButtonRed");
            CancelNote.SetValue(DockPanel.DockProperty, Dock.Right);
            MainWindow.Window.AddImage(CancelNote, "Cancel");
            CancelNote.PreviewMouseLeftButtonDown += Events.NodeCancel;

            AcceptNote = new Button();
            AcceptNote = new Button();
            AcceptNote.Margin = new Thickness(2, 0, 2, 5);
            AcceptNote.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
            AcceptNote.HorizontalAlignment = HorizontalAlignment.Right;
            AcceptNote.VerticalAlignment = VerticalAlignment.Top;
            AcceptNote.Height = AppSettings_User.FontSize * 1.75;
            AcceptNote.Width = AcceptNote.Height;
            AcceptNote.BorderThickness = new Thickness(0);
            AcceptNote.Style = ( Style ) AcceptNote.FindResource("PopupButtonGreen");
            AcceptNote.SetValue(DockPanel.DockProperty, Dock.Right);
            MainWindow.Window.AddImage(AcceptNote, "Tick");
            AcceptNote.PreviewMouseLeftButtonDown += Events.AddNode;

            Dock2.Children.Add(CancelNote);
            Dock2.Children.Add(AcceptNote);
            //Dock1.Children.Add(NoteNameLabel);
            //Dock1.Children.Add(AddNoteName);
            Dock1.Children.Add(NoteContentLabel);
            Dock1.Children.Add(AddNoteContent);

            AddNoteStack.Children.Add(NoteLabel);
            AddNoteStack.Children.Add(Dock2);
            AddNoteStack.Children.Add(Dock1);

            NotePanel.Children.Add(NoteHeader);
            NotePanel.Children.Add(AddNoteStack);
            NotePanel.Children.Add(scroll);

            MainWindow.Window.ContentPanel.Children.Add(NoteDock);
        }

        public static void LoadNotes()
        {
            NoteStack.Children.Clear();
            if (NoteOwner != null && NoteOwner.NoteManager != null && NoteOwner.NoteManager.GetItems() != null && NoteOwner.NoteManager.GetItems().Any())
            {
                NoteOwner.NoteManager.GetItems().ForEach(note =>
                {
                    new NoteEntry(note, NoteStack);
                });
            }
        }

        public static bool ValidateNote()
        {
            var _dataIsValid = true;

            foreach (TextBox c in Helper.FindWindowChildren<TextBox>(AddNoteStack))
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
            }

            return _dataIsValid;
        }

        public class NoteEntry
        {
            public DockPanel Container { get; set; }
            //public Label NoteName { get; set; }
            public TextBox NoteText { get; set; }
            public Button RemoveNote { get; set; }

            public Note AffectedNote { get; set; }

            public NoteEntry(Note note, StackPanel parent)
            {
                CreateNode(note, parent);
                Notes.Add(this);
            }

            private void CreateNode(Note note, StackPanel parent)
            {
                AffectedNote = note;

                Container = new DockPanel();
                Container.Margin = new Thickness(0, 0, 0, 10);
                Container.MaxHeight = MainWindow.Window.ContentPanel.ActualHeight * 0.5;
                Container.LastChildFill = true;

                //NoteName = new Label();
                //NoteName.HorizontalAlignment = HorizontalAlignment.Stretch;
                //NoteName.HorizontalContentAlignment = HorizontalAlignment.Center;
                //NoteName.VerticalContentAlignment = VerticalAlignment.Center;
                //NoteName.Margin = new Thickness(10,5,10,5);
                //NoteName.Height = AppSettings_User.FontSize * 1.75;
                //NoteName.Content = note.Name;
                //NoteName.FontFamily = new FontFamily(AppSettings_Static.Font2);
                //NoteName.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                //NoteName.FontSize = AppSettings_User.FontSize;
                //NoteName.BorderThickness = new Thickness(0);
                //NoteName.BorderBrush = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                //NoteName.Background = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                //NoteName.Style = ( Style ) NoteName.FindResource("RoundedLabel");
                //NoteName.SetValue(DockPanel.DockProperty, Dock.Top);

                NoteText = new TextBox();
                NoteText.HorizontalAlignment = HorizontalAlignment.Stretch;
                NoteText.VerticalAlignment = VerticalAlignment.Stretch;
                NoteText.HorizontalContentAlignment = HorizontalAlignment.Left;
                NoteText.VerticalContentAlignment = VerticalAlignment.Top;
                NoteText.MinHeight = AppSettings_User.FontSize * 2;
                NoteText.Margin = new Thickness(10, 5, 10, 5);
                NoteText.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
                NoteText.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
                NoteText.BorderThickness = new Thickness(1);
                NoteText.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                NoteText.Text = note.Content;
                NoteText.FontFamily = new FontFamily(AppSettings_Static.Font2);
                NoteText.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
                NoteText.FontSize = AppSettings_User.FontSize;
                NoteText.Padding = new Thickness(5);
                NoteText.TextWrapping = TextWrapping.Wrap;
                NoteText.AcceptsReturn = true;
                if (AppSettings_User.Language == BaSMaST_V3.Language.English)
                    NoteText.Language = System.Windows.Markup.XmlLanguage.GetLanguage("en-GB");
                else if (AppSettings_User.Language == BaSMaST_V3.Language.Deutsch)
                    NoteText.Language = System.Windows.Markup.XmlLanguage.GetLanguage("de-DE");
                NoteText.SpellCheck.IsEnabled = true;
                NoteText.Style = ( Style ) NoteText.FindResource("ComboBoxTextBox");
                NoteText.TextChanged += Events.UpdateNoteContent;

                RemoveNote = new Button();
                RemoveNote.Style = ( Style ) RemoveNote.FindResource("PopupButtonRed");
                RemoveNote.Margin = new Thickness(5, 5, 10, 5);
                RemoveNote.Background = null;
                RemoveNote.BorderBrush = null;
                RemoveNote.Height = AppSettings_User.FontSize * 2 + 2.5;
                RemoveNote.Width = RemoveNote.Height;
                RemoveNote.HorizontalAlignment = HorizontalAlignment.Right;
                RemoveNote.VerticalAlignment = VerticalAlignment.Top;
                RemoveNote.HorizontalContentAlignment = HorizontalAlignment.Center;
                RemoveNote.VerticalContentAlignment = VerticalAlignment.Center;
                RemoveNote.SetValue(DockPanel.DockProperty, Dock.Right);
                RemoveNote.BorderThickness = new Thickness(0);
                RemoveNote.PreviewMouseLeftButtonDown += Events.RemoveAffectedNote;
                MainWindow.Window.AddImage(RemoveNote, "Remove");

                //Container.Children.Add(NoteName);
                Container.Children.Add(RemoveNote);
                Container.Children.Add(NoteText);
                parent.Children.Add(Container);
            }
        }
    }
}
