using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace Keaf_s_Sound_Board
{
    public partial class Form1 : Form
    {
        public static List<Keybind> keybinds = new List<Keybind>();
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        public static bool IsKeyPushedDown(Keys vKey)
        {
            return 0 != (GetAsyncKeyState((int)vKey) & 0x8000);
        }
        private void KeyPressTimer(object sender, EventArgs e)
        {
            if (keybinds.Any(x => IsKeyPushedDown(x.Key1) && IsKeyPushedDown(x.Key2)))
            {
                //if more than one of the same keybinds play all
                keybinds.FindAll(x => IsKeyPushedDown(x.Key1) && IsKeyPushedDown(x.Key2)).ToList().ForEach(x =>
                {
                    WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer
                    {
                        URL = x.AudioUrl
                    };
                    wplayer.controls.play();
                });
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("KeyBinds.json"))
            {
                keybinds = JsonConvert.DeserializeObject<List<Keybind>>(File.ReadAllText("KeyBinds.json"));
            }
            foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
            {
                cbKeyBind1.Items.Add(key);
                cbKeyBind2.Items.Add(key);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string[] ValidStartingURL = { "file:///", "https://", "http://", "//" };
            if (cbKeyBind1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a value", "Key bind 1");
                return;
            }
            else if (cbKeyBind2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a value", "Key bind 2");
                return;
            }
            if (string.IsNullOrEmpty(tbName.Text)) 
            {
                MessageBox.Show("Please enter name for keybind", "Keybind name");
                return;
            }
            if (!ValidStartingURL.Any(tbAudioURL.Text.StartsWith))
            {
                MessageBox.Show("not valid URL", "Audio URL");
                return;
            }
            keybinds.Add(new Keybind { name = tbName.Text, Key1 = (Keys)cbKeyBind1.SelectedItem, Key2 = (Keys)cbKeyBind2.SelectedItem, AudioUrl = tbAudioURL.Text });
            File.WriteAllText("KeyBinds.json", JsonConvert.SerializeObject(keybinds, Formatting.Indented));
        }
    }

    public class Keybind 
    {
        public string name { get; set; }
        public Keys Key1 { get; set; }
        public Keys Key2 { get; set; }
        public string AudioUrl { get; set; }
    }
}
