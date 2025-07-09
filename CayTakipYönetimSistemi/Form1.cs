using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace CayTakipYönetimSistemi
{
    public partial class Form1 : Form
    {
        public Kayit kayit;
        public AlimBilgileri alimBilgileri;
        public Personel personel;
        public PersonelGiris personelGiris;
        public PersonelEkle PersonelEkle;

        public Form1()
        {
            kayit = new Kayit();
            kayit.frm1 = this;
            personelGiris = new PersonelGiris();
            personelGiris.frm1 = this;
            personel = new Personel();
            personel.frm1 = this;
            PersonelEkle = new PersonelEkle();
            PersonelEkle.frm1 = this;

            alimBilgileri = new AlimBilgileri();
            alimBilgileri.frm1 = this;

            InitializeComponent();

            // Sadece sayısal ve 11 karakterlik sınır
            textBox1.MaxLength = 11;
            textBox1.KeyPress += (s, e) =>
            {
                // Sayı değilse ve kontrol tuşları değilse (Backspace gibi) engelle
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            // Sadece harf
            textBox2.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
                    e.Handled = true;
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        

        public TextBox MyTexBox2
        {
            get { return textBox2; }
            set { textBox2 = value; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

       

       

     

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Boş alan bırakamazsınız.");
                return; // Boşsa işlemi durdur
            }

            OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb");
            OleDbCommand kmt = new OleDbCommand("select * from Kullanicilar where TC=@tc AND AdSoyad=@adsoyad", baglan);
            kmt.Parameters.AddWithValue("@tc", textBox1.Text);
            kmt.Parameters.AddWithValue("@adsoyad", textBox2.Text);

            baglan.Open();
            OleDbDataReader okuyucu = kmt.ExecuteReader();
            if (okuyucu.Read())
            {
                MessageBox.Show("Giriş Başarılı");

                alimBilgileri.kullaniciTC = textBox1.Text;
                alimBilgileri.kullaniciAdSoyad = textBox2.Text;

                this.Hide();
                alimBilgileri.Show();
            }
            else
            {
                MessageBox.Show("Giriş Başarısız");
            }
            baglan.Close();

        }


        private void button2_Click_1(object sender, EventArgs e)
        {

            this.Hide();
            personelGiris.Show();
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            kayit.Show();
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }
    }
}
