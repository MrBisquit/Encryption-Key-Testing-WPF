using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EncryptionKeyTestingWPF
{
    public class Encryption
    {
        public long dtn = 0;
        public int key = 0;

        public char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz123456789{}[]()!?,. '1234567890".ToCharArray();
        public void GetKey()
        {
            dtn = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            try
            {
                key = (int)dtn / DateTime.Now.Minute / DateTime.Now.Millisecond;

                if (key == 0)
                {
                    key = (int)dtn / DateTime.Now.Minute + 1 / DateTime.Now.Millisecond + 1;
                }
            }
            catch { }

            key = (int)dtn / (DateTime.Now.Minute + 1 * 100) / (DateTime.Now.Hour + 1 * 100);

            key = (int)dtn / (DateTime.Now.Month + 1) / (DateTime.Now.Minute + 1 * 100) / (DateTime.Now.Hour + 1 * 100);
        }

        public string Encrypt(string input)
        {
            string encrypted = "";

            char[] ech = input.ToCharArray();
            char[] nch = new char[ech.Length];

            for (int i = 0; i < ech.Length; i++)
            {
                nch[i] = CeaserCypher(ech[i]);
            }

            for (int i = 0; i < nch.Length; i++)
            {
                encrypted += nch[i];
            }

            return encrypted;
        }

        public char CeaserCypher(char input)
        {
            char cyphered = 'A';
            int atpoint = 0;
            int newpoint = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == input)
                {
                    atpoint = i;
                }
            }
            newpoint = (atpoint / chars.Length) + (key / chars.Length);

            int atnewpoint = atpoint;

            for (int i = 0; i < key; i++)
            {
                atnewpoint += 1;
                if (atnewpoint >= chars.Length)
                {
                    atnewpoint = 0;
                }
            }

            try
            {
                cyphered = chars[atnewpoint];
            }
            catch { }

            //Console.WriteLine($"{atpoint} {atnewpoint} {cyphered} {input}");

            return cyphered;
        }

        public string Decrypt(string input)
        {
            string decrypted = "";

            char[] ech = input.ToCharArray();
            char[] nch = new char[ech.Length];

            for (int i = 0; i < ech.Length; i++)
            {
                nch[i] = ReverseCeaserCypher(ech[i]);
            }

            for (int i = 0; i < nch.Length; i++)
            {
                decrypted += nch[i];
            }

            return decrypted;
        }

        public char ReverseCeaserCypher(char input)
        {
            char cyphered = 'A';
            int atpoint = 0;
            int newpoint = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == input)
                {
                    atpoint = i;
                }
            }
            newpoint = (atpoint / chars.Length) + (key / chars.Length);
            int atnewpoint = atpoint;

            for (int i = 0; i < key; i++)
            {
                atnewpoint -= 1;
                if (atnewpoint <= 0)
                {
                    atnewpoint = chars.Length;
                }
            }

            try
            {
                cyphered = chars[atnewpoint];
            }
            catch { }

            //Console.WriteLine($"{atpoint} {atnewpoint} {cyphered} {input}");

            return cyphered;
        }

        public byte[] EncryptBytes(byte[] input)
        {
            byte[] encrypted = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                encrypted[i] = CeaserCypherByte(input[i]);
            }

            return encrypted;
        }

        public byte CeaserCypherByte(byte input)
        {
            int startingpoint = (int)input;

            for (int i = 0; i < key; i++)
            {
                startingpoint += 1;
                if (startingpoint >= 255)
                {
                    startingpoint = 0;
                }
            }

            return (byte)startingpoint;
        }

        public byte[] DecryptBytes(byte[] input)
        {
            byte[] decrypted = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                decrypted[i] = ReverseCeaserCypherByte(input[i]);
            }

            return decrypted;
        }

        public byte ReverseCeaserCypherByte(byte input)
        {
            int startingpoint = (int)input;

            for (int i = 0; i < key; i++)
            {
                startingpoint -= 1;
                if (startingpoint <= 0)
                {
                    startingpoint = 255;
                }
            }

            return (byte)startingpoint;
        }
    }
}
