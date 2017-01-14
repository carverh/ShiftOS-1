/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects;
using ShiftOS.Objects.ShiftFS;

/// <summary>
/// Oobe.
/// </summary>
namespace ShiftOS.WinForms
{
	/// <summary>
	/// Oobe.
	/// </summary>
    public partial class Oobe : Form, IOobe
    {
		/// <summary>
		/// Gets my save.
		/// </summary>
		/// <value>My save.</value>
        public Save MySave { get; private set; }

		/// <summary>
		/// The is transfer mode.
		/// </summary>
        public bool IsTransferMode = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="ShiftOS.WinForms.Oobe"/> class.
		/// </summary>
        public Oobe()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Hides all.
		/// </summary>
		/// <returns>The all.</returns>
        public void HideAll()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl != this.pnlshadow)
                {
                    ctrl.Hide();
                }
            }
        }

		/// <summary>
		/// Setups the UI.
		/// </summary>
        public void SetupUI()
        {
            HideAll();
            Panel shown = null;
            if (IsTransferMode == false)
            {
                switch (MySave.StoryPosition)
                {
                    case 0:
                        shown = pnllanguage;
                        SetupLanguages();
                        break;
                    case 1:
                        shown = pnluserinfo;
                        break;
                    case 2:
                        shown = pnldiscourse;
                        break;
                    default:
                        shown = pnlterminaltutorial;
                        SetupTerminal();
                        break;
                }
            }
            else
            {
                switch (TransferProgress)
                {
                    case 0:
                        shown = pnlreenteruserinfo;
                        break;
                    case 1:
                        shown = pnlauthfail;
                        break;
                    case 2:
                        shown = pnlauthdone;
                        break;
                    case 4:
                        shown = pnlrelogin;
                        break;
                    case 5:
                        shown = pnlrelogerror;
                        break;
                }
            }
            
            if (shown != null)
            {
                shown.Location = new Point((int)(shown.Parent?.Width - shown.Width) / 2, (int)(shown?.Parent.Height - shown.Height) / 2);
                pnlshadow.Size = (Size)shown.Size;
                pnlshadow.Location = new Point((int)shown.Left + 15, (int)shown.Top + 15);
                shown.Show();
            }
        }

		/// <summary>
		/// The transfer progress.
		/// </summary>
        private int TransferProgress = 0;

		/// <summary>
		/// Setups the terminal.
		/// </summary>
        public void SetupTerminal()
        {
            //just so that the terminal can access our save
            SaveSystem.CurrentSave = MySave;

           Applications.Terminal.MakeWidget(txtterm);
            TerminalBackend.InStory = false;
            TerminalBackend.PrefixEnabled = true;
            Console.WriteLine("{TERMINAL_TUTORIAL_1}");
            SaveSystem.TransferCodepointsFrom("oobe", 50);

            Shiftorium.Installed += () =>
            {
                if (SaveSystem.CurrentSave.StoryPosition < 5)
                {
                    if (Shiftorium.UpgradeInstalled("mud_fundamentals"))
                    {
                        Console.WriteLine("{TERMINAL_TUTORIAL_2}");
                        txtterm.Height -= pgsystemstatus.Height - 4;
                        pgsystemstatus.Show();
                        
                        StartInstall();
                        
                    }
                }
            };

        }

		/// <summary>
		/// Gets or sets the things done.
		/// </summary>
		/// <value>The things done.</value>
        public int thingsDone
        {
            get
            {
                return pgsystemstatus.Value;
            }
            set
            {
                this.Invoke(new Action(() =>
                {
                    pgsystemstatus.Value = value;
                }));
            }
        }

		/// <summary>
		/// Gets or sets the things to do.
		/// </summary>
		/// <value>The things to do.</value>
        public int thingsToDo
        {
            get
            {
                return pgsystemstatus.Maximum;
            }
            set
            {
                this.Invoke(new Action(() =>
                {
                    pgsystemstatus.Maximum = value;
                }));
            }
        }

		/// <summary>
		/// Starts the install.
		/// </summary>
		/// <returns>The install.</returns>
        public void StartInstall()
        {
            Dictionary<string, string> Aliases = new Dictionary<string, string>();
            Aliases.Add("help", "sos.help");
            Aliases.Add("save", "sos.save");
            Aliases.Add("shutdown", "sys.shutdown");
            Aliases.Add("status", "sos.status");
            Aliases.Add("pong", "win.open{app:\"pong\"}");
            Aliases.Add("programs", "sos.help{topic:\"programs\"}");

            foreach (var path in Paths.GetAll())
            {
                Console.WriteLine("{CREATE}: " + path);
                thingsDone += 1;
                Thread.Sleep(500);
            }
            thingsToDo = Aliases.Count + Paths.GetAll().Length + 2;

            Console.WriteLine("{INSTALL}: {PONG}");
            thingsDone++;
            Thread.Sleep(200);

            Console.WriteLine("{INSTALL}: {TERMINAL}");
            thingsDone++;
            Thread.Sleep(200);

            foreach (var kv in Aliases)
            {
                Console.WriteLine($"{{ALIAS}}: {kv.Key} => {kv.Value}");
                thingsDone++;
                Thread.Sleep(450);
            }

            SaveSystem.CurrentSave.StoryPosition = 5;
            SaveSystem.SaveGame();
        }

		/// <summary>
		/// Gets the supported langs.
		/// </summary>
		/// <value>The supported langs.</value>
        List<string> supportedLangs
        {
            get
            {
                //return JsonConvert.DeserializeObject<List<string>>(Properties.Resources.languages);

                return new List<string>(new[] { "english" });
            }
        }

		/// <summary>
		/// Setups the languages.
		/// </summary>
		/// <returns>The languages.</returns>
        public void SetupLanguages()
        {
            lblanguage.Items.Clear();

            foreach (var supportedLang in supportedLangs)
            {
                lblanguage.Items.Add(supportedLang);
            }
        }

		/// <summary>
		/// Setups all controls.
		/// </summary>
		/// <returns>The all controls.</returns>
        public void SetupAllControls()
        {
            foreach (Control ctrl in this.Controls)
            {
                SetupControl(ctrl);
            }
        }

		/// <summary>
		/// Setups the control.
		/// </summary>
		/// <returns>The control.</returns>
		/// <param name="ctrl">Ctrl.</param>
        public void SetupControl(Control ctrl)
        {
            if (!string.IsNullOrWhiteSpace(ctrl.Text))
                ctrl.Text = Localization.Parse(ctrl.Text);
            try
            {
                foreach (Control child in ctrl.Controls)
                {
                    SetupControl(child);
                }
            }
            catch
            {
            }

        }

		/// <summary>
		/// Btnselectlangs the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void btnselectlang_Click(object sender, EventArgs e)
        {
            if (lblanguage.SelectedItem != null)
            {
                string l = lblanguage.SelectedItem as string;
                MySave.Language = l;
                MySave.StoryPosition = 1;
                SetupUI();
            }
        }

		/// <summary>
		/// Btnsetuserinfos the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void btnsetuserinfo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtusername.Text))
            {
                MySave.Username = txtusername.Text;

                MySave.Password = txtpassword.Text;

                MySave.SystemName = (string.IsNullOrWhiteSpace(txtsysname.Text)) ? "shiftos" : txtsysname.Text;

                MySave.StoryPosition = 2;

                SetupUI();
            }
        }

		/// <summary>
		/// Button1s the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtdiscoursename.Text))
            {
                MySave.DiscourseName = txtdiscoursename.Text;
                MySave.DiscoursePass = txtdiscoursepass.Text;
            }
            MySave.StoryPosition = 3;
            SetupUI();
        }

		/// <summary>
		/// Starts the showing.
		/// </summary>
		/// <returns>The showing.</returns>
		/// <param name="save">Save.</param>
        public void StartShowing(Save save)
        {
            IsTransferMode = false;
            MySave = save;
            SetupAllControls();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;

            this.Load += (o, a) =>
            {
                SetupUI();
            };
            SaveSystem.GameReady += () =>
            {
                this.Invoke(new Action(() =>
                {
                    this.Close();
                    (AppearanceManager.OpenForms[0] as WindowBorder).BringToFront();
                    Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                }));

            };
            this.Show();
        }

		/// <summary>
		/// Shows the save transfer.
		/// </summary>
		/// <returns>The save transfer.</returns>
		/// <param name="save">Save.</param>
        public void ShowSaveTransfer(Save save)
        {
            MySave = save;
            ServerManager.MessageReceived += (msg) =>
            {
                if(msg.Name == "mud_notfound")
                {
                    TransferProgress = 2;
                    this.Invoke(new Action(() => { SetupUI(); }));
                }
                else if(msg.Name == "mud_found")
                {
                    TransferProgress = 1;
                    this.Invoke(new Action(() => { SetupUI(); }));
                }
                else if(msg.Name == "mud_saved")
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            SaveSystem.CurrentSave = MySave;
                            this.Close();
                            Utils.Delete(Paths.SaveFileInner);
                        }));
                    }
                    catch { }
                }
            };
            IsTransferMode = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.Show();
            SetupUI();
        }

		/// <summary>
		/// Button2s the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            pnlreenteruserinfo.Hide();
            pnlshadow.Size = new Size(0, 0);
            ServerManager.SendMessage("mud_checkuserexists", $@"{{
    username: ""{txtruser.Text}"",
    password: ""{txtrpass.Text}""
}}");
            MySave.Username = txtruser.Text;
            MySave.Password = txtrpass.Text;
        }

		/// <summary>
		/// Button3s the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            TransferProgress = 0;
            SetupUI();
        }

		/// <summary>
		/// Button4s the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void button4_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("mud_save", JsonConvert.SerializeObject(MySave));
        }

		/// <summary>
		/// Prompts for login.
		/// </summary>
		/// <returns>The for login.</returns>
        public void PromptForLogin()
        {
            ServerManager.MessageReceived += (msg) =>
            {
                if (msg.Name == "mud_notfound")
                {
                    TransferProgress = 5;
                    this.Invoke(new Action(() => { SetupUI(); }));
                }
                else if (msg.Name == "mud_found")
                {
                    this.Invoke(new Action(() =>
                    {
                        Utils.WriteAllText(Paths.GetPath("user.dat"), $@"{{
    username: ""{txtluser.Text}"",
    password: ""{txtlpass.Text}""
}}");
                        SaveSystem.ReadSave();
                        this.Close();
                    }));
                }
            };
            IsTransferMode = true;
            TransferProgress = 4;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.Show();
            SetupUI();
        }

		/// <summary>
		/// Button6s the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void button6_Click(object sender, EventArgs e)
        {
            TransferProgress = 4;
            SetupUI();
        }

		/// <summary>
		/// Button5s the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void button5_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("mud_checkuserexists", $@"{{
    username: ""{txtluser.Text}"",
    password: ""{txtlpass.Text}""
}}");
        }

		/// <summary>
		/// Oobes the load.
		/// </summary>
		/// <returns>The load.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void Oobe_Load(object sender, EventArgs e)
        {

        }
    }
}
