using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Task_Manager.TaskManager.Data;

namespace Task_Manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void bEdit_Click(object sender, EventArgs e)
        {

        }

        private void dgTasks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlLiteRepository repo = new SqlLiteRepository();

            dgTasks.DataSource = repo.GetAll().ToList();
        }
    }
}
