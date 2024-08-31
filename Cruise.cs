/* 
Course: CSE445
Project: 2
Name:   Wei-Chieh Lee
ASU id: 1220097684
*/

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Assignment_3
{
    internal class Cruise
    {
        // Define a callback function that takes price of different cruises
        public delegate void Cruise_callback(double money, int cruise_id);
        public List<Cruise_callback> cruise_cb;

        // Variables
        private bool price_update;
        private double previous_price, current_price;
        private int counter;
        private HashSet<long> register_credit_num;
        private PriceModel price_mod;
        private int cruise_id;
        private double tax;
        private double location_charge;
        private static Random rand_num;
        Order order;

        struct order_data
        {
            public double tax;
            public double location_charge;
            public HashSet<long> register_credit_num;
            public Order order;
        }

        public Cruise(int cruise_id)
        {
            rand_num = new Random();
            tax = 10 * rand_num.NextDouble();
            location_charge = 30 * rand_num.NextDouble();

            price_mod = new PriceModel();
            register_credit_num = new HashSet<long>();
            cruise_cb = new List<Cruise_callback>();
            previous_price = 0.0;
            current_price = 0.0;
            counter = 0;
            price_update = true;
            this.cruise_id = cruise_id;
        }

        public bool to_be_updated() { 
            return price_update;
        }
        // Set callback function
        public void set_cruise_callback(Cruise_callback callback_function) { 
            cruise_cb.Add(callback_function);
        }

        public void get_new_price() {
            dynamic new_price = price_mod.get_price();

            // Update price
            previous_price = current_price;
            current_price = new_price;

            // Check if the price is reduced
            if (previous_price > current_price) {
                counter++;
                if (counter == 20)
                {
                    price_update = false;
                    Console.WriteLine("Cruise{0} stop to update price", cruise_id);
                    Console.WriteLine();
                }
            }

            foreach (dynamic item in cruise_cb)
            {
                item(new_price, cruise_id);
            }
        }

        public void get_order()
        {
            Order order;
            MultiCellBuffer.get_one_cell(out order, cruise_id);
            this.order = order;
        }
        public void create_processing()
        {
            if (this.order != null)
            {
                order_data data = new order_data();
                data.tax = this.tax;
                data.location_charge = this.location_charge;
                data.register_credit_num = this.register_credit_num;
                data.order = order;

                Thread t = new Thread(new ParameterizedThreadStart(order_threads));
                t.Start(data);
            }
        }
        private static void order_threads(object ob)
        {
            Thread.Sleep(rand_num.Next(1000, 3000));
            order_data data = (order_data)ob;
            dynamic tax = data.tax;
            dynamic location_charge = data.location_charge;
            dynamic register_credit_num = data.register_credit_num;
            dynamic order = data.order;

            Processing order_processing = new Processing();
            order_processing.set_tax(tax);
            order_processing.set_location_charge(location_charge);
            order_processing.set_register_credit_card(register_credit_num);
            order_processing.handle_order(order);

        }
        public void register_credit_card(long credit_num)
        {
            register_credit_num.Add(credit_num);
        }
    }
}
