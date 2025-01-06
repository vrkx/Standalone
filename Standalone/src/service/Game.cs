using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

using System.Net;
using Standalone.Properties;
using Stellar.Assets.Services;

namespace Standalone.src.services
{
    internal class Game
    {


        public void SelectFortnitePath()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.Title = "Selesct the Folder where FortniteGame and Engine are located";
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (!Directory.Exists(dialog.FileName + "\\FortniteGame"))
                    {
                        System.Windows.MessageBox.Show("Error: Path is incorrect, please check if your season is correctly installed !");
                        return;

                    }
                    else
                    {

                        Settings.Default.Path = dialog.FileName;
                        Settings.Default.Save();
                    }
                }
            }
        }

        public void InitializeFortnite()
        {
            string gamePath = Settings.Default.Path;

   
            Thread launchThread = new Thread(() =>
            {
                SuspendFortniteProcess(gamePath, "FortniteGame/Binaries/Win64/FortniteLauncher.exe", true);
                SuspendFortniteProcess(gamePath, "FortniteGame/Binaries/Win64/FortniteClient-Win64-Shipping_EAC.exe", true);
                StartFortnite(gamePath);
            });

            launchThread.Start();
        }

        private void SuspendFortniteProcess(string gamePath, string relativePath, bool suspend)
        {
            try
            {
                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = Path.Combine(gamePath, relativePath),
                        UseShellExecute = true
                    }
                };
                process.Start();

                if (suspend)
                {
                    SuspendProcess.Suspend(process);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la suspension du processus : {ex.Message}");
            }
        }

        private Process StartFortnite(string gamePath)
        {
            string launcherPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string dllPath = Path.Combine(launcherPath, "Starfall.dll");
            string MemoryPath = Path.Combine(launcherPath, "memory.dll");
            string ConsolePath = Path.Combine(launcherPath, "console.dll"); 
            {
                try
                {
                    if (!File.Exists(dllPath))
                    {
                        System.Windows.MessageBox.Show("DLL Not found, Please reinstall the launcher.");

                    }
                    else
                    {
                        Process Fortnitegame = new Process(); // Initialize Fortnite process
                        Fortnitegame.StartInfo.FileName = Path.Combine(gamePath, "FortniteGame/Binaries/Win64/FortniteClient-Win64-Shipping.exe"); // Locate fortnite path by using Settings.Default.Path
                        Fortnitegame.StartInfo.Arguments = $@"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={"vrkx@gmail.com"} -AUTH_PASSWORD={"StandaloneLocal"} -AUTH_TYPE=epic -NOTEXTURESTREAMING -USEALLAVAILABLECORES -HTTP=WinINet -NOSPLASH";
                        Fortnitegame.Start(); // Launch Fortnite

                      
                        ProcessExtension.InjectDll(Fortnitegame.Id, dllPath); // Inject redirector
                        Thread.Sleep(60000);
                        ProcessExtension.InjectDll(Fortnitegame.Id, MemoryPath); // Inject redirector
                        ProcessExtension.InjectDll(Fortnitegame.Id, ConsolePath); // Inject redirector

                        return Fortnitegame;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error starting Fortnite: {ex.Message}");
                }
            }
            return null;
        }
    }
}