using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotify_Latest
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
        }

        private void Loading_Load(object sender, EventArgs e)
        {
            prg_loading.Enabled = true;
            tmr_loading.Enabled = true;
            tmr_loading.Interval = 1000; // Set an appropriate interval in milliseconds
            tmr_loading.Start();
            prg_loading.Value = 0;
        }

        private void tmr_loading_Tick(object sender, EventArgs e)
        {
            prg_loading.Value += 10;

            if (prg_loading.Value >= 100)
            {
                // Stop the timer if progress reaches 100
                tmr_loading.Stop();

                new Spotify().Show();
                this.Hide();
            }
        }
    }
}
