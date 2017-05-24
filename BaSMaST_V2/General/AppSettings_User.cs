using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaSMaST_V3
{
    public static class AppSettings_User
    {
        public static List<Project> Projects { get; private set; }
        public static Project CurrentProject { get; private set; }
        public static Language Language { get; private set; }
        public static ColorSchema ColorSchema { get; private set; }
        public static int FontSize { get; private set; }

        public static void AddProject(Project pro)
        {
            if (Projects == null)
                Projects = new List<Project>();
            Projects.Add(pro);
        }

        public static bool RemoveProject(Project pro)
        {
            Projects.Remove(pro);
            try
            {
                new DirectoryInfo(pro.Location).Delete(true);
                DBConnector.ExecuteQuery($"DROP SCHEMA `{pro.Name}`;");
            }
            catch(Exception e)
            {
                Log.Error(pro,$"RemoveProject({pro.Name})",e);
                return false;
            }
            if(!Projects.Any())
                MainWindow.DisableProjectLoad();
            return true;
        }

        public static void ChangeCurrentProject(Project pro)
        {
            CurrentProject = pro;
        }

        public static void ChangeLanguage(Language language)
        {
            Language = language;
            Trait.SetTraitList();
        }

        public static void ChangeColorSchema(ColorSchema color)
        {
            ColorSchema = color;
        }

        public static void ChangeFontSize(int size)
        {
            FontSize = size;
        }
    }
}
