using System;
using System.Drawing;
using System.IO;
using System.Text;
using QRCoder;

namespace CFS_1507.Application.Utils
{
    public class GenerateQR
    {
        public string GenerateQRCode(string content)
        {
            var qrGenerator = new QRCodeGenerator();
            string qrContent = content;

            int byteLength = Encoding.UTF8.GetByteCount(content);

            byteLength = Encoding.UTF8.GetByteCount(qrContent);

            QRCodeGenerator.ECCLevel eccLevel;
            if (byteLength <= 1273)
                eccLevel = QRCodeGenerator.ECCLevel.H;
            else if (byteLength <= 1663)
                eccLevel = QRCodeGenerator.ECCLevel.Q;
            else if (byteLength <= 2331)
                eccLevel = QRCodeGenerator.ECCLevel.M;
            else if (byteLength <= 2953)
                eccLevel = QRCodeGenerator.ECCLevel.L;
            else
                throw new ArgumentException("Dữ liệu quá lớn ngay cả sau khi rút gọn URL.");

            var qrCodeData = qrGenerator.CreateQrCode(qrContent, eccLevel);
            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(5);

            string base64 = Convert.ToBase64String(qrCodeAsBitmapByteArr);
            return $"data:image/png;base64,{base64}";
        }
    }
}
