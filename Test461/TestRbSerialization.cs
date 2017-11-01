//
// Library: KaosCollections
// File:    TestRbSerializaton.cs
//

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
# if ! TEST_BCL
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    [Serializable]
    public class ScoreComparer : System.Collections.Generic.Comparer<Exam>
    { public override int Compare (Exam x1, Exam x2) => x1.Score - x2.Score; }

    [Serializable]
    public class Exam : ISerializable
    {
        public int Score { get; private set; }
        public string Name { get; private set; }

        public Exam (int score, string name)
        { this.Score = score; this.Name = name; }

        protected Exam (SerializationInfo info, StreamingContext context)
        {
            this.Score = (int) info.GetValue ("Score", typeof (int));
            this.Name = (string) info.GetValue ("Name", typeof (string));
        }

        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("Score", Score, typeof (int));
            info.AddValue ("Name", Name, typeof (string));
        }
    }


#if ! TEST_BCL
    public class ExamBag : RankedBag<Exam>
    {
        public ExamBag() : base (new ScoreComparer())
        { }

        public ExamBag (SerializationInfo info, StreamingContext context) : base (info, context)
        { }
    }


    public partial class TestBtree
    {
        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_Bin_ArgumentNull()
        {
            var bag = new ExamBag();
            ((ISerializable) bag).GetObjectData (null, new StreamingContext());
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRb_BinNullCB_Serialization()
        {
            var bag = new ExamBag ((SerializationInfo) null, new StreamingContext());
            ((IDeserializationCallback) bag).OnDeserialization (null);
        }

        [TestMethod]
        public void UnitRb_Serialization()
        {
            string fileName = "BagOfExams.bin";
            var bag1 = new RankedBag<Exam> (new ScoreComparer());
            bag1.Add (new Exam (5, "Floyd"));

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, bag1); }

            RankedBag<Exam> bag2 = null;
            using (var fs = new FileStream (fileName, FileMode.Open))
            { bag2 = (RankedBag<Exam>) formatter.Deserialize (fs); }

            Assert.AreEqual (1, bag2.Count);
        }
    }
#endif
}
