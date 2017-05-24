using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaSMaST_V3
{
    public class DBTableManager
    {
        public static void CreateTables()
        {
            List<string> sqlStatements = new List<string>();
            string projectName = AppSettings_User.CurrentProject.Name;

            sqlStatements.Add("SET @OLD_UNIQUE_CHECKS =@@UNIQUE_CHECKS, UNIQUE_CHECKS = 0; SET @OLD_FOREIGN_KEY_CHECKS =@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS = 0; SET @OLD_SQL_MODE =@@SQL_MODE, SQL_MODE = 'TRADITIONAL,ALLOW_INVALID_DATES';");

            sqlStatements.Add($"DROP SCHEMA IF EXISTS `{projectName}`;");
            sqlStatements.Add($"CREATE SCHEMA IF NOT EXISTS `{projectName}` DEFAULT CHARACTER SET utf8; USE `{projectName}`;");

            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Project>(TypeName.Project));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Book>(TypeName.Book));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Lore>(TypeName.Lore));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Plot>(TypeName.Plot));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Subplot>(TypeName.Subplot));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Event>(TypeName.Event));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Character>(TypeName.Character));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<MainCharacter>(TypeName.MainCharacter));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.AttachmentFigure));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.CharacterPresent));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Repeat>(TypeName.Repeat));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Relationship>(TypeName.Relationship));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<RelationshipPhase>(TypeName.RelationshipPhase));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.RelationshipPhaseLink));
            //sqlStatements.Add(DBDataManager.SqlQueryBuilder<CharacterEvolvement>(TypeName.Evolvement));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<EvolvementPhase>(TypeName.EvolvementPhase));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.EvolvementPhaseLink));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Location>(TypeName.Location));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.LocationLink));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<WeatherData>(TypeName.Weather));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Resource>(TypeName.Resource));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.ResourceLink));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Task>(TypeName.Task));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Aftermath>(TypeName.Aftermath));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.ItemLink));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<ItemTable>(TypeName.ItemTable));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder<Note>(TypeName.Note));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.PlotLink));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.LoreLink));
            sqlStatements.Add(DBDataManager.SqlQueryBuilder(TypeName.LorePlotLink));

            //sqlStatements.Add($"SET SQL_MODE =@OLD_SQL_MODE; SET FOREIGN_KEY_CHECKS = @OLD_FOREIGN_KEY_CHECKS;SET UNIQUE_CHECKS = @OLD_UNIQUE_CHECKS;");
            System.IO.File.WriteAllText($@"{AppSettings_User.CurrentProject.LogLocation}/SCHEMA.txt", $"\n{DateTime.Now}: SQL query for creating schema: {string.Join("\n",sqlStatements)}");

            sqlStatements.ForEach(s =>
            {
                DBConnector.ExecuteQuery(s);
            });
        }
    }
}
