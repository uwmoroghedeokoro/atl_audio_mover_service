using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace atl_audio_mover_service
{
    public partial class Service1 : ServiceBase
    {
        static Timer timer = new Timer(60000);
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
           WriteToFile("Start");
            timer.Elapsed += timer_Elapsed;
            //timer.Enabled;
            timer.Start();
            // test();

        }

        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                string ipaddress = "192.168.67.215";
                string hostServer = @"\\" + ipaddress + @"\monitor\";
                //  NetworkShare.ConnectToShare(hostServer, "atladmin", "7mmT@XAy"); //Connect with the new credentials
                NetworkShare.ConnectToShare(@"\\192.168.67.215\monitor", "atladmin", "7mmT@XAy");


                string filepath = ("C:\\webs\\atl_pbx_web\\recordings\\");
                DirectoryInfo diSource = new DirectoryInfo(hostServer);
                DirectoryInfo diTarget = new DirectoryInfo(filepath);

                foreach (FileInfo wav_file in diSource.GetFiles())
                {
                    if (wav_file.CreationTime < DateTime.Now.AddMinutes(-13))
                    {
                        if (File.Exists(Path.Combine(diTarget.FullName, wav_file.Name)))
                            File.Delete(Path.Combine(diTarget.FullName, wav_file.Name));

                        wav_file.MoveTo(Path.Combine(diTarget.FullName, wav_file.Name));
                        WriteToFile("Successfully moved file... " + wav_file.Name);
                    }
                }

                NetworkShare.DisconnectFromShare(hostServer, false); //Disconnect from the server.
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message.ToString());
            }
        }


        static void test()
        {
            try
            {
                string ipaddress = "192.168.67.215";
                string hostServer = @"\\" + ipaddress + @"\monitor\";
                //   NetworkShare.ConnectToShare(hostServer, "atladmin", "7mmT@XAy"); //Connect with the new credentials
                NetworkShare.ConnectToShare(@"\\192.168.67.215\monitor", "atladmin", "7mmT@XAy");


                string filepath = ("C:\\webs\\atl_pbx_web\\recordings\\");
                DirectoryInfo diSource = new DirectoryInfo(hostServer);
                DirectoryInfo diTarget = new DirectoryInfo(filepath);

                foreach (FileInfo wav_file in diSource.GetFiles())
                {
                    if (wav_file.CreationTime < DateTime.Now.AddMinutes(-13))
                    {
                        if (File.Exists(Path.Combine(diTarget.FullName, wav_file.Name)))
                            File.Delete(Path.Combine(diTarget.FullName, wav_file.Name));

                        wav_file.MoveTo(Path.Combine(diTarget.FullName, wav_file.Name));
                        WriteToFile("Successfully moved file... " + wav_file.Name);
                    }
                }
                NetworkShare.DisconnectFromShare(hostServer, false); //Disconnect from the server.
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message.ToString());
            }
        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        static void convert_mp3()
        {
            string filepath = ("c:\\recordings\\");
            DirectoryInfo diTarget = new DirectoryInfo(filepath);
            try
            {

                foreach (FileInfo raw in diTarget.GetFiles())
                {
                    string mp3file = Path.Combine(diTarget.FullName, raw.Name.Split(new string[] { ".wav" }, StringSplitOptions.None)[0] + ".mp3");
                    using (var reader = new WaveFileReader(raw.FullName))
                    {
                        try
                        {
                            MediaFoundationEncoder.EncodeToMp3(reader, mp3file);
                        }
                        catch (InvalidOperationException ex)
                        {
                            WriteToFile(ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            // MediaFoundationEncoder.EncodeToMp3
        }

        protected override void OnStop()
        {
        }
    }
}
