// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Criptografar e descriptografar");

string senha = "12345678901234562135";
var key = "|DD6F458B13274FD";

Console.WriteLine($"Senha: {senha}");

//string senhaCript = EncryptLogin(senha);
string senhaCript = EncryptString(senha, key);
Console.WriteLine($"Hash: {senhaCript}");


 

string senhaDec = DecryptString(senhaCript, key);
Console.WriteLine($"Descriptografada: {senhaDec}");




string EncryptLogin(string senha)
{
    //Impossivel descriptografar para chegar a mensagem original, 
    //então deve pegar a senha digitada, criptografar e comparar com a salva

    var key = "|DD6F458B13274FDC8782F3FABC749E396C4907F4A6F446DA96A69F9D20FB44DA";

    var arrayBytes = Encoding.UTF8.GetBytes(senha + key);

    byte[] hashBytes;
    using (var sha = SHA512.Create())
    {
        hashBytes = sha.ComputeHash(arrayBytes);
    }


    return Convert.ToBase64String(hashBytes);
}

static string EncryptString(string text, string keyString)
{
    var key = Encoding.UTF8.GetBytes(keyString);

    using (var aesAlg = Aes.Create())
    {
        using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
        {
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(text);
                }

                var iv = aesAlg.IV;

                var decryptedContent = msEncrypt.ToArray();

                var result = new byte[iv.Length + decryptedContent.Length];

                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                return Convert.ToBase64String(result);
            }
        }
    }
}

 static string DecryptString(string cipherText, string keyString)
{
    var fullCipher = Convert.FromBase64String(cipherText);

    var iv = new byte[16];
    var cipher = new byte[fullCipher.Length - iv.Length];

    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
    var key = Encoding.UTF8.GetBytes(keyString);

    using (var aesAlg = Aes.Create())
    {
        using (var decryptor = aesAlg.CreateDecryptor(key, iv))
        {
            string result;
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        result = srDecrypt.ReadToEnd();
                    }
                }
            }

            return result;
        }
    }
}


