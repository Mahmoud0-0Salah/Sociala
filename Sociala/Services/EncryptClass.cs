namespace EncryptServices
{
    public interface IEncrypt
    {
        public string Encrypt(string plaintext, string key);
        public string Decrypt(string ciphertext, string key);
    }
    public class EncryptClass : IEncrypt
    {
        public string Encrypt(string plaintext, string key)
        {
            string ciphertext = "";

            for (int i = 0; i < plaintext.Length; i++)
            {
                char c = plaintext[i];
                int shift = key[i % key.Length] % 256; // Ensure the shift value is within the range of ASCII characters

                char shifted = (char)(c + shift);
                ciphertext += shifted;
            }

            return ciphertext;
        }

        public string Decrypt(string ciphertext, string key)
        {
            string decryptedText = "";

            for (int i = 0; i < ciphertext.Length; i++)
            {
                char c = ciphertext[i];
                int shift = key[i % key.Length] % 256; // Ensure the shift value is within the range of ASCII characters

                char shifted = (char)(c - shift);
                decryptedText += shifted;
            }

            return decryptedText;
        }
    }
}