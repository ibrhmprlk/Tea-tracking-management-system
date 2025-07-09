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


namespace CayTakipYönetimSistemi
{
    public partial class Kayit : Form
    {
        public Form1 frm1;
        public Kayit()
        {
            InitializeComponent();
            textBox1.MaxLength = 11;
            textBox1.KeyPress += (s, e) =>
            {
                // Sayı değilse ve kontrol tuşları değilse (Backspace gibi) engelle
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
            textBox2.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
                    e.Handled = true;
            };
            textBox3.KeyPress += (s, e) =>
            {
                // Sayı değilse ve kontrol tuşları değilse (Backspace gibi) engelle
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
            textBox4.KeyPress += (s, e) =>
            {
                // Sayı değilse ve kontrol tuşları değilse (Backspace gibi) engelle
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
        }
        public TextBox MyTexBox4
        {
            get { return textBox4; }
            set { textBox4 = value; }
        }

        private void Kayit_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "" || textBox3.Text.Trim() == "" || textBox4.Text.Trim() == "")
            {
                MessageBox.Show("Boş Alan Bırakamazsınız...");
                return;
            }

            if (textBox1.Text.Length != 11)
            {
                MessageBox.Show("TC Numarası 11 haneli olmalıdır.");
                return;
            }

            OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb");
            OleDbCommand kmt = new OleDbCommand("SELECT * FROM Kullanicilar WHERE TC=@tc", baglan);
            kmt.Parameters.AddWithValue("@tc", textBox1.Text);
            baglan.Open();
            OleDbDataReader okuyucu = kmt.ExecuteReader();

            if (okuyucu.Read())
            {
                baglan.Close();
                MessageBox.Show("Böyle Bir Kullanıcı Zaten Var...");
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
            }
            else
            {
                baglan.Close();
                OleDbCommand kmt2 = new OleDbCommand("INSERT INTO Kullanicilar (TC,AdSoyad,CaylikAlan,Kota,KalanKota) VALUES (@tc, @adsoyad, @caylikalan, @kota,@KalanKota)", baglan);
                kmt2.Parameters.AddWithValue("@tc", textBox1.Text);
                kmt2.Parameters.AddWithValue("@adsoyad", textBox2.Text);
                kmt2.Parameters.AddWithValue("@caylikalan", textBox3.Text);
                kmt2.Parameters.AddWithValue("@kota", textBox4.Text);
                kmt2.Parameters.AddWithValue("@KalanKota", textBox4.Text);

                baglan.Open();
                kmt2.ExecuteNonQuery();
                baglan.Close();

                MessageBox.Show("Kayıt Başarılı");

                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";

                this.Hide();
                frm1.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm1.Show();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }
    }
}
