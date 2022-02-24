using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2020.Day25
{
    internal class AOC_Day25
    {

        [Test]
        public void TestPart1Solution()
        {
            var keyCard = new RFIDevice(PublicKey: 2084668);
            var doorLock = new RFIDevice(PublicKey: 3704642);

            var AOC_ResortRFIDScanner = new ComboBreaker();

            var scannedKeyCard = AOC_ResortRFIDScanner.BreakEncryptionKey(keyCard, doorLock);

            // Don't really need to break the door lock, but hey, why not?
            var scannedDoorLock = AOC_ResortRFIDScanner.BreakEncryptionKey(doorLock, keyCard);

            Console.WriteLine($"{scannedKeyCard.LoopSize}:{scannedDoorLock.LoopSize}");

            Assert.That(scannedKeyCard.EncryptionKey, Is.EqualTo(scannedDoorLock.EncryptionKey)); 
                            // Both should be equal to establish handshake.

            Assert.That(scannedKeyCard.EncryptionKey, Is.EqualTo(42668));
        }

        [Test]
        public void TestSample()
        {
            var keyCard = new RFIDevice(5764801);
            var doorLock = new RFIDevice(17807724);

            ComboBreaker lockPicker = new ComboBreaker();

            var scannedKeyCard = lockPicker.BreakEncryptionKey(keyCard, doorLock);
            var scannedDoorLock = lockPicker.BreakEncryptionKey(doorLock, keyCard);

            Assert.That(scannedKeyCard.EncryptionKey, Is.EqualTo(scannedDoorLock.EncryptionKey));
            Assert.That(scannedKeyCard.EncryptionKey, Is.EqualTo(14897079));
        }
    }

    /// <summary>
    /// Radio-frequency identification device
    /// </summary>
    /// <param name="PublicKey"></param>
    /// <param name="LoopSize">default value, 0 means that the value is hidden and not scannable.</param>
    /// <param name="EncryptionKey">default value, 0 means that the value is hidden and not scannable.</param>
    internal readonly record struct RFIDevice(int PublicKey, int LoopSize = 0, long EncryptionKey = 0);

    /// <summary>
    /// Custom Built RFIDevice scanner that can hack into the Resort's RFIDevices.
    /// </summary>
    internal class ComboBreaker
    {
        private readonly int Constant = 20201227; // Some provided constant key value.

        /// <summary>
        /// Cracks the value of internal loop size of the RFIDevice by bruteforce.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="SubjectNumber"></param>
        /// <returns>returns the cracked copy of RFIDevice</returns>
        public RFIDevice BreakLoopSize(RFIDevice device, int SubjectNumber = 7)
        {
            int temp = 1;
            int loopSize = 0;
            do
            {
                temp *= SubjectNumber;
                temp %= Constant;
                loopSize++;
            } while (temp != device.PublicKey);
            return new RFIDevice(device.PublicKey, loopSize, device.EncryptionKey);
        }

        /// <summary>
        /// Cracks the Encryption key stored in the RFIDevice by using the internal loopsize and
        /// public keys. However, when the internal loopsize is not detectable, this function
        /// calls the BreakLoopSize() function to get the necessary input to crack the 
        /// encryption Key.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public RFIDevice BreakEncryptionKey(RFIDevice device, RFIDevice other)
        {
            long encryptionKey = 1;
            int loopSize;

            // If the internal loop size cannot be detected, then crack it.
            // Of course, we can raise an exception on such occasions, but why?
            // What we are doing is illegal anyways; so, let's just get it done quickly and
            // get some rest inside.
            if(device.LoopSize == 0)
            {
                var temp = BreakLoopSize(device);
                loopSize = temp.LoopSize;
            }
            else
            {
                loopSize = device.LoopSize;
            }

            for(int i = 0; i < loopSize; i++)
            {
                encryptionKey *= other.PublicKey;
                encryptionKey %= Constant;
            }

            return new RFIDevice(device.PublicKey, loopSize, encryptionKey); 
        }
    }
}
