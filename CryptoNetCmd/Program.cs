using CryptoNetLib;

string? root = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
string privateKeySource = @$"{root}\test.certificate";

var content = "This content is confidential";

//var cryptoNet = new CryptoNet("correct key");
//Console.WriteLine("1- We will encrypt following:");
//Console.WriteLine(content);

//var encrypted = cryptoNet.Encrypt(content);
//Console.WriteLine("2- To:");
//Console.WriteLine(System.Text.Encoding.Default.GetString(encrypted));

//var cryptoNet1 = new CryptoNet("correct key");
//var decrypted1 = cryptoNet1.Decrypt(encrypted);
//Console.WriteLine("3- And we will decrypt it back with correct key:");
//Console.WriteLine(decrypted1);

//var cryptoNet2 = new CryptoNet("wrong key");
//var decrypted2 = cryptoNet2.Decrypt(encrypted);
//Console.WriteLine("4- And we will not be able decrypt it back with wrong key:");
//Console.WriteLine(decrypted2);

//// now we do it using self generated certificate
//var cryptoNet3 = new CryptoNet();
//cryptoNet3.ImportKey(cryptoNet3.LoadKey(privateKeySource));
//var encrypted3 = cryptoNet3.Encrypt(content);
//Console.WriteLine("5- This time we use a certificate to encrypt");
//Console.WriteLine(System.Text.Encoding.Default.GetString(encrypted3));

//var cryptoNet4 = new CryptoNet();
//cryptoNet4.ImportKey(cryptoNet4.LoadKey(privateKeySource));
//var encrypted4 = cryptoNet4.Decrypt(encrypted3);
//Console.WriteLine("6- And use the same certificate to decrypt");
//Console.WriteLine(encrypted4);

//// now we encrypt with public key and decrypt with private key
var cryptoNet5 = new CryptoNet();
var certificate5 = cryptoNet5.LoadKey(privateKeySource);
cryptoNet5.ImportKey(certificate5);
var privateKey5 = cryptoNet5.ExportPublicKey();
cryptoNet5.ImportKey(privateKey5);
Console.WriteLine(cryptoNet5.GetKeyType());
var encrypted5 = cryptoNet5.Encrypt(content);
Console.WriteLine("7- This time we use a certificate to encrypt");
Console.WriteLine(System.Text.Encoding.Default.GetString(encrypted5));

var cryptoNet6 = new CryptoNet();
var certificate6 = cryptoNet6.LoadKey(privateKeySource);
cryptoNet6.ImportKey(certificate6);
var privateKey6 = cryptoNet6.ExportPrivateKey();
cryptoNet6.ImportKey(privateKey6);
Console.WriteLine(cryptoNet6.GetKeyType());
var encrypted6 = cryptoNet6.Decrypt(encrypted5);
Console.WriteLine("8- And use the same certificate to decrypt");
Console.WriteLine(encrypted6);