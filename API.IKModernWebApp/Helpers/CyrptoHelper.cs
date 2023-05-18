using System;

namespace API.IKModernWebApp.Helpers
{
    public class CyrptoHelper
    {
        public static string DecryptPassword(string password, byte[] salt)
        {
            if (String.IsNullOrEmpty(password))
            {
                return String.Empty;
            }

            return EncryptDecryptText.DecryptStringAES(password, String20181017102918(), salt);
        }

        private static string String20181017102918()
        {
            uint[] _data = { 0xD, 0x11, 0x6B, 0x37, 0x7E, 0x61, 0x22, 0x62, 0x37, 0x1C, 0x2, 0x34, 0x20, 0x3, 0x1E, 0x1E, 0x43, 0xC, 0x5, 0x69, 0x6F, 0x20, 0x25, 0x1, 0x42, 0x4C, 0x58, 0x78, 0x6E, 0x75, 0x2C, 0x52, };
            uint[] _key = { 0xFFFFFF8B, 0xFFFFFFAC, 0xFFFFFFAD, 0xFFFFFFA0, 0xFFFFFFA7, 0xFFFFFFCB, 0xFFFFFFB1, 0xFFFFFFD8, 0xFFFFFF8A, 0xFFFFFFA2, 0xFFFFFF9E, 0xFFFFFFBB, 0xFFFFFF95, 0xFFFFFF97, 0xFFFFFFB6, 0xFFFFFF8D, 0xFFFFFFCD, 0xFFFFFFB2, 0xFFFFFF83, 0xFFFFFFB7, 0xFFFFFFB0, 0xFFFFFFB5, 0xFFFFFF8C, 0xFFFFFFA4, 0xFFFFFFCA, 0xFFFFFF95, 0xFFFFFFCA, 0xFFFFFFC7, 0xFFFFFFD6, 0xFFFFFFD5, 0xFFFFFFBF, 0xFFFFFFC6, };
            string _result = string.Empty;
            for (int i = 0; i < _data.Length; i++)
            {
                _result += (char)(_data[i] ^ ~_key[i % _key.Length]);
            }

            return _result;
        }
    }
}
