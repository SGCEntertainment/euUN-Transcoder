using System;
using System.Security.Cryptography;
using System.Text;

public static class AESUnity
{
	const string password = "qwerty12345Q";

	[Serializable]
	public struct AESEncryptedText
	{
		public string vector;
		public string codedString;

        public AESEncryptedText(string vector, string codedString)
        {
            this.vector = vector;
            this.codedString = codedString;
        }
    }

	private static byte[] ConvertToKeyBytes(SymmetricAlgorithm algorithm, string password)
	{
		algorithm.GenerateKey();

		var keyBytes = Encoding.UTF8.GetBytes(password);
		var validKeySize = algorithm.Key.Length;

		if (keyBytes.Length != validKeySize)
		{
			var newKeyBytes = new byte[validKeySize];
			Array.Copy(keyBytes, newKeyBytes, Math.Min(keyBytes.Length, newKeyBytes.Length));
			keyBytes = newKeyBytes;
		}

		return keyBytes;
	}

	public static AESEncryptedText Encrypt(string plainText)
	{
        var aes = Aes.Create();
        aes.GenerateIV();
        aes.Key = ConvertToKeyBytes(aes, password);

        var textBytes = Encoding.UTF8.GetBytes(plainText);

        var aesEncryptor = aes.CreateEncryptor();
        var encryptedBytes = aesEncryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

        return new AESEncryptedText
        {
            vector = Convert.ToBase64String(aes.IV),
            codedString = Convert.ToBase64String(encryptedBytes)
        };
    }

	public static string Decrypt(AESEncryptedText encryptedText, string password)
	{
		return Decrypt(encryptedText.codedString, encryptedText.vector, password);
	}

	public static string Decrypt(string encryptedText, string iv, string password)
	{
        Aes aes = Aes.Create();

        byte[] ivBytes = Convert.FromBase64String(iv);
        byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);

        ICryptoTransform decryptor = aes.CreateDecryptor(ConvertToKeyBytes(aes, password), ivBytes);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedTextBytes, 0, encryptedTextBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
