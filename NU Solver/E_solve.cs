﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NU_Solver
{
    
    public partial class E_solve : Form
    {
        public string filename = "";
        public string username = "";
        public string sub_code = "";
        public string current = "";
        int tot_row = 0;
        //int current_row = 0;
        login caller;
        public bool value_back = false;
        public bool error_only = false;
        //File_List caller;
        public DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        public E_solve(string filename, string username, string sub, login caller, bool needToSpan = false)
        {
            this.filename = filename;
            this.sub_code = sub;
            this.username = username;
            this.caller = caller;
            InitializeComponent();
            /*
            if (needToSpan)
                span();
              */
            CustomiseGridView();

    
            //displaying info
            this.Text = filename ;
            lbl_current_user.Text = username;
            lbl_bundle_name.Text = filename;
            lbltotal_record.Text = tot_row.ToString();
            ErrCount();
            tot_row=dataGridView1.RowCount;
            lbltotal_record.Text = tot_row.ToString();
            
            //current_row = dataGridView1.CurrentRow.Index + 1;
            //lblCurrentRecord.Text = current_row.ToString();
            esolve es= new esolve(filename, sub_code, username);
            
        }
        private void CustomiseGridView()
        {
            try
            {                
                SqlConnection con = Database.GetConnectionObj();
                SqlCommand cmd = new SqlCommand("", con);
                cmd.CommandText = string.Format("SELECT id, serial, err_dscr, exam_code, pap_code, regi, qr_code, scrpt_no, litho_1, litho_2,'' as '.' FROM dbo.E_solving where file_name = '{0}' ORDER BY serial", filename);
               
                da.SelectCommand = cmd;
                da.Fill(dt);
                this.dataGridView1.DataSource = dt;

                this.dataGridView1.Columns[0].Visible = false;
                this.dataGridView1.Columns[1].ReadOnly = true;
                this.dataGridView1.Columns[2].ReadOnly = true;
                this.dataGridView1.Columns[3].Visible = false;//exam_code

                this.dataGridView1.Columns[1].Width = 60;
                //this.dataGridView1.Columns[2].Width = 70;
                this.dataGridView1.Columns[2].Width = 250;
                //this.dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView1.Columns[3].Width = 70;
                this.dataGridView1.Columns[4].Width = 90;
                this.dataGridView1.Columns[5].Width = 150;
                this.dataGridView1.Columns[6].Width = 6;
                this.dataGridView1.Columns[6].Visible = false; // qr
                this.dataGridView1.Columns[7].Width = 200;
                this.dataGridView1.Columns[8].Width = 6;
                this.dataGridView1.Columns[9].Width = 6;
                //this.dataGridView1.Columns[10].Width = 6;
                this.dataGridView1.Columns[10].Visible = false;
                //this.dataGridView1.Columns[1].Width = 60;
                ////this.dataGridView1.Columns[2].Width = 70;
                //this.dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                //this.dataGridView1.Columns[3].Width = 70;
                //this.dataGridView1.Columns[4].Width = 90;
                //this.dataGridView1.Columns[5].Width = 150;
                //this.dataGridView1.Columns[6].Width = 6;
                //this.dataGridView1.Columns[7].Width = 180;
                //this.dataGridView1.Columns[8].Width = 40;
                //this.dataGridView1.Columns[9].Width = 403;
                //this.dataGridView1.Columns[10].Width = 6;
                //tot_row=Convert.ToInt32((DataGridViewTextBoxColumn)this.dataGridView1.Columns[1]);
                DataGridViewTextBoxColumn exam_code = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[3];
                exam_code.MaxInputLength = 3;
                DataGridViewTextBoxColumn sub_code = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[4];
                sub_code.MaxInputLength = 6;
                DataGridViewTextBoxColumn reg_no = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[5];
                reg_no.MaxInputLength = 11;
                DataGridViewTextBoxColumn qr = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[6];
                qr.MaxInputLength = 28;
                DataGridViewTextBoxColumn binary = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[7];
                binary.MaxInputLength = 10;
                DataGridViewTextBoxColumn l1 = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[8];
                l1.MaxInputLength = 32;
                DataGridViewTextBoxColumn l2 = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[9];
                l1.MaxInputLength = 32;
                con.Close();


            }
            catch (Exception ee)
            {
                MessageBox.Show("Database error.\n" + ee.StackTrace.ToString());
                Application.Exit();
            }
        }
        private void litho_correction()
        {

            LithoCorrection L = new LithoCorrection();

            var result = L.ShowDialog();
            if (result == DialogResult.OK)
            {
                //string val = form.ReturnValue1;            //values preserved after close
                //string dateString = form.ReturnValue2;
                ////Do something here with these values

                ////for example
                //this.txtSomething.Text = val;
                MessageBox.Show("OK");
            }
            else
                MessageBox.Show("cancel");
        }

        private void onSolveWindowClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        //private int mach_count
        private void keyPressedOnCell(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                //MessageBox.Show(this.dataGridView1.CurrentCell.RowIndex.ToString());
                //L.Show();

                
                DataGridViewCellStyle CellStyle = new DataGridViewCellStyle();
                CellStyle.BackColor = Color.DarkGray;
                CellStyle.ForeColor=Color.Red;
                dataGridView1.CurrentCell.Style = CellStyle;
                
                
                dataGridView1.Rows[dataGridView1.CurrentRow.Index].ErrorText = string.Empty;
                current = dataGridView1.CurrentCell.Value.ToString();

               // lbl_cell_val.Text = current;

                //MessageBox.Show(current);
                dataGridView1.BeginEdit(true);
                //litho_correction();
                e.Handled = true;
                
            }
            else if (e.KeyChar == 'e' || e.KeyChar == 'E')
            {
                ShowErrorCount();
                e.Handled = true;
            }
            else if (e.KeyChar == '-')
            {
                ReplaceSubject();
                e.Handled = true;
            }
            else if (e.KeyChar == '+')
            {
                GoToNextError();
                e.Handled = true;
            }
            else if (e.KeyChar == '/')
            {
                finish();
                e.Handled = true;
            }
            else if (e.KeyChar == 'r' || e.KeyChar == 'R')
            {
                relese();
                e.Handled = true;
            }
            else if (e.KeyChar == 'a' || e.KeyChar == 'A')
            {
                ShowAllError();
                e.Handled = true;
            }
            else if ((e.KeyChar == 's' || e.KeyChar == 'S') && (this.dataGridView1.CurrentCell.ColumnIndex == 5))
            {
                string commandtext = "";
                string cmdtext = "";
                string ccode = "";
                string reg = "";
                int tt;
                
                if (this.dataGridView1.CurrentCell.ColumnIndex == 5)
                {
                    if (dataGridView1.CurrentRow.Index == 0)
                    {
                        tt = dataGridView1.CurrentRow.Index + 1;
                    }
                    else
                    {
                        tt = dataGridView1.CurrentRow.Index - 1;    
                    }
                    
                   reg = this.dataGridView1.Rows[tt].Cells[5].Value.ToString();
                    if (!Regex.IsMatch(reg, @"^\d+$"))
                    {
                        reg = this.dataGridView1.Rows[tt+1].Cells[5].Value.ToString();
                    }
                    if (!Regex.IsMatch(reg, @"^\d+$"))
                    {
                        reg = this.dataGridView1.Rows[tt + 1+1].Cells[5].Value.ToString();
                    }

                    SqlConnection con = Database.GetConnectionObj();
                    if (con == null)
                    {
                        MessageBox.Show("Connection Problem!");
                        return;
                    }
                    SqlCommand cmd = new SqlCommand("", con);
                    cmd.CommandText = String.Format("Select pap_code PAPER, reg_no REGI, std_name NAME, c_code from pap_all where reg_no='{0}' and pap_code='{1}'", reg, sub_code.Substring(2, 4).ToString());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        //MessageBox.Show(reader[0].ToString());
                        //string line = reader["scanned_row"].ToString();
                        ccode = reader["c_code"].ToString();
                        
                    }
                    reader.Dispose();
                    con.Close();
                    //MessageBox.Show(ccode.ToString());
                    
                    //commandtext = String.Format("Select pap_code PAPER, reg_no REGI, std_name NAME,c_code from pap_all where reg_no='{0}' and pap_code='{1}'", reg, sub_code.Substring(2,4).ToString());
                    commandtext = String.Format("Select pap_code PAPER, reg_no REGI, std_name NAME,c_code CENTER from pap_all where c_code='{0}' and pap_code='{1}' order by reg_no", ccode, sub_code.Substring(2, 4).ToString());
                    //MessageBox.Show(commandtext.ToString());
                }
                
                sifview sif = new sifview(commandtext, reg);

                sif.ShowDialog();
                e.Handled = true;

            }       

        }
  

        private void relese()
        {            //throw new NotImplementedException();
            if (MessageBox.Show("Are you really want to RELESE this file?\n" +
                "All Corrections made will be lost." +
                "Press 'Yes' to RELESE this file.\n" +
                "Press 'No' to return to solving window.\n",
                "Finish Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SqlConnection con = Database.GetConnectionObj();
                    if (con == null) throw new Exception("Can't create and open a connection");
                    SqlCommand cmd = new SqlCommand("", con);

                    cmd.CommandText = string.Format(
                        "UPDATE dbo.file_list " +
                        "SET solver_name = '__NONE', solver_comments = solver_comments +' {0} Relesed on ' + CURRENT_TIMESTAMP " +
                        "WHERE file_name = '{1}' AND solver_name = '{0}' AND solve_status = 'SOLVING' ",
                        username, filename);

                    if (cmd.ExecuteNonQuery() == 0)
                        throw new Exception("DB Update failed!");
                    con.Close();
                    con.Dispose();

                    

                    esolve es = new esolve();
                    //espan(string username, string filename, string sub_code)
                    es.espan(filename, sub_code, username);
                    //pw.Close();
                    File_List fl = new File_List(username, caller);
                    fl.Show();
                    this.Hide();
                    //this.Close();
                   

                }

                catch (Exception ee)
                {
                    MessageBox.Show("Database error - Releasing failed.\n" + ee.StackTrace.ToString());
                    //Application.Exit();
                }
                // save file
                if (error_only)
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Empty;
                this.dataGridView1.Sort(dataGridView1.Columns["serial"], ListSortDirection.Ascending);

                StreamWriter sw = new StreamWriter(@"Backup\" + filename + ".SLV");
                for (int row = 0; row < dataGridView1.RowCount; row++)
                {
                    sw.WriteLine("{0,12}{1,4}{3,3}{4,6}{5,11}{6,28}{7,10}{8,32}{9,32}{2,40}",
                        dataGridView1.Rows[row].Cells[0].Value, //id
                        dataGridView1.Rows[row].Cells[1].Value, //serial
                        dataGridView1.Rows[row].Cells[2].Value, //err_dscr
                        dataGridView1.Rows[row].Cells[3].Value, //exam_code
                        dataGridView1.Rows[row].Cells[4].Value, //pap_code
                        dataGridView1.Rows[row].Cells[5].Value, //regi
                        dataGridView1.Rows[row].Cells[6].Value, //qr_code
                        dataGridView1.Rows[row].Cells[7].Value, //scrpt_no
                        dataGridView1.Rows[row].Cells[8].Value, //litho_1
                        dataGridView1.Rows[row].Cells[9].Value, //litho_2
                        ""
                        );
                }

                sw.Dispose();
            }
        }
        private void finish()
        {            //throw new NotImplementedException();
            if (MessageBox.Show("Are you really want to finish?\n" +
                "You will be respnsible for any remaining error." +
                "Press 'Yes' to finish solving this file.\n" +
                "Press 'No' to return to solving window.\n",
                "Finish Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                try
                {
                    SqlConnection con = Database.GetConnectionObj();
                    if (con == null) throw new Exception("Can't create and open a connection");
                    SqlCommand cmd = new SqlCommand("", con);

                    cmd.CommandText = string.Format(
                        "UPDATE dbo.file_list " +
                        "SET solve_status = 'SOLVED', end_date = CURRENT_TIMESTAMP " +
                        "WHERE file_name = '{1}' AND solver_name = '{0}' ",
                        username, filename);
                    
                    if (cmd.ExecuteNonQuery() == 0)
                        throw new Exception("DB Update failed");
                    con.Close();
                    con.Dispose();

                    File_List fl = new File_List(username, caller);
                    fl.Show();
                    this.Hide();
                    //this.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Database error - Finishing failed.\n" + ee.StackTrace.ToString());
                    //Application.Exit();
                }
                // save file

                if (error_only)
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Empty;
                this.dataGridView1.Sort(dataGridView1.Columns["serial"], ListSortDirection.Ascending);

                StreamWriter sw = new StreamWriter(@"Backup\" + filename + ".SLV");
                for (int row = 0; row < dataGridView1.RowCount; row++)
                {
                    sw.WriteLine("{0,12}{1,4}{3,3}{4,6}{5,11}{6,28}{7,10}{8,32}{9,32}{2,40}",
                        dataGridView1.Rows[row].Cells[0].Value, //id
                        dataGridView1.Rows[row].Cells[1].Value, //serial
                        dataGridView1.Rows[row].Cells[2].Value, //err_dscr
                        dataGridView1.Rows[row].Cells[3].Value, //exam_code
                        dataGridView1.Rows[row].Cells[4].Value, //pap_code
                        dataGridView1.Rows[row].Cells[5].Value, //regi
                        dataGridView1.Rows[row].Cells[6].Value, //qr_code
                        dataGridView1.Rows[row].Cells[7].Value, //scrpt_no
                        dataGridView1.Rows[row].Cells[8].Value, //litho_1
                        dataGridView1.Rows[row].Cells[9].Value, //litho_2
                        ""
                        );
                }
                sw.Dispose();
            }
        }
        //private string checkExamCode(string s) { return s != "201" ? "Exam code Err. " : ""; }
        private void cell_end_edit(object sender, DataGridViewCellEventArgs e)
        {
                       
        }
        private void cellValidaTING(object sender,
        DataGridViewCellValidatingEventArgs e)
        {

        } 

        private void cellValideTED(object sender, DataGridViewCellEventArgs e)
        {
            {
            
            }
        }

        private string getError(int row)
        {
            esolve es=new esolve(filename, sub_code, username);
            /***/
            return
                
                es.echeckExamCode(dataGridView1.Rows[row].Cells[3].Value.ToString()) +
                es.echeckSubjectCode(dataGridView1.Rows[row].Cells[4].Value.ToString()) +
                es.echeckRegi(dataGridView1.Rows[row].Cells[5].Value.ToString(), dataGridView1.Rows[row].Cells[4].Value.ToString()) +
                es.echeckLitho(dataGridView1.Rows[row].Cells[6].Value.ToString(),
                            dataGridView1.Rows[row].Cells[7].Value.ToString(),
                            dataGridView1.Rows[row].Cells[8].Value.ToString(),
                            dataGridView1.Rows[row].Cells[9].Value.ToString()
                );
                
        }

        private void cellValueChanged(object sender,DataGridViewCellEventArgs e)
        {
            ErrCount(); // display error count
            if (e.ColumnIndex < 3)
                return;
            string field_name = dataGridView1.Columns[e.ColumnIndex].Name;

            if (dataGridView1.Rows[e.RowIndex].ErrorText.ToString().Contains(field_name))
                return;
            string input = dataGridView1.CurrentCell.Value.ToString();
            string id = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            string error = getError(e.RowIndex);
            string qry = String.Format(
                "UPDATE E_solving SET " +
                "{0} = '{1}' " +
                ",err_dscr = '{4}' " +
                ",solver = '{2}' " +
                ",last_update = CURRENT_TIMESTAMP" +
                ",updates = "+
                "'({0}{1}{2}'+CONVERT(VARCHAR,CURRENT_TIMESTAMP)+')'+ updates "+
                "WHERE id = {3}",
                field_name,
                input,
                username,
                id,
                error
                );
            try
            {
                SqlConnection con = Database.GetConnectionObj();
                if (con == null) throw new Exception("Can't create and open a connection");
                SqlCommand cmd = new SqlCommand(qry, con);
                //MessageBox.Show(con.State.ToString());
                if (cmd.ExecuteNonQuery() == 0)
                {
                    MessageBox.Show("Database Update Faild. Try again Latter.\n");
                    dataGridView1.Rows[e.RowIndex].ErrorText = String.Format(
                                    "{1} entered on {0} not updated on database." ,
                                    field_name,
                                    input                                    
                                    );
                    dataGridView1.CurrentCell.Value = current;
                    //TODO: error
                }
                else
                    dataGridView1.Rows[e.RowIndex].Cells[2].Value = error;
                    //cmd.CommandText = qry;               
                    con.Close();
                    con.Dispose();
                    dataGridView1.Rows[e.RowIndex].Cells[2].Value = error;
            }
            catch (Exception ee)
            {
                MessageBox.Show("Database Update Faild. Try again Latter.\n" + ee.StackTrace.ToString());
                dataGridView1.Rows[e.RowIndex].ErrorText = String.Format(
                                    "{1} entered on {0} not updated on database.",
                                    field_name,
                                    input
                                    );
                dataGridView1.CurrentCell.Value = current;
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Do you exit the sove program?");
            finish(); 
        }

        private void btnReleaseFile_Click(object sender, EventArgs e)
        {
            relese();
            //e.Handled = true;
        }

        public void ErrCount()
        {
            int row = -1, count = 0, sub = 0, exm = 0, reg = 0, litho = 0;
            string tmp = "";
            while (++row < dataGridView1.RowCount - 1)
            {
                if (!string.IsNullOrEmpty(dataGridView1.Rows[row].Cells[2].Value.ToString()))
                {
                    count++;
                    tmp = dataGridView1.Rows[row].Cells[2].Value.ToString();
                    if (tmp.Contains("Exam")) exm++;
                    if (tmp.Contains("Sub")) sub++;
                    if (tmp.Contains("Regi")) reg++;
                    if (tmp.Contains("Litho")) litho++;
                }
            }
            lbltotal_error.Text = count.ToString();
            /*
            //toolStripStatusLabel1.Text = string.Format(
            tmp = string.Format(
                "Total Error Line       : {0}\n" +
                "Exam Code Error     : {1}\n" +
                "Subject Code Error : {2}\n" +
                "Registration Error   : {3}\n" +
                "Litho Code Error     : {4}", count, exm, sub, reg, litho);
            MessageBox.Show(tmp);
             * */
        }

        private void ShowErrorCount()
        {
            int row = -1, count = 0, sub = 0, exm = 0, reg = 0, litho = 0;
            string tmp = "";
            while (++row < dataGridView1.RowCount - 1)
            {
                if (!string.IsNullOrEmpty(dataGridView1.Rows[row].Cells[2].Value.ToString()))
                {
                    count++;
                    tmp = dataGridView1.Rows[row].Cells[2].Value.ToString();
                    if (tmp.Contains("Exam")) exm++;
                    if (tmp.Contains("Sub")) sub++;
                    if (tmp.Contains("Regi")) reg++;
                    if (tmp.Contains("Litho")) litho++;
                }
            }

            //toolStripStatusLabel1.Text = string.Format(
            tmp = string.Format(
                "Total Error Line       : {0}\n" +
                "Exam Code Error     : {1}\n" +
                "Subject Code Error : {2}\n" +
                "Registration Error   : {3}\n" +
                "Litho Code Error     : {4}", count, exm, sub, reg, litho);
            MessageBox.Show(tmp);
        }

        private void ReplaceSubject()
        {
            int col = this.dataGridView1.CurrentCell.ColumnIndex;
            if (col == 4)
            {
                if (!Regex.IsMatch(dataGridView1.CurrentCell.Value.ToString(), @"^\d+$"))
                {
                    dataGridView1.CurrentCell.Value = this.sub_code;
                    //e.Handled = true;
                }
            }
        }

        private void ShowAllError()
        {
            int current_pos = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value);
            if (current_pos == 0)
                current_pos++;
            error_only = !error_only;
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = error_only ? string.Format("err_dscr <> '{0}'", "") : string.Empty;
            int row = dataGridView1.RowCount - 1;
            while (Convert.ToInt32(dataGridView1.Rows[row].Cells[1].Value) > current_pos)
            {
                if (row == 0)
                    break;
                row--;
            }
            dataGridView1.CurrentCell = dataGridView1.Rows[row].Cells[2];
            //e.Handled = true;
       }

        private void cmdNextError_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAllError_Click(object sender, EventArgs e)
        {
            //string dc = dataGridView1.CurrentCell;
            ShowAllError();
            dataGridView1.Focus();
        }

        private void btnShowError_Click(object sender, EventArgs e)
        {
            ShowErrorCount();
            dataGridView1.Focus();
            //e.Handled = true;
        }
    
        private void GoToNextError()
        {
            int maxrow = this.dataGridView1.RowCount - 1;
            string cur_str = "";
            //string search_pattern = "";
            int col = this.dataGridView1.CurrentCell.ColumnIndex;
            int row = this.dataGridView1.CurrentCell.RowIndex; // +1;
            if (col <= 4)
            {
                //subject code
                col = 4;
                for (; row < this.dataGridView1.RowCount; row++)
                    if (this.dataGridView1.Rows[row].Cells[col].Value.ToString() != this.sub_code)
                    {
                        //MessageBox.Show(this.dataGridView1.Rows[row].Cells[col].Value.ToString());
                        this.dataGridView1.CurrentCell = this.dataGridView1.Rows[row].Cells[col];
                        //e.Handled = true;
                        return;
                    }
                col = 5;
                row = 0;
            }

            if (col == 5)
            {
                //subject code
                for (; row < this.dataGridView1.RowCount; row++)
                    if (this.dataGridView1.Rows[row].Cells[2].Value.ToString().Contains("Regi"))
                    {
                        //MessageBox.Show(this.dataGridView1.Rows[row].Cells[col].Value.ToString());
                        this.dataGridView1.CurrentCell = this.dataGridView1.Rows[row].Cells[col];
                        //e.Handled = true;
                        return;
                    }
                col = 7;
                row = 0;
            }
            if (col > 5)
            {
                //subject code
                //col = 6;//??
                //row = 0;//??
                for (; row < this.dataGridView1.RowCount; row++)
                    if (this.dataGridView1.Rows[row].Cells[2].Value.ToString().Contains("Litho"))
                    {
                        //MessageBox.Show(this.dataGridView1.Rows[row].Cells[col].Value.ToString());
                        this.dataGridView1.CurrentCell = this.dataGridView1.Rows[row].Cells[col];
                        //e.Handled = true;
                        return;
                    }
                //col = 6;
                //row = 0;
            }
            
        }
        private void btnNextError_Click(object sender, EventArgs e)
        {
            GoToNextError();
            dataGridView1.Focus();
            //e.Handled = true;
        }
        private void btnReplaceSubject_Click(object sender, EventArgs e)
        {
            ReplaceSubject();
            //e.Handled = true;
            dataGridView1.Focus();
        }
        private void btnReleaseFile_Click_1(object sender, EventArgs e)
        {
            relese();
            //e.Handled = true;
        }
        private void btnFinishPost_Click(object sender, EventArgs e)
        {
            finish();
            //e.Handled = true;
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            //changepassword chpw = new E_solve(filename, username, getSub(filename), caller, true);
            changepassword chpw = new changepassword(username);
            //this.Hide();
            chpw.Show();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            esolve es = new esolve(filename, sub_code);
            es.eDupRegi(sub_code);
            //File_List caller;
            dt.Rows.Clear();
            CustomiseGridView();
            dataGridView1.Refresh();
            dataGridView1.Focus();

        }
        
    }
}
