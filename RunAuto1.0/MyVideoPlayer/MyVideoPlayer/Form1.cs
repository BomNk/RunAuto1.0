using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.DirectX.AudioVideoPlayback;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace MyVideoPlayer
{
    public partial class Form1 : Form
    {
        //for your info, this only works on x86 projects
        //due to the library itself

        private Video video;
        private string[] videoPaths;
        private string folderPath = @"C:\Users\tanap\Documents\[C# Winforms] Create your own Video Player (DirectX)\";
        private int selectedIndex = 0;
        private Size formSize;
        private Size pnlSize;

        /// <summary>
        private SerialPort myport;

        string in_data = "";
        int data = 0;
        Stopwatch stopwatch = new Stopwatch();
        int H = 0, M = 0, S = 0, ss;

        int speedMotor = 0;
        public string text_portName = "";
        /// </summary>
        int state;

        public Form1()
        {
            InitializeComponent();
            //InitializeComponent();
            getAvailableComponent();
            timer_time.Start();
            circularProgressBar1.Minimum = 0;
            circularProgressBar1.Maximum = 200;
            circularProgressBar1.Value = 0;
            add.Enabled = false;
            down.Enabled = false;
            START.Enabled = false;
            STOP.Enabled = false;
            state = 0;
        }
        void getAvailableComponent()
        {
            String[] Ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(Ports);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formSize = new Size(this.Width, this.Height);
            pnlSize = new Size(pnlVideo.Width, pnlVideo.Height);

            videoPaths = Directory.GetFiles(folderPath, "*.wmv");

            int index = 0;
            video = new Video(@"C:\Users\tanap\Documents\[C# Winforms] Create your own Video Player (DirectX)\Road1.wmv", false);
            video.Owner = pnlVideo;
            pnlVideo.Size = pnlSize;

            video.Play();

            tmrVideo.Enabled = true;
            btnPlayPause.Text = "Pause";
            video.Ending += new EventHandler(BackLoop);
            //lstVideos.SelectedIndex = selectedIndex;
            video.Stop();
            // START.Visible = false;
        }







        private void myport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string str = myport.ReadLine();

                // data_serialport.Enabled = true;
                //data_serialport.Start();
                this.BeginInvoke(new LineReceivedEvent(LineReceived), str);
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error");
            }

            //Data_table.Text = in_data;
        }
        private delegate void LineReceivedEvent(string line);
        private void LineReceived(string str)
        {
            try
            {
                int x = 0;
                string[] aa = str.Split('/');
                /* for(int i = 0; i < IntParseFast(str); i++)
                 {
                     x++;
                 }*/
                x = Int32.Parse(aa[0]);
                //x[1] = Int32.Parse(aa[1]);
                text_kao.Text = aa[1];
                if (x >= 0 && x < 200)
                {
                    circularProgressBar1.Text = aa[0];
                    circularProgressBar1.Value = Int32.Parse(aa[0]);


                }
                else
                {
                    circularProgressBar1.Text = "Error";
                    circularProgressBar1.Value = 0;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error");
            }

            //circularProgressBar1.Update();


        }






       

      

       

        






       

        private void add_Click(object sender, EventArgs e)   /////////////add////////
        {
            if (speedMotor < 5)
            {
                speedMotor++;
                text_speed.Text = speedMotor + "";
            }
        }


        private void down_Click_1(object sender, EventArgs e)  ///  down /////
        {
            if (speedMotor > 0)
            {
                speedMotor--;
                text_speed.Text = speedMotor + "";
            }
        }

        private void START_Click(object sender, EventArgs e)  //// Start /////
        {
            if (state == 0)
            {
               
                myport.Open();
                myport.DataReceived += myport_DataReceived;
                stopwatch.Start();
                data_serialport.Start();
                video.Play();
                START.Text = "PAUSE";
                state = 1;
            }
            else
            {
                if (!video.Playing)
                {
                    video.Play();
                    tmrVideo.Enabled = true;
                    START.Text = "PAUSE";

                    stopwatch.Start();
                    data_serialport.Start();
                }
                else if (video.Playing)
                {
                    video.Pause();
                    //myport.Write("000");
                    tmrVideo.Enabled = false;
                    START.Text = "PLAY";

                    myport.Write("000");
                    stopwatch.Stop();
                    data_serialport.Stop();
                }

            }

        }

        private void STOP_Click(object sender, EventArgs e)   ////  Stop  ////
        {
            myport.Write("SSSSSS");
            circularProgressBar1.Value = 0;
            circularProgressBar1.Text = "0";
            myport.Close();
            video.Pause();
            state = 0;
            START.Text = "START";
            //myport.;
            stopwatch.Stop();
            data_serialport.Stop();
        }

        private void connect1_Click(object sender, EventArgs e)   //// connect /// 
        {
            try
            {
                myport = new SerialPort();
                myport.BaudRate = 9600;
                myport.PortName = comboBox1.Text;
                myport.Parity = Parity.None;
                myport.DataBits = 8;
                myport.StopBits = StopBits.One;
                add.Enabled = true;
                down.Enabled = true;
                START.Enabled = true;
                STOP.Enabled = true;
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"NOT Connect");
            }
        }

        private void timer_time_Tick(object sender, EventArgs e)
        {
            real_time.Text = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
            date_real.Text = DateTime.Now.Date.ToString().Substring(0, 10);
        }

        private void data_serialport_Tick_1(object sender, EventArgs e)
        {
            myport.Write(speedMotor + "");
            data++;
           

            set_time.Text = stopwatch.Elapsed.ToString().Substring(0, 8);


        }

        private void circularProgressBar1_Click(object sender, EventArgs e)
        {
            
        }

        private void BackLoop(object sender, EventArgs e)
        {

            video.CurrentPosition = 0;// Video keeps looping...

        }
        

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (!video.Playing)
            {
                video.Play();
                tmrVideo.Enabled = true;
                btnPlayPause.Text = "Pause";
            }
            else if (video.Playing)
            {
                video.Pause();
                tmrVideo.Enabled = false;
                btnPlayPause.Text = "Play";
            }
        }

       





        












        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
           /* if (e.KeyCode == Keys.Escape)
            {
                //exit full screen when escape is pressed
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
                this.Size = formSize;
                video.Owner = pnlVideo;
                pnlVideo.Size = pnlSize;
            }*/
        }

      
    }
}
