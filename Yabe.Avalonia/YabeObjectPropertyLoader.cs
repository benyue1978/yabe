using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.IO.BACnet;

namespace Yabe.Avalonia
{
    public class BacnetObjectDescription
    {
        public BacnetObjectTypes typeId { get; set; }
        public List<BacnetPropertyIds> propsId { get; set; } = new();
    }

    public static class YabeObjectPropertyLoader
    {
        private static Dictionary<BacnetObjectTypes, List<BacnetPropertyIds>> _typeToProps = new();
        private static bool _loaded = false;
        private const string DefaultFile = "Common Files/ReadSinglePropDescrDefault.xml";
        private const string ExternalFile = "Common Files/ReadSinglePropDescr.xml";

        public static void EnsureLoaded()
        {
            if (_loaded) return;
            string fileToLoad = File.Exists(ExternalFile) ? ExternalFile : DefaultFile;
            if (!File.Exists(fileToLoad)) return;
            try
            {
                using var fs = File.OpenRead(fileToLoad);
                var xs = new XmlSerializer(typeof(List<BacnetObjectDescription>));
                var list = (List<BacnetObjectDescription>)xs.Deserialize(fs);
                _typeToProps = list.ToDictionary(x => x.typeId, x => x.propsId);
                _loaded = true;
            }
            catch { }
        }

        public static List<BacnetPropertyIds> GetProperties(BacnetObjectTypes type)
        {
            EnsureLoaded();
            if (_typeToProps.TryGetValue(type, out var props))
                return props;
            return new List<BacnetPropertyIds>();
        }
    }
} 