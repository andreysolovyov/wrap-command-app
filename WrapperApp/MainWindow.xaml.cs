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
using System.Configuration;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace WrapperApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; set; }

        private NameValueCollection _appSettings;

        public MainWindow()
        {
            InitializeComponent();

            this.ViewModel = new MainWindowViewModel();
            this.DataContext = ViewModel;

            UnpackResources();
        }

        private void UnpackResources()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    this.ViewModel.InProgress = true;
                    this.ViewModel.LoadingIndicatorText = "Initialization...";

                    try
                    {
                        this._appSettings = ConfigurationManager.AppSettings;
                        var test = this._appSettings["unpackPath"];
                        if(string.IsNullOrEmpty(test))
                        {
                            throw new ConfigurationErrorsException();
                        }
                    }
                    catch (ConfigurationErrorsException)
                    {
                        this._appSettings = new NameValueCollection();
                        // using defaults
                        this._appSettings.Add("unpackPath", "C:/Data/WrapperApp/UnpackedProgram");
                        this._appSettings.Add("executableName", "nogui.exe");
                        this._appSettings.Add("btn1Args", "chrome");
                        this._appSettings.Add("btn2Args", "firefox");
                        this._appSettings.Add("btn3Args", "edge");
                    }
                    
                    var assembly = Assembly.GetExecutingAssembly();
                    var extractPath = this._appSettings["unpackPath"];
                    var executableName = this._appSettings["executableName"];

                    if (!Directory.Exists(extractPath) || !File.Exists(System.IO.Path.Combine(extractPath, executableName)))
                    {
                        this.ViewModel.LoadingIndicatorText = "Preparing executables...";
                        using (var stream = assembly.GetManifestResourceStream("WrapperApp.Content.InnerConsoleApp.zip"))
                        {
                            if (stream == null)
                            {
                                throw new FileNotFoundException("Cannot find executable archive resource");
                            }

                            if (!Directory.Exists(extractPath))
                            {
                                Directory.CreateDirectory(extractPath);
                            }
                            string tempZipPath = System.IO.Path.GetTempFileName();
                            using (var fileStream = File.OpenWrite(tempZipPath))
                            {
                                stream.CopyTo(fileStream);
                                fileStream.Flush();
                                fileStream.Close();
                            }

                            ZipFile.ExtractToDirectory(tempZipPath, extractPath);
                        }
                    }

                    this.ViewModel.LoadingIndicatorText = "Finished";
                }
                finally
                {
                    this.ViewModel.LoadingIndicatorText = string.Empty;
                    this.ViewModel.InProgress = false;
                    this.ViewModel.HasInstalled = true;
                }
            });
            
        }

        private void btnChrome_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteCommand(1);
        }        

        private void btnFirefox_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteCommand(2);
        }

        private void btnEdge_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteCommand(3);
        }

        private void ExecuteCommand(int commandType)
        {
            try
            {
                this.ViewModel.LoadingIndicatorText = "Running command...";
                this.ViewModel.InProgress = true;
                var extractPath = this._appSettings["unpackPath"];
                var executableName = this._appSettings["executableName"];

                var processInfo = Process.Start(System.IO.Path.Combine(extractPath, executableName),
                    this._appSettings[string.Format("btn{0}Args", commandType)]);

                processInfo.WaitForExit();
                this.ViewModel.InProgress = false;
                this.ViewModel.LoadingIndicatorText = string.Empty;
            }
            catch(Exception exc)
            {
                this.ViewModel.LoadingIndicatorText = exc.Message;
            }
            finally
            {
                this.ViewModel.InProgress = false;
            }
        }
    }
}
