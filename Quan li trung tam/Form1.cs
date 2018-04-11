using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Quan_li_trung_tam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string strConn = "Server=THAITHANG-PC;database=CSDL_QLTrungTam;User id=sa;pwd=thaithang1";
        SqlConnection connTT = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadTrungTamLenTreeView();
        }
        SqlCommand CreateCommand(string sql, SqlConnection conn)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Connection = conn;
            return command;
        }
        private void LoadTrungTamLenTreeView()
        {

            OpenConnection(ref connTT);
            SqlCommand commandTT = CreateCommand("select * from TRUNGTAM", connTT);

            SqlDataReader readerTT = commandTT.ExecuteReader();
            tvTrungTam.Nodes.Clear();
            while (readerTT.Read())
            {
                string matt = readerTT.GetString(0);
                string tentt = readerTT.GetString(1);
                TreeNode nodeTT = new TreeNode(tentt);
                nodeTT.Tag = matt;

                tvTrungTam.Nodes.Add(nodeTT);

                SqlConnection connLop = null;
                OpenConnection(ref connLop);
                SqlCommand commandLop = CreateCommand("select * from LOP where MaTT ='" + matt + "'", connLop);
                SqlDataReader readerLop = commandLop.ExecuteReader();
                while (readerLop.Read())
                {
                    string malop = readerLop.GetString(0);
                    string tenlop = readerLop.GetString(1);

                    TreeNode nodeLop = new TreeNode(tenlop);
                    nodeLop.Tag = malop;
                    nodeTT.Nodes.Add(nodeLop);

                    SqlConnection connSV = null;
                    OpenConnection(ref connSV);
                    SqlCommand commandSV = CreateCommand("select * from SINHVIEN where MaLop ='" + malop + "'", connSV);
                    SqlDataReader readerSV = commandSV.ExecuteReader();
                    while (readerSV.Read())
                    {
                        string masv = readerSV.GetString(0);
                        string tensv = readerSV.GetString(1);

                        TreeNode nodeSV = new TreeNode(tensv);
                        nodeSV.Tag = masv;
                        nodeLop.Nodes.Add(nodeSV);
                    }
                    readerSV.Close();
                    CloseConnection(ref connSV);
                }
                readerLop.Close();
                CloseConnection(ref connLop);
            }
            readerTT.Close();
            CloseConnection(ref connTT);
        }

        private void OpenConnection(ref SqlConnection conn)
        {
            if (conn == null)
                conn = new SqlConnection(strConn);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }
        private void CloseConnection(ref SqlConnection conn)
        {
            if (conn != null && conn.State == ConnectionState.Open)
                conn.Close();
        }

        private void tvTrungTam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                if (e.Node.Level == 0)//nhấn vào nút trung tâm
                {
                    string matt = e.Node.Tag.ToString();
                    HienThiLopLenListView(matt);
                }
                else if( e.Node.Level == 1)
                {
                    string malop = e.Node.Tag.ToString();
                    HienThiSinhVienLenListView(malop);
                }
            }
        }

        private void HienThiSinhVienLenListView(string malop)
        {
            lvData.Items.Clear();
            lvData.Columns.Clear();
            lvData.Columns.Add("Mã SV", 100);
            lvData.Columns.Add("Tên SV", 250);
            lvData.Columns.Add("SĐT", 150);

            SqlConnection conn = null;
            OpenConnection(ref conn);
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from SINHVIEN where MaLop ='" + malop + "'";
            command.Connection = conn;

            SqlDataReader readerSV = command.ExecuteReader();
            while (readerSV.Read())
            {
                string masv = readerSV.GetString(0);
                string tensv = readerSV.GetString(1);
                string sdt = readerSV.GetString(2);

                ListViewItem lvi = new ListViewItem(masv);
                lvi.SubItems.Add(tensv);
                lvi.SubItems.Add(sdt);

                lvData.Items.Add(lvi);
            }
            readerSV.Close();
        }

        private void HienThiLopLenListView(string matt)
        {
            lvData.Items.Clear();
            lvData.Columns.Clear();
            lvData.Columns.Add("Mã Lớp", 100);
            lvData.Columns.Add("Tên Lớp", 300);

            SqlConnection conn = null;
            OpenConnection(ref conn);
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from LOP where MaTT ='" + matt + "'";
            command.Connection = conn;

            SqlDataReader readerLop = command.ExecuteReader();
            while (readerLop.Read())
            {
                string malop = readerLop.GetString(0);
                string tenlop = readerLop.GetString(1);

                ListViewItem lvi = new ListViewItem(malop);
                lvi.SubItems.Add(tenlop);

                lvData.Items.Add(lvi);
            }
            readerLop.Close();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            lvData.Items.Clear();
            lvData.Columns.Clear();
            lvData.Columns.Add("Mã SV", 100);
            lvData.Columns.Add("Tên SV", 250);
            lvData.Columns.Add("SĐT", 150);

            SqlConnection conn = null;
            OpenConnection(ref conn);
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from SINHVIEN where TenSV like @ten";
            command.Connection = conn;

            command.Parameters.Add("@ten", SqlDbType.NVarChar).Value = "%"+txtSearchSV.Text+"%";

            SqlDataReader readerSV = command.ExecuteReader();
            while (readerSV.Read())
            {
                string masv = readerSV.GetString(0);
                string tensv = readerSV.GetString(1);
                string sdt = readerSV.GetString(2);

                ListViewItem lvi = new ListViewItem(masv);
                lvi.SubItems.Add(tensv);
                lvi.SubItems.Add(sdt);

                lvData.Items.Add(lvi);
            }
            readerSV.Close();
        }
    }
}
