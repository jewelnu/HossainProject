using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace NU_Solver
{
    public partial class upload : Form
    {
        //private string folder = "";

        string folder = @"D:\Scanned\";
        public upload()
        {
            InitializeComponent();
            
        }

        private void upload_Click(object sender, EventArgs e)
        {
            label3.Text = "Processing file: ";
            this.Refresh();
            DirectoryInfo dr = new DirectoryInfo(folder);

            for (int i = 0; i < dr.GetFiles().Length; i++)
            {
                //this.listBox1.Items.Add(dr.GetFiles()[i].Name);
                
                uploadfile(dr.GetFiles()[i].Name);
                
                progressBar1.Maximum = dr.GetFiles().Length;

                if (progressBar1.Value != dr.GetFiles().Length)
                {
                    progressBar1.Value++;
                }
                this.Refresh();
            }
            file_load_listview();
            label2.Text = "Finished...";
            MessageBox.Show("File(s) upload finished","Upload", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //this.Close();
        }
        private void uploadfile(string filename)
        {
            string fullpath = folder + filename;
            string sub_code = "";
            int tot_rec = File.ReadAllLines(fullpath).Length;
            int file_size = 0;
            lblCurrentFile.Text = fullpath.ToString();

            FileInfo info = new FileInfo(fullpath);
            long value =info.Length;
            file_size=Convert.ToInt32(value / 1024.0); // for kb

            //MessageBox.Show(file_size.ToString());
            //MessageBox.Show(tot_rec.ToString());
            if (filename.Length >= 7)
                sub_code = "31" + filename.Substring(1, 4);
            string filename_insert = String.Format("INSERT INTO dbo.[file_list] ([file_name],[pap_code],[insert_date],[rows],[file_size]) values('{0}','{1}', CURRENT_TIMESTAMP,'{2}','{3}')", filename, sub_code, tot_rec, file_size);
            //this.textBox1.Text = this.textBox1.Text + filename_insert;
            //this.richTextBox1.Text = this.richTextBox1.Text +"\n"+ filename_insert;
            //return;
            try
            {
                SqlConnection con = Database.GetConnectionObj();
                if (con == null) throw new Exception("Can't create and open a connection");
                SqlCommand cmd = new SqlCommand(filename_insert, con);
                SqlDataReader reader = cmd.ExecuteReader();

                //listBox1.Items.Add(filename.ToString());
                this.richTextBox1.Text = filename + " Uploaded.\n" + this.richTextBox1.Text;
                SqlConnection con2 = Database.GetConnectionObj();
                StreamReader sr = new StreamReader(fullpath);

                int sl = 0;
                while (sr.EndOfStream == false)
                {
                    sl++;
                    string line = sr.ReadLine();
                    //sl = Int32.Parse(line.Substring(0,10));
                    string insert_line = string.Format(
                        "INSERT INTO [dbo].[E_solving] " +
                        "([file_name] " +
                        ",[scanned_row]" +
                        ",[serial]) VALUES ('{0}','{1}','{2}')",
                        filename, line, sl);
                        //richTextBox1.AppendText(filename);


                    //esolve es=new esolve();
                    //espan(string username, string filename, string sub_code)
                    
                    //upload level span
                    //es.espan(filename, sub_code, "");
                    
                    int tr = 0;
                Back:
                    tr++;
                    try
                    {
                        SqlCommand cmd1 = new SqlCommand(insert_line, con2);
                        cmd1.ExecuteNonQuery();
                        //SqlDataReader reader1 = cmd1.ExecuteReader();                        
                    }
                    catch (Exception ee)
                    {
                        if (tr < 100)
                            goto Back;
                        else
                        {
                            this.richTextBox1.Text = insert_line + "\n" + this.richTextBox1.Text;

                        }
                    }
                    StreamWriter lg = new StreamWriter("log.log");
                    lg.Write(this.richTextBox1.Text.ToString());
                    lg.Dispose();
                }
                sr.Dispose();
                //con2.Close();
                con.Close();
                con.Dispose();
            }
            catch (Exception ee)
            {
                this.richTextBox1.Text = filename + " Failed to Upload! May duplicate file name, already exissts.\n" + this.richTextBox1.Text;
            }
        }

        private void exit(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dirsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            folder = e.Node.FullPath;
            MessageBox.Show(e.Node.FullPath);
            //MessageBox.Show(e.Node.Text);
        }

        private void dirsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                if (e.Node.Nodes[0].Text == "..." && e.Node.Nodes[0].Tag == null)
                {
                    e.Node.Nodes.Clear();

                    //get the list of sub direcotires
                    string[] dirs = Directory.GetDirectories(e.Node.Tag.ToString());

                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        TreeNode node = new TreeNode(di.Name, 0, 1);

                        try
                        {
                            //keep the directory's full path in the tag for use later
                            node.Tag = dir;

                            //if the directory has sub directories add the place holder
                            
                            //if (di.GetDirectories().Count() > 0)
                                node.Nodes.Add(null, "...", 0, 0);
                              
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //display a locked folder icon
                            node.ImageIndex = 12;
                            node.SelectedImageIndex = 12;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "DirectoryLister",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);
                        }
                    }
                }
            }
        }

        private void upload_Load(object sender, EventArgs e)
        {
            //get a list of the drives
            string[] drives = Environment.GetLogicalDrives();

            foreach (string drive in drives)
            {
                DriveInfo di = new DriveInfo(drive);
                int driveImage;

                switch (di.DriveType)    //set the drive's icon
                {
                    case DriveType.CDRom:
                        driveImage = 3;
                        break;
                    case DriveType.Network:
                        driveImage = 6;
                        break;
                    case DriveType.NoRootDirectory:
                        driveImage = 8;
                        break;
                    case DriveType.Unknown:
                        driveImage = 8;
                        break;
                    default:
                        driveImage = 2;
                        break;
                }

                TreeNode node = new TreeNode(drive.Substring(0, 1), driveImage, driveImage);
                node.Tag = drive;

                if (di.IsReady == true)
                    node.Nodes.Add("...");

                dirsTreeView.Nodes.Add(node);
            }            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = Database.GetConnectionObj();
                if (con == null) throw new Exception("Can't create and open a connection");
                SqlCommand cmd = new SqlCommand("", con);
                
                //cmd.CommandText = ("SELECT file_name, pap_code FROM dbo.file_list WHERE span_stat is null OR span_stat=''");
                cmd.CommandText = ("SELECT file_name, pap_code, rows FROM dbo.file_list");
                //MessageBox.Show(cmd.CommandText);
                int cnt=6;
                
                //progressBar2.Maximum = cnt;

                SqlDataReader reader = cmd.ExecuteReader();
                
                progressBar2.Maximum = cnt;
                //MessageBox.Show(cnt.ToString());
                
                while (reader.Read())
                {
                    string sub_code = "";
                    string filename = "";
                    int noofrows = 0;
                    filename = reader[0].ToString();
                    sub_code = reader[1].ToString();
                    noofrows = Convert.ToInt32(reader[2]);
                    
                    //to span file
                    lblspanfile.Text = filename;
                    esolve es = new esolve();
                    es.espan(filename, sub_code, "");
                    //cnt++;
                    if (progressBar2.Value != cnt)
                    {
                        progressBar2.Value++;
                    }
                    // Clears all the items.
                    listView1.Items.Clear();
                    file_load_listview();
                    this.Refresh();
                }
                reader.Dispose();
                con.Close();
                con.Dispose();
                MessageBox.Show("Span completed!");
            }
            catch (Exception ee)
            {
                MessageBox.Show("Database error.\n" + ee.StackTrace.ToString());
                Application.Exit();
            }
           
        }
        private void file_load_listview()
        {
            try
            {
               

                listView1.View = View.Details;
                SqlConnection con = Database.GetConnectionObj();
                if (con == null) throw new Exception("Connection failed! Please try again.");

                SqlDataAdapter ada = new SqlDataAdapter("SELECT * FROM dbo.file_list", con);
                DataTable dt = new DataTable();
                ada.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    ListViewItem listitem = new ListViewItem(dr["file_name"].ToString());
                    listitem.SubItems.Add(dr["file_size"].ToString());
                    listitem.SubItems.Add(dr["rows"].ToString());
                    //listitem.SubItems.Add(dr["solve_status"].ToString());
                    listitem.SubItems.Add(dr["span_stat"].ToString());
                    listView1.Items.Add(listitem);
                    
                    //upload level span
                    // esolve es = new esolve();
                    //es.espan(filename, sub_code, "");
                }
                dt.Dispose();
                con.Close();
                con.Dispose();
            }
            catch (Exception ee)
            {
                MessageBox.Show("Database error.\n" + ee.StackTrace.ToString());
                Application.Exit();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
            
    }
}
