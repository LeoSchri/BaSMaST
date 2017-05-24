
namespace BaSMaST_V3
{
    public class WeatherData:Base
    {
        private static long _weatherNextID;

        public PointInTime Begin { get; private set; }
        public PointInTime End { get; private set; }
        public WeatherType Type { get; private set; }

        public WeatherData(string name, PointInTime begin, PointInTime end, WeatherType type, string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.Weather ].IDLetter}{_weatherNextID++}",name)
        {
            Begin = begin;
            End = end;
            Type = type;

            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Weather.ToString());
            else SetID(id);
            _weatherNextID = Helper.GetNumeric(ID)+2;
        }
    }
}
