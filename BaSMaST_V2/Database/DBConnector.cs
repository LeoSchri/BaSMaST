using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace BaSMaST_V3
{ 
    public static class DBConnector
    {
        public static MySqlConnection Con { get; set; }

        public static void Connect()
        {
            Con.Open();
        }

        public static void Disconnect()
        {
            Con.Close();
        }

        public static DataTable Select(List<string> columns, string table, Dictionary<string,string> variableValuePairs=null, string schema =null)
        {
            int myInt;
            List<string> whereClauses = new List<string>();
            if(variableValuePairs != null)
            {
                foreach (KeyValuePair<string, string> entry in variableValuePairs)
                {
                    if (!int.TryParse(entry.Value, out myInt))
                    {
                        whereClauses.Add($" `{entry.Key}` = \"{entry.Value}\"");
                    }
                    else
                    {
                        whereClauses.Add($" `{entry.Key}` = {entry.Value}");
                    }
                    
                }
            }
            var cmd = new MySqlCommand($"SELECT {string.Join(",",columns)} FROM `{(string.IsNullOrEmpty(schema)?AppSettings_User.CurrentProject.Name:schema)}`.`{table}`{(variableValuePairs!=null?$" WHERE {string.Join(" AND", whereClauses)}":"")};",Con);
            cmd.Prepare();
            var dataTable = new DataTable();

            try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dataTable);
                }
            }
            catch(Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject,$"Select({string.Join(",", columns)},{table},Keys:{(variableValuePairs!=null?string.Join(",", variableValuePairs.Keys):"")},Values:{(variableValuePairs!=null?string.Join(",", variableValuePairs.Values):"")},{schema})",e);
            }

            return dataTable;
        }

        public static bool Insert(string table, Dictionary<string, string> variableValueWithTypePairs)
        {
            int myInt;

            var values = variableValueWithTypePairs.Values.ToList();
            for (int i =0; i< values.Count; i++)
            {
                if(!int.TryParse(values[i], out myInt))
                {
                    values[i] = $"\"{values[i].Replace(@"\","/")}\"";
                }
            }
            var keys = variableValueWithTypePairs.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[ i ] == "ID")
                    keys[ i ] = $"id{table}";
                keys[ i ] = $"`{keys[ i ]}`";
            }

            var cmd = new MySqlCommand($"INSERT INTO `{AppSettings_User.CurrentProject.Name}`.`{table}` ({string.Join(",",keys)} ) VALUES ( {string.Join(",",values)} );", Con);
            cmd.Prepare();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject,$"Insert({table},Keys:{string.Join(",", variableValueWithTypePairs.Keys)},Values:{string.Join(",", variableValueWithTypePairs.Values)})",e);
                return false;
            }

            return true;
        }

        public static bool DeletePerID(AppSettings_Static.TypeInfo info, string ID)
        {
            var cmd = new MySqlCommand($"DELETE FROM `{AppSettings_User.CurrentProject.Name}`.`{info.Name}` WHERE `id{info.Name}` = \"{ID}\"", Con);
            cmd.Prepare();

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"DeletePerName({info.Name},{ID})",e);
                return false;
            }

            return true;
        }

        public static bool DeleteLinkOfSameType(AppSettings_Static.TypeInfo info, Base obj, Base linkedObj = null)
        {
            var cmd = new MySqlCommand($"DELETE FROM `{AppSettings_User.CurrentProject.Name}`.`{info.Name}` WHERE `{info.Links[0]}` = \"{obj.ID}\" {(linkedObj==null?"OR":"AND")} `{info.Links[ 1 ]}` = \"{(linkedObj==null?obj.ID:linkedObj.ID)}\";", Con);
            cmd.Prepare();

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"DeleteLinkOfSameType({info.Name},{obj.Name},{(linkedObj==null?null:linkedObj.Name)})",e);
                return false;
            }

            return true;
        }

        public static bool DeleteLink(AppSettings_Static.TypeInfo info, TypeName type, Base obj)
        {
            var cmd = new MySqlCommand($"DELETE FROM `{AppSettings_User.CurrentProject.Name}`.`{info.Name}` WHERE `{type}` = \"{obj.ID}\";", Con);
            cmd.Prepare();

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"DeleteLink({info.Name},{type},{obj.Name})",e);
                return false;
            }

            return true;
        }

        public static bool Update(string table, string ID, string propName, string value)
        {
            if (Helper.IsDate(value))
            {
                var date = new DateTime();
                DateTime.TryParse(value, out date);
                if (date != null)
                {
                    value = date.ToString("yyyy-MM-dd hh:mm:ss");
                }
            }
            int myInt;
            string keyValue = $"`{propName}`=\"{value}\"";
            if (int.TryParse(value, out myInt))
            {
                keyValue = $"`{propName}`={value}";
            }
            
            var cmd = new MySqlCommand($"UPDATE `{AppSettings_User.CurrentProject.Name}`.`{table}` SET {keyValue} WHERE `id{table}`=\"{ID}\";", Con);
            cmd.Prepare();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"Update({table},{ID},{propName},{value})",e);
                return false;
            }

            return true;
        }

        public static bool AddColumn(string table, string colName, string valueType, bool allowsNull)
        {
            var cmd = new MySqlCommand($"ALTER TABLE `{AppSettings_User.CurrentProject.Name}`.`{table}` ADD {colName} {valueType} {(!allowsNull?"": "NOT NULL" )}", Con);
            cmd.Prepare();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"AddColumn({table},{colName},{valueType},{allowsNull})",e);
                return false;
            }

            return true;
        }

        public static bool RemoveColumn(string table, string colName)
        {
            var cmd = new MySqlCommand($"ALTER TABLE `{AppSettings_User.CurrentProject.Name}`.`{table}` DROP {colName}", Con);
            cmd.Prepare();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"RemoveColumn({table},{colName})",e);
                return false;
            }

            return true;
        }

        public static bool ExecuteQuery(string query)
        {
            var cmd = new MySqlCommand(query, Con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"ExecuteQuery({query})",e);
                return false;
            }
            return true;
        }

        public static DataTable ExecuteQueryWithResult(string query)
        {
            var cmd = new MySqlCommand(query, Con);
            cmd.Prepare();
            var dataTable = new DataTable();

            try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dataTable);
                }
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"ExecuteQueryWithResult({query})",e);
            }

            return dataTable;
        }

        public static string GetStringByQuery(string query)
        {
            var cmd = new MySqlCommand(query, Con);
            cmd.Prepare();
            string result = "";

            try
            {
                result = cmd.ExecuteScalar().ToString();
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"GetStringByQuery({query})",e);
            }

            return result;
        }

        public static int GetIntByQuery(string query)
        {
            var cmd = new MySqlCommand(query, Con);
            cmd.Prepare();
            int result= 0;

            try
            {
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"GetIntByQuery({query})",e);
            }

            return result;
        }
    }
}
