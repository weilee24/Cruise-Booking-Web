/* 
Course: CSE445
Project: 2
Name:   Wei-Chieh Lee
ASU id: 1220097684
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
using System.Runtime.InteropServices;

namespace Assignment_3
{
    public static class Program
    {
        // Define variables
        private static Cruise[] cruises;
        private static Random rand_num = new Random();
        // Define a set of credit card number to pay for tickets
        private static long[] credit_num = {
            3640493918180971,
            0074518302547277,
            8982519850854759,
            2427023401922153,
            5710695744466654,
            9102068220263125,
            1331768380374201,
            8990131700637719,
            3893411070204109,
            6837033375981720
        };
        // Create three cruises and 5 ticket agents(threads);
        public static int cruise_number = 3;
        public static int ticket_agent_number = 5;
        public static User user;
        
        // Define queues that connects with Form1.cs
        public static Queue<string> sys_msg_out;
        public static Queue<string> agent_output1;
        public static Queue<string> agent_output2;
        public static Queue<string> agent_output3;
        public static Queue<string> agent_output4;
        public static Queue<string> agent_output5;

        struct ticket_data_level1
        {
            public int thread_id;
            public Agents t;
        }

        struct ticket_data_level2
        {
            public Agents t;
            public int thread_id;
            public int cruise_id;

        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            user = new User();
            // Create queues to store values and sent access them in Form1.cs
            sys_msg_out = new Queue<string>();
            agent_output1 = new Queue<string>();
            agent_output2 = new Queue<string>();
            agent_output3 = new Queue<string>();
            agent_output4 = new Queue<string>();
            agent_output5 = new Queue<string>();

            Application.Run(user);
        }

        public static void run_program()
        {
            cruises = new Cruise[cruise_number];
            for (int i = 0; i < cruise_number; i++)
            {
                cruises[i] = new Cruise(i);
            }

            Agents tt = new Agents();
            Agents[] ticket_agents = new Agents[ticket_agent_number];

            // Create agent objects
            for (int i = 0; i < ticket_agent_number; i++)
            {
                ticket_agents[i] = new Agents();
            }

            // Print messages
            for (int i = 0; i < cruise_number; i++)
            {
                for (int j = 0; j < ticket_agent_number; j++)
                {
                    cruises[i].set_cruise_callback(ticket_agents[j].get_cruise_price);
                    int id = rand_num.Next(0, 10);
                    cruises[i].register_credit_card(credit_num[id]);

                    sys_msg_out.Enqueue("Agent" + j + " subscribed to Cruise" + (i+1));
                }
            }

            // Create threads
            for (int i = 0; i < ticket_agent_number; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(book_ticket));
                ticket_data_level1 t1 = new ticket_data_level1();
                t1.thread_id = i;
                t1.t = ticket_agents[i];
                t.Start(t1);
            }

            for (int i = 0; i < cruise_number; i++)
            {
                Thread thread_cruise_price_update = new Thread(cruise_update_price);
                Thread thread_cruise_handle_order = new Thread(cruise_get_cell);

                thread_cruise_price_update.Start(cruises[i]);
                Thread.Sleep(6000);
                thread_cruise_handle_order.Start(cruises[i]);
            }
        }
        // The method that gets random numbers as new prices
        private static void cruise_update_price(object cruise)
        {
            dynamic c = (Cruise)cruise;
            bool loop_continue = true;
            while (loop_continue == true)
            {
                Monitor.Enter(c);
                try
                {
                    c.get_new_price();
                }
                finally
                {
                    Monitor.Exit(c);
                }

                Thread.Sleep(rand_num.Next(1000, 3000));

                Monitor.Enter(c);
                try
                {
                    loop_continue = c.to_be_updated();
                }
                finally
                {
                    Monitor.Exit(c);
                }
            }
        }

        // Buffer -> Cruise
        private static void cruise_get_cell(object ob)
        {
            dynamic c = (Cruise)ob;
            int cnt = 0;
            while (cnt < 30)
            {
                if (MultiCellBuffer.has_order() == false)
                {
                    cnt++;
                }
                else
                {
                    c.get_order();
                    c.create_processing();
                    cnt = 0;
                }
                Thread.Sleep(1000);
            }
        }

        private static void book_ticket(object ob)
        {
            ticket_data_level1 data = (ticket_data_level1)ob;
            dynamic agent = data.t;
            int num = data.thread_id;
                // Create threads based on cruise number and execute them 
                for (int i = 0; i < cruise_number; i++)
                {
                    // Assign callback function
                    Thread thread = new Thread(new ParameterizedThreadStart(ticket_agent_action));
                    ticket_data_level2 data2 = new ticket_data_level2();
                    data2.t = agent;
                    data2.thread_id = num;
                    data2.cruise_id = i;

                    thread.Start(data2);
                }
        }

        private static void ticket_agent_action(object ob)
        {
            ticket_data_level2 data = (ticket_data_level2)ob;
            dynamic agent = data.t;
            dynamic cruise_id = data.cruise_id;
            bool loop_continue = true;
            int num = data.thread_id;

            while (loop_continue == true)
            {
                if (agent.discount_price(cruise_id) && agent.change_current_price(cruise_id) > 1)
                {
                    dynamic price = agent.change_current_price(cruise_id);
                    dynamic id = rand_num.Next(0, 10);
                    Order order = new Order();
                    order.set_sender_id(num.ToString());
                    order.set_card_num(credit_num[id]);
                    order.set_quantity(rand_num.Next(1, 10));
                    order.set_unit_price(price);
                    order.set_receiver_id(cruise_id);
                    agent.buy_ticket(ref order);
                    
                    // Store messages in the queues and access them in Form1.cs
                    if (num == 0)
                    {
                        string txt = "Agent 1 ordered " + order.get_quantity().ToString() + " ticket(s) from Cruise " + (cruise_id+1).ToString() + ". The total price is " + price.ToString() + " paid with credit card: " + credit_num[id].ToString();
                        agent_output1.Enqueue(txt);
                    }
                    else if (num == 1)
                    {
                        string txt = "Agent 2 ordered " + order.get_quantity().ToString() + " ticket(s) from Cruise " + (cruise_id + 1).ToString() + ". The total price is " + price.ToString() + " paid with credit card: " + credit_num[id].ToString();
                        agent_output2.Enqueue(txt);
                    }
                    else if (num == 2)
                    {
                        string txt = "Agent 3 ordered " + order.get_quantity().ToString() + " ticket(s) from Cruise " + (cruise_id + 1).ToString() + ". The total price is " + price.ToString() + " paid with credit card: " + credit_num[id].ToString();
                        agent_output3.Enqueue(txt);
                    }
                    else if (num == 3)
                    {
                        string txt = "Agent 4 ordered " + order.get_quantity().ToString() + " ticket(s) from Cruise " + (cruise_id + 1).ToString() + ". The total price is " + price.ToString() + " paid with credit card: " + credit_num[id].ToString();
                        agent_output4.Enqueue(txt);
                    }
                    else if (num == 4)
                    {
                        string txt = "Agent 5 ordered " + order.get_quantity().ToString() + " ticket(s) from Cruise " + (cruise_id + 1).ToString() + ". The total price is " + price.ToString() + " paid with credit card: " + credit_num[id].ToString();
                        agent_output5.Enqueue(txt);
                    }
                }
                Thread.Sleep(rand_num.Next(1000, 3000));

                Monitor.Enter(cruises[cruise_id]);
                try
                {
                    // Loop continues until there is no update
                    loop_continue = cruises[cruise_id].to_be_updated();
                }
                finally
                {
                    Monitor.Exit(cruises[cruise_id]);
                }
            }

            sys_msg_out.Enqueue("Agent" + (data.thread_id+1) + " stopped buying ticket from Cruise" + (cruise_id+1));
        }
    }
}
