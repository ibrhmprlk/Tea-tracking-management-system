using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CayTakipYönetimSistemi
{
    public partial class PersonelGiris : Form
    {
        public Form1 frm1;
        public string yetki;

        public PersonelGiris()
        {
            InitializeComponent();

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox1.MaxLength = 11;

            textBox1.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
        }

        private void PersonelGiris_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Personel");
            comboBox1.Items.Add("Mudur");
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("TC alanı boş bırakılamaz.");
                return;
            }
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Yetki seçimi boş bırakılamaz.");
                return;
            }

            using (OleDbConnection baglan = new OleDbConnection(
                "Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
            {
                OleDbCommand kmt = new OleDbCommand("SELECT * FROM Personel WHERE TC=@tc AND Yetki=@yetki", baglan);
                kmt.Parameters.AddWithValue("@tc", textBox1.Text);
                kmt.Parameters.AddWithValue("@yetki", comboBox1.Text);

                baglan.Open();
                OleDbDataReader okuyucu = kmt.ExecuteReader();

                if (okuyucu.Read())
                {
                    yetki = comboBox1.Text;
                    baglan.Close();
                    textBox1.Text = "";
                    comboBox1.SelectedIndex = 0;
                    
                    this.Hide();
                    frm1.personel.Show();
                    textBox1.Text = "";
                }
                else
                {
                    MessageBox.Show("Giriş Başarısız");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm1.Show();
            textBox1.Text = "";
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}