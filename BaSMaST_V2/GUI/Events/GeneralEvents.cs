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

namespace BaSMaST_V3
{
    public class GeneralEvents
    {
        public static List<string> SetMonthDayCount(int year, int month)
        {
            var days = new List<string>();
            var daysPerMonth = DateTime.DaysInMonth(year, month);

            for (int i = 1; i < daysPerMonth + 1; i++)
            {
                days.Add(i.ToString());
            }

            return days;
        }

        public static void ResetMonthDayCounts(object sender, TextChangedEventArgs e)
        {
            if (Config.BookConfigs != null)
            {
                var bookConfig = Config.BookConfigs.Find(bC => bC.ComboBoxes.Find(c => c.ComboBoxTextBox == ( sender as TextBox )) != null);
                if (bookConfig == null)
                    bookConfig = Config.BookConfigs.Find(bC => bC.BookDateFromYear == ( sender as TextBox ));

                var monthAsInt = bookConfig.Months[ bookConfig.ComboBoxes.Find(c => c.Name.Contains("Month")).ComboBoxTextBox.Text ] + 1;
                var list = SetMonthDayCount(Convert.ToInt32(bookConfig.BookDateFromYear.Text), monthAsInt);
                bookConfig.ComboBoxes.Find(c => c.Name.Contains("Day")).ChangeItems(list, list[ 0 ]);
            }
            else if(Config.Window.ComboBoxes.Count != 0)
            {
                var monthAsInt = Config.Window.Months[ Config.Window.ComboBoxes.Find(c => c.Name.Contains("Month")).ComboBoxTextBox.Text ] + 1;
                var list = SetMonthDayCount(Convert.ToInt32(Config.Window.DateYear.Text), monthAsInt);
                Config.Window.ComboBoxes.Find(c => c.Name.Contains("Day")).ChangeItems(list, list[ 0 ]);
            }
        }
    }
}
