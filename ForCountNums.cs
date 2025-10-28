using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotSimple
{
    static class ForCountNums
    {

        public static int CountNums(string originString)
        {
            int sum = 0;
            string[] numsMas = originString.Split(" ");
            for (int i = 0; i < numsMas.Length; i++)
            {
                sum += Convert.ToInt32(numsMas[i]);
            }
            return sum;
        }

    }
}
