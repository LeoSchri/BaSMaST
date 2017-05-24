using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSMaST_V3
{
    public class TextCatalog
    {
        public static string GetName(string name)
        {
            var match = Words.Find(w => w.Name == name);
            if (match == null)
                return name;
            else return match.GetNameInCurrentLanguage();
        }

        public static string GetSpecifier(string word)
        {
            var match = Words.Find(w => w.German == word);

            if (match == null)
            match = Words.Find(w => w.English == word);

            if (match == null)
                return word;

            return match.Name;
        }

        public static List<Word> Words { get; private set; } = new List<Word>()
        {
            
            //Project
            new Word("Project","Projekt"),
            new Word("Projects","Projekte"),
            new Word("Project configuration","Projektkonfigurierung"),
            new Word("Project name","Projektname"),
            new Word("Project location","Projektverzeichnis"),
            new Word("Backup location","Verzeichnis der Sicherungskopie"),
            new Word("New Project","Neues Projekt"),
            new Word("Open Project","Projekt öffnen"),
            new Word("Delete Project","Projekt löschen"),
            new Word("Edit Project","Projekteinstellungen"),
            new Word("Created on","Erstellt am"),
            new Word("Last modified on","Zuletzt geändert am"),
            //General
            new Word("File","Datei"),
            new Word("Settings","Einstellungen"),
            new Word("Window","Ansicht"),
            new Word("Help","Hilfe"),
            new Word("User documentation","Benutzerhandbuch"),
            new Word("Exit","Schließen"),
            new Word("Language","Sprache"),
            new Word("Font size","Schriftgröße","FontSize"),
            new Word("Welcome!","Willkommen!"),
            new Word("Search","Suche"),
            new Word("Browse","Durchsuchen"),
            new Word("Books","Bücher"),
            new Word("Book","Buch"),
            //Popup Messages
            new Word("Restart necessary","Neustart nötig"),
            new Word("Confirm deletion","Löschbestätigung"),
            new Word("To apply these changes a restart is necessary.","Damit die Änderungen in Kraft treten können,\nist ein Neustart erforderlich.", "RestartNecessary"),
            new Word("Are you sure you want to delete","Sind Sie sich sicher, dass Sie", "DeleteI"),
            new Word("irrevocably?","unwideruflich löschen wollen?", "DeleteII"),
            new Word("already exists. Would you like to overwrite it?","existiert bereits. Möchten Sie es überschreiben?","Overwrite?"),
            new Word("Overwriting","Überschreiben"),
            new Word("Save changes?","Änderungen speichern?","SaveChanges"),
            new Word("Do you want to save your current changes? Otherwise all changes are lost.","Möchten Sie die Änderungen speichern. Andernfalls gehen alle Änderungen verloren.","SaveChanges?"),
            //Months
            new Word("January","Januar"),
            new Word("February","Februar"),
            new Word("March","März"),
            new Word("April","April"),
            new Word("May","Mai"),
            new Word("June","Juni"),
            new Word("July","Juli"),
            new Word("August","August"),
            new Word("September","September"),
            new Word("October","Oktober"),
            new Word("November","November"),
            new Word("December","Dezember"),
            //Task
            new Word("Tasks","Aufgaben"),
            new Word("New Task","Neue Aufgabe"),
            new Word("New Attribute","Neues Attribute"),
            new Word("New Note","Neue Notiz"),
            new Word("The task is overdue", "Die Aufgabe ist überfällig", "Overdue"),
            new Word("Don't show done tasks","Getane Aufgaben nicht anzeigen", "HideDoneTasks"),
            new Word("Show done tasks","Getane Aufgaben anzeigen", "ShowDoneTasks"),
            //Attributes
            new Word("Attribute","Attribut"),
            new Word("Attributes","Attribute"),
            new Word("Allows null","Darf null sein","AllowsNull"),
            new Word("Not null","Darf nicht null sein","NotNull"),
            new Word("Whole number","Ganze Zahl"),
            new Word("Floating-point number","Gleitkommazahl","Double"),
            new Word("True or False","Ja oder Nein","Boolean"),
            new Word("List of Texts","Liste aus Texten","ListOfString"),
            new Word("List of whole numbers","Liste aus ganzen Zahlen", "ListOfInt"),
            new Word("List of floating-point numbers","Liste aus Gleitkommazahlen", "ListOfDouble"),
            new Word("Date","Datum"),
            //Table
            new Word("Revert changes","Änderungen verwerfen"),
            new Word("Apply changes","Änderungen übernehmen"),
            new Word("Add/Remove Columns","Spalten hinzufügen/entfernen"),
            new Word("Switch to Plots, Subplots and Events","Zu Plots, Subplots und Events wechseln","SwitchToPlot"),
            new Word("Switch to Lore","Zu geschichtlichen Ereignissen wechseln","SwitchToLore"),
            new Word("Switch to Characters","Zu den Charakteren wechseln","SwitchToCharacters"),
            new Word("Switch to Main Characters","Zu Hauptcharakteren wechseln","SwitchToMainCharacters"),
            new Word("Plots, Subplots and Events","Plots, Subplots und Events","Plots"),
            new Word("Lore","Geschichtliche Ereignisse"),
            new Word("Notes","Notizen"),
            new Word("Content","Inhalt"),
            new Word("There is no corresponding","Es gibt keinen passenden"),
            new Word("for","für"),
            new Word("Missing entry","Fehlender Eintrag"),
            //DayTimes
            new Word("Early morning","Früher Morgen","EarlyMorning"),
            new Word("Morning","Morgen"),
            new Word("Late morning", "Später Morgen", "LateMorning"),
            new Word("Early noon", "Vormittag","EarlyNoon"),
            new Word("Noon","Mittag"),
            new Word("Afternoon", "Nachmittag", "LateNoon"),
            new Word("Early evening", "Früher Abend", "EarlyEvening"),
            new Word("Evening","Abend"),
            new Word("Late evening", "Später Abend", "LateEvening"),
            new Word("Early night", "Frühe Nacht", "EarlyNight"),
            new Word("Midnight","Mitternacht","MidNight"),
            new Word("Late night", "Späte Nacht", "LateNight"),
            //None
            new Word("None","Keine"),
            //Importance
            new Word("High","Hoch"),
            new Word("Medium","Mittel"),
            new Word("Low","Niedrig"),
            //TableHeader
            new Word("Importance", "Priorität"),
            new Word("Description", "Beschreibung")
        };

        public class Word
        {
            public string Name { get; private set; }
            public string English { get; private set; }
            public string German { get; private set; }

            public Word(string english, string german, string name = null)
            {
                if (name == null)
                    Name = english;
                else Name = name;
                English = english;
                German = german;
            }

            public string GetNameInCurrentLanguage()
            {
                if (AppSettings_User.Language == Language.Deutsch)
                    return German;
                return English;
            }
        }
    }
}
