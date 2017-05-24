using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace BaSMaST_V3
{
    public class Log
    {
        public static void Error(Project project, string method, Exception e)
        {
            if(project != null && Directory.Exists(project.LogLocation))
                File.AppendAllText(Path.Combine(project.LogLocation, "Errors.txt"), $"\n{DateTime.Now}: Method: {method} => {e}");
            else
            {
                ErrorOut(method, e);
            }
        }

        public static void Info(Project project, string method, bool result)
        {
            if(project != null && Directory.Exists(project.LogLocation))
                File.AppendAllText(Path.Combine( project.LogLocation, "Process.txt"), $"\n{DateTime.Now}: Method: {method} => {result}");
        }

        public static void ErrorOut(string method, Exception e)
        {
            MessageBox.Show(null, $"{DateTime.Now}: Method: {method} => {e}","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
    }
}
