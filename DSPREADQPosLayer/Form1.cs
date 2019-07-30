using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPREADQPosLayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnDecryptDukpt_Click(object sender, EventArgs e)
        {
            //decrypt ipek
            string decIpek = EMVSwipeTLV.TripleDES.decrypt_ECB(txtIpek.Text, txtTerminalMasterKey.Text);
            string key = EMVSwipeTLV.DUKPTServer.GetDataKeyFromIPEK(txtKsn.Text, decIpek);
            string tracks = EMVSwipeTLV.TripleDES.decrypt_CBC(txtEntracks.Text, key);

            txtResult.Text = tracks;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtBdk.Text = "0123456789ABCDEFFEDCBA9876543210";
            txtTerminalMasterKey.Text = "5F8B2B8818966C5CD4CC393AF9FC7722";
            txtIpek.Text = "A0A5C7174F34C84C0E64BF98D16DB177";
            txtKsn.Text = "01436022300041E00001";
            txtEntracks.Text = "384E6BBE09EACDE3A875AD0944BB2BB82299AE6701E04AA9";
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
