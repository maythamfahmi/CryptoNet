// <copyright file="Program.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

namespace CryptoNet.Cli
{
    internal class Program
    {
        protected Program() { }

        public static void Main()
        {
            ExampleAes.Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_SymmetricKey();
            ExampleAes.Example_2_SelfGenerated_And_Save_SymmetricKey();
            ExampleAes.Example_3_Encrypt_Decrypt_Content_With_Own_SymmetricKey();
            ExampleAes.Example_4_Encrypt_Decrypt_Content_With_Human_Readable_Key_Secret_SymmetricKey();
            ExampleAes.Example_5_Encrypt_And_Decrypt_PdfFile_With_SymmetricKey_Test("TestFiles\\test.docx");
            ExampleAes.Example_5_Encrypt_And_Decrypt_PdfFile_With_SymmetricKey_Test("TestFiles\\test.xlsx");
            ExampleAes.Example_5_Encrypt_And_Decrypt_PdfFile_With_SymmetricKey_Test("TestFiles\\test.pdf");
            ExampleAes.Example_5_Encrypt_And_Decrypt_PdfFile_With_SymmetricKey_Test("TestFiles\\test.png");

            ExampleRsa.Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey();
            ExampleRsa.Example_2_SelfGenerated_And_Save_AsymmetricKey();
            ExampleRsa.Example_3_Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content();
        }
    }
}
