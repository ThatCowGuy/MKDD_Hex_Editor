using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Diagnostics;

namespace MKDD_Hexer
{
    public partial class Form1 : Form
    {
        public dtm_handler DTM_Handler;
        public bool checkboxes_affect_data = false;
        public bool dtm_loaded = false;

        public void display_selected_frame()
        {
            this.checkboxes_affect_data = false;
            int frame_num = (int) this.numericUpDown1.Value;
            byte[] data = DTM_Handler.read_framedata(frame_num);

            Console.WriteLine("Printing FrameData...");
            DTM_Handler.print_framedata(frame_num, data);

            // byte #0
            this.s1.Checked = ((data[0] & dtm_handler.BUTTON_S) > 0);
            this.a1.Checked = ((data[0] & dtm_handler.BUTTON_A) > 0);
            this.b1.Checked = ((data[0] & dtm_handler.BUTTON_B) > 0);
            this.x1.Checked = ((data[0] & dtm_handler.BUTTON_X) > 0);
            this.y1.Checked = ((data[0] & dtm_handler.BUTTON_Y) > 0);
            this.z1.Checked = ((data[0] & dtm_handler.BUTTON_Z) > 0);
            this.du1.Checked = ((data[0] & dtm_handler.BUTTON_DU) > 0);
            this.dd1.Checked = ((data[0] & dtm_handler.BUTTON_DD) > 0);
            // byte #1
            this.dl1.Checked = ((data[1] & dtm_handler.BUTTON_DL) > 0);
            this.dr1.Checked = ((data[1] & dtm_handler.BUTTON_DR) > 0);
            this.l1.Checked = ((data[1] & dtm_handler.BUTTON_L) > 0);
            this.r1.Checked = ((data[1] & dtm_handler.BUTTON_R) > 0);
            this.conn1.Checked = ((data[1] & dtm_handler.SPECIAL_CONN) > 0);

            // byte #8
            this.s2.Checked = ((data[8] & dtm_handler.BUTTON_S) > 0);
            this.a2.Checked = ((data[8] & dtm_handler.BUTTON_A) > 0);
            this.b2.Checked = ((data[8] & dtm_handler.BUTTON_B) > 0);
            this.x2.Checked = ((data[8] & dtm_handler.BUTTON_X) > 0);
            this.y2.Checked = ((data[8] & dtm_handler.BUTTON_Y) > 0);
            this.z2.Checked = ((data[8] & dtm_handler.BUTTON_Z) > 0);
            this.du2.Checked = ((data[8] & dtm_handler.BUTTON_DU) > 0);
            this.dd2.Checked = ((data[8] & dtm_handler.BUTTON_DD) > 0);
            // byte #9
            this.dl2.Checked = ((data[9] & dtm_handler.BUTTON_DL) > 0);
            this.dr2.Checked = ((data[9] & dtm_handler.BUTTON_DR) > 0);
            this.l2.Checked = ((data[9] & dtm_handler.BUTTON_L) > 0);
            this.r2.Checked = ((data[9] & dtm_handler.BUTTON_R) > 0);
            this.conn2.Checked = ((data[9] & dtm_handler.SPECIAL_CONN) > 0);

            // stick bytes
            this.ana1x.Value = data[4];
            this.trackBar2.Value = data[4];
            this.numericUpDown5.Value = data[5];
            this.trackBar4.Value = data[5];
            this.numericUpDown3.Value = data[6];
            this.trackBar1.Value = data[6];
            this.numericUpDown4.Value = data[7];
            this.trackBar3.Value = data[7];
            this.numericUpDown9.Value = data[12];
            this.trackBar8.Value = data[12];
            this.numericUpDown7.Value = data[13];
            this.trackBar6.Value = data[13];
            this.numericUpDown8.Value = data[14];
            this.trackBar7.Value = data[14];
            this.numericUpDown6.Value = data[15];
            this.trackBar5.Value = data[15];

            this.checkboxes_affect_data = true;
            this.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int frame_num = (int)this.numericUpDown1.Value;
            byte[] data = DTM_Handler.read_framedata(frame_num);
            DTM_Handler.write_framedata(frame_num + 1, data);

            // increase frame num and MANUALLY fire a change-event
            this.numericUpDown1.Value += 1;
            this.numericUpDown1_ValueChanged(this.numericUpDown1, EventArgs.Empty);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int frame_num = (int)this.numericUpDown1.Value;
            DTM_Handler.insert_frame_copy_at(frame_num);

            // increase frame num and MANUALLY fire a change-event
            this.numericUpDown1.Value += 1;
            this.numericUpDown1_ValueChanged(this.numericUpDown1, EventArgs.Empty);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int frame_num = (int)this.numericUpDown1.Value;
            DTM_Handler.remove_frame_at(frame_num);
            this.display_selected_frame();
        }

        public Form1()
        {
            InitializeComponent();
            this.DTM_Handler = new dtm_handler();

            // disable EVERY control except for the load button at the start, so that nobody crashes the App
            foreach (Control control in this.Controls)
            {
                if (control != button1)
                {
                    control.Enabled = false;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine(String.Format("new val {0}", numericUpDown1.Value));
            this.display_selected_frame();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int frame_num = (int) this.numericUpDown1.Value;
            DTM_Handler.apply_ztech(frame_num);
            this.display_selected_frame();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            string path = Directory.GetCurrentDirectory();
            if (path.EndsWith("MKDD_Hexer") == false)
            {
                // Go back 2 directories from here
                path = Directory.GetParent(path).FullName;
                path = Directory.GetParent(path).FullName;
            }
            OFD.InitialDirectory = path;
            OFD.Filter = "*.dtm|*.dtm";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                DTM_Handler.read_file(OFD.FileName);
                this.display_selected_frame();

                System.Console.WriteLine("Loaded File: " + OFD.FileName);
                // now I can safely re-enable all of the components
                foreach (Control control in this.Controls)
                    control.Enabled = true;

                this.textBox2.Text = DTM_Handler.filename;
                this.textBox2.Refresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();

            string path = Directory.GetCurrentDirectory();
            if (path.EndsWith("MKDD_Hexer") == false)
            {
                // Go back 2 directories from here
                path = Directory.GetParent(path).FullName;
                path = Directory.GetParent(path).FullName;
            }
            // automatically create a file that appends "_copy" if that's not already the case
            if (DTM_Handler.filename.Contains("_copy") == false)
            {
                SFD.FileName = Path.GetFileNameWithoutExtension(DTM_Handler.filename) + "_copy.dtm";
            }
            SFD.Filter = "*.dtm|*.dtm";

            DialogResult res = SFD.ShowDialog();
            if (res == DialogResult.OK)
            {
                DTM_Handler.save_file(SFD.FileName);
                System.Console.WriteLine("Saved File: " + SFD.FileName);
            }
        }


        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 0] ^= dtm_handler.BUTTON_S;
            DTM_Handler.content[frame_address + 0 + 0x10] = DTM_Handler.content[frame_address + 0];
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 0] ^= dtm_handler.BUTTON_A;
            DTM_Handler.content[frame_address + 0 + 0x10] = DTM_Handler.content[frame_address + 0];
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 0] ^= dtm_handler.BUTTON_B;
            DTM_Handler.content[frame_address + 0 + 0x10] = DTM_Handler.content[frame_address + 0];
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 0] ^= dtm_handler.BUTTON_X;
            DTM_Handler.content[frame_address + 0 + 0x10] = DTM_Handler.content[frame_address + 0];
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 0] ^= dtm_handler.BUTTON_Y;
            DTM_Handler.content[frame_address + 0 + 0x10] = DTM_Handler.content[frame_address + 0];
        }
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 0] ^= dtm_handler.BUTTON_Z;
            DTM_Handler.content[frame_address + 0 + 0x10] = DTM_Handler.content[frame_address + 0];
        }
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 0] ^= dtm_handler.BUTTON_DU;
            DTM_Handler.content[frame_address + 0 + 0x10] = DTM_Handler.content[frame_address + 0];
        }
        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 0] ^= dtm_handler.BUTTON_DD;
            DTM_Handler.content[frame_address + 0 + 0x10] = DTM_Handler.content[frame_address + 0];
        }
        private void dl1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 1] ^= dtm_handler.BUTTON_DL;
            DTM_Handler.content[frame_address + 1 + 0x10] = DTM_Handler.content[frame_address + 1];
        }
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 1] ^= dtm_handler.BUTTON_DR;
            DTM_Handler.content[frame_address + 1 + 0x10] = DTM_Handler.content[frame_address + 1];
        }
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 1] ^= dtm_handler.BUTTON_L;
            DTM_Handler.content[frame_address + 1 + 0x10] = DTM_Handler.content[frame_address + 1];
        }
        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 1] ^= dtm_handler.BUTTON_R;
            DTM_Handler.content[frame_address + 1 + 0x10] = DTM_Handler.content[frame_address + 1];
        }


        private void s2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 8] ^= dtm_handler.BUTTON_S;
            DTM_Handler.content[frame_address + 8 + 0x10] = DTM_Handler.content[frame_address + 8];
        }
        private void a2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 8] ^= dtm_handler.BUTTON_A;
            DTM_Handler.content[frame_address + 8 + 0x10] = DTM_Handler.content[frame_address + 8];
        }
        private void b2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 8] ^= dtm_handler.BUTTON_B;
            DTM_Handler.content[frame_address + 8 + 0x10] = DTM_Handler.content[frame_address + 8];
        }
        private void x2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 8] ^= dtm_handler.BUTTON_X;
            DTM_Handler.content[frame_address + 8 + 0x10] = DTM_Handler.content[frame_address + 8];
        }
        private void y2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 8] ^= dtm_handler.BUTTON_Y;
            DTM_Handler.content[frame_address + 8 + 0x10] = DTM_Handler.content[frame_address + 8];
        }
        private void z2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 8] ^= dtm_handler.BUTTON_Z;
            DTM_Handler.content[frame_address + 8 + 0x10] = DTM_Handler.content[frame_address + 8];
        }
        private void du2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 8] ^= dtm_handler.BUTTON_DU;
            DTM_Handler.content[frame_address + 8 + 0x10] = DTM_Handler.content[frame_address + 8];
        }
        private void dd2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 8] ^= dtm_handler.BUTTON_DD;
            DTM_Handler.content[frame_address + 8 + 0x10] = DTM_Handler.content[frame_address + 8];
        }
        private void dl2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 9] ^= dtm_handler.BUTTON_DL;
            DTM_Handler.content[frame_address + 9 + 0x10] = DTM_Handler.content[frame_address + 9];
        }
        private void dr2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 9] ^= dtm_handler.BUTTON_DR;
            DTM_Handler.content[frame_address + 9 + 0x10] = DTM_Handler.content[frame_address + 9];
        }
        private void l2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 9] ^= dtm_handler.BUTTON_L;
            DTM_Handler.content[frame_address + 9 + 0x10] = DTM_Handler.content[frame_address + 9];
        }
        private void r2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int) this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 9] ^= dtm_handler.BUTTON_R;
            DTM_Handler.content[frame_address + 9 + 0x10] = DTM_Handler.content[frame_address + 9];
        }






        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }


        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 4] = (byte) ana1x.Value;
            DTM_Handler.content[frame_address + 4 + 0x10] = DTM_Handler.content[frame_address + 4];
            this.trackBar2.Value = (int)ana1x.Value;
            this.trackBar2.Refresh();
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 4] = (byte) trackBar2.Value;
            DTM_Handler.content[frame_address + 4 + 0x10] = DTM_Handler.content[frame_address + 4];
            this.ana1x.Value = (int)trackBar2.Value;
            this.ana1x.Refresh();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 5] = (byte)numericUpDown5.Value;
            DTM_Handler.content[frame_address + 5 + 0x10] = DTM_Handler.content[frame_address + 5];
            this.trackBar4.Value = (int)numericUpDown5.Value;
            this.trackBar4.Refresh();
        }
        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 5] = (byte)trackBar4.Value;
            DTM_Handler.content[frame_address + 5 + 0x10] = DTM_Handler.content[frame_address + 5];
            this.numericUpDown5.Value = (int)trackBar4.Value;
            this.numericUpDown5.Refresh();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 6] = (byte)numericUpDown3.Value;
            DTM_Handler.content[frame_address + 6 + 0x10] = DTM_Handler.content[frame_address + 6];
            this.trackBar1.Value = (int)numericUpDown3.Value;
            this.trackBar1.Refresh();
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 6] = (byte)trackBar1.Value;
            DTM_Handler.content[frame_address + 6 + 0x10] = DTM_Handler.content[frame_address + 6];
            this.numericUpDown3.Value = (int)trackBar1.Value;
            this.numericUpDown3.Refresh();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 7] = (byte)numericUpDown4.Value;
            DTM_Handler.content[frame_address + 7 + 0x10] = DTM_Handler.content[frame_address + 7];
            this.trackBar3.Value = (int)numericUpDown4.Value;
            this.trackBar3.Refresh();
        }
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 7] = (byte)trackBar3.Value;
            DTM_Handler.content[frame_address + 7 + 0x10] = DTM_Handler.content[frame_address + 7];
            this.numericUpDown4.Value = (int)trackBar3.Value;
            this.numericUpDown4.Refresh();
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 12] = (byte)numericUpDown9.Value;
            DTM_Handler.content[frame_address + 12 + 0x10] = DTM_Handler.content[frame_address + 12];
            this.trackBar8.Value = (int)numericUpDown9.Value;
            this.trackBar8.Refresh();
        }
        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 12] = (byte)trackBar8.Value;
            DTM_Handler.content[frame_address + 12 + 0x10] = DTM_Handler.content[frame_address + 12];
            this.numericUpDown9.Value = (int)trackBar8.Value;
            this.numericUpDown9.Refresh();
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 13] = (byte)numericUpDown7.Value;
            DTM_Handler.content[frame_address + 13 + 0x10] = DTM_Handler.content[frame_address + 13];
            this.trackBar6.Value = (int)numericUpDown7.Value;
            this.trackBar6.Refresh();
        }
        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 13] = (byte)trackBar6.Value;
            DTM_Handler.content[frame_address + 13 + 0x10] = DTM_Handler.content[frame_address + 13];
            this.numericUpDown7.Value = (int)trackBar6.Value;
            this.numericUpDown7.Refresh();
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 14] = (byte)numericUpDown8.Value;
            DTM_Handler.content[frame_address + 14 + 0x10] = DTM_Handler.content[frame_address + 14];
            this.trackBar7.Value = (int)numericUpDown8.Value;
            this.trackBar7.Refresh();
        }
        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 14] = (byte)trackBar7.Value;
            DTM_Handler.content[frame_address + 14 + 0x10] = DTM_Handler.content[frame_address + 14];
            this.numericUpDown8.Value = (int)trackBar7.Value;
            this.numericUpDown8.Refresh();
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 15] = (byte)numericUpDown6.Value;
            DTM_Handler.content[frame_address + 15 + 0x10] = DTM_Handler.content[frame_address + 15];
            this.trackBar5.Value = (int)numericUpDown6.Value;
            this.trackBar5.Refresh();
        }
        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            if (this.checkboxes_affect_data == false) return;
            int frame_num = (int)this.numericUpDown1.Value;
            int frame_address = dtm_handler.frame_num_2_hex_addr(frame_num);
            DTM_Handler.content[frame_address + 15] = (byte)trackBar5.Value;
            DTM_Handler.content[frame_address + 15 + 0x10] = DTM_Handler.content[frame_address + 15];
            this.numericUpDown6.Value = (int)trackBar5.Value;
            this.numericUpDown6.Refresh();
        }
    }
}
