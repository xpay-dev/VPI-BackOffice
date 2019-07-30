using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Format6_Decoder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

//            IDC_KSN.Text = "20100401000030E00024";
//            IDC_EncTrack_Hex.Text = "37F47FE84EAAE3B4E69C05A09F598FC25AD8D9FDB0E4839DBA0601F4C6EFA0547CB0D6D77C306BF8";
            IDC_BDK.Text = "0123456789ABCDEFFEDCBA9876543210";
        }

        private void IDC_Decrypt_Click(object sender, EventArgs e)
        {
            Decryptor decrypt;
            int iResult;

            decrypt = new Decryptor(IDC_BDK.Text);
//            if (IDC_EncTrack_Hex.Text.Length != 0)
                iResult = decrypt.Decrypt(IDC_KSN.Text, IDC_EncTrack_Hex.Text);
//            else
//                iResult = decrypt.Decrypt_Base64(IDC_KSN.Text, IDC_EncTrack_Base64.Text);

            if (iResult == 0)
                IDC_Track2.Text = decrypt.GetTrack2();
            else
                IDC_Track2.Text = "Fail to decrypt";
        }
    }
}
