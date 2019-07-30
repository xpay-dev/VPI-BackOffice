using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGDAL
{
    public class CodeGenerator
    {
        Random random;

        public CodeGenerator()
        {
            random = new Random();
        }

        public string GenerateCode(int LengOfString)
        {
            string possibleChar = "QAZWSXEDCRFVTGBYHNUJMIKLP123456789";
            decimal randNum;
            StringBuilder builder = new StringBuilder();

            for (var I = 1; I <= LengOfString; I++)
            {
                randNum = random.Next(1, possibleChar.Length);
                string ch = possibleChar.Substring(System.Convert.ToInt32(randNum), 1);
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public string FormatCode(int occurence, string activationCode, string delimiter)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < activationCode.Length; i++)
            {
                if (i % occurence == 0 && i != 0)
                    sb.Append(delimiter);

                sb.Append(activationCode[i]);
            }

            return sb.ToString();
        }
    }
}
