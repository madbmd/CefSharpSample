using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace EmuleRQ3270
{
    public partial class Form1 : Form
    {
        public ChromiumWebBrowser chromiumWebBrowser;
        public GlobalKeyboardHook gHook;
        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeChromium();

            //Start listening keybord events
            gHook = new GlobalKeyboardHook();
            gHook.KeyDown += new KeyEventHandler(gHook_KeyDown);
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                gHook.HookedKeys.Add(key);
            }
            gHook.hook();
        }

        private void InitializeChromium()
        {
            //this.KeyPreview = true;
            //this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);
            // Autoriser Celsharp à utiliser les ressources locales
            
            //chromiumWebBrowser.BrowserSettings = browserSettings;
            //Cef.Initialize(browserSettings);

            String page = string.Format(@"{0}\index.htm", Application.StartupPath);
            if (!File.Exists(page))
            {
                MessageBox.Show("Erreur, la page WEB est inéxistante : " + page);
            }
            chromiumWebBrowser = new ChromiumWebBrowser(page);
            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            chromiumWebBrowser.BrowserSettings = browserSettings;
            toolStripContainer1.ContentPanel.Controls.Add(chromiumWebBrowser);
            //this.Controls.Add(chromiumWebBrowser);

            

            chromiumWebBrowser.Dock = DockStyle.Fill;
        }

        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine("Bouton pressé: " + e.KeyChar.ToString());
            if (e.KeyChar >= 112 && e.KeyChar <= 123)
            {
                //MessageBox.Show("Bouton pressé: " + e.KeyChar.ToString());
                //browser.ExecuteScriptAsyncWhenPageLoaded("doEnter();");
            }
        }

        public void gHook_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Bouton pressé: " + e.KeyData.ToString());
            if (ApplicationFocus.ApplicationIsActivated())
            {
                if (e.KeyData.ToString().ToUpper().IndexOf("Shift".ToUpper()) >= 0
                   && e.KeyValue == 66)//B = 66
                {
                    //ALT + B 
                    new Thread(() =>
                    {
                        // execute this on the gui thread. (winforms)
                        this.Invoke(new Action(delegate
                        {
                            //tosBrowserBtnBack_Click(this, new EventArgs());
                            chromiumWebBrowser.ExecuteScriptAsyncWhenPageLoaded("doEnter();");
                        }));

                    }).Start();
                }
                else if (e.KeyData.ToString().ToUpper().IndexOf("Return".ToUpper()) >= 0)//Enter
                {
                    //ALT + F
                    new Thread(() =>
                    {
                        // execute this on the gui thread. (winforms)
                        this.Invoke(new Action(delegate
                        {
                            //tosBrowserBtnFor_Click(this, new EventArgs());
                            chromiumWebBrowser.ExecuteScriptAsyncWhenPageLoaded("doF1();");
                        }));

                    }).Start();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
            gHook.unhook();
        }


        
    }
}
