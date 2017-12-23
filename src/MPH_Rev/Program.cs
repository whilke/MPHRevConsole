using Deedle;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace MPH_Rev
{

    #region JSON data contracts
    class Amount
    {
        public double amount { get; set; }
    }

    class Balance
    {
        public double confirmed { get; set; }
        public double unconfirmed { get; set; }
        public double orphaned { get; set; }
    }

    class Data
    {
        public Balance balance_for_auto_exchange { get; set; }
        public string balance_on_exchange { get; set; }
        public Amount recent_credits_24hours { get; set; }
    }

    class DashboardData
    {
        public Data data { get; set; }
    }

    class MPHWrapper
    {
        public DashboardData getdashboarddata { get; set; }
    }
    #endregion

    class CoinEntry
    {
        public string Name;
        public string Symbol;
        public bool isConverted;

        public CoinEntry(string n, string s)
        {
            Name = n;
            Symbol = s;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tip jar");
            Console.WriteLine("BTC: 1tfZDwHYQiKVfXNJzY1339uTquiknmsq7");
            Console.WriteLine("BCH: 1657vgUNN2f7PzY1s9jcVrAY5NpCT2MyLt");
            Console.WriteLine("ETH: 0xf89d8344D615F3F918c47Bf45B327B3b3816baaF");
            Console.WriteLine("LTC: LSCDvpuT8xZTnAZtgrLtTmDvi9ebV4Brb2");
            Console.WriteLine("");

            if (args.Length < 3)
            {
                Console.WriteLine("MPHRevConsole.exe <autoconvert_coinname> <currency> <MPH_API>");
                Console.WriteLine("i.e. MPH_earnings.exe litecoin USD <API> - show 24h earnings in US dollars with LTC as your converted coin");
                Console.WriteLine("i.e. MPH_earnings.exe digibyte-skein BTC <API> - show 24h earnings in bitcoin with DBG as your converted coin");
                return;
            }

            var autoCoin = args[0];
            var currencySymbol = args[1];
            string apiKey = args[2];
            int waitTimeMinutes = 1;

            var coins = CreateCoins(autoCoin);

            Console.WriteLine("Last 24 hour earnings | Currency | Timestamp");

            while(true)
            {
                double total24hEarnings = 0d;
                foreach (var coin in coins)
                {
                    var d = Calc24hEarnings(coin.Name, apiKey, coin.isConverted);
                    if (d > 0d)
                    {
                        d = ConvertToCurrency(d, coin.Symbol, currencySymbol);
                    }
                    total24hEarnings += d;
                }

                total24hEarnings = RoundEarnings(total24hEarnings);

                Console.WriteLine($"{total24hEarnings,21} | {currencySymbol,8} | {DateTime.Now.ToString()}");

                Thread.Sleep(1000 * 60 * waitTimeMinutes);

            }


        }


        static List<CoinEntry> CreateCoins(string autoConvertCoin)
        {
            List<CoinEntry> coins = new List<CoinEntry>();
            coins.Add(new CoinEntry("adzcoin", "ADZ"));
            coins.Add(new CoinEntry("auroracoin-qubit", "AUR"));
            coins.Add(new CoinEntry("bitcoin", "BTC"));
            coins.Add(new CoinEntry("bitcoin-cash", "BCC"));
            coins.Add(new CoinEntry("bitcoin-gold", "BTG"));
            coins.Add(new CoinEntry("dash", "DASH"));
            coins.Add(new CoinEntry("digibyte-groestl", "DGB"));
            coins.Add(new CoinEntry("digibyte-qubit", "DGB"));
            coins.Add(new CoinEntry("digibyte-skein", "DGB"));
            coins.Add(new CoinEntry("electroneum", "ETN"));
            coins.Add(new CoinEntry("ethereum", "ETH"));
            coins.Add(new CoinEntry("ethereum-classic", "ETC"));
            coins.Add(new CoinEntry("expanse", "EXP"));
            coins.Add(new CoinEntry("feathercoin", "FTC"));
            coins.Add(new CoinEntry("gamecredits", "GAME"));
            coins.Add(new CoinEntry("geocoin", "GEO"));
            coins.Add(new CoinEntry("globalboosty", "BSTY"));
            coins.Add(new CoinEntry("groestlcoin", "GRS"));
            coins.Add(new CoinEntry("litecoin", "LTC"));
            coins.Add(new CoinEntry("maxcoin", "MAX"));
            coins.Add(new CoinEntry("monacoin", "MONA"));
            coins.Add(new CoinEntry("monero", "XMR"));
            coins.Add(new CoinEntry("musicoin", "MUSIC"));
            coins.Add(new CoinEntry("myriadcoin-groestl", "XMY"));
            coins.Add(new CoinEntry("myriadcoin-skein", "XMY"));
            coins.Add(new CoinEntry("myriadcoin-yescrypt", "XMY"));
            coins.Add(new CoinEntry("sexcoin", "SXC"));
            coins.Add(new CoinEntry("siacoin", "SC"));
            coins.Add(new CoinEntry("startcoin", "START"));
            coins.Add(new CoinEntry("verge-scrypt", "XVG"));
            coins.Add(new CoinEntry("vertcoin", "VTC"));
            coins.Add(new CoinEntry("zcash", "ZEC"));
            coins.Add(new CoinEntry("zclassic", "ZCL"));
            coins.Add(new CoinEntry("zcoin", "XZC"));
            coins.Add(new CoinEntry("zencash", "ZEN"));

            coins.First(e => e.Name == autoConvertCoin).isConverted = true;

            return coins;

        }


        private static double RoundEarnings(double d)
        {
            if (d < 1d)
            {
                return Math.Round(d, 5);
            }
            else
            {
                return Math.Round(d, 2);
            }
        }
        private static double ConvertToCurrency(double d, string symbol, string outSymbol)
        {
            var prices = GetPriceInfo(symbol, outSymbol);
            return double.Parse(prices[outSymbol]) * d;
        }

        static double Calc24hEarnings(string coinName, string apiKey, bool autoConvert = false)
        {
            RestClient client = new RestClient($"https://{coinName}.miningpoolhub.com");
            RestRequest req = new RestRequest("index.php", Method.GET);
            req.RequestFormat = DataFormat.Json;
            req.AddQueryParameter("page", "api");
            req.AddQueryParameter("action", "getdashboarddata");
            req.AddQueryParameter("api_key", apiKey);
            var res = client.Execute(req);

            if (res.IsSuccessful)
            {
                var data = JsonConvert.DeserializeObject<MPHWrapper>(res.Content);


                if (autoConvert)
                {
                    return data.getdashboarddata.data.recent_credits_24hours.amount;
                }
                else
                {
                    double bal_on_exh = 0d;
                    double.TryParse(data.getdashboarddata.data.balance_on_exchange, out bal_on_exh);
                    return bal_on_exh
                        + data.getdashboarddata.data.balance_for_auto_exchange.confirmed
                        + data.getdashboarddata.data.balance_for_auto_exchange.unconfirmed;
                }

            }

            return 0d;
        }

        static Dictionary<string, string> GetPriceInfo(string inputSym, string outputSym)
        {

            RestClient client = new RestClient("https://min-api.cryptocompare.com");
            var req = new RestRequest("data/price", Method.GET);
            req.RequestFormat = DataFormat.Json;
            req.AddQueryParameter("fsym", inputSym);
            req.AddQueryParameter("tsyms", outputSym);
            var res = client.Execute(req);

            if (res.IsSuccessful)
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(res.Content);
                return data;
            }
            return null;
        }
    }

    static class Utils
    {
        public static double Median<TColl, TValue>(
            this IEnumerable<TColl> source,
            Func<TColl, TValue> selector)
        {
            return source.Select<TColl, TValue>(selector).Median();
        }

        public static double Median<T>(
            this IEnumerable<T> source)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
                source = source.Where(x => x != null);

            int count = source.Count();
            if (count == 0)
                return 0;

            source = source.OrderBy(n => n);

            int midpoint = count / 2;
            if (count % 2 == 0)
                return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
            else
                return Convert.ToDouble(source.ElementAt(midpoint));
        }
    }
}
