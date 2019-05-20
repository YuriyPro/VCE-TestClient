using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsForms_TestClient.Classes;

namespace WindowsForms_TestClient.Forms
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Register.ActiveForm.Hide();
            Entry en=new Entry();
            en.ShowDialog();
            Close();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (MyContext context = new MyContext())
            {
                //Status status = new Status()
                //{
                //    Name = "Students"
                //};
                //context.Status.Add(status);
                //context.SaveChanges();

                User person = new User()
                {
                    Name = textBox1.Text,
                    Password = Additional.CreateMD5Hash(textBox2.Text),
                    StatusId = context.Status.Where(x => x.Name == "Students").Select(x => x.Id).Single()

                };
                context.Users.Add(person);
                context.SaveChanges();
                label3.Visible = true;
            }
        }
    }
}
