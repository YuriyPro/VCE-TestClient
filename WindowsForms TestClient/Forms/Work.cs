using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using WindowsForms_TestClient.Classes;

namespace WindowsForms_TestClient.Forms
{
    [Serializable]
   public partial class Work : Form
    {
        private Socket clientSocket;

        Dictionary<RadioButton, TextBox> answersDictionary = new Dictionary<RadioButton, TextBox>(); //кнопка - заппитання
        List<Question> questList = new List<Question>(); //список запитань та відповідей
        List<Answer>answersList=new List<Answer>();

        private SomeTest testClass;
        private Question questionClass;
        private Answer answerClass;

        // The thread in which the file will be received
        private Thread thrDownload;
        // The stream for writing the file to the hard-drive
        private Stream strLocal;
        // The network stream that receives the file
        private NetworkStream strRemote;
        // The TCP listener that will listen for connections
        private TcpListener tlsServer;
        // Delegate for updating the logging textbox
        private delegate void UpdateStatusCallback(string StatusMessage);
        // Delegate for updating the progressbar
        private delegate void UpdateProgressCallback(Int64 BytesRead, Int64 TotalBytes);
        // For storing the progress in percentages
        private static int PercentProgress;
        int counter = 0;
        private float mark = 0,temp=0;
        private RadioButton r = new RadioButton() ;
        //private RadioButton [] rm = new RadioButton[9];
        public Work()
        {
            InitializeComponent();
            timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
         
        

            using (var db = new MyContext())
            {
                var rez = db.Tests.ToList();  
                foreach (var i in rez) 
                {
                    listBox2.Items.Add( i.PathToFile);
                }
               db.Dispose();
            }


        }
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            int rez = questList.Count * 10; //120->2 min
            double a = (rez * 0.8);
            double b = (rez * 0.2);
            if (counter <a )
            {
                // Run your procedure here.  
                // Increment counter.
                label7.Visible = true;
                counter = counter + 1;
                label7.Text = "Test Run: " + counter.ToString();
            }
            if (counter > a)
            {
                label7.ForeColor = Color.Red;
                counter = counter + 1;
                label7.Text = "Test Run: " + counter.ToString();
            }

            if (counter == rez * 1)
            {
                // Exit loop code.  
                timer1.Enabled = false;
                counter = 0;
                label7.Text = "Test Close !!!";
                listBox1.Items.Clear();
                button2.Visible = false;
                float finalMark = mark;
                label9.Text = "Your final mark : " + finalMark;
                using (MyContext context = new MyContext())
                {
                    TestResult tr = new TestResult()
                    {
                        DateTest = DateTime.Now,
                        Mark = finalMark,
                        UserId = TakeUsersId.TakeId,
                        TestId = TakeFileId.FileId
                    };
                    context.TestResults.Add(tr);
                    context.SaveChanges();
                    context.Dispose();
                }

                //TakeUsersId.TakeId = 0;
                //TakeFileId.FileId = 0;
            }
        }
        
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listBox2.SelectedIndex == -1) return;
            string w = this.listBox2.SelectedItem.ToString();
            using (MyContext context = new MyContext())
            {

                int rez = context.Tests.Where(x => x.PathToFile == w).Select(x => x.Id).Single();
                TakeFileId.FileId = rez;
                context.Dispose();
            }
            
            XmlSerializer serializer = new XmlSerializer(typeof(SomeTest));

            StreamReader reader = new StreamReader(w);
           
             testClass = (SomeTest) serializer.Deserialize(reader);
            foreach (var i in testClass.quest)
            {
              questList.Add(i);    
            }
            textBox2.Text =testClass.TestName;
            textBox3.Text =testClass.Subject;
            textBox4.Text =testClass.Author;
                                     
            listBox1.Items.Clear();
            foreach (var i in testClass.quest)
            {
                listBox1.Items.Add(i.Title);
            }

            

            listBox2.Visible = false;
            reader.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Run this procedure in an appropriate event.  
            //counter = 0;
            timer1.Interval = 1000;
            timer1.Enabled = true;
            // Hook up timer's tick event handler.  
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //

            if (listBox1.SelectedIndex == -1) return;
            timer1.Start();
            var qustList = new List<Question> {questList[listBox1.SelectedIndex]};
            answersDictionary.Clear();
            
            foreach (var q in qustList)
            {
                label3.Text = q.Title;
                textBox1.Text = q.Text;
                
                int x = 0, y = 20, a = 0;
                do
               {
                   foreach (var item in q.answers)
                   {
                       


                           a++;
                           temp = item.Weight;
                          
                            RadioButton r = new RadioButton(); //створ радіокнопку 
                            r.Location = new Point(x = 5, y); //координати
                            r.Parent = groupBox1; //щоб відображались на полі drodBox

                            
                            TextBox t = new TextBox(); //створюєм текстове поле
                            t.Location = new Point(x = 110, y); //задаємо йому кординати
                            t.Size = new Size(300, 30); //розмір
                            t.Parent = groupBox1; //добавляємо батька щоб текстове поле було над drobBox
                            t.Text = (item.Text);
                            t.ReadOnly = true;
                            y += 30; //перша точка-0,2-га - 30,3-тя- 60,4-та- 90,,,

                            answersDictionary.Add(r, t); //колекція полів збережеться в Dictionary
                        
                          
                       
                   }
               } while (q.answers.Count != a);

            }
            
        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            label3.Text = "Question";
            textBox1.Text = "";

            var qustList = new List<Question> { questList[listBox1.SelectedIndex] };
            int count = answersDictionary.Count;
            foreach (var ql in qustList)
            {
                for (int i = 0; i < count; i++)
                 {
               
                    if (ql.answers[i].Correct ==(this. r.Checked==true))
                    {
                        mark += temp;
                        label9.Text = mark.ToString();
                        temp = 0;

                    }

                    if (mark > 100)
                    {
                        float finalMark = mark;
                        label9.Text = "Your final mark : " + finalMark;
                        using (MyContext context = new MyContext())
                        {
                            TestResult tr = new TestResult()
                            {
                                DateTest = DateTime.Now,
                                Mark = finalMark,
                                UserId = TakeUsersId.TakeId,
                                TestId = TakeFileId.FileId
                            };
                            context.TestResults.Add(tr);
                            context.SaveChanges();
                            context.Dispose();
                        }

                        //TakeUsersId.TakeId = 0;
                        //TakeFileId.FileId = 0;
                    }
                    else
                    {
                        mark += 0;
                        i = count;
                    }
                    groupBox1.Controls.Clear();
                }
            }
            answersDictionary.Clear();
        }
        private void CreateConnect()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPHostEntry iPHost = Dns.Resolve(textBox2.Text); //Dns.GetHostEntry("localhost"); //(textBox2.Text)
                IPAddress iPAddress = iPHost.AddressList[0];
                IPEndPoint iPEnd = new IPEndPoint(iPAddress, 5555);
                clientSocket.Connect(iPEnd);
                Thread ListenThread = new Thread(ListenThreadFunction);
                ListenThread.IsBackground = true;
                ListenThread.Start(clientSocket);
            }
            catch (Exception e)
            {
                MessageBox.Show("No Conection!!! Try again !");
            }
            
        }
        //Button Take Message
        private void button4_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = true;
            label2.Visible = true;
            textBox5.Visible = true;
            textBox6.Visible = true;
            CreateConnect();
        }
        private void ListenThreadFunction(Object obj)
        {
            if (obj != null)
            {
                Socket s = obj as Socket;
                while (true)
                {
                    try
                    {
                        byte[] bytes = new byte[1024];
                        int count = s.Receive(bytes);
                        string str = Encoding.ASCII.GetString(bytes, 0, count);
                        //MessageBox.Show(str);
                        textBox6.Invoke(new Action(() => { textBox6.Text = str; }));
                    }
                    catch (Exception e)
                    {
                        textBox6.Invoke(new Action(() => { textBox6.Text = "Connection Close"; }));
                    }
                    
                }
            }
        }
        //Button Exit from message
        private void button6_Click(object sender, EventArgs e)
        {
            textBox6.Text = "";
            clientSocket.Close();
            button1.Visible = true;
            button6.Visible = false;
            label2.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
        }
        //Button Online Options
        private void button1_Click(object sender, EventArgs e)
        {
            button4.Visible = true;
            button5.Visible = true;
        }
        //Button Take File
        private void button5_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            lblPort.Visible = true;
            txtPort.Visible = true;
            txtLog.Visible = true;
            prgDownload.Visible = true;
            btnStop.Visible = true;
            thrDownload = new Thread(StartReceiving);
            thrDownload.Start();
        }
        private void StartReceiving()
        {

            // There are many lines in here that can cause an exception
            try
            {
                // Get the hostname of the current computer (the server)
                string hstServer = Dns.GetHostName();
                // Get the IP of the first network device, however this can prove unreliable on certain configurations
                IPAddress ipaLocal = Dns.GetHostEntry(hstServer).AddressList[1];
                // If the TCP listener object was not created before, create it
                if (tlsServer == null)
                {
                    // Create the TCP listener object using the IP of the server and the specified port
                    tlsServer = new TcpListener(ipaLocal, Convert.ToInt32(txtPort.Text));
                }
                // Write the status to the log textBox on the form (txtLog)
                this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "Starting the server...\r\n" });
                // Start the TCP listener and listen for connections
                tlsServer.Start();
                // Write the status to the log textBox on the form (txtLog)
                this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "The server has started. Please connect the client to " + ipaLocal.ToString() + "\r\n" });
                // Accept a pending connection
                TcpClient tclServer = tlsServer.AcceptTcpClient();
                // Write the status to the log textBox on the form (txtLog)
                this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "The server has accepted the client\r\n" });
                // Receive the stream and store it in a NetworkStream object
                strRemote = tclServer.GetStream();
                // Write the status to the log textBox on the form (txtLog)
                this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "The server has received the stream\r\n" });

                // For holding the number of bytes we are reading at one time from the stream
                int bytesSize = 0;

                // The buffer that holds the data received from the client
                byte[] downBuffer = new byte[1024];
                // Read the first buffer (2048 bytes) from the stream - which represents the file name
                bytesSize = strRemote.Read(downBuffer, 0, 1024);
                // Convert the stream to string and store the file name
                string FileName = System.Text.Encoding.ASCII.GetString(downBuffer, 0, bytesSize);
                // Set the file stream to the path D:\ plus the name of the file that was on the sender's computer
                strLocal = new FileStream(@"D:\TestFromTeacher" + FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                // The buffer that holds the data received from the client
                downBuffer = new byte[1024];
                // Read the next buffer (2048 bytes) from the stream - which represents the file size
                bytesSize = strRemote.Read(downBuffer, 0,1024);
                // Convert the file size from bytes to string and then to long (Int64)
                long FileSize = Convert.ToInt64(System.Text.Encoding.ASCII.GetString(downBuffer, 0, bytesSize));

                // Write the status to the log textBox on the form (txtLog)
                this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "Receiving file " + FileName + " (" + FileSize + " bytes)\r\n" });

                // The buffer size for receiving the file
                downBuffer = new byte[1024];

                // From now on we read everything that's in the stream's buffer because the file content has started
                while ((bytesSize = strRemote.Read(downBuffer, 0, downBuffer.Length)) > 0)
                {
                    // Write the data to the local file stream
                    strLocal.Write(downBuffer, 0, bytesSize);
                    // Update the progressbar by passing the file size and how much we downloaded so far to UpdateProgress()
                    this.Invoke(new UpdateProgressCallback(this.UpdateProgress), new object[] { strLocal.Length, FileSize });
                }
                // When this point is reached, the file has been received and stored successfuly

            }
            finally
            {
                // This part of the method will fire no matter weather an error occured in the above code or not

                // Write the status to the log textBox on the form (txtLog)
                this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "The file was received. Closing streams.\r\n" });

                // Close the streams
                strLocal.Close();
                strRemote.Close();

                // Write the status to the log textBox on the form (txtLog)
                this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "Streams are now closed.\r\n" });

                // Start the server (TCP listener) all over again
                StartReceiving();
            }
        }
        private void UpdateStatus(string StatusMessage)
        {
            // Append the status to the log textBox text 
            txtLog.Text += StatusMessage;
        }
        private void UpdateProgress(Int64 BytesRead, Int64 TotalBytes)
        {
            if (TotalBytes > 0)
            {
                // Calculate the download progress in percentages
                PercentProgress = Convert.ToInt32((BytesRead * 100) / TotalBytes);
                // Make progress on the progress bar
                prgDownload.Value = PercentProgress;
            }
            else
                prgDownload.Value = 0;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            txtLog.Text += "Streams are now closed.\r\n";
            prgDownload.Value = 0;
            strLocal.Close();
            strRemote.Close();
            
            txtLog.Text = "";
            button1.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            lblPort.Visible = false;
            txtPort.Visible = false;
            txtLog.Visible = false;
            prgDownload.Visible = false;
            btnStop.Visible = false;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
