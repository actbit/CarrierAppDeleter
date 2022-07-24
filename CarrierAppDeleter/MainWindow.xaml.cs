using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CarrierAppDeleter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            var path = System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "platform-tools", "adb.exe");
            if (!File.Exists(path))
            {
                string dlpath = System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "platform-tools-dl");
                System.Net.WebClient wc = new System.Net.WebClient();
                wc.DownloadFile("https://dl.google.com/android/repository/platform-tools-latest-windows.zip", dlpath+".zip");
                wc.Dispose();
                if (Directory.Exists(dlpath))
                {
                    Directory.Delete(dlpath, true);
                }
                System.IO.Compression.ZipFile.ExtractToDirectory(dlpath + ".zip", dlpath);
                if (Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.Delete(Directory.GetParent(path).FullName, true);
                }
                Directory.Move(System.IO.Path.Combine(dlpath, "platform-tools"), Directory.GetParent(path).FullName);
                Directory.Delete(dlpath);
                File.Delete(dlpath + ".zip");
            }

            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        string[] DisasterApps = new string[] { "com.kddi.disasterapp", "jp.co.nttdocomo.saigaiban", "jp.softbank.mb.cbrl", "jp.softbank.mb.dmb" };
        string[] WiFiApps = new string[] { "jp.co.softbank.wispr.froyo", "com.kddi.android.au_wifi_connect2" };
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            count = 0;
            var path = System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "platform-tools", "adb.exe");
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(path, "devices") { UseShellExecute = false, CreateNoWindow = true,RedirectStandardOutput = true,RedirectStandardError = true};
            
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.EnableRaisingEvents = true;
            process.Exited += Process_Exited;
            DeviceComboBox.Items.Clear(); 
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Log.ScrollToEnd();
            }));
            try
            {
                ((Process)sender).Dispose();

            }
            catch
            {

            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data) && !string.IsNullOrWhiteSpace(e.Data))
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    AppList.Children.Add(new CheckBox() { Content = e.Data.Replace("\n",""), IsChecked = true });
                    Log.Text += e.Data + "\n";
                }));
            }

        }
        int count = 0;
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data) && !string.IsNullOrWhiteSpace(e.Data))
            {
                if (count > 0 && e.Data != "")
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        DeviceComboBox.Items.Add(e.Data.Split('\t')[0]);

                        DeviceComboBox.SelectedIndex = 0;

                        //Log.ScrollToEnd();
                    }));
                }
                else
                {

                }
                count++;
            }



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }

        string[] carrier = new string[] { "kddi", "auone", "docomo", "ntt", "rakuten", "yahoo", "softbank" };
        void loadList()
        {
            AppList.Children.Clear();
            var path = System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "platform-tools", "adb.exe");
            Process process = new Process();
            string addWord = "";
            if (DeviceComboBox.Items.Count > 0 && DeviceComboBox.SelectedIndex != -1)
            {
                addWord = "-s " + DeviceComboBox.SelectedItem.ToString() + " ";
            }
            string addWord2 = "";
            if (R_Disabled.IsChecked == true)
            {
                addWord2 = "-e ";
            }
            if (!R_Repair.IsChecked == true)
            {
                process.StartInfo = new ProcessStartInfo(path, addWord + "shell pm list package -s " + addWord2) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };

                process.OutputDataReceived += Process_OutputDataReceived1;
                process.ErrorDataReceived += Process_ErrorDataReceived1;
                process.EnableRaisingEvents = true;

                process.Exited += Process_Exited;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            else
            {
                Task task = new Task(() =>
                {
                    try
                    {

                        for (int i = 0; i < carrier.Length; i++)
                        {
                            ProcessStartInfo pf = new ProcessStartInfo(path, addWord + "shell pm list package -s " + carrier[i]) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };

                            var t = Process.Start(pf);
                            t.WaitForExit();

                            string readword = t.StandardOutput.ReadToEnd();
                            if(string.IsNullOrEmpty(readword) || string.IsNullOrWhiteSpace(readword))
                            {
                                continue;
                            }
                            string[] installed = readword.Replace("package:", "").Replace("\r", "").Split('\n');
                           
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                Log.Text += readword;
                                Log.Text += t.StandardError.ReadToEnd();
                                Log.ScrollToEnd();

                            }));
                            t.Dispose();
                            pf = new ProcessStartInfo(path, addWord + "shell pm list package -s -u " + carrier[i]) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };

                            t = Process.Start(pf);
                            t.WaitForExit();
                            readword = t.StandardOutput.ReadToEnd();
                            string[] all = readword.Replace("package:", "").Replace("\r", "").Split('\n');

                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                for (int k = 0; k < all.Length; k++)
                                {
                                    if (!installed.Contains(all[k])&&!string.IsNullOrEmpty(all[k])&&!string.IsNullOrWhiteSpace(all[k]))
                                    {
                                        AppList.Children.Add(new CheckBox() { Content = all[k], IsChecked = false });

                                    }
                                }
                                Log.Text += readword;
                                Log.Text += t.StandardError.ReadToEnd();
                                Log.ScrollToEnd();
                            }));
                            t.Dispose();
                        }
                    }
                    catch
                    {

                    }
                


                });
                task.Start();
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            loadList();
        }

        private void Process_ErrorDataReceived1(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data) && !string.IsNullOrWhiteSpace(e.Data))
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Log.Text += e.Data.Replace("\n","") + "\n";
                }));
            }
        }

        private void Process_OutputDataReceived1(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data)&&!string.IsNullOrWhiteSpace(e.Data))
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    bool add = false;
                    for(int i = 0;i< carrier.Length; i++)
                    {
                        if(e.Data.Contains(carrier[i]))
                        {
                            add = true;
                            break;
                        }
                    }
                    if (add)
                    {
                        AppList.Children.Add(new CheckBox() { Content = e.Data.Replace("package:", ""), IsChecked = true });

                    }
                    Log.Text += e.Data + "\n";
                }));
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < AppList.Children.Count; i++)
            {
                if (DisasterApps.Contains(((CheckBox)AppList.Children[i]).Content))
                {
                    ((CheckBox)AppList.Children[i]).IsChecked = CheckAdd;
                }
            }
        }
        bool CheckAdd = false;
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < AppList.Children.Count; i++)
            {
                if (WiFiApps.Contains(((CheckBox)AppList.Children[i]).Content))
                {
                    ((CheckBox)AppList.Children[i]).IsChecked = CheckAdd;
                }
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < AppList.Children.Count; i++)
            {

                ((CheckBox)AppList.Children[i]).IsChecked = true;
                
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < AppList.Children.Count; i++)
            {

                ((CheckBox)AppList.Children[i]).IsChecked = false;

            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {

            var RemoveApps = new List<string>();
            string RaS = "";
            for(int i = 0;i< AppList.Children.Count; i++)
            {
                if ((bool)((CheckBox)AppList.Children[i]).IsChecked)
                {
                    RemoveApps.Add(((CheckBox)AppList.Children[i]).Content.ToString());
                    
                }
            }
            if (RemoveApps.Count > 0)
            {
                if(MessageBox.Show(RemoveApps.Count+"個のアプリが削除されますが実行しますか\n\n", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            DeleteButton.IsEnabled = false;
            DeleteButton.Content = "実行中";
            var path = System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "platform-tools", "adb.exe");
            bool backup = (bool)BackupCheckBox.IsChecked;
            object selectedDevice = DeviceComboBox.SelectedItem;
            ProgressBar1.Maximum = RemoveApps.Count;
            
            Task task = new Task(() =>
            {
                for(int i = 0;i < RemoveApps.Count; i++)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Log.Text += RemoveApps[i]+"を削除します。\n";
                        Log.ScrollToEnd();

                    }));
                    string addCommad = "";
                    if(selectedDevice!=null&&selectedDevice is string&&!string.IsNullOrEmpty((string)selectedDevice)&& !string.IsNullOrWhiteSpace((string)selectedDevice))
                    {
                        addCommad = "-s " + selectedDevice + " ";
                    }
                    ProcessStartInfo pf = new ProcessStartInfo(path, addCommad+ "shell pm uninstall -k --user 0 " + RemoveApps[i]) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
                    var t = Process.Start(pf);
                    t.WaitForExit();

                    string readword = t.StandardOutput.ReadToEnd();
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        if (readword.Contains("Success"))
                        {
                            ProgressBar1.Value++;
                        }
                        Log.Text += readword;
                        Log.Text += t.StandardError.ReadToEnd();
                        Log.ScrollToEnd();

                    }));
                    t.Dispose();
                }
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if(ProgressBar1.Value== ProgressBar1.Maximum)
                    {
                        MessageBox.Show("指定されたすべてのアプリを削除しました。");

                    }
                    else
                    {
                        MessageBox.Show((ProgressBar1.Maximum - ProgressBar1.Value) + "個のアプリを削除できませんでした。");

                    }
                    DeleteButton.IsEnabled = true;

                    DeleteButton.Content = "削除を実行";
                    ProgressBar1.Value = 0;

                    loadList();
                }));

                //    if (backup)
                //    {
                //        ProcessStartInfo pf = new ProcessStartInfo(path, "shell pm path " + RemoveApps[i]+"") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
                //        var t = Process.Start(pf);
                //        t.WaitForExit();

                //        string readword = t.StandardOutput.ReadToEnd();
                //        this.Dispatcher.Invoke((Action)(() =>
                //        {

                //            Log.Text += readword;
                //        }));
                //        t.Dispose();
                //以下バックアップ機能のコード
                //バックアップ機能はAndroidの仕様変更の影響で実装なし。
                //var backupPath = System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "Backup");
                //if (Directory.Exists(backupPath))
                //{
                //    Directory.CreateDirectory(backupPath);
                //}
                //for (int i = 0; i < RemoveApps.Count; i++)
                //{

                //    bool copy = true;
                //    if (backup)
                //    {
                //        ProcessStartInfo pf = new ProcessStartInfo(path, "shell pm path " + RemoveApps[i]+"") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
                //        var t = Process.Start(pf);
                //        t.WaitForExit();

                //        string readword = t.StandardOutput.ReadToEnd();
                //        this.Dispatcher.Invoke((Action)(() =>
                //        {

                //            Log.Text += readword;
                //        }));
                //        t.Dispose();


                //        t.Dispose();
                //        var removePath = readword.Replace("package:", "").Replace(RemoveApps[i], "").Replace("\r","").Replace("\n","");
                //        pf = new ProcessStartInfo(path, "shell cp "+removePath+ " /storage/emulated/0/Download/") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
                //        t = Process.Start(pf);
                //        t.WaitForExit();
                //        readword = t.StandardOutput.ReadToEnd();
                //        this.Dispatcher.Invoke((Action)(() =>
                //        {


                //            Log.Text += readword + "\n";
                //            Log.Text += t.StandardError.ReadToEnd()+ "\n";

                //        }));
                //        t.Dispose();


                //        pf = new ProcessStartInfo(path, "pull " + "/storage/emulated/0/Download/base.apk") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
                //        t = Process.Start(pf);
                //        t.WaitForExit();
                //        readword = t.StandardOutput.ReadToEnd();

                //        if (File.Exists(System.IO.Path.Combine(backupPath, removePath + ".apk")))
                //        {
                //            File.Delete(System.IO.Path.Combine(backupPath, removePath + ".apk"));
                //        }
                //        if(File.Exists(System.IO.Path.Combine(Directory.GetParent(path).FullName, "base.apk")))
                //        {
                //            File.Move(System.IO.Path.Combine(Directory.GetParent(path).FullName, "base.apk"), System.IO.Path.Combine(backupPath, removePath + ".apk"));
                //            this.Dispatcher.Invoke((Action)(() =>
                //            {

                //                Log.Text += readword+"\n"+removePath + "を" + removePath + ".apkとして" + backupPath + "に保存しました。\n";
                //            }));
                //        }
                //        else
                //        {
                //            this.Dispatcher.Invoke((Action)(() =>
                //            {

                //                Log.Text += readword + "\n" + removePath + "のバックアップに失敗しました。\n";

                //            }));
                //            copy = false;

                //        }
                //        t.Dispose();



                //        pf = new ProcessStartInfo(path, "rm -f " + "/storage/emulated/0/Download/base.apk") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
                //        t = Process.Start(pf);
                //        t.WaitForExit();
                //        readword = t.StandardOutput.ReadToEnd();
                //        this.Dispatcher.Invoke((Action)(() =>
                //        {

                //            Log.Text += readword + "\n";
                //            Log.ScrollToEnd();

                //        }));
                //    }

                //}

            });
            task.Start();
            
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {


            var RemoveApps = new List<string>();
            string RaS = "";
            for (int i = 0; i < AppList.Children.Count; i++)
            {
                if ((bool)((CheckBox)AppList.Children[i]).IsChecked)
                {
                    RemoveApps.Add(((CheckBox)AppList.Children[i]).Content.ToString());

                }
            }
            if (RemoveApps.Count > 0)
            {
                if (MessageBox.Show(RemoveApps.Count + "個のアプリが無効化されますが実行しますか\n\n", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            DisableButton.IsEnabled = false;

            DisableButton.Content = "実行中";
            var path = System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "platform-tools", "adb.exe");
            bool backup = (bool)BackupCheckBox.IsChecked;
            object selectedDevice = DeviceComboBox.SelectedItem;
            ProgressBar1.Maximum = RemoveApps.Count;

            Task task = new Task(() =>
            {
                for (int i = 0; i < RemoveApps.Count; i++)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Log.Text += RemoveApps[i] + "を無効化します。\n";
                        Log.ScrollToEnd();

                    }));
                    string addCommad = "";
                    if (selectedDevice != null && selectedDevice is string && !string.IsNullOrEmpty((string)selectedDevice) && !string.IsNullOrWhiteSpace((string)selectedDevice))
                    {
                        addCommad = "-s " + selectedDevice + " ";
                    }
                    ProcessStartInfo pf = new ProcessStartInfo(path, addCommad + "shell pm disable-user --user 0 " + RemoveApps[i]) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
                    var t = Process.Start(pf);
                    t.WaitForExit();

                    string readword = t.StandardOutput.ReadToEnd();
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        if (readword.Contains("disabled-user"))
                        {
                            ProgressBar1.Value++;
                        }
                        Log.Text += readword;
                        Log.Text += t.StandardError.ReadToEnd();
                        Log.ScrollToEnd();

                    }));
                    t.Dispose();
                }
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (ProgressBar1.Value == ProgressBar1.Maximum)
                    {
                        MessageBox.Show("指定されたすべてのアプリを無効化しました。");

                    }
                    else
                    {
                        MessageBox.Show((ProgressBar1.Maximum - ProgressBar1.Value) + "個のアプリを無効化できませんでした。");

                    }
                    DisableButton.IsEnabled = true;

                    DisableButton.Content = "無効化を実行";
                    ProgressBar1.Value = 0;
                    loadList();

                }));


            });
            task.Start();
        }

        private void R_Repair_Checked(object sender, RoutedEventArgs e)
        {
            RepairButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Hidden;
            DisableButton.Visibility = Visibility.Hidden;
            AppList.Children.Clear();
            WiFiButton.Content = "Wi-Fiアプリを追加する";
            DisasterButton.Content = "防災系アプリを追加する";

            CheckAdd = true;
        }

        private void R_Uninstall_Checked(object sender, RoutedEventArgs e)
        {
            DeleteButton.Visibility = Visibility.Visible;
            DisableButton.Visibility = Visibility.Hidden;
            RepairButton.Visibility = Visibility.Hidden;
            AppList.Children.Clear();
            CheckAdd = false;
            DisasterButton.Content = "防災系アプリを除外する";
            WiFiButton.Content = "Wi-Fiアプリを除外する";

        }

        private void R_Disabled_Checked(object sender, RoutedEventArgs e)
        {
            DisableButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Hidden;
            RepairButton.Visibility = Visibility.Hidden;
            AppList.Children.Clear();
            WiFiButton.Content = "Wi-Fiアプリを除外する";
            DisasterButton.Content = "防災系アプリを除外する";
            CheckAdd = false;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            R_Uninstall.IsChecked = true;
        }

        private void RepairButton_Click(object sender, RoutedEventArgs e)
        {

            var RemoveApps = new List<string>();
            string RaS = "";
            for (int i = 0; i < AppList.Children.Count; i++)
            {
                if ((bool)((CheckBox)AppList.Children[i]).IsChecked)
                {
                    RemoveApps.Add(((CheckBox)AppList.Children[i]).Content.ToString());

                }
            }
            if (RemoveApps.Count > 0)
            {
                if (MessageBox.Show(RemoveApps.Count + "個のアプリが復元されますが実行しますか\n\n", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            RepairButton.IsEnabled = false;

            RepairButton.Content = "実行中";
            var path = System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "platform-tools", "adb.exe");
            bool backup = (bool)BackupCheckBox.IsChecked;
            object selectedDevice = DeviceComboBox.SelectedItem;
            ProgressBar1.Maximum = RemoveApps.Count;

            Task task = new Task(() =>
            {
                for (int i = 0; i < RemoveApps.Count; i++)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Log.Text += RemoveApps[i] + "を復元します。\n";
                        Log.ScrollToEnd();

                    }));
                    string addCommad = "";
                    if (selectedDevice != null && selectedDevice is string && !string.IsNullOrEmpty((string)selectedDevice) && !string.IsNullOrWhiteSpace((string)selectedDevice))
                    {
                        addCommad = "-s " + selectedDevice + " ";
                    }
                    ProcessStartInfo pf = new ProcessStartInfo(path, addCommad + "shell cmd package install-existing " + RemoveApps[i]) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
                    var t = Process.Start(pf);
                    t.WaitForExit();

                    string readword = t.StandardOutput.ReadToEnd();
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        if (readword.Contains("installed for user"))
                        {
                            ProgressBar1.Value++;
                        }
                        Log.Text += readword;
                        Log.Text += t.StandardError.ReadToEnd();
                        Log.ScrollToEnd();

                    }));
                    t.Dispose();
                }
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (ProgressBar1.Value == ProgressBar1.Maximum)
                    {
                        MessageBox.Show("指定されたすべてのアプリを復元しました。");

                    }
                    else
                    {
                        MessageBox.Show((ProgressBar1.Maximum - ProgressBar1.Value) + "個のアプリを復元できませんでした。");

                    }
                    RepairButton.IsEnabled = true;

                    RepairButton.Content = "復元を実行";
                    ProgressBar1.Value = 0;
                    loadList();

                }));


            });
            task.Start();
        }
    }
}
