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
    // https://tasvideos.org/EmulatorResources/Dolphin/DTM
    public class dtm_handler
    {
        public string filename;
        public byte[] content;
        public int frame_count;
        public long rerec_count;

        // byte #0
        public static byte BUTTON_S = 0b0000_0001;
        public static byte BUTTON_A = 0b0000_0010;
        public static byte BUTTON_B = 0b0000_0100;
        public static byte BUTTON_X = 0b0000_1000;
        public static byte BUTTON_Y = 0b0001_0000;
        public static byte BUTTON_Z = 0b0010_0000;
        public static byte BUTTON_DU = 0b0100_0000;
        public static byte BUTTON_DD = 0b1000_0000;
        // byte #1
        public static byte BUTTON_DL = 0b0000_0001;
        public static byte BUTTON_DR = 0b0000_0010;
        public static byte BUTTON_L = 0b0000_0100;
        public static byte BUTTON_R = 0b0000_1000;
        public static byte SPECIAL_DISC = 0b0001_0000;
        public static byte SPECIAL_RESET = 0b0010_0000;
        public static byte SPECIAL_CONN = 0b0100_0000;
        public static byte SPECIAL_ANA = 0b1000_0000;

        public void apply_ztech(int start_frame)
        {
            // lets keep count
            this.rerec_count++;

            int start_address = frame_num_2_hex_addr(start_frame);
            Console.WriteLine(String.Format("[Z-Teching] Starting Address: 0x{0:X8}", start_address));

            // first check if Z is being pressed on this frame, and if so UNDO the ztech
            if ((this.content[start_address + 0x00] & BUTTON_Z) > 0 && (this.content[start_address + 0x08] & BUTTON_Z) > 0)
            {
                Console.WriteLine(String.Format("[Z-Teching] Undoing Z-Tech..."));
                this.content[start_address + 0x00] -= (byte)(this.content[start_address + 0x00] & BUTTON_Z);
                this.content[start_address + 0x08] -= (byte)(this.content[start_address + 0x08] & BUTTON_Z);
                this.content[start_address + 0x10] -= (byte)(this.content[start_address + 0x10] & BUTTON_Z);
                this.content[start_address + 0x18] -= (byte)(this.content[start_address + 0x18] & BUTTON_Z);
            }
            else
            {
                Console.WriteLine(String.Format("[Z-Teching] Inserting Z-Tech..."));
                this.content[start_address + 0x00] |= BUTTON_Z;
                this.content[start_address + 0x08] |= BUTTON_Z;
                this.content[start_address + 0x10] |= BUTTON_Z;
                this.content[start_address + 0x18] |= BUTTON_Z;
            }

            // swap the first 8 with the second 8 bytes in every line
            byte buffer;
            for (int i = start_address; i < this.content.Length; i += 0x10)
            {
                // Console.WriteLine(String.Format("cur Address: 0x{0:X8}", i));
                for (int k = 0; k < 0x08; k++)
                {
                    buffer = this.content[i + k];
                    this.content[i + k] = this.content[i + k + 0x08];
                    this.content[i + k + 0x08] = buffer;
                }
            }
            Console.WriteLine(String.Format("[Z-Teching] Done!"));
        }
        public void insert_frame_copy_at(int start_frame)
        {
            // lets keep count
            this.rerec_count++;

            int start_address = frame_num_2_hex_addr(start_frame);
            Console.WriteLine(String.Format("[Insert Copy] Starting Address: 0x{0:X8}", start_address));

            byte[] buffer_A = new byte[0x10];
            byte[] buffer_B = new byte[0x10];
            for (int k = 0; k < 0x10; k++)
            {
                // put the current frame into a first buffer
                buffer_A[k] = this.content[start_address + k];
            }
            // then we swap between the 2 buffers to shift everything back by 0.5 frames (0x10)
            for (int i = start_address; (i + 0x20) < this.content.Length; i += 0x20)
            {
                for (int k = 0; k < 0x10; k++)
                {
                    // put the next frame into a buffer
                    buffer_B[k] = this.content[i + k + 0x10];
                    // overwrite the next frame with the older buffer
                    this.content[i + k + 0x10] = buffer_A[k];

                    // put the next frame into a different buffer
                    buffer_A[k] = this.content[i + k + 0x20];
                    // overwrite the next frame with the first buffer
                    this.content[i + k + 0x20] = buffer_B[k];
                }
            }
            Console.WriteLine(String.Format("[Insert Copy] Done!"));
        }
        public void remove_frame_at(int start_frame)
        {
            // lets keep count
            this.rerec_count++;

            int start_address = frame_num_2_hex_addr(start_frame);
            Console.WriteLine(String.Format("[Remove Frame] Starting Address: 0x{0:X8}", start_address));

            // this one doesnt need any buffers
            for (int i = start_address; (i+0x10) < this.content.Length; i++)
                this.content[i] = this.content[i + 0x10];

            Console.WriteLine(String.Format("[Remove Frame] Done!"));
        }


        public byte[] read_framedata(int frame_num)
        {
            // lets keep count
            this.rerec_count++;

            int frame_address = frame_num_2_hex_addr(frame_num);
            byte[] data = new byte[0x10];
            for (int i = 0; i < 0x10; i++)
                data[i] = this.content[frame_address + i];
            return data;
        }
        public byte[] write_framedata(int frame_num, byte[] data)
        {
            // lets keep count
            this.rerec_count++;

            int frame_address = frame_num_2_hex_addr(frame_num);
            for (int i = 0; i < 0x10; i++)
            {
                this.content[frame_address + i] = data[i];
                this.content[frame_address + i + 0x10] = data[i];
            }
            return data;
        }

        public void print_framedata(int frame_num, byte[] data)
        {
            int frame_address = frame_num_2_hex_addr(frame_num);
            Console.Write(String.Format("{0:X8}", frame_address) + "  :  ");

            for (int i = 0; i < 0x10; i++)
            {
                if (i % 0x04 == 0) Console.Write(" | ");
                Console.Write(data[i].ToString("X2") + " ");
            }
            Console.Write("\n");
        }

        public void read_file(string filename)
        {
            try
            {
                // Open the binary file using a FileStream
                using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    // update internal filename
                    this.filename = filename;

                    // Create a byte array to hold the binary data
                    this.content = new byte[fileStream.Length];
                    // Read the entire file into the buffer
                    int bytesRead = fileStream.Read(this.content, 0, this.content.Length);

                    // Process the binary data here (e.g., convert to a string or work with the bytes directly)
                    // For demonstration purposes, we'll just print the first few bytes as hexadecimal values
                    for (int i = 0; i < this.content.Length; i++)
                    {
                        if (i % 0x10 == 0)
                        {
                            Console.Write(String.Format("{0:X8}", i) + "  :  ");
                        }
                        else if (i % 0x04 == 0)
                        {
                            Console.Write(" | ");
                        }
                        Console.Write(this.content[i].ToString("X2") + " ");
                        if ((i+1) % 0x10 == 0)
                        {
                            Console.Write("\n");
                        }

                        if (i > 0x200) break;
                    }

                    this.frame_count = (this.content.Length - 0x100) / 0x20;
                    this.rerec_count = (this.content[0x2F] << 0x10) + (this.content[0x2E] << 0x08) + (this.content[0x2D] << 0x00);
                    Console.WriteLine("Rerecs: " + this.rerec_count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        public void save_file(string filename)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                // update internal filename
                this.filename = filename;

                fileStream.Write(this.content, 0, this.content.Length);
            }
        }

        public static int frame_num_2_hex_addr(int frame_num)
        {
            return 0x100 + (frame_num * 0x20);
        }

    }
}
