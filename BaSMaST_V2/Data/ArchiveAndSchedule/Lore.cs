using System;
using System.Collections.Generic;

namespace BaSMaST_V3
{
    public class Lore:Base
    {
        private static long _loreNextID;
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                DBDataManager.UpdateDatabase(this, TypeName.Lore.ToString(), "Description");
            }
        }
        public DateTime? Begin { get; set; }
        public DateTime? End { get; set; }
        public Importance Importance { get; set; }
        public Manager<Aftermath> AftermathManager { get; private set; }

        internal Lore(string name, string desc, Importance impo, DateTime? begin = null, DateTime? end = null, List<Aftermath> aftermaths = null, string id=null) : base($"{AppSettings_Static.TypeInfos[ TypeName.Lore ].IDLetter}{_loreNextID++}", name)
        {
            _description = desc;
            Begin = begin;
            End = end;
            Importance = impo;
            AftermathManager = Manager<Aftermath>.Create();

            if (begin != null && end != null)
            {
                AddToSchedule(begin, end);
            }

            var arch = AppSettings_User.CurrentProject.Archive;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Lore.ToString());
            else SetID(id);
            _loreNextID = Helper.GetNumeric(ID)+2;
        }

        public void AddToSchedule(DateTime? begin, DateTime? end)
        {
            var schedule = AppSettings_User.CurrentProject.Schedule;

            Begin = begin;
            End = end;

            schedule.AddItem(this);
        }

        public void RemoveFromSchedule()
        {
            var schedule = AppSettings_User.CurrentProject.Schedule;

            Begin = null;
            End = null;

            schedule.RemoveItems(new List<Lore>() { this }, TypeName.Lore);
        }

        public static void RemoveLoreFromSchedule(List<Lore> lore)
        {
            var schedule = AppSettings_User.CurrentProject.Schedule;

            lore.ForEach(l =>
            {
                l.Begin = null;
                l.End = null;
            });

            schedule.RemoveItems(lore, TypeName.Lore);
        }

        public void RemoveAllLinks()
        {
            AppSettings_User.CurrentProject.ResourceLinkManager.RemoveAllLinksForItem(this, TypeName.Lore, TypeName.ResourceLink);
            AppSettings_User.CurrentProject.LoreLinkManager.RemoveAllLinksForItem(this, TypeName.Lore, TypeName.LoreLink);
            AppSettings_User.CurrentProject.PlotLoreLinkManager.RemoveAllLinksForItem(this, TypeName.Lore, TypeName.LorePlotLink);
            RemoveAllLinks(TypeName.Lore);
        }
    }
}
