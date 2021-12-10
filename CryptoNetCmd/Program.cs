using CryptoNetLib;

var content = "This content is confidential";

//var cryptoNet = new CryptoNet.Service.CryptoNet("my business name");

//Console.WriteLine(content);
//Console.WriteLine("is encrypted to");

//var encrypted = cryptoNet.Encrypt(content);
//var encryptStr = System.Text.Encoding.Default.GetString(encrypted);
//Console.WriteLine(encryptStr);
//Console.WriteLine("and decrypted back to");

//var decrypted = cryptoNet.Decrypt(encrypted);
//Console.WriteLine(decrypted);

//var cryptoNet1 = new CryptoNet.Service.CryptoNet("my business name");
//var encrypted1 = cryptoNet1.Encrypt(content);
//cryptoNet1.Save(@"C:\Certificates\encrypted.txt", encrypted1);
//var load = cryptoNet1.Load(@"C:\Certificates\encrypted.txt");
//var decrypted1 = cryptoNet1.Decrypt(load);
//Console.WriteLine(decrypted1);

var cryptoNet2 = new CryptoNet("my business name2");
//cryptoNet2.SaveKey(@"C:\Certificates\test.certificate1.pem", );
var encrypted2 = cryptoNet2.Encrypt(content);
cryptoNet2.Save(@"C:\Certificates\encrypted.txt", encrypted2);
Console.WriteLine(1);

var cryptoNet3 = new CryptoNet("my business name2");
cryptoNet3.LoadKey(@"C:\Certificates\test.certificate.pem");
var load3 = cryptoNet3.Load(@"C:\Certificates\encrypted.txt");
cryptoNet3.InitAsymmetricKeys();
var decrypted3 = cryptoNet3.Decrypt(load3);
Console.WriteLine(decrypted3);
Console.WriteLine(2);