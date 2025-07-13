using System.Net;

namespace StudentAttendanceApp.Helpers
{
    public static class IpHelper
    {
        public static bool IsIpAllowed(string ipAddress, List<string> allowedRanges)
        {
            var ip = IPAddress.Parse(ipAddress);

            foreach (var range in allowedRanges)
            {
                var parts = range.Split('/');
                var baseIp = IPAddress.Parse(parts[0]);
                int prefixLength = int.Parse(parts[1]);

                var baseIpBytes = baseIp.GetAddressBytes();
                var ipBytes = ip.GetAddressBytes();

                if (baseIpBytes.Length != ipBytes.Length)
                    continue;

                int byteCount = prefixLength / 8;
                int bitCount = prefixLength % 8;

                bool matched = true;
                for (int i = 0; i < byteCount; i++)
                {
                    if (baseIpBytes[i] != ipBytes[i])
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched && bitCount > 0)
                {
                    int mask = (byte)~(255 >> bitCount);
                    if ((baseIpBytes[byteCount] & mask) != (ipBytes[byteCount] & mask))
                        matched = false;
                }

                if (matched)
                    return true;
            }

            return false;
        }
    }
}
