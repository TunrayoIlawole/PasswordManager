using System.Security.Cryptography;

namespace PasswordManager.Services {
    public class EncryptionService {
        private readonly byte[] _key;

        public EncryptionService(string keyString) {
            _key = Convert.FromBase64String(keyString);
        }

        public string EncryptPassword(string password) {
            using (Aes aes = Aes.Create()) {
                aes.Key = _key;
                aes.GenerateIV();

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream()) {
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs)) {
                        sw.Write(password);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                 }
            }       
        }

        public string DecryptPassword(string encryptedPassWord) {
            byte[] fullCipher = Convert.FromBase64String(encryptedPassWord);

            using (Aes aes = Aes.Create()) {
                aes.Key = _key;

                byte[] iv = new byte[aes.BlockSize / 8];
                byte[] cipherText = new byte[fullCipher.Length - iv.Length];

                Array.Copy(fullCipher, iv, iv.Length);
                Array.Copy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipherText))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs)) {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}