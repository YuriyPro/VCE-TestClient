using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsForms_TestClient.Classes;
using WindowsForms_TestClient.Forms;

namespace WindowsForms_TestClient
{
    public partial class Entry : Form
    {
        public Entry()
        {
            InitializeComponent();
        }
        //Registeration
        private void label3_Click(object sender, EventArgs e)
        {
            Entry.ActiveForm.Hide();
            Register r=new Register();
            r.ShowDialog();
            Close();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text))
            {
                using (MyContext context = new MyContext())
                {
                    string str = Additional.CreateMD5Hash(textBox2.Text);
                    User user = context.Users.Where(x => x.Name == textBox1.Text && x.Password == str && x.Status.Name == "Students").SingleOrDefault();

                    if (user != null)
                    {
                        MessageBox.Show("Hi!!!");

                        Entry.ActiveForm.Hide();
                        Work w = new Work();
                        w.ShowDialog();
                        Close();
                    }
                    else MessageBox.Show("Login or Password is incorect");

                }
            }
        }
    }
}
