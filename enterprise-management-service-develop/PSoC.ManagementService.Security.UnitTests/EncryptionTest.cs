using System;
using System.Collections;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Security;

namespace PSoC.ManagementService.Security.UnitTests
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void Encryption_Encrypt_Test()
        {
            string s1 = "Hello World";
            string es1 = Encryption.Instance.Encrypt(s1);

            Assert.IsFalse(s1.Equals(es1));

        }

        [TestMethod]
        public void Encryption_Decrypt_Test()
        {
            string s1 = "Hello World";
            string es1 = Encryption.Instance.Encrypt(s1);

            string s2 = Encryption.Instance.Decrypt(es1);


            Assert.IsFalse(s2.Equals(es1));
            Assert.IsTrue(s2.Equals(s1));
        }

        [TestMethod]
        public void Encryption_Hash_Test()
        {
            string s = "Hello World";
            var h1 = Convert.FromBase64String("Ck1VqNd45QIvq3AZd8XYQLvEhtA=");
            var h2 = Encryption.Instance.ComputeHash(s);
            var h3 = Encryption.Instance.ComputeHash(Encoding.UTF8.GetBytes(s));

            IStructuralEquatable equ = h1;

            Assert.IsTrue(equ.Equals(h2, StructuralComparisons.StructuralEqualityComparer));
            Assert.IsTrue(equ.Equals(h3, StructuralComparisons.StructuralEqualityComparer));
        }
    }
}