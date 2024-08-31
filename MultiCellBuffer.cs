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
    internal class MultiCellBuffer
    {
        private static Order[] orders;
        private static int index;
        private static Semaphore _pool;


        static MultiCellBuffer()
        {
            _pool = new Semaphore(0, 1);
            _pool.Release(1);
            orders = new Order[2];
            index = 0;
        }

        public static bool set_one_cell(ref Order order)
        {
            _pool.WaitOne();
            orders[index] = order;

            bool is_set = false;
            for (int i = 0; i < 2; i++)
            {
                if (orders[index] != null)
                {
                    orders[index] = order;
                    is_set = true;
                }
            }

            _pool.Release();

            return is_set;
        }

        public static void get_one_cell(out Order order, int cruise_id)
        {

            _pool.WaitOne();
            order = null;
            for (int i = 0; i < 2; i++)
            {
                if (orders[i] != null)
                {
                    if (orders[i].get_receiver_id() == cruise_id)
                    {
                        order = orders[i];
                        orders[i] = null;
                        break;
                    }
                }
            }
            _pool.Release();
        }

        public static bool has_order()
        {
            _pool.WaitOne();
            int cnt = 0;
            for (int i = 0; i < 2; i++)
            {
                if (orders[i] == null)
                {
                    cnt++;
                }
            }
            _pool.Release();


            if (cnt == 2)
                return false;
            else
                return true;
        }
    }
}
