l﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using WMPLib;

namespace VideoApp
{

    public class MainWindow : Form
    {
        public WMPPlayState playState { get; }
        private int countTimer = 300;
        private readonly string currentComp = System.Environment.MachineName;
        private int id;
        public System.Boolean fullScreen { get; set; }

        private Timer timer1 { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
              countTimer--;
              countdown.Text = countTimer.ToString();
            if (countTimer == 0)
            {
                dataGridView1.DataSource = null;
                //starts sql connection
                SqlConnection con = new SqlConnection(@"Server = Server;Initial Catalog = Catalog;Persist Security Info=True;User ID=User;Password=Password");
                con.Open();

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select Something from Somewhere where This = That";
                cmd.Parameters.AddWithValue("@WcData", System.Environment.MachineName);
                // Setting command timeout to 60 second
                cmd.CommandTimeout = 60;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {

                }

                //queries/fills table
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                object IdCode;
                IdCode = dt.Compute("Sum(Something)", "");
                if (string.IsNullOrEmpty(Convert.ToString(IdCode)))
                {
                    if (axWindowsMediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying && id == 0)
                    {
                        //rechecks to see if still in running with out restarting the video below
                        countTimer = 500;
                    }
                    else
                    {
                        //when not running is in running or no video in sql query play demo video
                        axWindowsMediaPlayer.URL = @"Somewhere";
                        axWindowsMediaPlayer.Ctlcontrols.play();
                        axWindowsMediaPlayer.settings.setMode("loop", true);
                        countTimer = 500;
                        id = 0;
                    }
                }
                else if (id == Convert.ToInt32(IdCode))
                {
                    //resets the timer since the video didnt change instead of reloading the video
                    countTimer = 500;
                }
                else
                {
                    //plays new video based on sql query from Database
                    axWindowsMediaPlayer.URL = @"Somewhere".Replace("%IdCode%", Convert.ToString(IdCode) + ".wmv");
                    axWindowsMediaPlayer.Ctlcontrols.play();
                    axWindowsMediaPlayer.settings.setMode("loop", true);
                    countTimer = 500;
                    id = Convert.ToInt32(IdCode);
                }
            }
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            //loads all the settings and timer for the video player upon start of app           
            textBox1.Text = currentComp;
            axWindowsMediaPlayer.settings.autoStart = true;
            axWindowsMediaPlayer.Ctlcontrols.play();
            axWindowsMediaPlayer.settings.setMode("loop", true);
            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 1;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = true;
            axWindowsMediaPlayer_PlayStateChange();
        }
        private void axWindowsMediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //sets the video player to fullscreen if playing
            if (this.axWindowsMediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    this.axWindowsMediaPlayer.fullScreen = true;
                }
        }
    }
}
