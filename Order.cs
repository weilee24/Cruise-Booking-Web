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
using System.Threading.Tasks;

namespace Assignment_3
{
    internal class Order
    {
        private string sender_id;
        private long card_num;
        private int quantity;
        private int receiver_id;
        private double unit_price;

        public Order()
        {

        }
        // Constructor
        public Order(string sender_id, long card_num, int receiver_id, int quantity, double unit_price)
        {
            this.sender_id = sender_id;
            this.card_num = card_num;
            this.quantity = quantity;
            this.receiver_id = receiver_id;
            this.unit_price = unit_price;
        }
        
        // Setters and getters
        public void set_receiver_id(int receiver_id)
        {
            this.receiver_id = receiver_id;
        }

        public int get_receiver_id()
        {
            return this.receiver_id;
        }

        public void set_sender_id(string sender_id)
        {
            this.sender_id = sender_id;
        }

        public string get_sender_id()
        {
            return sender_id;
        }

        public void set_card_num(long card_num)
        {
            this.card_num = card_num;
        }

        public long get_card_num()
        {
            return card_num;
        }

        public void set_quantity(int quantity)
        {
            this.quantity = quantity;
        }

        public int get_quantity()
        {
            return quantity;
        }

        public void set_unit_price(double unit_price)
        {
            this.unit_price = unit_price;
        }

        public double get_unit_price()
        {
            return unit_price;
        }
    }
}
