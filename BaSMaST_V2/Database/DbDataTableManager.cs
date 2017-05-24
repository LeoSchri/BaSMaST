using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSMaST_V3
{
    public partial class DBDataManager
    {
        public static string SqlQueryBuilder<T>(TypeName type, string tableName = null) where T : Base
        {
            var props = typeof(T).GetProperties().ToList();
            var parts = new List<string>();
            parts.Add($"CREATE TABLE IF NOT EXISTS `{AppSettings_User.CurrentProject.Name}`.`{type.ToString()}{(string.IsNullOrEmpty(tableName)?"":tableName)}`(");
            parts.Add($"`id{(string.IsNullOrEmpty(tableName)?type.ToString():tableName)}` VARCHAR(10) NOT NULL,`Name` VARCHAR(45) NOT NULL,");
            parts.Add(VariableBuilder(props));
            parts.Add(VariableBuilder(type));
            parts.Add($"PRIMARY KEY(`id{( string.IsNullOrEmpty(tableName) ? type.ToString() : tableName )}`)");
            parts.Add(")ENGINE = InnoDB DEFAULT CHARACTER SET = utf8;");
            return string.Join("", parts);
        }

        public static string SqlQueryBuilder(TypeName type)
        {
            var parts = new List<string>();
            parts.Add($"CREATE TABLE IF NOT EXISTS `{AppSettings_User.CurrentProject.Name}`.`{type.ToString()}`( ");
            parts.Add($"`id{type}` INT NOT NULL AUTO_INCREMENT,");
            parts.Add(VariableBuilder(type));
            parts.Add($"PRIMARY KEY(`id{type}`)");
            parts.Add(")ENGINE = InnoDB DEFAULT CHARACTER SET = utf8;");

            return string.Join("", parts);
        }

        private static string VariableBuilder(List<System.Reflection.PropertyInfo> props)
        {
            var infos = new List<string>();
            props.ForEach(info =>
            {
                if (info.PropertyType == typeof(string) || info.PropertyType == typeof(List<string>) || info.PropertyType == typeof(List<int>) || Helper.IsListOfEnum(info.PropertyType))
                {
                    if (info.Name != "ID" && info.Name != "Name")
                    {
                        infos.Add($"`{info.Name}` VARCHAR(255) NULL,");
                    }
                }
                else if (info.PropertyType == typeof(Base))
                {
                    infos.Add($"`{info.Name}` VARCHAR(10) NOT NULL,");
                }
                else if (info.PropertyType == typeof(bool))
                {
                    infos.Add($"`{info.Name}` TINYINT(1) NOT NULL,");
                }
                else if (info.PropertyType == typeof(int) || info.PropertyType == typeof(long))
                {
                    infos.Add($"`{info.Name}` INT NOT NULL,");
                }
                else if (info.PropertyType == typeof(DateTime) || info.PropertyType == typeof(DateTime?))
                {
                    infos.Add($"`{info.Name}{( info.Name.Contains("Date") ? "" : "Date" )}` DATETIME NULL,");
                }
                else if (info.PropertyType == typeof(PointInTime))
                {
                    infos.Add($"`{info.Name}{( info.Name.Contains("Date") ? "" : "Date" )}` DATE NULL, `{info.Name}Daytime` VARCHAR(45) NULL,");
                }
                else if (info.PropertyType.IsEnum)
                {
                    infos.Add($"`{info.Name}` VARCHAR(45) NOT NULL,");
                }
            });

            return string.Join("", infos);
        }

        private static string VariableBuilder(TypeName type)
        {
            var vars = new List<string>();
            var links = AppSettings_Static.TypeInfos[ type ].Links;
            links.ForEach(l =>
            {
                vars.Add($"`{l.PropName}` VARCHAR(10) NOT NULL,");
            });

            return string.Join("", vars);
        }

        //private static string PrimaryKeys(TypeName type)
        //{
        //    var links = AppSettings_Static.TypeInfos[ type ].Links;
        //    if (!links.Any())
        //        return $"PRIMARY KEY(`id{type}`)";
        //    var link = new List<string>();
        //    links.ForEach(l =>
        //    {
        //        if (!( l.PropName == "Owner" && ( type == TypeName.ResourceLink || type == TypeName.ItemLink ) ))
        //            link.Add($"`{l.PropName}`");
        //    });

        //    return $"PRIMARY KEY(`id{type}`,{string.Join(",", link)}),";
        //}

        //private static string IndexBuilder(TypeName type)
        //{
        //    var links = AppSettings_Static.TypeInfos[ type ].Links;
        //    if (!links.Any())
        //        return "";
        //    var indexes = new List<string>();
        //    links.ForEach(l =>
        //    {
        //        if (!( l.PropName == "Owner" && ( type == TypeName.ResourceLink || type == TypeName.ItemLink ) ))
        //            indexes.Add($"INDEX `fk_{type}_{l.PropName}_idx` (`{l.PropName}` ASC),");
        //    });
        //    return string.Join("", indexes);
        //}

        //private static string ConstraintBuilder(TypeName type)
        //{
        //    var links = AppSettings_Static.TypeInfos[ type ].Links;
        //    if (!links.Any())
        //        return "";
        //    var constraints = new List<string>();
        //    links.ForEach(l =>
        //    {
        //        if (!( l.PropName == "Owner" && ( type == TypeName.ResourceLink || type == TypeName.ItemLink ) ))
        //        {
        //            constraints.Add($"CONSTRAINT `fk_{type}_{l.PropName}` FOREIGN KEY (`{l.PropName}`) REFERENCES `{AppSettings_User.CurrentProject.Name}`.`{l.Name}` (`id{l.Name}`) ON DELETE NO ACTION ON UPDATE NO ACTION");
        //        }
        //    });
        //    return string.Join(",", constraints);
        //}
    }
}
