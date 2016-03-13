using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace NU_Solver
{
    class esolve 
    {
        public string username = "";
        public string filename = "";
        public string sub_code = "";
        public esolve()
        {
            
        }
        public esolve(string filename, string sub_code)
        {
            this.filename = filename;
            this.sub_code = sub_code;
            //this.username = username;
        }
        public esolve(string filename, string sub_code, string username)
        {
            this.filename = filename;
            this.sub_code = sub_code;
            this.username = username;
        }

        public void espan(string filename, string sub_code, string username)
        {
            this.filename = filename;
            this.sub_code = sub_code;
            //this.username = username;
            try
            {
                SqlConnection con = Database.GetConnectionObj();
                if (con == null)
                {
                    MessageBox.Show("Connection Problem!");
                    return;
                }
                plswait pw = new plswait();
                pw.Show();
                pw.Refresh();

                SqlCommand cmd = new SqlCommand("", con);
                cmd.CommandText = string.Format("SELECT id, scanned_row, file_name FROM dbo.E_solving WHERE file_name = '{0}' order by id", filename);
                SqlDataReader reader = cmd.ExecuteReader();
                //int serial = 1;
                int affectedRow = 0;
                int upRow = 0;
                SqlConnection con2 = Database.GetConnectionObj();
                while (reader.Read())
                {
                    //MessageBox.Show(reader[0].ToString());
                    int id = Convert.ToInt32(reader["id"]);
                    string line = reader["scanned_row"].ToString();
                    if (line.Length < 140)
                        line = line + "            ";
                    //int n = dataGridView1.Rows.Add();
                    //string scan_sl = line.Substring(0, 50);
                    //string scan_sl = line.Substring(0, 50).Replace(' ', '*');
                    string dexcode = line.Substring(50, 32).Replace(' ', '0');
                    string exam_code = line.Substring(82, 3).Replace(' ', '*');
                    string reg_no = line.Substring(85, 11).Replace(' ', '*');
                    string pap_code = line.Substring(96, 6).Replace(' ', '*');
                    string hexcode = line.Substring(102, 32).Replace(' ', '0');
                    //string err_dscr = "error goes here";
                    string err_dscr = echeckExamCode(exam_code) +
                        echeckSubjectCode(pap_code) +
                        echeckRegi(reg_no, pap_code) +
                        echeckLitho("", "", dexcode, hexcode);

                    string update = string.Format(
                        //"UPDATE dbo.E_solving SET err_dscr = '{0}',exam_code = '{1}',pap_code = '{2}',regi = '{3}',litho_1 = '{4}',litho_2 = '{5}',updates = '{6}', serial = {7} WHERE id = '{8}'",
                        "UPDATE dbo.E_solving SET err_dscr = '{0}',exam_code = '{1}',pap_code = '{2}',regi = '{3}',litho_1 = '{4}',litho_2 = '{5}',updates = '{6}', solver='{6}', last_update='{6}' WHERE id = '{7}'",
                        err_dscr, exam_code, pap_code, reg_no, dexcode, hexcode, "", id);
                    //MessageBox.Show(update);
                    //cmd.CommandText = update;
                    SqlCommand command = new SqlCommand(update, con2);
                    //SqlDataReader reader2 = command.ExecuteReader();
                    //command.ExecuteReader();
                    try
                    {
                        affectedRow = command.ExecuteNonQuery();
                    }
                    catch (Exception E)
                    {
                    }
                    //MessageBox.Show(affectedRow.ToString()+" rows affected for id "+ id.ToString());
                    pw.Refresh();
                }
                //file span status update to file list
                
                SqlConnection con3 = Database.GetConnectionObj();
                string cmdUP = string.Format("UPDATE file_list set span_stat='{0}' WHERE file_name = '{1}'", "T", filename);
                SqlCommand command1 = new SqlCommand(cmdUP,con3);
                try
                {
                    upRow = command1.ExecuteNonQuery();
                }
                catch (Exception E)
                {
                }
                pw.Close();
                reader.Dispose();
            }
            catch (Exception ee)
            {
                MessageBox.Show("Database error.\n" + ee.StackTrace.ToString());
                Application.Exit();
            }
        }

        public string echeckExamCode(string s)
        { return ""; }

        public string echeckSubjectCode(string s)
        {
            //s =s.Substring(3,4).ToString();
            return s != this.sub_code ? "Sub. Err. " : "";
        }//{ return s != this.sub_code ? "Subject code is not " + this.sub_code + "." : ""; }

        public string echeckRegi(string regi, string pap_code)
        {
            string regi_err = "";
            string sifREG = echeckSifRegi(regi, pap_code);
            //return sifREG;

            if (!Regex.IsMatch(regi, @"^\d+$"))
                regi_err = "Regi. IC.";
            else if (regi.Length != 11)
                regi_err = "Regi. Len";
            // else if (regi.Substring(0,2) != "00") 
            // return "Regi. 13";
            //return "Regi Contains Invalid Caracter.";

            if (regi_err == "")
            {
                return sifREG;
            }
            else
            {
                return regi_err;
            }

        }
        public string echeckSifRegi(string regi, string pap_code) //regi and papcode check in SIF
        {

            int affectedRow = 0;
            SqlConnection con1 = Database.GetConnectionObj();
            if (con1 == null)
            {
                MessageBox.Show("Connection Problem for Registration sif checking!");
                return "";
            }
            SqlCommand cmd = new SqlCommand("", con1);
            cmd.CommandText = string.Format("SELECT count(*) FROM pap_all WHERE pap_code = '{0}' and reg_no='{1}'", pap_code.Substring(2, 4), regi);
            affectedRow = (int)cmd.ExecuteScalar();
            con1.Close();
            if (affectedRow < 1)
            {
                //MessageBox.Show(affectedRow.ToString());
                return "RG NF";
            }
            else
            {
                return "";
            }

        }

        public string eDupRegCheck(string pap_code) //regi and papcode check in SIF
        {

            
            SqlConnection con3 = Database.GetConnectionObj();
            if (con3 == null)
            {
                MessageBox.Show("Connection Problem for Registration sif checking!");
                return "";
            }
            SqlCommand cmd = new SqlCommand("", con3);
            cmd.CommandText = string.Format("SELECT pap_code, regi, count(regi) as total FROM e_solving WHERE pap_code = '{0}' GROUP BY pap_code, regi HAVING COUNT(regi)>1 ", pap_code);
            var affectedRow = cmd.ExecuteScalar();
            con3.Close();
            if (affectedRow == null)
            {
                //MessageBox.Show(affectedRow.ToString());
                return "";
            }
            else
            {
                return "DUP";
            }

        }
        public void eDupRegi(string pap_code) //regi and papcode check in SIF
        {

            int affectedRow = 0;
            SqlConnection con1 = Database.GetConnectionObj();
            if (con1 == null)
            {
                MessageBox.Show("Connection Problem for Registration sif checking!");
                //return "";
            }
            SqlCommand cmd = new SqlCommand("", con1);
            if (eDupRegCheck(pap_code) == "DUP")
            {
                cmd.CommandText =
                    string.Format(
                        "SELECT pap_code, regi, count(regi) as total FROM e_solving WHERE pap_code = '{0}' GROUP BY pap_code, regi HAVING COUNT(regi)>1 ",
                        pap_code);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //MessageBox.Show(reader[0].ToString());
                    affectedRow = Convert.ToInt32(reader["total"]);
                    string reg_no = reader["regi"].ToString();
                    string pap = reader["pap_code"].ToString();

                    string update =
                        string.Format(
                            "UPDATE dbo.E_solving SET err_dscr =err_dscr +'DUP RG' WHERE regi = '{0}' AND pap_code='{1}'",reg_no, pap);

                    //MessageBox.Show(update);
                    //cmd.CommandText = update;
                    SqlConnection con2 = Database.GetConnectionObj();
                    SqlCommand command = new SqlCommand(update, con2);
                    //SqlDataReader reader2 = command.ExecuteReader();
                    //command.ExecuteReader();
                    try
                    {
                        affectedRow = command.ExecuteNonQuery();
                    }
                    catch (Exception E)
                    {
                    }
                    //MessageBox.Show(affectedRow.ToString()+" rows affected for id "+ id.ToString());

                }
                reader.Dispose();
            }
            else
            {
                string update = string.Format("UPDATE dbo.E_solving SET err_dscr = ' ' WHERE err_dscr like  '%{0}%' AND pap_code='{1}'", "DUP", pap_code);
                //MessageBox.Show(update);
                //cmd.CommandText = update;
                SqlConnection con2 = Database.GetConnectionObj();
                SqlCommand command = new SqlCommand(update, con2);
                //SqlDataReader reader2 = command.ExecuteReader();
                //command.ExecuteReader();
                try
                {
                    affectedRow = command.ExecuteNonQuery();
                }
                catch (Exception E)
                {
                }    
            }
            
            con1.Close();
            
        }
        public string echeckLitho(string qr, string scrpt_no, string litho1, string litho2)
        {
            if (qr != "")
            {
                if (scrpt_no.Length != 28)
                    return "Litho QR length should 28.";
            }
            else if (scrpt_no != "")
            {
                if (!Regex.IsMatch(scrpt_no, @"^[\d]+$"))
                    return "Litho Script IC";
                if (scrpt_no.Length != 10)
                    return "Litho Script Len";
            }
            else if (!Regex.IsMatch(litho1, @"^[10]+$"))
                return "Litho 1 IC.";
            else if (!Regex.IsMatch(litho2, @"^[10]+$"))
                return "Litho 2 IC";
            else if (litho1.Length != 32)
                return "Litho 1 32";
            else if (litho2.Length != 32)
                return "Litho 2 32";
            else if (litho1 != litho2)
                return "Litho 1,2 NE";
            else //litho1 == litho2
            {
                //MessageBox.Show(litho1.Substring(0, 3) + "-" + litho1.Substring(28, 29));
                if (litho1.Substring(0, 4) == "1111" && litho1.Substring(28, 4) == "0001") //R8 F1 E type
                    return "";
                else if (litho1.Substring(0, 4) == "1111" && litho1.Substring(28, 4) == "1100") //R8 FC E type
                    return "";
                else return "Litho Shift";
            }
            return "";
        }

    }
}
