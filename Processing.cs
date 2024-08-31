/* 
Course: CSE445
Project: 2
Name:   Wei-Chieh Lee
ASU id: 1220097684
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment_3
{
    internal class Processing
    {
        private HashSet<long> register_credit_card_num;
        private double tax;
        private double location_charge;
        private static int order_num;
        private static ReaderWriterLock read_write_lock;
        public static Queue<string> msg1_out = new Queue<string>();

        // Constructor
        static Processing()
        {
            order_num = 0;
            read_write_lock = new ReaderWriterLock();
        }
        // Setters
        public void set_tax(double t)
        {
            tax = t;
        }

        public void set_location_charge(double l)
        {
            location_charge = l;
        }

        public void set_register_credit_card(HashSet<long> credit)
        {
            register_credit_card_num = credit;
        }
        // Select credit card randomly and decide whether it is valid
        private bool is_valid(long credit_card)
        {
            if (register_credit_card_num.Contains(credit_card) == false)
            {
                return false;
            }
            return true;
        }

        public void handle_order(Order order)
        {
            dynamic sender_id = order.get_sender_id();
            dynamic credit_card = order.get_card_num();
            dynamic receiver_id = order.get_receiver_id();
            dynamic quantity = order.get_quantity();
            dynamic unit_price = order.get_unit_price();

            int no;
            read_write_lock.AcquireWriterLock(300);
            try
            {
                no = order_num;
                order_num++;
            }
            finally
            {
                read_write_lock.ReleaseLock();
            }

            // Error message for invalid credit cards
            if (!is_valid(credit_card))
            {
                msg1_out.Enqueue("Order #" + no + " failed(X)\n"
                    + "Credit card " + credit_card + " hasn't registered."
                    + "\nOrder ticket agent: " + (sender_id + 1)
                    + ".\nOrder reciver Id: " + (receiver_id + 1)
                    + ".\nOrder quantity: " + quantity
                    + ".\nOrder unit price: " + unit_price + ".");
                return;
            }

            // Message for valid purchases
            double totalAmount = unit_price * quantity + tax + location_charge;
            msg1_out.Enqueue("Order #" + no + " established(O)\n"
                    + "Agent" + sender_id + " buy Cruise" + receiver_id + ", credit card is " + credit_card
                    + "\nOrder ticket agent: " + (sender_id+1)
                    + ".\nOrder reciver Id: " + (receiver_id+1)
                    + ".\nOrder quantity: " + quantity
                    + ".\nOrder unit price: " + unit_price
                    + ".\nThe total amount is " + unit_price + "(price/ticket) * " + quantity + "(quantity) + " 
                    + Math.Round(tax, 2) + "(tax) + " + Math.Round(location_charge, 2) + "(location charge) = " 
                    + Math.Round(totalAmount, 2) + ".");
        }
    }
}
