//
// Library: KaosCollections
// File:    TestRdSerializaton.cs
//

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    [Serializable]
    public class PlayerComparer : System.Collections.Generic.Comparer<Player>
    {
        public override int Compare (Player x, Player y)
        {
            int cp = String.Compare (x.Clan, y.Clan);
            return cp != 0 ? cp : String.Compare (x.Name, y.Name);
        }
    }

    [Serializable]
    public class Player : ISerializable
    {
        public string Clan { get; private set; }
        public string Name { get; private set; }

        public Player (string clan, string name)
        { this.Clan = clan; this.Name = name; }

        protected Player (SerializationInfo info, StreamingContext context)
        {
            this.Clan = (string) info.GetValue ("Clan", typeof (String));
            this.Name = (string) info.GetValue ("Name", typeof (String));
        }

        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("Clan", Clan, typeof (String));
            info.AddValue ("Name", Name, typeof (String));
        }

        public override string ToString() => Clan + "." + Name;
    }

    [Serializable]
#if TEST_BCL
    public class PlayerDary : SortedDictionary<Player,int>
#else
    public class PlayerDary : RankedDictionary<Player,int>
#endif
    {
        public PlayerDary() : base (new PlayerComparer())
        { }

#if ! TEST_BCL
        public PlayerDary (SerializationInfo info, StreamingContext context) : base (info, context)
        { }
#endif
    }


    public partial class TestBtree
    {
#if ! TEST_BCL
        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRdz_ArgumentNull()
        {
            var dary = new PlayerDary();
            ((ISerializable) dary).GetObjectData (null, new StreamingContext());
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRdz_NullCB()
        {
            var dary = new PlayerDary ((SerializationInfo) null, new StreamingContext());
            ((IDeserializationCallback) dary).OnDeserialization (null);
        }
#endif


        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRdz_BadCount()
        {
            string fileName = @"Targets\DaryBadCount.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var dary = (PlayerDary) formatter.Deserialize (fs); }
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRdz_MismatchKV()
        {
            string fileName = @"Targets\DaryMismatchKV.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var dary = (PlayerDary) formatter.Deserialize (fs); }
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRdz_MissingKeys()
        {
            string fileName = @"Targets\DaryMissingKeys.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var dary = (PlayerDary) formatter.Deserialize (fs); }
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRdz_MissingValues()
        {
            string fileName = @"Targets\DaryMissingValues.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var dary = (PlayerDary) formatter.Deserialize (fs); }
        }


        [TestMethod]
        public void UnitRdz_Serialization()
        {
            string fileName = "DaryScores.bin";
            var p1 = new PlayerDary();
            p1.Add (new Player ("GG", "Floyd"), 11);
            p1.Add (new Player (null, "Betty"), 22);
            p1.Add (new Player (null, "Alvin"), 33);
            p1.Add (new Player ("GG", "Chuck"), 44);
            p1.Add (new Player ("A1", "Ziggy"), 55);
            p1.Add (new Player ("GG", null), 66);

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, p1); }

            PlayerDary p2 = null;
            using (var fs = new FileStream (fileName, FileMode.Open))
            { p2 = (PlayerDary) formatter.Deserialize (fs); }

            Assert.AreEqual (6, p2.Count);
        }
    }
}
