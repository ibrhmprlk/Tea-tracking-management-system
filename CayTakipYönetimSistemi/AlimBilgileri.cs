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
    public partial class AlimBilgileri : Form
    {
        public string kullaniciTC;
        public string kullaniciAdSoyad;
        public Form1 frm1;
        public Kayit kayit1;

        public AlimBilgileri()
        {
            kayit1 = new Kayit();
            InitializeComponent();

            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
            textBox8.ReadOnly = true;
            textBox9.ReadOnly = true;
            textBox3.BackColor = SystemColors.Control;
            textBox4.BackColor = SystemColors.Control;
            textBox5.BackColor = SystemColors.Control;
            textBox6.BackColor = SystemColors.Control;
            textBox7.BackColor = SystemColors.Control;
            textBox8.BackColor = SystemColors.Control;
            textBox9.BackColor = SystemColors.Control;

            textBox2.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            textBox1.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
        }

        public int kg;
        public double islaklikfiresi;
        public double netMubaya;
        public double KalanKota;
        public double BezDarasi;
        public double BrutAgirlik;

        private void AlimBilgileri_Load(object sender, EventArgs e)
        {
            button1.Visible = false;
            button3.Visible = false;
            groupBox1.Visible = false;
            groupBox2.Visible = false;

            // **KULLANICIYA ÖZEL ALIM BİLGİLERİNİ DATAGRID'E YÜKLE**
            LoadKullaniciAlimBilgileri();
        }

        // *** YENİ EKLENDİ ***
        private void LoadKullaniciAlimBilgileri()
        {
            try
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=" + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb"))
                {
                    baglan.Open();
                    string sorgu = "SELECT * FROM AlimBilgileri WHERE TC = ?";
                    using (OleDbCommand cmd = new OleDbCommand(sorgu, baglan))
                    {
                        cmd.Parameters.AddWithValue("?", kullaniciTC);

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" )
            {
                button1.Visible = true;
                button3.Visible = true;
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" )
            {
                button1.Visible = true;
                button3.Visible = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
         
            groupBox1.Visible = false;

            BezDarasi = int.Parse(textBox1.Text);
            kg = int.Parse(textBox2.Text);

            BrutAgirlik = kg - BezDarasi;
            textBox3.Text = BrutAgirlik.ToString();

            islaklikfiresi = 0;
            netMubaya = BrutAgirlik - islaklikfiresi;
            textBox4.Text = netMubaya.ToString();

            OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=" + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb");

            try
            {
                baglan.Open();

                OleDbCommand kmt = new OleDbCommand("SELECT KalanKota FROM Kullanicilar WHERE TC = ?", baglan);
                kmt.Parameters.AddWithValue("?", kullaniciTC);

                object sonuc = kmt.ExecuteScalar();

                if (sonuc != null && sonuc != DBNull.Value)
                {
                    double kalanKotaDegeri = double.Parse(sonuc.ToString());
                    double yeniKota = kalanKotaDegeri - netMubaya;

                    if (yeniKota < 0)
                    {
                        MessageBox.Show("İşlem yapılamaz. Kalan kota yetersiz!");
                        return;
                    }
                    groupBox1.Visible = true;
                    textBox5.Text = yeniKota.ToString();

                    OleDbCommand kmt2 = new OleDbCommand("UPDATE Kullanicilar SET KalanKota = ? WHERE TC = ?", baglan);
                    kmt2.Parameters.AddWithValue("?", yeniKota);
                    kmt2.Parameters.AddWithValue("?", kullaniciTC);
                    kmt2.ExecuteNonQuery();

                    MessageBox.Show("Kota Değeri Güncellendi");

                    // TC varsa güncelle, yoksa ekle
                    OleDbCommand kontrolCmd = new OleDbCommand(
                        "SELECT COUNT(*) FROM AlimBilgileri WHERE TC = ?", baglan);
                    kontrolCmd.Parameters.AddWithValue("?", kullaniciTC);

                    int kayitVarMi = (int)kontrolCmd.ExecuteScalar();

                    if (kayitVarMi > 0)
                    {
                        // güncelle
                        OleDbCommand updateCmd = new OleDbCommand(@"
                    UPDATE AlimBilgileri 
                    SET AdSoyad=?, TartilanMiktar=?, IslaklikFiresi=?, BrutAgirlik=?, NetMubaya=?, KalanKota=?
                    WHERE TC=?", baglan);

                        updateCmd.Parameters.AddWithValue("?", kullaniciAdSoyad);
                        updateCmd.Parameters.AddWithValue("?", kg);
                        updateCmd.Parameters.AddWithValue("?", islaklikfiresi);
                        updateCmd.Parameters.AddWithValue("?", BrutAgirlik);
                        updateCmd.Parameters.AddWithValue("?", netMubaya);
                        updateCmd.Parameters.AddWithValue("?", yeniKota);
                        updateCmd.Parameters.AddWithValue("?", kullaniciTC);

                        int sonucUpdate = updateCmd.ExecuteNonQuery();
                        if (sonucUpdate > 0)
                        {
                            MessageBox.Show("Alım bilgileri başarıyla güncellendi.");
                        }
                        else
                        {
                            MessageBox.Show("Alım bilgileri güncellenemedi.");
                        }
                    }
                    else
                    {
                        // ekle
                        OleDbCommand insertCmd = new OleDbCommand(
                          "INSERT INTO AlimBilgileri (TC, AdSoyad, TartilanMiktar, IslaklikFiresi, BrutAgirlik, NetMubaya, KalanKota) VALUES (?, ?, ?, ?, ?, ?, ?)", baglan);

                        insertCmd.Parameters.AddWithValue("?", kullaniciTC);
                        insertCmd.Parameters.AddWithValue("?", kullaniciAdSoyad);
                        insertCmd.Parameters.AddWithValue("?", kg);
                        insertCmd.Parameters.AddWithValue("?", islaklikfiresi);
                        insertCmd.Parameters.AddWithValue("?", BrutAgirlik);
                        insertCmd.Parameters.AddWithValue("?", netMubaya);
                        insertCmd.Parameters.AddWithValue("?", yeniKota);

                        int sonucInsert = insertCmd.ExecuteNonQuery();
                        if (sonucInsert > 0)
                        {
                            MessageBox.Show("Alım bilgileri başarıyla eklendi.");
                        }
                        else
                        {
                            MessageBox.Show("Alım bilgileri eklenemedi.");
                        }
                    }

                    LoadKullaniciAlimBilgileri();
                }
                else
                {
                    MessageBox.Show("Kayıt bulunamadı veya Kota sütunu boş.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglan.Close();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            groupBox2.Visible = false;  // başta gizli

            BezDarasi = int.Parse(textBox1.Text);
            kg = int.Parse(textBox2.Text);

            BrutAgirlik = kg - BezDarasi;
            textBox6.Text = BrutAgirlik.ToString();

            islaklikfiresi = BrutAgirlik * 0.1;
            textBox7.Text = islaklikfiresi.ToString();

            netMubaya = BrutAgirlik - islaklikfiresi;
            textBox8.Text = netMubaya.ToString();

            OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=" + Application.StartupPath + "\\CayTakipYönetimSistemi.accdb");

            try
            {
                baglan.Open();

                OleDbCommand kmt = new OleDbCommand("SELECT KalanKota FROM Kullanicilar WHERE TC = ?", baglan);
                kmt.Parameters.AddWithValue("?", kullaniciTC);

                object sonuc = kmt.ExecuteScalar();

                if (sonuc != null && sonuc != DBNull.Value)
                {
                    double kalanKotaDegeri = double.Parse(sonuc.ToString());
                    double yeniKota = kalanKotaDegeri - netMubaya;

                    if (yeniKota < 0)
                    {
                        MessageBox.Show("İşlem yapılamaz. Kalan kota yetersiz!");
                        return;
                    }

                    groupBox2.Visible = true;  // kota yeterli olduğunda göster
                    textBox9.Text = yeniKota.ToString();

                    OleDbCommand kmt2 = new OleDbCommand("UPDATE Kullanicilar SET KalanKota = ? WHERE TC = ?", baglan);
                    kmt2.Parameters.AddWithValue("?", yeniKota);
                    kmt2.Parameters.AddWithValue("?", kullaniciTC);
                    kmt2.ExecuteNonQuery();

                    MessageBox.Show("Kota Değeri Güncellendi");

                    // TC varsa güncelle yoksa ekle
                    OleDbCommand kontrolCmd = new OleDbCommand(
                        "SELECT COUNT(*) FROM AlimBilgileri WHERE TC = ?", baglan);
                    kontrolCmd.Parameters.AddWithValue("?", kullaniciTC);

                    int kayitVarMi = (int)kontrolCmd.ExecuteScalar();

                    if (kayitVarMi > 0)
                    {
                        // güncelle
                        OleDbCommand updateCmd = new OleDbCommand(@"
            UPDATE AlimBilgileri 
            SET AdSoyad=?, TartilanMiktar=?, IslaklikFiresi=?, BrutAgirlik=?, NetMubaya=?, KalanKota=?
            WHERE TC=?", baglan);

                        updateCmd.Parameters.AddWithValue("?", kullaniciAdSoyad);
                        updateCmd.Parameters.AddWithValue("?", kg);
                        updateCmd.Parameters.AddWithValue("?", islaklikfiresi);
                        updateCmd.Parameters.AddWithValue("?", BrutAgirlik);
                        updateCmd.Parameters.AddWithValue("?", netMubaya);
                        updateCmd.Parameters.AddWithValue("?", yeniKota);
                        updateCmd.Parameters.AddWithValue("?", kullaniciTC);

                        int sonucUpdate = updateCmd.ExecuteNonQuery();
                        if (sonucUpdate > 0)
                        {
                            MessageBox.Show("Alım bilgileri başarıyla güncellendi.");
                        }
                        else
                        {
                            MessageBox.Show("Alım bilgileri güncellenemedi.");
                        }
                    }
                    else
                    {
                        // ekle
                        OleDbCommand insertCmd = new OleDbCommand(
                            "INSERT INTO AlimBilgileri (TC, AdSoyad, TartilanMiktar, IslaklikFiresi, BrutAgirlik, NetMubaya, KalanKota) VALUES (?, ?, ?, ?, ?, ?, ?)", baglan);

                        insertCmd.Parameters.AddWithValue("?", kullaniciTC);
                        insertCmd.Parameters.AddWithValue("?", kullaniciAdSoyad);
                        insertCmd.Parameters.AddWithValue("?", kg);
                        insertCmd.Parameters.AddWithValue("?", islaklikfiresi);
                        insertCmd.Parameters.AddWithValue("?", BrutAgirlik);
                        insertCmd.Parameters.AddWithValue("?", netMubaya);
                        insertCmd.Parameters.AddWithValue("?", yeniKota);

                        int sonucInsert = insertCmd.ExecuteNonQuery();
                        if (sonucInsert > 0)
                        {
                            MessageBox.Show("Alım bilgileri başarıyla eklendi.");
                        }
                        else
                        {
                            MessageBox.Show("Alım bilgileri eklenemedi.");
                        }
                    }

                    LoadKullaniciAlimBilgileri();
                }
                else
                {
                    MessageBox.Show("Kayıt bulunamadı veya Kota sütunu boş.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglan.Close();
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox1.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            button1.Visible = false;
            button3.Visible = false;
            groupBox2.Visible = false;
            groupBox1.Visible = false;
        }

        

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
