using diveWebAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using static System.Net.WebRequestMethods;


namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewebpayController : ControllerBase
    {
        private readonly string _merchantId = "MS155251338";
        private readonly string _hashKey = "WCoM9RYJVUVFT5l6B8uetwDnPFG9yokX";
        private readonly string _hashIv = "C7LtLLQPVzXyn5FP";
        private readonly string _newebPayUrl = "https://ccore.newebpay.com/MPG/mpg_gateway";
        private readonly string _notifyUrl = "https://8b0d-1-160-26-54.ngrok-free.app/api/Newebpay/GetPaymentData";
        private readonly string _returnUrl = "https://www.newebpay.com/SuccessPage.html";
        private readonly string _redirectUrl = "http://localhost:4200/#/courseorderreceived";


        [HttpPost]
        public IActionResult CreatePayment(NewebPayPaymentDTO newebPayPaymentDTO)
        {
            var tradeInfo = new Dictionary<string, string>
        {
            { "MerchantID", _merchantId },
            { "RespondType", "JSON" },
            { "TimeStamp", ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString() },
            { "Version", "2.0" },
            { "MerchantOrderNo", newebPayPaymentDTO.OrderId},
            { "Amt", newebPayPaymentDTO.Amount.ToString() },
            { "ItemDesc", newebPayPaymentDTO.ProductName },
            { "TradeLimit", "600" },
            //{ "ReturnURL", _returnUrl }, // 交易回傳 URL
            { "NotifyURL", _notifyUrl },  // 後端交易通知 URL
            {"ClientBackURL", _redirectUrl}
        };
            var tradeInfoStr = string.Join("&", tradeInfo.Select(kv => $"{kv.Key}={kv.Value}"));
            var encryptedTradeInfo = EncryptAESHex(tradeInfoStr, _hashKey, _hashIv);
            var tradeSha = EncryptSHA256($"HashKey={_hashKey}&{encryptedTradeInfo}&HashIV={_hashIv}");
            //var tradeInfoMer = $"MerchantID={_merchantId}&TradeInfo={encryptedTradeInfo}&TradeSha={tradeSha}";
            return Ok(new { paymentUrl = _newebPayUrl, MerchantID= _merchantId, TradeInfo= encryptedTradeInfo , TradeSha = tradeSha });
        }

        [HttpPost("GetPaymentData")]
        public IActionResult GetPaymentData([FromForm]NewebPayReturn data)
        {
            return Ok(new { data = data });
        }


        //加密
        public static string EncryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                var encryptValue = EncryptAES(Encoding.UTF8.GetBytes(source), cryptoKey, cryptoIV);

                if (encryptValue != null)
                {
                    result = BitConverter.ToString(encryptValue)?.Replace("-", string.Empty)?.ToLower();
                }
            }

            return result;
        }
        public static byte[] EncryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = dataKey;
                aes.IV = dataIV;

                using (var encryptor = aes.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(source, 0, source.Length);
                }
            }
        }

        public static string EncryptSHA256(string source)
        {
            string result = string.Empty;

            using (SHA256 algorithm = SHA256.Create())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

                if (hash != null)
                {
                    result = BitConverter.ToString(hash)?.Replace("-", string.Empty)?.ToUpper();
                }

            }

            return result;
        }
    }
}
