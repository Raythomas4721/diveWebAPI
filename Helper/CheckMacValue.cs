using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace diveWebAPI.Helper
{
    public class CheckMacValue
    {

        /// <summary>
        /// 產生 CheckMacValue (SHA256)
        /// </summary>
         
        public static string GenerateCheckMacValue(Dictionary<string, string> parameters, string hashKey, string hashIV)
        {
            // 1. 過濾掉空值或 CheckMacValue 參數
            var filtered = parameters
                .Where(x => !string.IsNullOrWhiteSpace(x.Value) && x.Key.ToLower() != "checkmacvalue")
                .OrderBy(x => x.Key.ToLower()) // 2. 依照 key 小寫後進行 ASCII 排序
                .ToList();

            // 3. 串接排序後的參數
            var raw = new StringBuilder();
            raw.Append($"HashKey={hashKey}&");
            foreach (var kv in filtered)
            {
                raw.Append($"{kv.Key}={kv.Value}&");
            }
            raw.Append($"HashIV={hashIV}");
            // ★ log: 顯示「排序後 + 串接」的原始字串
            Console.WriteLine("[CheckMac] Raw before UrlEncode: " + raw.ToString());

            // 4. URL encode 大寫轉小寫
            var urlEncoded = HttpUtility.UrlEncode(raw.ToString()).ToLower();
            // ★ log: 顯示「UrlEncode後」的結果
            Console.WriteLine("[CheckMac] After UrlEncode: " + urlEncoded);

            // 5. 取代保留字元 (官方文件中建議做更多替換，可視情況調整)
            //   如有出現 %20, %21 等符號的處理，需比對官方加密規範
            //   這裡僅示範最常見的 + 號替換
            urlEncoded = urlEncoded
                .Replace("%2d", "-")
                .Replace("%5f", "_")
                .Replace("%2e", ".")
                .Replace("%21", "!")
                .Replace("%2a", "*")
                .Replace("%28", "(")
                .Replace("%29", ")")
                .Replace("%20", "+");
            Console.WriteLine("[CheckMac] After Replacements: " + urlEncoded);

            // 6. 透過 SHA256 產生雜湊
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(urlEncoded);
            var hash = sha256.ComputeHash(bytes);
            var result = BitConverter.ToString(hash).Replace("-", "").ToUpper();
            Console.WriteLine("[CheckMac] Final: " + result);

            // 7. 轉大寫 16 進位
            var checkMacValue = BitConverter.ToString(hash).Replace("-", "").ToUpper();
            return checkMacValue;
        }

        /// <summary>
        /// 驗證回傳參數的 CheckMacValue 是否正確
        /// </summary>
        public static bool ValidateCheckMacValue(Dictionary<string, string> parameters, string hashKey, string hashIV)
        {
            if (!parameters.ContainsKey("CheckMacValue"))
                return false;

            var returnedCheckMac = parameters["CheckMacValue"];
            var calculatedCheckMac = GenerateCheckMacValue(parameters, hashKey, hashIV);

            return string.Equals(returnedCheckMac, calculatedCheckMac, StringComparison.OrdinalIgnoreCase);
        }
    }
}

