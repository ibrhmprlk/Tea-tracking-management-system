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
    public partial class PersonelEkle : Form
    {
        public Form1 frm1;

        private void Listele()
        {
            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
            {
                DataTable tablo = new DataTable();
                tablo.Clear();
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM Personel", baglan);
                adapter.Fill(tablo);
                dataGridView1.DataSource = tablo;
                dataGridView1.Columns[0].HeaderText = "TC Nosu";
                dataGridView1.Columns[1].HeaderText = "Adı Soyadi";
                dataGridView1.Columns[2].HeaderText = "Yetki";
            }
        }

        public PersonelEkle()
        {
            InitializeComponent();

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox1.MaxLength = 11;

            textBox1.KeyPress += (s, e) =>
            {
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
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
        }

        private void PersonelEkle_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Mudur");
            comboBox1.Items.Add("Personel");

            button6.Visible = true;     // Arama butonu açık, kapatma yok
            Listele();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private bool TcVarMi(string tc)
        {
            bool varMi = false;
            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
            {
                baglan.Open();
                OleDbCommand kmt = new OleDbCommand("SELECT COUNT(*) FROM Personel WHERE TC = @tc", baglan);
                kmt.Parameters.AddWithValue("@tc", tc);
                int sayi = (int)kmt.ExecuteScalar();
                varMi = sayi > 0;
            }
            return varMi;
        }

        private bool MudurVarMi()
        {
            bool varMi = false;
            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
            {
                baglan.Open();
                OleDbCommand kmt = new OleDbCommand("SELECT COUNT(*) FROM Personel WHERE Yetki = 'Mudur'", baglan);
                int sayi = (int)kmt.ExecuteScalar();
                varMi = sayi > 0;
            }
            return varMi;
        }

        private void button1_Click(object sender, EventArgs e) // Güncelle
        {
            string seciliTC = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            string seciliYetki = dataGridView1.CurrentRow.Cells[2].Value.ToString();

            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "" || comboBox1.Text.Trim() == "")
            {
                MessageBox.Show("Boş Alan Bırakamazsınız...");
                return;
            }

            if (textBox1.Text.Length != 11)
            {
                MessageBox.Show("TC Numarası 11 haneli olmalıdır.");
                return;
            }

            if (textBox1.Text != seciliTC && TcVarMi(textBox1.Text))
            {
                MessageBox.Show("Bu TC numarası başka bir personelde kayıtlı!");
                return;
            }

            if (seciliYetki == "Mudur")
            {
                MessageBox.Show("Müdür bilgileri güncellenemez!");
                return;
            }

            if (comboBox1.Text == "Mudur" && MudurVarMi())
            {
                MessageBox.Show("Sistemde zaten bir Müdür var. Başka Müdür yapamazsınız!");
                return;
            }

            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
            {
                OleDbCommand kmt = new OleDbCommand("UPDATE Personel SET TC=@tc, AdSoyad=@adsoyad, Yetki=@yetki WHERE TC=@eskiTC", baglan);
                kmt.Parameters.AddWithValue("@tc", textBox1.Text);
                kmt.Parameters.AddWithValue("@adsoyad", textBox2.Text);
                kmt.Parameters.AddWithValue("@yetki", comboBox1.Text);
                kmt.Parameters.AddWithValue("@eskiTC", seciliTC);

                baglan.Open();
                kmt.ExecuteNonQuery();
                baglan.Close();
            }

            Listele();
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.Text = "";

            comboBox1.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e) // Sil
        {
            string seciliTC = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            string seciliYetki = dataGridView1.CurrentRow.Cells[2].Value.ToString();

            if (seciliYetki == "Mudur")
            {
                MessageBox.Show("Müdür silinemez!");
                return;
            }

            DialogResult uyari = MessageBox.Show("Silmek istediğinize Emin Misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (uyari == DialogResult.Yes)
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                    + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
                {
                    baglan.Open();
                    string query = "DELETE FROM Personel WHERE TC = ?";
                    using (OleDbCommand kmt = new OleDbCommand(query, baglan))
                    {
                        kmt.Parameters.AddWithValue("?", seciliTC);
                        kmt.ExecuteNonQuery();
                    }
                }
                Listele();
            }
        }

        private void button3_Click(object sender, EventArgs e) // Düzenlemeye Hazırla
        {
            button6.Visible = true;

            textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
           
            comboBox1.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();

            // Önce tüm alanları aktif et
            comboBox1.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;

            // Eğer müdürse tekrar kapat
            if (comboBox1.Text == "Mudur")
            {
                comboBox1.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;

            }
        }

        private void button5_Click(object sender, EventArgs e) // Ekle
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "" || comboBox1.Text.Trim() == "")
            {
                MessageBox.Show("Boş Alan Bırakamazsınız...");
                return;
            }

            if (textBox1.Text.Length != 11)
            {
                MessageBox.Show("TC Numarası 11 haneli olmalıdır.");
                return;
            }

            if (TcVarMi(textBox1.Text))
            {
                MessageBox.Show("Bu TC numarası zaten kayıtlı!");
                return;
            }

            if (comboBox1.Text == "Mudur" && MudurVarMi())
            {
                MessageBox.Show("Sistemde zaten bir Müdür var. Yeni Müdür ekleyemezsiniz!");
                return;
            }

            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
            {
                OleDbCommand kmt = new OleDbCommand("INSERT INTO Personel(TC,AdSoyad,Yetki) VALUES (@tc, @adsoyad, @yetki)", baglan);
                kmt.Parameters.AddWithValue("@tc", textBox1.Text);
                kmt.Parameters.AddWithValue("@adsoyad", textBox2.Text);
                kmt.Parameters.AddWithValue("@yetki", comboBox1.Text);

                baglan.Open();
                kmt.ExecuteNonQuery();
                baglan.Close();
            }

            Listele();
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Visible = true;  // Arama textboxu hep açık değil, ama kapatma yok
            textBox3.Focus();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() == "")
            {
                Listele();
            }
            else
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                    + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
                {
                    DataTable tablo = new DataTable();
                    tablo.Clear();
                    OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM Personel WHERE TC LIKE '%" + textBox3.Text + "%'", baglan);
                    adapter.Fill(tablo);
                    dataGridView1.DataSource = tablo;
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm1.personel.Show();
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.Text = "";

        }

   
        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Listele();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox1.Text = "";
        }
    }
}
