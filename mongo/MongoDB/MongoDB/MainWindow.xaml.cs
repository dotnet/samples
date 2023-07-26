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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            start();
        }
        
        public void start()
        {
            var connectionString = "mongodb+srv://dhkwon:dnehd0941@atlascluster.3wnpd3y.mongodb.net/AtlasCluster?retryWrites=true&w=majority";
            if (connectionString == null)
            {
                Console.WriteLine("You must set your 'MONGODB_URI' environmental variable. See\n\t https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
                Environment.Exit(0);
            }
            var client = new MongoClient(connectionString);
            var collection = client.GetDatabase("sample_mflix").GetCollection<BsonDocument>("movies");
            var filter = Builders<BsonDocument>.Filter.Eq("title", "Back to the Future");
            var document = collection.Find(filter).First();
            Console.WriteLine(document);
        }
    }
}
