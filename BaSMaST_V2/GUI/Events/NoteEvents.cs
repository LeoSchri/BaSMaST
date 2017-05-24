using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Collections.Generic;

namespace BaSMaST_V3
{
    public partial class Events
    {
        public static void SetOwner(object sender, SelectionChangedEventArgs e)
        {
            var itemIndex = ( sender as DataGrid ).SelectedIndex;
            if (itemIndex == -1)
                NoteManager.NoteOwner = null;
            else
            {
                var table = ( ( sender as DataGrid ).ItemsSource as System.Data.DataView ).ToTable();
                var title = TableManager.TableTitles[ TableManager.Tables.IndexOf(( sender as DataGrid )) ];
                if (table.Rows.Count < 1 || itemIndex >= table.Rows.Count)
                    return;
                var item = Helper.GetOwner(AppSettings_User.CurrentProject, table.Rows[ itemIndex ][ $"id{title.Content}" ].ToString());
                NoteManager.NoteOwner = item;
                NoteManager.LoadNotes();
                if (TableManager.Tables.Count > 1)
                {
                    TableManager.Tables.ForEach(t =>
                    {
                        if (t != sender)
                        {
                            if((sender as DataGrid).Name.Contains(TypeName.Aftermath.ToString()) && !t.Name.Contains(TypeName.Lore.ToString()))
                                t.SelectedItem = null;
                        }
                    });
                }
                SetTablesForOwner();
            }
        }

        public static void OpenCloseNotesPage(object sender, MouseButtonEventArgs e)
        {
            if (NoteManager.NoteBorder.Visibility == Visibility.Collapsed)
            {
                NoteManager.NoteBorder.Visibility = Visibility.Visible;
                NoteManager.OpenCloseNoteImg.Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowRight{ AppSettings_User.ColorSchema.Name }")));
            }
            else
            {
                NoteManager.NoteBorder.Visibility = Visibility.Collapsed;
                NoteManager.OpenCloseNoteImg.Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowLeft{ AppSettings_User.ColorSchema.Name }")));
            }
        }

        public static void AddNodeBlock(object sender, MouseButtonEventArgs e)
        {
            //MainWindow.Window.AddNoteName.Text = string.Empty;
            NoteManager.AddNoteContent.Text = string.Empty;
            NoteManager.AddNoteStack.Visibility = Visibility.Visible;
            NoteManager.AddNoteBtn.Visibility = Visibility.Hidden;
        }

        public static void NodeCancel(object sender, MouseButtonEventArgs e)
        {
            NoteManager.AddNoteStack.Visibility = Visibility.Collapsed;
            NoteManager.AddNoteBtn.Visibility = Visibility.Visible;
        }

        public static void AddNode(object sender, MouseButtonEventArgs e)
        {
            if (!NoteManager.ValidateNote())
                return;

            var note = new Note(/*MainWindow.Window.AddNoteName.Text*/"", NoteManager.AddNoteContent.Text, NoteManager.NoteOwner);
            NoteManager.NoteOwner.NoteManager.AddItem(note);
            if (AppSettings_User.CurrentProject.NoteManager == null)
                AppSettings_User.CurrentProject.NoteManager = Manager<Note>.Create();
            AppSettings_User.CurrentProject.NoteManager.AddItem(note);

            new NoteManager.NoteEntry(note, NoteManager.NoteStack);

            //MainWindow.Window.AddNoteName.Text = string.Empty;
            NoteManager.AddNoteContent.Text = string.Empty;
            NoteManager.AddNoteStack.Visibility = Visibility.Collapsed;
            NoteManager.AddNoteBtn.Visibility = Visibility.Visible;
        }

        public static void RemoveAffectedNote(object sender, MouseButtonEventArgs e)
        {
            var noteEntry = NoteManager.Notes.Find(t => t.RemoveNote == sender);
            Popup.ShowWindow(Helper.MakeDeletionText("Note", noteEntry.AffectedNote.Name), TextCatalog.GetName("Confirm deletion"), PopupButtons.YesNo, PopupType.Delete, noteEntry.AffectedNote);
        }

        public static void UpdateNoteContent(object sender, TextChangedEventArgs e)
        {
            var noteEntry = NoteManager.Notes.Find(n => n.NoteText == sender);
            noteEntry.AffectedNote.Content = noteEntry.NoteText.Text;
        }
    }
}
