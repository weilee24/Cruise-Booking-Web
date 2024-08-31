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
    internal class PriceModel
    {
        private System.Random rand_num = new System.Random();
        public double get_price()
        {
            // Let price be a number between 40 and 201
            double price = Math.Round((rand_num.NextDouble() * 161 + 40), 2);

            // If 200 <= price < 201, rounds down to 200, else return price
            if (price >= 200) { return 200; }
            else
                return price;
        }
    }
}
