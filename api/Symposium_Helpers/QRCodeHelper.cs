using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using QRCoder;
using log4net;

namespace Symposium.Helpers
{
    public class QRCodeHelper
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Image GenerateQRImage(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                try
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(2);
                    return (Image) qrCodeImage;
                }
                catch(Exception ex)
                {
                    logger.Error("QRCodeGenerator error: " + ex.ToString());

                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
