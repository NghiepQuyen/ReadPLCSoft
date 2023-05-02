using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;

namespace ReadPLCSoft
{
    public partial class Form1 : Form
    {
        System.Data.DataTable dt_data = new System.Data.DataTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_action_Click(object sender, EventArgs e)
        {
            //ushort[] data; // Mảng lưu trữ các giá trị thanh ghi
            int[] data; // Mảng lưu trữ các giá trị thanh ghi
            string g_ipAddress = tb_ipAddress.Text.Trim();

            // Kiểm tra địa chỉ IP
            if (!IPAddress.TryParse(g_ipAddress, out IPAddress ipAddress))
            {
                MessageBox.Show("Địa chỉ IP không hợp lệ");
                return;
            }

            // Khởi tạo đối tượng ModbusClient
            ModbusClient modbusClient = new ModbusClient(g_ipAddress, 502);

            try
            {
                // Kết nối tới thiết bị
                modbusClient.Connect();

                // Đọc các thanh ghi từ thiết bị
                int startingAddress = 0; // Địa chỉ bắt đầu của thanh ghi
                int quantity = 10; // Số lượng thanh ghi cần đọc

                data = modbusClient.ReadHoldingRegisters(startingAddress, quantity); // Đọc các thanh ghi từ thiết bị và lưu vào mảng 'data'

                // Kiểm tra độ dài của mảng data
                if (data.Length != quantity)
                {
                    MessageBox.Show("Lỗi đọc dữ liệu từ thiết bị");
                    return;
                }

                // In ra giá trị của các thanh ghi
                for (int i = 0; i < quantity; i++)
                {
                    //Console.WriteLine("Giá trị thanh ghi " + (startingAddress + i) + ": " + data[i]);
                    DataRow row = dt_data.NewRow();
                    row["Col1"] = startingAddress + i;
                    row["Col2"] = data[i];
                    dt_data.Rows.Add(row);
                }

                dt_data.DefaultView.Sort = "Col1";
                dgv_main.DataSource = dt_data.DefaultView.ToTable();
            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                {
                    MessageBox.Show("Không thể kết nối tới thiết bị");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
            finally
            {
                // Đóng kết nối với thiết bị
                modbusClient.Disconnect();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dt_data.Clear();
            dt_data.Columns.Add(new DataColumn("Col1", typeof(String)));
            dt_data.Columns.Add(new DataColumn("Col2", typeof(String)));
        }
    }
}
