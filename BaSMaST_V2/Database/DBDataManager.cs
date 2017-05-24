using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BaSMaST_V3
{
    public partial class DBDataManager
    {
        public static DataTable GetTableFromDatabase(TypeName table, List<string> columns = null, string valueName = null, string value = null)
        {
            return DBConnector.Select(columns==null?new List<string>() {"*"}:columns,table.ToString(), valueName != null && value != null?new Dictionary<string, string>() { { valueName, value } }:null);
        }

        public static void InsertIntoDatabase<T>(T item, string table) where T : class
        {
            var listOfVariables = new Dictionary<string, string>();
            var props = item.GetType().GetProperties();
            props.ToList().ForEach(p =>
            {
                if (p.PropertyType == typeof(Base) || (p.PropertyType.IsSubclassOf(typeof(Base)) && p.PropertyType != typeof(CharacterEvolvement) && p.PropertyType != typeof(CharacterRelationships)))
                {
                    listOfVariables.Add(p.Name, ( p.GetValue(item) as Base ) != null?( p.GetValue(item) as Base ).ID:"NULL");
                }
                else if (p.PropertyType == typeof(string) || p.PropertyType == typeof(long) || p.PropertyType == typeof(int) || p.PropertyType.IsEnum)
                    listOfVariables.Add(p.Name, p.GetValue(item).ToString());
                else if(p.PropertyType == typeof(List<string>))
                    listOfVariables.Add(p.Name, string.Join(",",(p.GetValue(item) as List<string>)));
                else if (p.PropertyType == typeof(List<int>))
                    listOfVariables.Add(p.Name, string.Join(",", ( p.GetValue(item) as List<int> )));
                else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?) || p.PropertyType == typeof(Nullable<DateTime>))
                {
                    var val = "null";
                    if (( p.GetValue(item) as DateTime? ) != null && ( p.GetValue(item) as DateTime? ).Value != null)
                    {
                        val = ( p.GetValue(item) as DateTime? ).Value.ToString("u");
                        val = val.Remove(val.Length - 1);
                    }
                    if (val != "null")
                        listOfVariables.Add($"{p.Name}{( p.Name.Contains("Date") ? "" : "Date" )}", val);
                }
                else if (p.PropertyType == typeof(PointInTime))
                {
                    var val = "null";
                    if(( p.GetValue(item) as PointInTime ) != null && ( p.GetValue(item) as PointInTime ).Date != null)
                    {
                        val = ( p.GetValue(item) as PointInTime ).Date.Value.ToString("u");
                        val = val.Remove(val.Length - 1);
                    }
                    if(val != "null")
                        listOfVariables.Add($"{p.Name}{( p.Name.Contains("Date") ? "" : "Date" )}", val);

                    var val2 = DayTime.None;
                    if (( p.GetValue(item) as PointInTime ) != null)
                        val2 = ( p.GetValue(item) as PointInTime ).DayTime;
                    listOfVariables.Add($"{p.Name}Daytime", val2.ToString());
                }
                else if (p.PropertyType == typeof(bool))
                {
                    if (( p.GetValue(item) as bool? ) == true)
                        listOfVariables.Add(p.Name, "1");
                    else listOfVariables.Add(p.Name, "0");
                }
                else if (Helper.IsListOfEnum(p.PropertyType))
                    if (item != null && p.GetValue(item) != null)
                        listOfVariables.Add(p.Name, string.Join(",", ( p.GetValue(item) as List<Enum> ).ToString()));
            });
            DBConnector.Insert(table, listOfVariables);
            if(!(item is Project))
                AppSettings_User.CurrentProject.LastModified = DateTime.Now;
        }

        public static void InsertIntoDatabase(AppSettings_Static.TypeInfo info, Base obj, Base linkedObj, TypeName objType = TypeName.None)
        {

            var listOfVariables = new Dictionary<string, string>();
            if (objType != TypeName.None)
            {
                if(objType == info.Links[ 0 ].Name)
                {
                    listOfVariables.Add(info.Links[ 0 ].PropName, obj.ID);
                    listOfVariables.Add(info.Links[ 1 ].PropName, linkedObj.ID);
                }
                else
                {
                    listOfVariables.Add(info.Links[ 0 ].PropName, linkedObj.ID);
                    listOfVariables.Add(info.Links[ 1 ].PropName, obj.ID);
                }
            }
            else
            {
                listOfVariables.Add(info.Links[0].PropName, obj.ID);
                listOfVariables.Add(info.Links[ 1 ].PropName, linkedObj.ID);
            }
            DBConnector.Insert(info.Name.ToString(), listOfVariables);
            AppSettings_User.CurrentProject.LastModified = DateTime.Now;
        }

        public static void RemoveFromDatabase(AppSettings_Static.TypeInfo info, string ID)
        {
            DBConnector.DeletePerID(info, ID);
            AppSettings_User.CurrentProject.LastModified = DateTime.Now;
        }

        public static void RemoveLinkOfSameTypeFromDatabase(AppSettings_Static.TypeInfo info, Base obj, Base linkedObj = null)
        {
            DBConnector.DeleteLinkOfSameType(info, obj, linkedObj);
            AppSettings_User.CurrentProject.LastModified = DateTime.Now;
        }

        public static void RemoveLinkFromDatabase(AppSettings_Static.TypeInfo info, TypeName type, Base obj)
        {
            DBConnector.DeleteLink(info, type,obj);
            AppSettings_User.CurrentProject.LastModified = DateTime.Now;
        }

        public static void UpdateDatabase<T>(T item, string table, string propName) where T : class
        {
            var name = "";
            var value = "";
            var props = item.GetType().GetProperties();
            props.ToList().ForEach(p =>
            {
                if(p.Name == propName && propName != "ID")
                {
                    name = p.Name;
                    if (p.PropertyType == typeof(Base) || p.PropertyType.IsSubclassOf(typeof(Base)))
                    {
                        value = ( p.GetValue(item) as Base ) != null ? ( p.GetValue(item) as Base ).ID : "NULL";
                    }
                    else if (p.PropertyType == typeof(string) || p.PropertyType == typeof(long) || p.PropertyType.IsEnum)
                    {
                        value = p.GetValue(item).ToString();
                    }
                    else if (p.PropertyType == typeof(List<string>))
                    {
                        value = string.Join(",",(p.GetValue(item) as List<string>));
                    }
                    else if (p.PropertyType == typeof(List<Enum>))
                    {
                        value = string.Join(",", ( p.GetValue(item) as List<Enum> ).ToString());
                    }
                    else if(p.PropertyType == typeof(List<int>))
                    {
                        value = string.Join(",", ( p.GetValue(item) as List<int> ));
                    }
                    else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?) || p.PropertyType == typeof(Nullable<DateTime>))
                    {
                        name = $"{p.Name}{( p.Name.Contains("Date") ? "" : "Date" )}";
                        var val = "";
                        if (( p.GetValue(item) as DateTime? ) != null && ( p.GetValue(item) as DateTime? ).Value != null)
                        {
                            val = ( p.GetValue(item) as DateTime? ).Value.ToString("u");
                            val = val.Remove(val.Length - 1);
                        }
                    }
                    else if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
                    {
                        if ((p.GetValue(item) as bool?)== true)
                            value = "1";
                        else if(( p.GetValue(item) as bool? ) == false)
                            value = "0";
                        else value = null;
                    }
                    else if(p.PropertyType == typeof(PointInTime))
                    {
                        var val = string.Empty;
                        if (( p.GetValue(item) as PointInTime ) != null && ( p.GetValue(item) as PointInTime ).Date != null)
                        {
                            val = ( p.GetValue(item) as PointInTime ).Date.Value.ToString("u");
                            val = val.Remove(val.Length - 1);
                        }
                        DBConnector.Update(table, ( item as Base ).ID, $"{p.Name}{( p.Name.Contains("Date") ? "" : "Date" )}", val);

                        var val2 = DayTime.None;
                        if (( p.GetValue(item) as PointInTime ) != null)
                            val2 = ( p.GetValue(item) as PointInTime ).DayTime;
                        name = $"{p.Name}Daytime";
                        value = val2.ToString();

                        name = $"{p.Name}Daytime";
                        value = (p.GetValue(item) as PointInTime).DayTime.ToString();
                    }
                }
            });
            if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                DBConnector.Update(table, (item as Base).ID, name, value);
            if(!(item is Project && name == "LastModifiedDate"))
                AppSettings_User.CurrentProject.LastModified = DateTime.Now;
        }

        public static void UpdateDatabaseAttribute(Base obj, string table, Attribute attribute, Property prop)
        {
            if (!string.IsNullOrEmpty(attribute.Name) && !string.IsNullOrEmpty(prop.Value))
                DBConnector.Update(table, obj.ID, attribute.Name, prop.Value);
            AppSettings_User.CurrentProject.LastModified = DateTime.Now;
        }

        public static bool AddColumnToTable(string table, string colName, Type valueType, bool allowNull)
        {
            try
            {
                var type = "VARCHAR(255)";
                if (valueType == typeof(int))
                    type = "INT";
                if (valueType == typeof(long))
                    type = "INT";
                if (valueType == typeof(bool) || valueType == typeof(bool?))
                    type = "TINYINT(1)";
                if (valueType == typeof(DateTime) || valueType == typeof(DateTime?))
                    type = "DATETIME";

                DBConnector.AddColumn(table,colName,type,allowNull);
                AppSettings_User.CurrentProject.LastModified = DateTime.Now;
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"AddColumnToTable({table},{colName},{valueType},{allowNull})",e);
                return false;
            }
            return true;
        }

        public static bool RemoveColumnFromTable(string table, string colName)
        {
            try
            {
                DBConnector.RemoveColumn(table, colName);
                AppSettings_User.CurrentProject.LastModified = DateTime.Now;
            }
            catch (Exception e)
            {
                Log.Error(AppSettings_User.CurrentProject, $"RemoveColumnFromTable({table},{colName})",e);
                return false;
            }
            return true;
        }

        public static bool SynchronizeProjects()
        {
            try
            {
                var dbNames = DBConnector.ExecuteQueryWithResult("SHOW DATABASES;");
                var schemas = new List<string>();
                if (dbNames.Rows.Count < 4)
                    return false;
                for (int i = 0; i < dbNames.Rows.Count; i++)
                {
                    if (dbNames.Rows[ i ][ 0 ].ToString() != "mysql" && dbNames.Rows[ i ][ 0 ].ToString() != "information_schema" && dbNames.Rows[ i ][ 0 ].ToString() != "performance_schema")
                        schemas.Add(Helper.FirstLetterUppercase(dbNames.Rows[ i ][ 0 ].ToString()));
                }

                schemas.ForEach(s =>
                {
                    var data = DBConnector.Select(new List<string>() { "*" }, TypeName.Project.ToString(), schema: s);
                    var p = new Project(data.Rows[ 0 ][ "Name" ].ToString(),
                                        data.Rows[ 0 ][ "Location" ].ToString(),
                                        data.Rows[ 0 ][ "BackupLocation" ].ToString(),
                                        data.Rows[ 0 ][ $"id{TypeName.Project}" ].ToString());
                });
            }
            catch (Exception e)
            {
                if (AppSettings_User.Projects.Any())
                    Log.Error(null,$"SynchronizeProjects()",e);
                return false;
            }
            return true;
        }

        public static bool ConnectToDatabase()
        {
            try
            {
                if (!Helper.ProgramIsRunning(@"C:\xampp\mysql\bin\mysqld.exe"))
                {
                    Helper.StartProgram(@"C:/xampp/xampp-control.exe");
                    Helper.StartProgram(@"C:/xampp/mysql/bin/mysqld.exe");
                }
                if (DBConnector.Con == null)
                    DBConnector.Con = new MySqlConnection($"server={AppSettings_Static.Server};user={AppSettings_Static.User};password={AppSettings_Static.Password};Allow User Variables=True; convert zero datetime=True");
                if (DBConnector.Con.State != System.Data.ConnectionState.Open)
                    DBConnector.Connect();
            }
            catch (Exception e)
            {
                Log.Error(null,$"ConnectToDatabase()",e);
                return false;
            }
            return true;
        }

        public static void DisconnectFromDatabase()
        {
            DBConnector.Disconnect();
        }
    }
}
