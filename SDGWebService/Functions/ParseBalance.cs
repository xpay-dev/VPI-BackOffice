using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDGDAL;

namespace SDGWebService.Functions
{
    public class ParseBalance
    {
        public static BalanceInquiry ParseBalanceInquiry(string amountToParse)
        {
            BalanceInquiry response = new BalanceInquiry();

            #region Parse Current Balance

            string CurrentBalData = amountToParse.Substring(0, 20);
            int CurrentCurrency = Convert.ToInt32(CurrentBalData.Substring(4, 3));

            string CurrentAmount = CurrentBalData.Substring(8, 10);
            string CurrentAmountDecimal = CurrentBalData.Substring(10, 2);
            string CurrfinalAmount = Convert.ToString(Convert.ToDecimal(CurrentAmount) + "." + CurrentAmountDecimal);

            response.CurrentCurrCode = Convert.ToString((SDGDAL.Enums.CurrencyISOCode)CurrentCurrency);
            response.CurrentBalance = CurrfinalAmount;

            #endregion

            #region Parse Available Balance

            string AvailBal = amountToParse.Substring(20, 20);
            int AvailCurrency = Convert.ToInt32(AvailBal.Substring(4, 3));

            string AmountAvail = AvailBal.Substring(8, 10);
            string AvailAmountDecimal = AvailBal.Substring(10, 2);
            string AvailfinalAmount = Convert.ToString(Convert.ToDecimal(AmountAvail) + "." + AvailAmountDecimal);

            response.AvailableCurrCode = Convert.ToString((SDGDAL.Enums.CurrencyISOCode)AvailCurrency);
            response.AvailableBalance = AvailfinalAmount;

            #endregion

            return response;
        }

        public class BalanceInquiry
        {
            public string CurrentCurrCode { get; set; }
            public string CurrentBalance { get; set; }
            public string AvailableCurrCode { get; set; }
            public string AvailableBalance { get; set; }
        }
    }
}