//
// Library: KaosCollections
// File:    TestRmSerializaton.cs
//

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.Collections;

namespace Kaos.Test.Collections
{
    [Serializable]
    public class PlayerMap : RankedMap<Player,int>
    {
        public PlayerMap() : base (new PlayerComparer())
        { }

        public PlayerMap (SerializationInfo info, StreamingContext context) : base (info, context)
        { }
    }

    [Serializable]
    public class BadPlayerMap : RankedMap<Player,int>, IDeserializationCallback
    {
        public BadPlayerMap() : base (new PlayerComparer())
        { }

        public BadPlayerMap (SerializationInfo info, StreamingContext context) : base (info, context)
        { }

        void IDeserializationCallback.OnDeserialization (Object sender)
        {
            // This double call is for coverage purposes only.
            OnDeserialization (sender);
            OnDeserialization (sender);
        }
    }


    public partial class TestRm
    {
        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRmz_ArgumentNull()
        {
            var map = new PlayerMap();
            ((ISerializable) map).GetObjectData (null, new StreamingContext());
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRmz_NullCB()
        {
            var map = new PlayerMap ((SerializationInfo) null, new StreamingContext());
            ((IDeserializationCallback) map).OnDeserialization (null);
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRmz_BadCount()
        {
            string fileName = @"Targets\MapBadCount.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var map = (PlayerMap) formatter.Deserialize (fs); }
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRmz_MismatchKV()
        {
            string fileName = @"Targets\MapMismatchKV.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var map = (PlayerMap) formatter.Deserialize (fs); }
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRmz_MissingKeys()
        {
            string fileName = @"Targets\MapMissingKeys.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var map = (PlayerMap) formatter.Deserialize (fs); }
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRmz_MissingValues()
        {
            string fileName = @"Targets\MapMissingValues.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var map = (PlayerMap) formatter.Deserialize (fs); }
        }


        [TestMethod]
        public void UnitRmz_Serialization()
        {
            string fileName = "MapScores.bin";
            var map1 = new PlayerMap();
            map1.Add (new Player ("GG", "Floyd"), 11);
            map1.Add (new Player (null, "Betty"), 22);
            map1.Add (new Player (null, "Alvin"), 33);
            map1.Add (new Player ("GG", "Chuck"), 44);
            map1.Add (new Player ("A1", "Ziggy"), 55);
            map1.Add (new Player ("GG", null), 66);

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, map1); }

            PlayerMap map2 = null;
            using (var fs = new FileStream (fileName, FileMode.Open))
            { map2 = (PlayerMap) formatter.Deserialize (fs); }

            Assert.AreEqual (6, map2.Count);
        }


        [TestMethod]
        public void UnitRmz_BadSerialization()
        {
            string fileName = "BadMapScores.bin";
            var map1 = new BadPlayerMap();
            map1.Add (new Player ("VV", "Vicky"), 11);

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, map1); }

            BadPlayerMap map2 = null;
            using (var fs = new FileStream (fileName, FileMode.Open))
            { map2 = (BadPlayerMap) formatter.Deserialize (fs); }

            Assert.AreEqual (1, map2.Count);
        }
    }
}
