using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace StokGudang
{
    public partial class Form1 : Form
    {
        List<Kardus> listKardus = new List<Kardus>();
        public int kardusID = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            RefreshDataGrid();
        }

        private void SaveData()
        {
            if (File.Exists("data.csv"))
                File.Delete("data.csv");
            StreamWriter sw = new StreamWriter("data.csv");
            sw.WriteLine("#kardusID, kardusNama, kardusBerat, kardusIsi, kardusKadaluarsa");
            foreach (Kardus getKardus in listKardus)
                sw.WriteLine(getKardus.ID.ToString()+","+ getKardus.Nama.ToString() + "," + getKardus.Berat.ToString() + "," + getKardus.Isi.ToString() + "," + getKardus.Kadaluarsa.ToString() + "");
            sw.Close();
        }

        private void LoadData()
        {
            if (File.Exists("data.csv"))
            {
                StreamReader sr = new StreamReader("data.csv");
                string line = sr.ReadLine();
                while (line != null)
                {
                    if (!line.Contains("#"))
                    {
                        string[] strSplit = line.Split(',');
                        int id = int.Parse(strSplit[0]);
                        string nama = strSplit[1];
                        int berat = int.Parse(strSplit[2]);
                        int isi = int.Parse(strSplit[3]);
                        DateTime kadaluarsa = DateTime.Parse(strSplit[4]);
                        Kardus newKardus = new Kardus();
                        newKardus.IsiKardus(id, nama, berat, isi, kadaluarsa);
                        listKardus.Add(newKardus);
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
            }
        }

        private void RefreshDataGridKadaluarsa()
        {
            dataGridView1.Rows.Clear();
            foreach (Kardus getKardus in listKardus)
            {
                if ((getKardus.Kadaluarsa - DateTime.Now).TotalDays <= 30)
                {
                    string[] newRow = { "", "", "", "", "" };
                    newRow[0] = getKardus.ID.ToString();
                    newRow[1] = getKardus.Nama;
                    newRow[2] = getKardus.Berat.ToString();
                    newRow[3] = getKardus.Isi.ToString();
                    newRow[4] = getKardus.Kadaluarsa.ToString();
                    dataGridView1.Rows.Add(newRow);
                }
            }
        }

        private void RefreshDataGrid()
        {
            dataGridView1.Rows.Clear();
            foreach (Kardus getKardus in listKardus)
            {
                string[] newRow = { "", "", "", "", "" };
                newRow[0] = getKardus.ID.ToString();
                newRow[1] = getKardus.Nama;
                newRow[2] = getKardus.Berat.ToString();
                newRow[3] = getKardus.Isi.ToString();
                newRow[4] = getKardus.Kadaluarsa.ToString();
                dataGridView1.Rows.Add(newRow);
            }
        }

        private int GetFreeID()
        {
            int nowID = 0;
            while (true)
            {
                bool adaYgSama = false;
                foreach (Kardus checkKardus in listKardus)
                {
                    if (checkKardus.ID == nowID)
                        adaYgSama = true;
                }
                if (adaYgSama)
                    nowID += 1;
                else
                    break;
            }
            return nowID;
        }

        private void btnTambahDus_Click(object sender, EventArgs e)
        {
            Kardus kardusBaru = new Kardus();
            kardusID = GetFreeID();
            kardusBaru.IsiKardus(kardusID, txtNamaDus.Text, (int)numBeratDus.Value, (int)numIsiDus.Value, dateTimePicker1.Value);
            listKardus.Add(kardusBaru);
            RefreshDataGrid();
            SaveData();
        }

        private Kardus SelectKardus()
        {
            int getID = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i += 1)
            {
                if (dataGridView1.Rows[i].Selected)
                {
                    getID = int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    break;
                }
            }
            Kardus getKardus = new Kardus();
            foreach (Kardus checkKardus in listKardus)
            {
                if (checkKardus.ID == getID)
                    getKardus = checkKardus;
            }
            return getKardus;
        }

        private void btnHapusDus_Click(object sender, EventArgs e)
        {
            Kardus getKardus = SelectKardus();
            if (listKardus.Contains(getKardus))
                listKardus.Remove(getKardus);
            RefreshDataGrid();
            SaveData();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Kardus getKardus = SelectKardus();
            groupBox3.Enabled = true;
            txtEditDusNama.Text = getKardus.Nama;
            numEditDusBerat.Value = getKardus.Berat;
            numEditDusIsi.Value = getKardus.Isi;
            dateTimePicker2.Value = getKardus.Kadaluarsa;
        }

        private void btnEditDus_Click(object sender, EventArgs e)
        {
            Kardus getKardus = SelectKardus();
            getKardus.EditKardus(txtEditDusNama.Text, (int)numEditDusBerat.Value, (int)numEditDusIsi.Value, dateTimePicker2.Value);
            RefreshDataGrid();
            SaveData();
            groupBox3.Enabled = false;
        }

        private void checkBoxKadaluarsa_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxKadaluarsa.Checked)
                RefreshDataGridKadaluarsa();
            else
                RefreshDataGrid();
        }
    }
}
