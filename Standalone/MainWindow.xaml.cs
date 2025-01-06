using System.Diagnostics;
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
using Standalone.Properties;
using Standalone.src.services;

namespace Standalone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.Default.Username = Username.Text;
            Settings.Default.Save();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                Game game = new Game();
                game.SelectFortnitePath();

                PathFol.Content = Settings.Default.Path;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to select path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process[] processes = Process.GetProcesses();

            // Check if any process has "FortniteClient-Win64-Shipping" in its process name
            bool isFortniteRunning = processes.Any(p => p.ProcessName.Contains("FortniteClient-Win64-Shipping"));

            if (isFortniteRunning)
            {
                // Get the process name of Fortnite (may vary slightly depending on the installation)
                string fortniteProcessName = "FortniteClient-Win64-Shipping";

                // Get all running Fortnite processes
                Process[] fortniteProcesses = Process.GetProcessesByName(fortniteProcessName);

                // Close each Fortnite process
                foreach (Process process in fortniteProcesses)
                {
                    try
                    {

                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions (e.g., access denied)

                    }
                }

                Launch.Content = "Launch";


            }
            else
            {

                try
                {
                    Game game = new Game();
                    game.InitializeFortnite();

                    Launch.Content = "Launching..";
                    Thread.Sleep(1000);

                    Launch.Content = "Close";

                    Console.WriteLine("User is Logged In!");
                }
                catch (Exception ex)
                {
                    Launch.Content = "Launch";

                    System.Windows.MessageBox.Show($"Error starting Fortnite: {ex.Message}");
                }
            }
        }
    }
}
