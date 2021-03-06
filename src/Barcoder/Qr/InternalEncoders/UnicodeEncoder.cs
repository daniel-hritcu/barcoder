using System;
using Barcoder.Utils;
using TextEncoding = System.Text.Encoding;

namespace Barcoder.Qr.InternalEncoders
{
    internal sealed class UnicodeEncoder : InternalEncoderBase
    {
        public override (BitList, VersionInfo) Encode(string content, ErrorCorrectionLevel errorCorrectionLevel)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            byte[] data = TextEncoding.UTF8.GetBytes(content);

            EncodingMode encodingMode = EncodingMode.Byte;
            var versionInfo = VersionInfo.FindSmallestVersionInfo(errorCorrectionLevel, encodingMode, data.Length * 8);
            if (versionInfo == null)
                throw new InvalidOperationException("Too much data to encode");

            var bits = new BitList();
            bits.AddBits((uint)encodingMode, 4);
            bits.AddBits((uint)content.Length, versionInfo.CharCountBits(encodingMode));
            foreach (var b in data)
                bits.AddByte(b);
            AddPaddingAndTerminator(ref bits, versionInfo);
            return (bits, versionInfo);
        }
    }
}
