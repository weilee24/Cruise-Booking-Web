/* 
Course: CSE445
Project: 2
Name:   Wei-Chieh Lee
ASU id: 1220097684
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Assignment_3
{
    public partial class User : Form
    {
        public User()
        {
            InitializeComponent();
            Agent1.Visible = false;
            Agent2.Visible = false;
            Agent3.Visible = false;
            Agent4.Visible = false;
            Agent5.Visible = false;
            Msg1.Visible = false;
        }

        private void Upd_price_Click(object sender, EventArgs e)
        {
            Program.run_program();
            
            // Stop after 30 price updates
            int cnt = 0;
            while (cnt < 30)
            {
                if (MultiCellBuffer.has_order() == false)
                {
                    cnt++;
                }
                else
                {
                    cnt = 0;
                    // Update the texts on the GUI
                    if (Program.sys_msg_out.Count != 0)
                    {
                        string msg = Program.sys_msg_out.Dequeue();
                        set_system_msg(msg);
                    }
                    if (Program.agent_output1.Count != 0)
                    {
                        string msg = Program.agent_output1.Dequeue();
                        set_agent1_text(msg);
                    }
                    if (Program.agent_output2.Count != 0)
                    {
                        string msg = Program.agent_output2.Dequeue();
                        set_agent2_text(msg);
                    }
                    if (Program.agent_output3.Count != 0)
                    {
                        string msg = Program.agent_output3.Dequeue();
                        set_agent3_text(msg);
                    }
                    if (Program.agent_output4.Count != 0)
                    {
                        string msg = Program.agent_output4.Dequeue();
                        set_agent4_text(msg);
                    }
                    if (Program.agent_output5.Count != 0)
                    {
                        string msg = Program.agent_output5.Dequeue();
                        set_agent5_text(msg);
                    }
                    if (Processing.msg1_out.Count != 0)
                    {
                        string msg = Processing.msg1_out.Dequeue();
                        set_Msg1_text(msg);
                    }
                }
                Thread.Sleep(500);
            }

        }
        // Setters of GUI messages
        private void set_system_msg(string text)
        {
            SystemMsg.Text = "System: " + text;
            SystemMsg.Update();
        }
        public void set_agent1_text(string text) 
        {
            Agent1.Visible = true;
            Agent1.Text = text;
            Agent1.Update();
        }
        public void set_agent2_text(string text)
        {
            Agent2.Visible = true;
            Agent2.Text = text;
            Agent2.Update();
        }
        public void set_agent3_text(string text)
        {
            Agent3.Visible = true;
            Agent3.Text = text;
            Agent3.Update();
        }
        public void set_agent4_text(string text)
        {
            Agent4.Visible = true;
            Agent4.Text = text;
            Agent4.Update();
        }
        public void set_agent5_text(string text)
        {
            Agent5.Visible = true;
            Agent5.Text = text;
            Agent5.Update();
        }
        public void set_Msg1_text(string text)
        {
            Msg1.Visible = true;
            Msg1.Text = text;
            Msg1.Update();
        }
    }
}
