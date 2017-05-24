using System.IO;

namespace BaSMaST_V3
{
    public class Resource:Base
    {
        private static long _resourceNextID;

        public string FilePath { get; set; }
        public FileType Type { get; private set; }

        public Resource(string name, string path, string id = null):base($"{AppSettings_Static.TypeInfos[ TypeName.Resource ].IDLetter}{_resourceNextID++}", name)
        {
            FilePath = path;

            switch(Path.GetExtension(FilePath))
            {
                case ".png":
                case ".jpg": Type = FileType.Image; break;
                case ".txt":
                case ".doc":
                case ".docx":
                case ".odt": Type = FileType.TextDocument; break;
                default: Type = FileType.Other; break;
            }
            var destFile = AppSettings_User.CurrentProject.ResourceLocation + name + Path.GetExtension(FilePath);
            File.Copy(FilePath,destFile);
            FilePath = destFile;


            if (string.IsNullOrEmpty(id))
                DBDataManager.InsertIntoDatabase(this, TypeName.Resource.ToString());
            else SetID(id);
            _resourceNextID = Helper.GetNumeric(ID)+2;
        }

        public void Remove()
        {
            File.Delete(FilePath);
        }

        public void RemoveAllLinks()
        {
            RemoveAllLinks(TypeName.Resource);
        }
    }
}
