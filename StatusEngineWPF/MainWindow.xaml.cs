using CommunityToolkit.Mvvm.ComponentModel;
using Confluent.Kafka;
using Newtonsoft.Json;
using StatusCommon;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StatusEngineWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }
    }

    public partial class MainWindowViewModel : ObservableObject
    {
        private const string BootstrapServers = "localhost:9092";
        private const string TopicName = "ping";
        private const string GroupId = "test-group";

        private IConsumer<Ignore, string> consumer;

        [ObservableProperty]
        private ObservableCollection<ModelNew> models;

        public MainWindowViewModel()
        {
            this.Models = new ObservableCollection<ModelNew>();
            var config = new ConsumerConfig
            {
                BootstrapServers = BootstrapServers,
                GroupId = GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            // Create a new consumer
            consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(TopicName);
            var task1 = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(50));

                        // Check if there are messages
                        if (consumeResult == null) continue;

                        var data = JsonConvert.DeserializeObject<Model>(consumeResult.Message.Value);
                        await Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            var existing = Models.FirstOrDefault(p => p.AppName == data.AppName);

                            if (existing == null) Models.Add(new ModelNew() { AppName = data.AppName, Date = data.Date });
                            else existing.Date = data.Date;
                        });
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
    }

    public partial class ModelNew : ObservableObject
    {
        [ObservableProperty]
        public string appName;

        [ObservableProperty]
        public DateTime date;
    }
}