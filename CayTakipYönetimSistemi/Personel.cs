using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System;

namespace CayTakipYönetimSistemi
{
    public partial class Personel : Form
    {
        public Form1 frm1;

        private void Listele()
        {
            OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb");
            DataTable tablo = new DataTable();
            tablo.Clear();
            OleDbDataAdapter adapter = new OleDbDataAdapter("select * from Kullanicilar", baglan);
            adapter.Fill(tablo);
            dataGridView1.DataSource = tablo;
            dataGridView1.Columns[0].HeaderText = "TC Nosu";
            dataGridView1.Columns[1].HeaderText = "Adı Soyadi";
            dataGridView1.Columns[2].HeaderText = "Caylik Alan";
            dataGridView1.Columns[3].HeaderText = "Kota";
            dataGridView1.Columns[4].HeaderText = "KalanKota";


        }

        public Personel()
        {
            InitializeComponent();

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

            textBox4.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            textBox5.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            textBox6.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };


        }

        private void Personel_Shown(object sender, EventArgs e)
        {

        }

        private void Personel_Load(object sender, EventArgs e)
        {
            if (frm1.personelGiris.yetki == "Personel")
                button7.Visible = false;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Listele();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Cells.Count >= 5)
            {
                button6.Visible = true;
                textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value?.ToString() ?? "";
                textBox2.Text = dataGridView1.CurrentRow.Cells[1].Value?.ToString() ?? "";
                textBox3.Text = dataGridView1.CurrentRow.Cells[2].Value?.ToString() ?? "";
                textBox4.Text = dataGridView1.CurrentRow.Cells[3].Value?.ToString() ?? "";
                textBox5.Text = dataGridView1.CurrentRow.Cells[4].Value?.ToString() ?? "";
            }
            else
            {
                MessageBox.Show("Lütfen bir satır seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Lütfen güncellemek için bir satır seçin.");
                return;
            }

            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "" &&
                textBox3.Text.Trim() != "" && textBox4.Text.Trim() != "" && textBox5.Text.Trim() != "")
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                    + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    // Kullanicilar tablosu için update
                    OleDbCommand kmt = new OleDbCommand("UPDATE Kullanicilar SET TC=@tc, AdSoyad=@adsoyad, CaylikAlan=@caylikalan, Kota=@kota, KalanKota=@kalankota WHERE TC=@eskiTC", baglan);
                    kmt.Parameters.AddWithValue("@tc", textBox1.Text);
                    kmt.Parameters.AddWithValue("@adsoyad", textBox2.Text);
                    kmt.Parameters.AddWithValue("@caylikalan", textBox3.Text);
                    kmt.Parameters.AddWithValue("@kota", textBox4.Text);
                    kmt.Parameters.AddWithValue("@kalankota", textBox5.Text);
                    kmt.Parameters.AddWithValue("@eskiTC", dataGridView1.CurrentRow.Cells[0].Value.ToString());
                    kmt.ExecuteNonQuery();

                    // AlimBilgileri tablosu için update
                    OleDbCommand kmt1 = new OleDbCommand("UPDATE AlimBilgileri SET TC=@tc, AdSoyad=@adsoyad, KalanKota=@kalankota WHERE TC=@eskiTC", baglan);
                    kmt1.Parameters.AddWithValue("@tc", textBox1.Text);
                    kmt1.Parameters.AddWithValue("@adsoyad", textBox2.Text);
                    kmt1.Parameters.AddWithValue("@kalankota", textBox5.Text);
                    kmt1.Parameters.AddWithValue("@eskiTC", dataGridView1.CurrentRow.Cells[0].Value.ToString());
                    kmt1.ExecuteNonQuery();

                    baglan.Close();
                }

                Listele();
                button6.Visible = true;
            }
            else
            {
                MessageBox.Show("Boş Alan Bırakamazsınız...");
            }

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }

        private bool TcVarMi(string tc)
        {
            bool varMi = false;
            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
            {
                baglan.Open();
                OleDbCommand kmt = new OleDbCommand("SELECT COUNT(*) FROM Kullanicilar WHERE TC = @tc", baglan);
                kmt.Parameters.AddWithValue("@tc", tc);
                int sayi = (int)kmt.ExecuteScalar();
                varMi = sayi > 0;
            }
            return varMi;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            /*
            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "" &&
                textBox3.Text.Trim() != "" && textBox4.Text.Trim() != "" && textBox5.Text.Trim() != "")
            {
                OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                    + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb");
                OleDbCommand kmt = new OleDbCommand("INSERT INTO Kullanicilar(TC,AdSoyad,CaylikAlan,Kota,KalanKota) Values ('"
                    + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" + textBox5.Text + "')", baglan);
                baglan.Open();
                kmt.ExecuteNonQuery();
                baglan.Close();
                Listele();
            }
            else
            {
                MessageBox.Show("Boş Alan Bırakamazsınız...");
            }
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            */

            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "" || textBox3.Text.Trim() == "" || textBox4.Text.Trim() == "" || textBox5.Text.Trim() == "")
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



            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
            {
                OleDbCommand kmt = new OleDbCommand("INSERT INTO Kullanicilar(TC,AdSoyad,CaylikAlan,Kota,KalanKota) VALUES (@tc, @adsoyad,@caylikalan,@kota,@kalankota )", baglan);
                kmt.Parameters.AddWithValue("@tc", textBox1.Text);
                kmt.Parameters.AddWithValue("@adsoyad", textBox2.Text);
                kmt.Parameters.AddWithValue("@caylikalan", textBox3.Text);
                kmt.Parameters.AddWithValue("@kota", textBox4.Text);
                kmt.Parameters.AddWithValue("@kalankota", textBox5.Text);

                baglan.Open();
                kmt.ExecuteNonQuery();
                baglan.Close();
            }

            Listele();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Lütfen silmek için bir satır seçiniz.");
                return;
            }

            DialogResult uyari = MessageBox.Show("Silmek istediğinize Emin Misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (uyari == DialogResult.Yes)
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                    + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
                {
                    baglan.Open();
                    string silinecekTC = dataGridView1.CurrentRow.Cells[0].Value.ToString();

                    string query = "DELETE FROM Kullanicilar WHERE TC = ?";
                    using (OleDbCommand kmt = new OleDbCommand(query, baglan))
                    {
                        kmt.Parameters.AddWithValue("?", silinecekTC);
                        kmt.ExecuteNonQuery();
                    }

                    string query1 = "DELETE FROM AlimBilgileri WHERE TC = ?";
                    using (OleDbCommand kmt1 = new OleDbCommand(query1, baglan))
                    {
                        kmt1.Parameters.AddWithValue("?", silinecekTC);
                        kmt1.ExecuteNonQuery();
                    }

                    baglan.Close();
                }
                Listele();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox6.Visible = true;

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.Text.Trim() == "")
            {
                Listele();
            }
            else
            {
                OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source="
                    + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb");
                DataTable tablo = new DataTable();
                tablo.Clear();
                OleDbDataAdapter adapter = new OleDbDataAdapter("select * From Kullanicilar where TC like '%" + textBox6.Text + "%'", baglan);
                adapter.Fill(tablo);
                dataGridView1.DataSource = tablo;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm1.PersonelEkle.Show();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Listele();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }
    }
}
