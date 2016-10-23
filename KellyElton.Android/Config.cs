using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xamarin.Forms;

namespace KellyElton
{
    public class Config
    {
        public static IConfig Default {
            get { return _instance; }
            set { _instance = value; }
        }

        private static IConfig _instance;

        static Config() {
            _instance = CreateDefaultConfig();
        }

        private static IConfig CreateDefaultConfig() {
            Type type = typeof( Config );
            string resource = type.Assembly.GetName().Name + ".config.xml";
            var names = type.Assembly.GetManifestResourceNames();
            using( Stream stream = type.Assembly.GetManifestResourceStream( resource ) )
            using( StreamReader reader = new StreamReader( stream ) ) {
                XDocument configDocument = XDocument.Parse( reader.ReadToEnd() );
                return new XMLConfig( configDocument );
            }
        }
    }

    public class XMLConfig : IConfig
    {
        public string GoogleProjectNumber { get; }

        public string OneSignalAppId { get; }

        private readonly XDocument _configDocument;

        public XMLConfig( XDocument document ) {
            _configDocument = document;
            GoogleProjectNumber = GetValue( _configDocument, nameof( GoogleProjectNumber ) );
            OneSignalAppId = GetValue( _configDocument, nameof( OneSignalAppId ) );
        }

        private static string GetValue( XDocument xmlDocument, string name ) {
            return ReadEntryValue( xmlDocument, name );
        }

        private static string ReadEntryValue( XDocument xmlDocument, string name ) => xmlDocument
            .Element( "config" )
            .Elements( "entry" )
            .Single( x => x.Attribute( "name" ).Value.Equals( name ) )
            .Attribute("value").Value
       ;
    }
}