using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections;

namespace BaSMaST_V3
{
    public partial class Events
    {
        public static void ReloadTable(object sender, MouseButtonEventArgs e)
        {
            TableManager.LoadTables(TableManager.CurrentTables.ToArray());
        }

        public static void SaveTableEntries(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Character)
                {
                    SaveTable(TableManager.Tables.FirstOrDefault(), TypeName.Character, AppSettings_User.CurrentProject.CharacterManager, true);
                    TableManager.LoadTables(TableManager.CurrentTables.ToArray());
                }
                else if (TableManager.CurrentTables.FirstOrDefault() == TypeName.MainCharacter)
                {
                    SaveTable(TableManager.Tables.FirstOrDefault(), TypeName.MainCharacter, AppSettings_User.CurrentProject.CharacterManager, false);
                    TableManager.LoadTables(TableManager.CurrentTables.ToArray());
                }
                else if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Plot)
                {
                    var plotTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.Plot.ToString()));
                    var subplotTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.Subplot.ToString()));
                    var eventTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.Event.ToString()));
                    var plotLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.PlotLink.ToString()));
                    var plotloreLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.LorePlotLink.ToString()));
                    var attachmentFigureTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.AttachmentFigure.ToString()));
                    var characterPresentTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.CharacterPresent.ToString()));
                    var locationLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.LocationLink.ToString()));

                    SaveTable(plotTable, TypeName.Plot, AppSettings_User.CurrentProject.Archive, true);

                    if (SaveTable(subplotTable, TypeName.Subplot, TypeName.Plot,"Parent") == true
                        && SaveTable(eventTable, TypeName.Event, TypeName.Subplot, "Parent") == true
                        && SaveTable(plotLinkTable, TypeName.PlotLink, TypeName.Plot) == true
                        && SaveTable(plotloreLinkTable, TypeName.LorePlotLink, TypeName.Plot) == true
                        && SaveTable(attachmentFigureTable, TypeName.AttachmentFigure, TypeName.Character) == true
                        && SaveTable(characterPresentTable, TypeName.CharacterPresent, TypeName.Character) == true
                        && SaveTable(locationLinkTable, TypeName.LocationLink, TypeName.Character) == true)
                    {
                        TableManager.LoadTables(TableManager.CurrentTables.ToArray());
                    }
                }
                else if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Lore)
                {
                    var loreTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.Lore.ToString()));
                    var aftermathTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.Aftermath.ToString()));
                    var loreLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.LoreLink.ToString()));
                    var plotloreLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.LorePlotLink.ToString()));

                    SaveTable(loreTable, TypeName.Lore, AppSettings_User.CurrentProject.Archive, false);

                    if(SaveTable(aftermathTable, TypeName.Aftermath, TypeName.Lore, "Parent") == true
                        && SaveTable(plotloreLinkTable, TypeName.LorePlotLink, TypeName.Lore) == true
                        && SaveTable(loreLinkTable, TypeName.LoreLink, TypeName.Lore) == true)
                    {
                        TableManager.LoadTables(TableManager.CurrentTables.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(AppSettings_User.CurrentProject, $"SaveTableEntries()", ex);
            }
        }

        public static bool? SaveTable(DataGrid table, TypeName type, TypeName parentType, string columnName = null)
        {
            if (table.ItemsSource == null)
                return true;
            var data = ( table.ItemsSource as System.Data.DataView ).ToTable();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (data.Rows[ i ].HasErrors)
                    return null;
            }

            if(parentType == TypeName.Plot || parentType == TypeName.Subplot)
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    if (AppSettings_User.CurrentProject.Archive == null || AppSettings_User.CurrentProject.Archive.GetItems<Plot>() == null || !AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Any())
                    {
                        if (type == TypeName.Subplot || type == TypeName.Event)
                            Popup.ShowWindow(Helper.MakeParentNeccessaryText(TypeName.Plot.ToString(), type.ToString(), data.Rows[ i ][ "Name" ].ToString()), TextCatalog.GetName("Missing entry"), PopupButtons.OK, PopupType.Message);
                        return false;
                    }
                    Base parent = null;
                    if (type == TypeName.Subplot || type == TypeName.Event)
                        parent = Helper.GetOwner(AppSettings_User.CurrentProject, data.Rows[ i ][ columnName ].ToString());
                    else parent = NoteManager.NoteOwner;
                    if (parent == null)
                    {
                        if (type == TypeName.Subplot || type == TypeName.Event) 
                            Popup.ShowWindow(Helper.MakeParentNeccessaryText(parentType.ToString(), type.ToString(), data.Rows[ i ][ "Name" ].ToString()), TextCatalog.GetName("Missing entry"), PopupButtons.OK, PopupType.Message);
                        return false;
                    }
                    if (parentType == TypeName.Plot)
                    {
                        if(type == TypeName.Subplot)
                            SaveTableEntry(( parent as Plot ), TypeName.Subplot, data.Rows[ i ]);
                        else if (type == TypeName.PlotLink)
                            SaveTableEntry(( parent as Plot ), TypeName.PlotLink, data.Rows[ i ]);
                        else if (type == TypeName.LorePlotLink)
                            SaveTableEntry(( parent as Plot ), TypeName.LorePlotLink, data.Rows[ i ]);
                    }
                    else if (parentType == TypeName.Subplot)
                        SaveTableEntry(( parent as Subplot ), TypeName.Event, data.Rows[ i ]);
                    else if (parentType == TypeName.Event)
                    {
                        if (type == TypeName.AttachmentFigure)
                            SaveTableEntry(( parent as Event ), TypeName.AttachmentFigure, data.Rows[ i ]);
                        else if (type == TypeName.CharacterPresent)
                            SaveTableEntry(( parent as Event ), TypeName.CharacterPresent, data.Rows[ i ]);
                        else if (type == TypeName.LocationLink)
                            SaveTableEntry(( parent as Event ), TypeName.LocationLink, data.Rows[ i ]);
                    }
                }
            }
            else if (parentType == TypeName.Lore)
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    if (AppSettings_User.CurrentProject.Archive == null || AppSettings_User.CurrentProject.Archive.GetItems<Lore>() == null || !AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Any())
                    {
                        if (type == TypeName.Aftermath)
                            Popup.ShowWindow(Helper.MakeParentNeccessaryText(TypeName.Lore.ToString(), type.ToString(), data.Rows[ i ][ "Name" ].ToString()), TextCatalog.GetName("Missing entry"), PopupButtons.OK, PopupType.Message);
                        return false;
                    }
                    Base parent = null;
                    var itemIndex = TableManager.Tables[ 3 ].SelectedIndex;
                    if (itemIndex == -1)
                        parent = null;
                    else
                    {
                        var datatable = ( TableManager.Tables[ 3 ].ItemsSource as System.Data.DataView ).ToTable();
                        var title = TableManager.TableTitles[ TableManager.Tables.IndexOf(TableManager.Tables[ 3 ]) ];
                        if (datatable.Rows.Count < 1 || itemIndex >= datatable.Rows.Count)
                            parent = null;
                        var item = Helper.GetOwner(AppSettings_User.CurrentProject, datatable.Rows[ itemIndex ][ $"id{parentType}" ].ToString());
                        parent = item;
                    }
                    if (parent == null)
                    {
                        if (type == TypeName.Aftermath)
                            Popup.ShowWindow(Helper.MakeParentNeccessaryText(parentType.ToString(), type.ToString(), data.Rows[ i ][ "Name" ].ToString()), TextCatalog.GetName("Missing entry"), PopupButtons.OK, PopupType.Message);
                        return false;
                    }
                    SaveTableEntry(( parent as Lore ), type, data.Rows[ i ]);
                }
            }

            for (int i = 0; i < data.Rows.Count; i++)
            {
                Base parent = null;
                if (type == TypeName.Subplot || type == TypeName.Event)
                    parent = Helper.GetOwner(AppSettings_User.CurrentProject, data.Rows[ i ][ columnName ].ToString());
                else if (type == TypeName.Aftermath)
                {
                    var itemIndex = TableManager.Tables[ 3 ].SelectedIndex;
                    if (itemIndex == -1)
                        parent = null;
                    else
                    {
                        var datatable = ( TableManager.Tables[ 3 ].ItemsSource as System.Data.DataView ).ToTable();
                        var title = TableManager.TableTitles[ TableManager.Tables.IndexOf(TableManager.Tables[ 3 ]) ];
                        if (datatable.Rows.Count < 1 || itemIndex >= datatable.Rows.Count)
                            parent = null;
                        var item = Helper.GetOwner(AppSettings_User.CurrentProject, datatable.Rows[ itemIndex ][ $"id{parentType}" ].ToString());
                        parent = item;
                    }
                }
                else parent = NoteManager.NoteOwner;
                if (parent != null)
                    if (parentType == TypeName.Plot)
                    {
                        if (type == TypeName.Subplot)
                            RemoveDeletedEntries(data, ( parent as Plot ), TypeName.Subplot);
                        else if (type == TypeName.PlotLink)
                            RemoveDeletedEntries(data, ( parent as Plot ), TypeName.PlotLink);
                        else if (type == TypeName.LorePlotLink)
                            RemoveDeletedEntries(data, ( parent as Plot ), TypeName.LorePlotLink);
                    }
                    else if (parentType == TypeName.Subplot)
                        RemoveDeletedEntries(data, ( parent as Subplot ), TypeName.Event);
                    else if (parentType == TypeName.Lore)
                    {
                        if (type == TypeName.Aftermath)
                            RemoveDeletedEntries(data, ( parent as Lore ), TypeName.Aftermath);
                        else if (type == TypeName.LorePlotLink)
                            RemoveDeletedEntries(data, ( parent as Lore ), TypeName.LorePlotLink);
                        else if (type == TypeName.LoreLink)
                            RemoveDeletedEntries(data, ( parent as Lore ), TypeName.LoreLink);
                    }
            }
            return true;
        }

        public static void RemoveDeletedEntries<T>(System.Data.DataTable data, T parentItem, TypeName type) where T : Base
        {
            if (typeof(T) == typeof(Plot))
            {
                if(type == TypeName.Subplot)
                {
                    var manager = ( parentItem as Plot ).SubplotManager;
                    if (manager == null)
                        return;
                    if (manager.GetItems() != null && manager.GetItems().Any())
                    {
                        if (manager.GetItems().Count > data.Rows.Count)
                        {
                            var items = manager.GetItems();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var exists = false;
                                for (int j = 0; j < data.Rows.Count; j++)
                                {
                                    if (items[ i ].ID == data.Rows[ j ][ $"id{type}" ].ToString())
                                        exists = true;
                                }
                                if (!exists)
                                    manager.RemoveItem(items[ i ], type);
                            }
                        }
                    }
                }
                else if (type == TypeName.PlotLink)
                {
                    var manager = AppSettings_User.CurrentProject.PlotLinkManager;
                    if (manager == null)
                        return;
                    if (manager.GetLinks() != null && manager.GetLinks().Any())
                    {
                        if (manager.GetLinks().Count > data.Rows.Count)
                        {
                            var items = manager.GetLinks();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var exists = false;
                                for (int j = 0; j < data.Rows.Count; j++)
                                {
                                    if (items[ i ].LinkObject2.ID == data.Rows[ j ][ "OtherPlot" ].ToString())
                                        exists = true;
                                }
                                if (!exists)
                                    manager.RemoveLink(items[ i ].LinkObject1, items[ i ].LinkObject2, type,TypeName.PlotLink);
                            }
                        }
                    }
                }
                else if (type == TypeName.LorePlotLink)
                {
                    var manager = AppSettings_User.CurrentProject.PlotLoreLinkManager;
                    if (manager == null)
                        return;
                    if (manager.GetLinks() != null && manager.GetLinks().Any())
                    {
                        if (manager.GetLinks().Count > data.Rows.Count)
                        {
                            var items = manager.GetLinks();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var exists = false;
                                for (int j = 0; j < data.Rows.Count; j++)
                                {
                                    if (items[ i ].LinkObject2.ID == data.Rows[ j ][ "Lore" ].ToString())
                                        exists = true;
                                }
                                if (!exists)
                                    manager.RemoveLink(items[ i ].LinkObject1, items[ i ].LinkObject2, type, TypeName.LorePlotLink);
                            }
                        }
                    }
                }
            }
            else if (typeof(T) == typeof(Subplot))
            {
                var manager = ( parentItem as Subplot ).EventManager;
                if (manager == null)
                    return;
                if (manager.GetItems() != null && manager.GetItems().Any())
                {
                    if (manager.GetItems().Count > data.Rows.Count)
                    {
                        var items = manager.GetItems();
                        for (int i = 0; i < items.Count; i++)
                        {
                            var exists = false;
                            for (int j = 0; j < data.Rows.Count; j++)
                            {
                                if (items[ i ].ID == data.Rows[ j ][ $"id{type}" ].ToString())
                                    exists = true;
                            }
                            if (!exists)
                                manager.RemoveItem(items[ i ], type);
                        }
                    }
                }
            }
            if (typeof(T) == typeof(Lore))
            {
                if (type == TypeName.Aftermath)
                {
                    var manager = ( parentItem as Lore ).AftermathManager;
                    if (manager == null)
                        return;
                    if (manager.GetItems() != null && manager.GetItems().Any())
                    {
                        if (manager.GetItems().Count > data.Rows.Count)
                        {
                            var items = manager.GetItems();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var exists = false;
                                for (int j = 0; j < data.Rows.Count; j++)
                                {
                                    if (items[ i ].ID == data.Rows[ j ][ $"id{type}" ].ToString())
                                        exists = true;
                                }
                                if (!exists)
                                    manager.RemoveItem(items[ i ], type);
                            }
                        }
                    }
                }
                else if (type == TypeName.LoreLink)
                {
                    var manager = AppSettings_User.CurrentProject.LoreLinkManager;
                    if (manager == null)
                        return;
                    if (manager.GetLinks() != null && manager.GetLinks().Any())
                    {
                        if (manager.GetLinks().Count > data.Rows.Count)
                        {
                            var items = manager.GetLinks();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var exists = false;
                                for (int j = 0; j < data.Rows.Count; j++)
                                {
                                    if (items[ i ].LinkObject2.ID == data.Rows[ j ][ "OtherLore" ].ToString())
                                        exists = true;
                                }
                                if (!exists)
                                    manager.RemoveLink(items[ i ].LinkObject1, items[ i ].LinkObject2, type, TypeName.PlotLink);
                            }
                        }
                    }
                }
                else if (type == TypeName.LorePlotLink)
                {
                    var manager = AppSettings_User.CurrentProject.PlotLoreLinkManager;
                    if (manager == null)
                        return;
                    if (manager.GetLinks() != null && manager.GetLinks().Any())
                    {
                        if (manager.GetLinks().Count > data.Rows.Count)
                        {
                            var items = manager.GetLinks();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var exists = false;
                                for (int j = 0; j < data.Rows.Count; j++)
                                {
                                    if (items[ i ].LinkObject2.ID == data.Rows[ j ][ "Lore" ].ToString())
                                        exists = true;
                                }
                                if (!exists)
                                    manager.RemoveLink(items[ i ].LinkObject1, items[ i ].LinkObject2, type, TypeName.LorePlotLink);
                            }
                        }
                    }
                }
            }
        }

        public static void SaveTableEntry<T>(T parentItem, TypeName type, System.Data.DataRow row) where T : Base
        {
            if (typeof(T) == typeof(Plot))
            {
                if(type == TypeName.Subplot)
                {
                    var manager = ( parentItem as Plot ).SubplotManager;
                    if (manager == null)
                        manager = Manager<Subplot>.Create();
                    if (manager.GetItems() == null || !manager.GetItems().Any())
                    {
                        manager.AddItem(DBDataManager.AddSubplot(( parentItem as Plot ), row));
                    }
                    else
                    {
                        var match = Helper.GetOwner(AppSettings_User.CurrentProject, row[ $"id{type}" ].ToString());
                        if (match == null && !string.IsNullOrEmpty(row[ "Name" ].ToString()))
                        {
                            manager.AddItem(DBDataManager.AddSubplot(( parentItem as Plot ), row));
                        }
                        else
                        {
                            UpdateSubplot(match as Subplot, row);
                        }
                    }
                }
                else if (type == TypeName.PlotLink)
                {
                    var manager = AppSettings_User.CurrentProject.PlotLinkManager;
                    if (manager == null)
                        manager = LinkManager<Plot,Plot>.Create();
                    if (manager.GetLinks() == null || !manager.GetLinks().Any())
                    {
                        if(AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Any())
                        {
                            var otherPlot = AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Find(plot => plot.ID == row[ "OtherPlot" ].ToString());
                            if(otherPlot != null)
                                manager.AddLink(parentItem as Plot, otherPlot, TypeName.PlotLink);
                        }
                    }
                    else
                    {
                        if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Any())
                        {
                            var otherPlot = AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Find(plot => plot.ID == row[ "OtherPlot" ].ToString());
                            if (otherPlot != null)
                            {
                                var plotLink = manager.GetLinks().Find(pL => pL.LinkObject2 == otherPlot);
                                if(plotLink == null)
                                    manager.AddLink(parentItem as Plot, otherPlot, TypeName.PlotLink);
                            }
                        }
                    }
                }
                else if (type == TypeName.LorePlotLink)
                {
                    var manager = AppSettings_User.CurrentProject.PlotLoreLinkManager;
                    if (manager == null)
                        manager = LinkManager<Plot, Lore>.Create();
                    if (manager.GetLinks() == null || !manager.GetLinks().Any())
                    {
                        if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Any())
                        {
                            var lore = AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Find(l => l.ID == row[ "Lore" ].ToString());
                            if (lore != null)
                                manager.AddLink(parentItem as Plot, lore, TypeName.LorePlotLink);
                        }
                    }
                    else
                    {
                        if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Any())
                        {
                            var lore = AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Find(plot => plot.ID == row[ "Lore" ].ToString());
                            if (lore != null)
                            {
                                var plotLink = manager.GetLinks().Find(pL => pL.LinkObject2 == lore);
                                if (plotLink == null)
                                    manager.AddLink(parentItem as Plot, lore, TypeName.LorePlotLink);
                            }
                        }
                    }
                }
            }
            else if (typeof(T) == typeof(Subplot))
            {
                var manager = ( parentItem as Subplot ).EventManager;
                if (manager == null)
                    manager = Manager<Event>.Create();
                if (manager.GetItems() == null || !manager.GetItems().Any())
                {
                    manager.AddItem(DBDataManager.AddEvent(( parentItem as Subplot ), row));
                }
                else
                {
                    var match = Helper.GetOwner(AppSettings_User.CurrentProject, row[ $"id{type}" ].ToString());
                    if (match == null && !string.IsNullOrEmpty(row[ "Name" ].ToString()))
                    {
                        manager.AddItem(DBDataManager.AddEvent(( parentItem as Subplot ), row));
                    }
                    else
                    {
                        UpdateEvent(match as Event, row);
                    }
                }
            }
            else if (typeof(T) == typeof(Event))
            {
                if (type == TypeName.AttachmentFigure)
                {
                    var manager = AppSettings_User.CurrentProject.EventAttachmentCharacterLinkManager;
                    if (manager == null)
                        manager = LinkManager<Event, Character>.Create();
                    if (manager.GetLinks() == null || !manager.GetLinks().Any())
                    {
                        if (Helper.GetAllEvents(AppSettings_User.CurrentProject) == null)
                        {
                            var character = AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>().Find(c => c.ID == row[ "Character" ].ToString());
                            if (character != null)
                                manager.AddLink(parentItem as Event, character, TypeName.AttachmentFigure);
                        }
                    }
                    else
                    {
                        if (Helper.GetAllEvents(AppSettings_User.CurrentProject) != null)
                        {
                            var character = AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>().Find(c => c.ID == row[ "Character" ].ToString());
                            if (character != null)
                            {
                                var attachmentFigure = manager.GetLinks().Find(aF => aF.LinkObject2 == character);
                                if (attachmentFigure == null)
                                    manager.AddLink(parentItem as Event, character, TypeName.AttachmentFigure);
                            }
                        }
                    }
                }
                else if (type == TypeName.CharacterPresent)
                {
                    var manager = AppSettings_User.CurrentProject.EventCharacterPresentLinkManager;
                    if (manager == null)
                        manager = LinkManager<Event, Character>.Create();
                    if (manager.GetLinks() == null || !manager.GetLinks().Any())
                    {
                        if (Helper.GetAllEvents(AppSettings_User.CurrentProject) == null)
                        {
                            var character = AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>().Find(c => c.ID == row[ "Character" ].ToString());
                            if (character != null)
                                manager.AddLink(parentItem as Event, character, TypeName.CharacterPresent);
                        }
                    }
                    else
                    {
                        if (Helper.GetAllEvents(AppSettings_User.CurrentProject) != null)
                        {
                            var character = AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>().Find(c => c.ID == row[ "Character" ].ToString());
                            if (character != null)
                            {
                                var characterPresent = manager.GetLinks().Find(aF => aF.LinkObject2 == character);
                                if (characterPresent == null)
                                    manager.AddLink(parentItem as Event, character, TypeName.CharacterPresent);
                            }
                        }
                    }
                }
                else if (type == TypeName.LocationLink)
                {
                    var manager = AppSettings_User.CurrentProject.EventLocationLinkManager;
                    if (manager == null)
                        manager = LinkManager<Event, Location>.Create();
                    if (manager.GetLinks() == null || !manager.GetLinks().Any())
                    {
                        if (Helper.GetAllEvents(AppSettings_User.CurrentProject) == null)
                        {
                            var location = AppSettings_User.CurrentProject.LocationManager.GetItems().Find(l => l.ID == row[ "Location" ].ToString());
                            if (location != null)
                                manager.AddLink(parentItem as Event, location, TypeName.LocationLink);
                        }
                    }
                    else
                    {
                        if (Helper.GetAllEvents(AppSettings_User.CurrentProject) != null)
                        {
                            var location = AppSettings_User.CurrentProject.LocationManager.GetItems().Find(l => l.ID == row[ "Location" ].ToString());
                            if (location != null)
                            {
                                var locationLink = manager.GetLinks().Find(lL => lL.LinkObject2 == location);
                                if (locationLink == null)
                                    manager.AddLink(parentItem as Event, location, TypeName.LocationLink);
                            }
                        }
                    }
                }
            }
            else if (typeof(T) == typeof(Lore))
            {
                if (type == TypeName.Aftermath)
                {
                    var manager = ( parentItem as Lore ).AftermathManager;
                    if (manager == null)
                        manager = Manager<Aftermath>.Create();
                    if (manager.GetItems() == null || !manager.GetItems().Any())
                    {
                        manager.AddItem(DBDataManager.AddAftermath(( parentItem as Lore ), row));
                    }
                    else
                    {
                        var match = Helper.GetOwner(AppSettings_User.CurrentProject, row[ $"id{type}" ].ToString());
                        if (match == null && !string.IsNullOrEmpty(row[ "Name" ].ToString()))
                        {
                            manager.AddItem(DBDataManager.AddAftermath(( parentItem as Lore ), row));
                        }
                        else
                        {
                            UpdateAftermath(match as Aftermath, row);
                        }
                    }
                }
                else if (type == TypeName.LoreLink)
                {
                    var manager = AppSettings_User.CurrentProject.LoreLinkManager;
                    if (manager == null)
                        manager = LinkManager<Lore, Lore>.Create();
                    if (manager.GetLinks() == null || !manager.GetLinks().Any())
                    {
                        if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Any())
                        {
                            var otherLore = AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Find(lore => lore.ID == row[ "OtherLore" ].ToString());
                            if (otherLore != null)
                                manager.AddLink(parentItem as Lore, otherLore, TypeName.LoreLink);
                        }
                    }
                    else
                    {
                        if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Any())
                        {
                            var otherLore = AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Find(lore => lore.ID == row[ "OtherLore" ].ToString());
                            if (otherLore != null)
                            {
                                var loreLink = manager.GetLinks().Find(lL => lL.LinkObject2 == otherLore);
                                if (loreLink == null)
                                {
                                    loreLink = manager.GetLinks().Find(lL => lL.LinkObject1 == otherLore);
                                    if(loreLink == null)
                                        manager.AddLink(parentItem as Lore, otherLore, TypeName.LoreLink);
                                }
                            }
                        }
                    }
                }
                else if (type == TypeName.LorePlotLink)
                {
                    var manager = AppSettings_User.CurrentProject.PlotLoreLinkManager;
                    if (manager == null)
                        manager = LinkManager<Plot, Lore>.Create();
                    if (manager.GetLinks() == null || !manager.GetLinks().Any())
                    {
                        if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Any())
                        {
                            var plot = AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Find(p => p.ID == row[ "Plot" ].ToString());
                            if (plot != null)
                                manager.AddLink(plot, parentItem as Lore, TypeName.LorePlotLink);
                        }
                    }
                    else
                    {
                        if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Any())
                        {
                            var plot = AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Find(p => p.ID == row[ "Plot" ].ToString());
                            if (plot != null)
                            {
                                var plotLink = manager.GetLinks().Find(pL => pL.LinkObject1 == plot);
                                if (plotLink == null)
                                    manager.AddLink(plot, parentItem as Lore, TypeName.LorePlotLink);
                            }
                        }
                    }
                }
            }
        }

        public static void SaveTable<T, U>(DataGrid table, TypeName type, Manager<T, U> manager, bool UseFirstList) where T : Base where U : Base
        {
            try
            {
                if (table.ItemsSource == null)
                    return;
                var data = ( table.ItemsSource as System.Data.DataView ).ToTable();
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    if (data.Rows[ i ].HasErrors)
                        return;
                }
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    if (UseFirstList)
                    {
                        if (manager.GetItems<T>() == null || !manager.GetItems<T>().Any())
                        {
                            if (typeof(T) == typeof(Character))
                            {
                                manager.AddItem(DBDataManager.AddCharacter(data.Rows[ i ]));
                            }
                            else if (typeof(T) == typeof(MainCharacter))
                            {
                                manager.AddItem(DBDataManager.AddMainCharacter(data.Rows[ i ], AppSettings_User.CurrentProject));
                            }
                            else if (typeof(T) == typeof(Plot))
                            {
                                manager.AddItem(DBDataManager.AddPlot(data.Rows[ i ]));
                            }
                            else if (typeof(T) == typeof(Lore))
                            {
                                manager.AddItem(DBDataManager.AddLore(data.Rows[ i ]));
                            }
                        }
                        else
                        {
                            var match = manager.GetItems<T>().Find(c => c.ID == data.Rows[ i ][ $"id{TableManager.CurrentTables.FirstOrDefault()}" ].ToString());
                            if (match == null && !string.IsNullOrEmpty(data.Rows[ i ][ "Name" ].ToString()))
                            {
                                if (typeof(T) == typeof(Character))
                                {
                                    manager.AddItem(DBDataManager.AddCharacter(data.Rows[ i ]));
                                }
                                else if (typeof(T) == typeof(MainCharacter))
                                {
                                    manager.AddItem(DBDataManager.AddMainCharacter(data.Rows[ i ], AppSettings_User.CurrentProject));
                                }
                                else if (typeof(T) == typeof(Plot))
                                {
                                    manager.AddItem(DBDataManager.AddPlot(data.Rows[ i ]));
                                }
                                else if (typeof(T) == typeof(Lore))
                                {
                                    manager.AddItem(DBDataManager.AddLore(data.Rows[ i ]));
                                }
                            }
                            else
                            {
                                if (typeof(T) == typeof(Character))
                                {
                                    UpdateCharacter(match as Character, data.Rows[ i ]);
                                }
                                else if (typeof(T) == typeof(MainCharacter))
                                {
                                    UpdateMainCharacter(match as MainCharacter, data.Rows[ i ]);
                                }
                                else if (typeof(T) == typeof(Plot))
                                {
                                    UpdatePlot(match as Plot, data.Rows[ i ]);
                                }
                                else if (typeof(T) == typeof(Lore))
                                {
                                    UpdateLore(match as Lore, data.Rows[ i ]);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (manager.GetItems<U>() == null || !manager.GetItems<U>().Any())
                        {
                            if (typeof(U) == typeof(Character))
                            {
                                manager.AddItem(DBDataManager.AddCharacter(data.Rows[ i ]));
                            }
                            else if (typeof(U) == typeof(MainCharacter))
                            {
                                manager.AddItem(DBDataManager.AddMainCharacter(data.Rows[ i ], AppSettings_User.CurrentProject));
                            }
                            else if (typeof(U) == typeof(Plot))
                            {
                                manager.AddItem(DBDataManager.AddPlot(data.Rows[ i ]));
                            }
                            else if (typeof(U) == typeof(Lore))
                            {
                                manager.AddItem(DBDataManager.AddLore(data.Rows[ i ]));
                            }
                        }
                        else
                        {
                            var match = manager.GetItems<U>().Find(c => c.ID == data.Rows[ i ][ $"id{TableManager.CurrentTables.FirstOrDefault()}" ].ToString());
                            if (match == null && !string.IsNullOrEmpty(data.Rows[ i ][ "Name" ].ToString()))
                            {
                                if (typeof(U) == typeof(Character))
                                {
                                    manager.AddItem(DBDataManager.AddCharacter(data.Rows[ i ]));
                                }
                                else if (typeof(U) == typeof(MainCharacter))
                                {
                                    manager.AddItem(DBDataManager.AddMainCharacter(data.Rows[ i ], AppSettings_User.CurrentProject));
                                }
                                else if (typeof(U) == typeof(Plot))
                                {
                                    manager.AddItem(DBDataManager.AddPlot(data.Rows[ i ]));
                                }
                                else if (typeof(U) == typeof(Lore))
                                {
                                    manager.AddItem(DBDataManager.AddLore(data.Rows[ i ]));
                                }
                            }
                            else
                            {
                                if (typeof(U) == typeof(Character))
                                {
                                    UpdateCharacter(match as Character, data.Rows[ i ]);
                                }
                                else if (typeof(U) == typeof(MainCharacter))
                                {
                                    UpdateMainCharacter(match as MainCharacter, data.Rows[ i ]);
                                }
                                else if (typeof(U) == typeof(Plot))
                                {
                                    UpdatePlot(match as Plot, data.Rows[ i ]);
                                }
                                else if (typeof(U) == typeof(Lore))
                                {
                                    UpdateLore(match as Lore, data.Rows[ i ]);
                                }
                            }
                        }
                    }
                }

                if (UseFirstList)
                {
                    if (manager.GetItems<T>() != null && manager.GetItems<T>().Any())
                    {
                        if (manager.GetItems<T>().Count > data.Rows.Count)
                        {
                            var items = manager.GetItems<T>();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var exists = false;
                                for (int j = 0; j < data.Rows.Count; j++)
                                {
                                    if (items[ i ].ID == data.Rows[ j ][ $"id{TableManager.CurrentTables.FirstOrDefault()}" ].ToString())
                                        exists = true;
                                }
                                if (!exists)
                                    manager.RemoveItem(items[ i ], type);
                            }
                        }
                    }
                }
                else
                {
                    if (manager.GetItems<U>() != null && manager.GetItems<U>().Any())
                    {
                        if (manager.GetItems<U>().Count > data.Rows.Count)
                        {
                            var items = manager.GetItems<U>();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var exists = false;
                                for (int j = 0; j < data.Rows.Count; j++)
                                {
                                    if (items[ i ].ID == data.Rows[ j ][ $"id{TableManager.CurrentTables.FirstOrDefault()}" ].ToString())
                                        exists = true;
                                }
                                if (!exists)
                                    manager.RemoveItem(items[ i ], type);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(AppSettings_User.CurrentProject, $"SaveTable({table},{type},{manager},{UseFirstList})", ex);
            }
        }

        public static void UpdateCharacter(Character character, System.Data.DataRow row)
        {
            var name = row[ "Name" ].ToString();
            if (name != character.Name)
            {
                character.Name = name;
            }

            var isMain = row[ "IsMainCharacter" ].ToString() == "True" ? true : false;
            if (isMain != character.IsMainCharacter)
            {
                character.IsMainCharacter = isMain;
            }

            if (character.Props != null && character.Props.Any())
            {
                character.Props.ForEach(p =>
                {
                    var pValue = row[ p.Attribute.Name ].ToString();
                    if (pValue != p.Value)
                    {
                        p.ChangeValue(character, TypeName.Character, row[ p.Attribute.Name ].ToString());
                    }
                });
            }
        }

        public static void UpdateMainCharacter(MainCharacter character, System.Data.DataRow row)
        {
            var name = row[ "Name" ].ToString();
            if (name != character.Name)
            {
                character.Name = name;
            }

            if (character.Props != null && character.Props.Any())
            {
                character.Props.ForEach(p =>
                {
                    var pValue = row[ p.Attribute.Name ].ToString();
                    if (pValue != p.Value)
                    {
                        p.ChangeValue(character, TypeName.MainCharacter, row[ p.Attribute.Name ].ToString());
                    }
                });
            }
        }

        public static void UpdatePlot(Plot plot, System.Data.DataRow row)
        {
            var name = row[ "Name" ].ToString();
            if (name != plot.Name)
            {
                plot.Name = name;
            }

            var desc = row[ "Description" ].ToString();
            if (desc != plot.Description)
            {
                plot.Description = desc;
            }

            var isSuper = ( bool ) row[ "IsSuperPlot" ] == true ? true : false;
            if (isSuper != plot.IsSuperPlot)
            {
                plot.IsSuperPlot = isSuper;
            }
        }

        public static void UpdateSubplot(Subplot subplot, System.Data.DataRow row)
        {
            var name = row[ "Name" ].ToString();
            if (name != subplot.Name)
            {
                subplot.Name = name;
            }

            var desc = row[ "Description" ].ToString();
            if (desc != subplot.Description)
            {
                subplot.Description = desc;
            }

            var parentID = row[ "Parent" ].ToString();
            var parent = AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Find(plot => plot.ID == parentID);
            if(parent != subplot.Parent)
            {
                subplot.ChangeParent(parent);
            }
        }

        public static void UpdateEvent(Event ev, System.Data.DataRow row)
        {
            var name = row[ "Name" ].ToString();
            if (name != ev.Name)
            {
                ev.Name = name;
            }

            var desc = row[ "Description" ].ToString();
            if (desc != ev.Description)
            {
                ev.Description = desc;
            }

            DateTime? beginDate = null;
            DateTime? endDate = null;
            DateTime bDate;
            DateTime eDate;

            if (DateTime.TryParse(row[ "BeginDate" ].ToString(), out bDate)|| !string.IsNullOrEmpty(row[ "BeginDate" ].ToString()))
                beginDate = bDate;
            if(DateTime.TryParse(row[ "EndDate" ].ToString(), out eDate) || !string.IsNullOrEmpty(row[ "EndDate" ].ToString()))
                endDate = eDate;

            var beginDaytime = Helper.GetType<DayTime>(TextCatalog.GetSpecifier(row[ "BeginDaytime" ].ToString()));
            var endDaytime = Helper.GetType<DayTime>(TextCatalog.GetSpecifier(row[ "EndDaytime" ].ToString()));

            var BeginPointInTime = new PointInTime(beginDate, beginDaytime);
            var EndPointInTime = new PointInTime(endDate, endDaytime);
            if (ev.Begin != BeginPointInTime)
            {
                ev.Begin = BeginPointInTime;
            }
            if (ev.End != EndPointInTime)
            {
                ev.End = EndPointInTime;
            }

            var parentID = row[ "Parent" ].ToString();
            var parent = Helper.FindSubplot(AppSettings_User.CurrentProject, parentID);
            if (parent != ev.Parent)
            {
                ev.ChangeParent(parent);
            }
        }

        public static void UpdateLore(Lore lore, System.Data.DataRow row)
        {
            var name = row[ "Name" ].ToString();
            if (name != lore.Name)
            {
                lore.Name = name;
            }

            var desc = row[ "Description" ].ToString();
            if (desc != lore.Description)
            {
                lore.Description = desc;
            }

            var importance = Helper.GetType<Importance>(TextCatalog.GetSpecifier(row[ "Importance" ].ToString()));
            if (importance != lore.Importance)
                lore.Importance = importance;

            DateTime? beginDate = null;
            DateTime? endDate = null;
            DateTime bDate;
            DateTime eDate;

            if (DateTime.TryParse(row[ "BeginDate" ].ToString(), out bDate))
                beginDate = bDate;
            if (DateTime.TryParse(row[ "EndDate" ].ToString(), out eDate))
                endDate = eDate;

            if (beginDate != lore.Begin)
                lore.Begin = beginDate;

            if (endDate != lore.End)
                lore.End = endDate;
        }

        public static void UpdateAftermath(Aftermath aftermath, System.Data.DataRow row)
        {
            var name = row[ "Name" ].ToString();
            if (name != aftermath.Name)
            {
                aftermath.Name = name;
            }

            var desc = row[ "Description" ].ToString();
            if (desc != aftermath.Description)
            {
                aftermath.Description = desc;
            }

            DateTime? endDate = null;
            DateTime eDate;

            if (DateTime.TryParse(row[ "EndDate" ].ToString(), out eDate))
                endDate = eDate;

            if (endDate != aftermath.End)
                aftermath.End = endDate;
        }

        public static void AlterColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (Regex.IsMatch(e.Column.Header.ToString(), "id[A-Z]{1}[A-Za-z]*") || e.Column.Header.ToString() == $"Character" || e.Column.Header.ToString() == $"Source" || e.Column.Header.ToString() == $"Moods")
                e.Column.Visibility = Visibility.Collapsed;

            if (e.Column.Header.ToString() == $"Parent")
            {
                if (( sender as DataGrid ) != TableManager.Tables.Find(t => t.Name.Contains(TypeName.Aftermath.ToString())))
                {
                    e.Cancel = true;
                    var index = TableManager.Tables.IndexOf(sender as DataGrid);
                    var title = TableManager.TableTitles[ index ];
                    if (title.Content.ToString() == TypeName.Subplot.ToString())
                        AddColumnFromList<Plot>(sender, e, "Parent");
                    else if (title.Content.ToString() == TypeName.Event.ToString())
                        AddColumnFromList<Subplot>(sender, e, "Parent");
                }
                else e.Column.Visibility = Visibility.Collapsed;
            }

            CheckColumnForEnum(sender, e);

            if (e.PropertyType == typeof(DateTime))
            {
                Binding binding = new Binding($"{e.Column.Header}");
                binding.Converter = new Helper.DateConverter();

                var dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
                {
                    dataGridTextColumn.Binding = binding;
                }
            }

            e.Column.Header = TextCatalog.GetName(e.Column.Header.ToString());
            e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
        }

        private static void CheckColumnForEnum(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == $"Importance")
            {
                AddColumnFromEnum<Importance>(sender, e);
            }
            else if (e.Column.Header.ToString() == $"Mood")
            {
                AddColumnFromEnum<Mood>(sender, e);
            }
            else if (e.Column.Header.ToString() == $"Climate")
            {
                AddColumnFromEnum<Climate>(sender, e);
            }
            else if (e.Column.Header.ToString() == $"WeatherType")
            {
                AddColumnFromEnum<WeatherType>(sender, e);
            }
            else if (e.Column.Header.ToString() == $"Orientation")
            {
                AddColumnFromEnum<Orientation>(sender, e);
            }
            else if (e.Column.Header.ToString() == $"LocationType")
            {
                AddColumnFromEnum<LocationType>(sender, e);
            }
            else if (e.Column.Header.ToString() == $"Relation")
            {
                AddColumnFromEnum<Relation>(sender, e);
            }
            else if (e.Column.Header.ToString() == $"Closeness")
            {
                AddColumnFromEnum<Closeness>(sender, e);
            }
            else if (e.Column.Header.ToString() == $"TraitType")
            {
                AddColumnFromEnum<TraitType>(sender, e);
            }
            else if (e.Column.Header.ToString().Contains("Daytime"))
            {
                AddColumnFromEnum<DayTime>(sender, e);
            }
            else if (e.Column.Header.ToString().Contains("Mood"))
            {
                AddColumnFromEnum<Mood>(sender, e);
            }
        }

        private static void AddColumnFromEnum<T>(object sender, DataGridAutoGeneratingColumnEventArgs e) where T : struct, IConvertible
        {
            e.Cancel = true;
            var table = sender as DataGrid;
            Binding binding = new Binding($"{e.Column.Header}");
            binding.Converter = new  Helper.EnumConverter();
            var col =new DataGridComboBoxColumn();
            col.SelectedItemBinding = binding;
            col.Header = TextCatalog.GetName(e.Column.Header.ToString());
            col.MinWidth = 100;
            col.ItemsSource = Helper.GetTypesAsListWithoutNone<T>();
            table.Columns.Add(col);
        }

        private static void AddColumnFromList<T>(object sender, DataGridAutoGeneratingColumnEventArgs e, string bindingName, bool excludeItem = false) where T : Base
        {
            AddColumnFromList<T>(sender as DataGrid, e.Column.Header.ToString(), excludeItem);
        }

        public static void AddColumnFromList<T>(DataGrid table, string bindingName, bool excludeItem = false)
        {
            Binding binding = new Binding(bindingName);
            binding.Converter = new Helper.ParentConverter();

            var col = new DataGridComboBoxColumn();
            col.SelectedItemBinding = binding;
            col.Header = bindingName;
            col.MinWidth = 100;
            if (typeof(T) == typeof(Plot))
            {
                if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Any())
                {
                    var plots = AppSettings_User.CurrentProject.Archive.GetItems<Plot>();
                    var plotInfos = new List<string>();
                    plots.ForEach(plot =>
                    {
                        if (excludeItem)
                        {
                            if (NoteManager.NoteOwner != plot)
                                plotInfos.Add($"{plot.Name} ({plot.ID})");
                        }
                        else plotInfos.Add($"{plot.Name} ({plot.ID})");
                    });
                    col.ItemsSource = plotInfos;
                }
            }
            else if (typeof(T) == typeof(Subplot))
            {
                if (Helper.GetAllSubplots(AppSettings_User.CurrentProject) != null && Helper.GetAllSubplots(AppSettings_User.CurrentProject).Any())
                {
                    var subplots = Helper.GetAllSubplots(AppSettings_User.CurrentProject);
                    var subPlotInfos = new List<string>();
                    subplots.ForEach(subplot =>
                    {
                        subPlotInfos.Add($"{subplot.Name} ({subplot.ID})");
                    });
                    col.ItemsSource = subPlotInfos;
                }
            }
            else if (typeof(T) == typeof(Lore))
            {
                if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Any())
                {
                    var lore = AppSettings_User.CurrentProject.Archive.GetItems<Lore>();
                    var loreInfos = new List<string>();
                    lore.ForEach(l =>
                    {
                        if (excludeItem)
                        {
                            if (NoteManager.NoteOwner != l)
                                loreInfos.Add($"{l.Name} ({l.ID})");
                        }
                        else loreInfos.Add($"{l.Name} ({l.ID})");
                    });
                    col.ItemsSource = loreInfos;
                }
            }
            else if (typeof(T) == typeof(Character))
            {
                if (AppSettings_User.CurrentProject.CharacterManager != null && AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>() != null && AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>().Any())
                {
                    var character = AppSettings_User.CurrentProject.CharacterManager.GetItems<Character>();
                    var charInfos = new List<string>();
                    character.ForEach(c =>
                    {
                        if (excludeItem)
                        {
                            if (NoteManager.NoteOwner != c)
                                charInfos.Add($"{c.Name} ({c.ID})");
                        }
                        else charInfos.Add($"{c.Name} ({c.ID})");
                    });
                    col.ItemsSource = charInfos;
                }
            }
            else if (typeof(T) == typeof(Location))
            {
                if (AppSettings_User.CurrentProject.LocationManager != null && AppSettings_User.CurrentProject.LocationManager.GetItems() != null && AppSettings_User.CurrentProject.LocationManager.GetItems().Any())
                {
                    var location = AppSettings_User.CurrentProject.LocationManager.GetItems();
                    var locInfos = new List<string>();
                    location.ForEach(l =>
                    {
                        if (excludeItem)
                        {
                            if (NoteManager.NoteOwner != l)
                                locInfos.Add($"{l.Name} ({l.ID})");
                        }
                        else locInfos.Add($"{l.Name} ({l.ID})");
                    });
                    col.ItemsSource = locInfos;
                }
            }
            table.Columns.Add(col);
        }

        public static void CellValueChanged(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                TableManager.ApplyChanges.Visibility = Visibility.Visible;
                TableManager.RevertChanges.Visibility = Visibility.Visible;
            }
        }

        public static void Open_CloseTable(object sender, MouseButtonEventArgs e)
        {
            var table = TableManager.Tables.Find(t => t.Name.Contains(( sender as DockPanel ).Name));

            if (table.Visibility == Visibility.Collapsed)
            {
                table.Visibility = Visibility.Visible;
                TableManager.OpenCloseTableImgs[ TableManager.Tables.IndexOf(table) ].Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowLeft{ AppSettings_User.ColorSchema.Name }")));
                if (table.Name.Contains(TypeName.Plot.ToString()))
                {
                    TableManager.AdditionalTables[ 0 ].Visibility = Visibility.Visible;
                }
                if (table.Name.Contains(TypeName.Event.ToString()))
                {
                    TableManager.AdditionalTables[ 2 ].Visibility = Visibility.Visible;
                }
            }
            else
            {
                table.Visibility = Visibility.Collapsed;
                TableManager.OpenCloseTableImgs[ TableManager.Tables.IndexOf(table) ].Source = new BitmapImage(new Uri(Helper.GetIcon($"ArrowRight{ AppSettings_User.ColorSchema.Name }")));
                if (table.Name.Contains(TypeName.Plot.ToString()))
                {
                    TableManager.AdditionalTables[ 0 ].Visibility = Visibility.Collapsed;

                }
                if (table.Name.Contains(TypeName.Event.ToString()))
                {
                    TableManager.AdditionalTables[ 2 ].Visibility = Visibility.Collapsed;

                }
            }
        }

        public static void SwitchView(object sender, MouseButtonEventArgs e)
        {
            if (TableManager.RevertChanges.Visibility == Visibility.Visible)
                Popup.ShowWindow(TextCatalog.GetName("SaveChanges?"), TextCatalog.GetName("SaveChanges"), PopupButtons.YesNo, PopupType.Save, sender as Button);
            else
            {
                if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Plot || TableManager.CurrentTables.FirstOrDefault() == TypeName.Lore)
                {
                    if (sender == TableManager.SwitchToFirstTable)
                        TableManager.LoadTables(TypeName.Plot, TypeName.Subplot, TypeName.Event);
                    else if (sender == TableManager.SwitchToSecondTable)
                        TableManager.LoadTables(TypeName.Lore);
                }
                else if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Character || TableManager.CurrentTables.FirstOrDefault() == TypeName.MainCharacter)
                {
                    if (sender == TableManager.SwitchToFirstTable)
                        TableManager.LoadTables(TypeName.Character);
                    else if (sender == TableManager.SwitchToSecondTable)
                        TableManager.LoadTables(TypeName.MainCharacter);
                }
            }
        }

        public static void RowDeleted(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                TableManager.ApplyChanges.Visibility = Visibility.Visible;
                TableManager.RevertChanges.Visibility = Visibility.Visible;
            }
        }

        public static void SetTablesForOwner()
        {
            if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Plot)
            {
                var plotLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.PlotLink.ToString()));
                plotLinkTable.Visibility = Visibility.Collapsed;
                var plotloreLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.LorePlotLink.ToString()));
                plotloreLinkTable.Visibility = Visibility.Collapsed;

                var attachmentFigureTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.AttachmentFigure.ToString()));
                attachmentFigureTable.Visibility = Visibility.Collapsed;
                var characterPresentTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.CharacterPresent.ToString()));
                characterPresentTable.Visibility = Visibility.Collapsed;
                var locationLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.LocationLink.ToString()));
                locationLinkTable.Visibility = Visibility.Collapsed;

                if (NoteManager.NoteOwner == null)
                    return;

                if(NoteManager.NoteOwner is Plot)
                {
                    if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Any())
                    {
                        var plot = AppSettings_User.CurrentProject.Archive.GetItems<Plot>().Find(p => p.ID == NoteManager.NoteOwner.ID);
                        if (plot != null)
                        {
                            plotLinkTable.Visibility = Visibility.Visible;
                            plotloreLinkTable.Visibility = Visibility.Visible;

                            var plotLinkDatatable = DBDataManager.GetTableFromDatabase(TypeName.PlotLink, new List<string>() { "*" }, $"{TypeName.Plot}", NoteManager.NoteOwner.ID);
                            plotLinkTable.ItemsSource = plotLinkDatatable.DefaultView;
                            plotLinkTable.Columns.Clear();
                            AddColumnFromList<Plot>(plotLinkTable, "OtherPlot", true);

                            var plotloreLinkDatatable = DBDataManager.GetTableFromDatabase(TypeName.LorePlotLink, new List<string>() { "*" }, $"{TypeName.Plot}", NoteManager.NoteOwner.ID);
                            plotloreLinkTable.ItemsSource = plotloreLinkDatatable.DefaultView;
                            plotloreLinkTable.Columns.Clear();
                            AddColumnFromList<Lore>(plotloreLinkTable, "Lore");
                        }
                    }
                }
                else if (NoteManager.NoteOwner is Event)
                {
                    if (Helper.GetAllEvents(AppSettings_User.CurrentProject) != null)
                    {
                        var ev = Helper.FindEvent(AppSettings_User.CurrentProject, NoteManager.NoteOwner.ID);
                        if (ev != null)
                        {
                            attachmentFigureTable.Visibility = Visibility.Visible;
                            characterPresentTable.Visibility = Visibility.Visible;
                            locationLinkTable.Visibility = Visibility.Visible;

                            var attachmentFigureDatatable = DBDataManager.GetTableFromDatabase(TypeName.AttachmentFigure, new List<string>() { "*" }, $"{TypeName.Event}", NoteManager.NoteOwner.ID);
                            attachmentFigureTable.ItemsSource = attachmentFigureDatatable.DefaultView;
                            attachmentFigureTable.Columns.Clear();
                            AddColumnFromList<Character>(attachmentFigureTable, "Character");

                            var characterPresentDatatable = DBDataManager.GetTableFromDatabase(TypeName.CharacterPresent, new List<string>() { "*" }, $"{TypeName.Event}", NoteManager.NoteOwner.ID);
                            characterPresentTable.ItemsSource = characterPresentDatatable.DefaultView;
                            characterPresentTable.Columns.Clear();
                            AddColumnFromList<Character>(characterPresentTable, "Character");

                            var locationLinkDatatable = DBDataManager.GetTableFromDatabase(TypeName.LocationLink, new List<string>() { "*" }, $"{TypeName.Event}", NoteManager.NoteOwner.ID);
                            locationLinkTable.ItemsSource = locationLinkDatatable.DefaultView;
                            locationLinkTable.Columns.Clear();
                            AddColumnFromList<Location>(locationLinkTable, "Location");
                        }
                    }
                }
            }
            else if (TableManager.CurrentTables.FirstOrDefault() == TypeName.Lore)
            {
                var aftermathTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.Aftermath.ToString()));

                if (NoteManager.NoteOwner != null && NoteManager.NoteOwner is Lore)
                {
                    aftermathTable.Visibility = Visibility.Collapsed;
                }

                var loreLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.LoreLink.ToString()));
                loreLinkTable.Visibility = Visibility.Collapsed;

                var plotloreLinkTable = TableManager.Tables.Find(t => t.Name.Contains(TypeName.LorePlotLink.ToString()));
                plotloreLinkTable.Visibility = Visibility.Collapsed;

                if (AppSettings_User.CurrentProject.Archive != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>() != null && AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Any())
                {
                    if (NoteManager.NoteOwner != null)
                    {
                        var lore = AppSettings_User.CurrentProject.Archive.GetItems<Lore>().Find(l => l.ID == NoteManager.NoteOwner.ID);
                        if (lore != null)
                        {
                            aftermathTable.Visibility = Visibility.Visible;
                            loreLinkTable.Visibility = Visibility.Visible;
                            plotloreLinkTable.Visibility = Visibility.Visible;

                            var aftermathDatatable = DBDataManager.GetTableFromDatabase(TypeName.Aftermath, new List<string>() { "*" }, "Parent", NoteManager.NoteOwner.ID);
                            aftermathTable.ItemsSource = aftermathDatatable.DefaultView;

                            var loreLinkDatatable = DBDataManager.GetTableFromDatabase(TypeName.LoreLink, new List<string>() { "*" }, $"{TypeName.Lore}", NoteManager.NoteOwner.ID);
                            loreLinkTable.ItemsSource = loreLinkDatatable.DefaultView;
                            loreLinkTable.Columns.Clear();
                            AddColumnFromList<Lore>(loreLinkTable, "OtherLore", true);

                            var plotloreLinkDatatable = DBDataManager.GetTableFromDatabase(TypeName.LorePlotLink, new List<string>() { "*" }, $"{TypeName.Lore}", NoteManager.NoteOwner.ID);
                            plotloreLinkTable.ItemsSource = plotloreLinkDatatable.DefaultView;
                            plotloreLinkTable.Columns.Clear();
                            AddColumnFromList<Plot>(plotloreLinkTable, "Plot");
                        }
                    }
                }
            }
        }
    }
}
