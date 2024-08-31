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
    internal class Agents
    {
        private List<double> previous_price, current_price, price_sum;
        private List<int> price_count;
        private ReaderWriterLock read_write_lock;

        public Agents()
        {
            previous_price = new List<double>();
            current_price = new List<double>();
            price_sum = new List<double>();
            price_count = new List<int>();

            for (int i = 0; i < Program.cruise_number; i++)
            {
                previous_price.Add(0.0);
                current_price.Add(0.0);
                price_sum.Add(0.0);
                price_count.Add(0);
            }

            read_write_lock = new ReaderWriterLock();
        }
        public void get_cruise_price(double price, int cruise_id)
        {
            read_write_lock.AcquireWriterLock(300);
            try
            {
                current_price[cruise_id] = price;
                previous_price[cruise_id] = current_price[cruise_id];
                price_sum[cruise_id] += price;
                price_count[cruise_id] += 1;
            }
            finally
            {
                read_write_lock.ReleaseWriterLock();
            }
        }

        public bool discount_price(int cruise_id)
        {
            bool discount = false;

            read_write_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (price_sum[cruise_id] / price_count[cruise_id] < current_price[cruise_id] && previous_price[cruise_id] > current_price[cruise_id])
                {
                    discount = false;
                }
                else
                {
                    discount = true;
                }
            }
            finally
            {
                read_write_lock.ReleaseLock();
            }
            return discount;
        }

        public double change_current_price(int cruise_id)
        {
            double p;
            read_write_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                p = current_price[cruise_id];
            }
            finally
            {
                read_write_lock.ReleaseLock();
            }
            return p;

        }

        public double average_price(int cruise_id)
        {
            double avg_price;

            read_write_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (price_count[cruise_id] < 1)
                {
                    return 0.0;
                }
                avg_price = price_sum[cruise_id] / price_count[cruise_id];
            }
            finally
            {
                read_write_lock.ReleaseLock();
            }


            return avg_price;
        }
        public void buy_ticket(ref Order order)
        {
            while (MultiCellBuffer.set_one_cell(ref order) == false) { };
        }
    }
}
