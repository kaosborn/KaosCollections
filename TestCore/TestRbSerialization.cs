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
    [Serializable]
    public class ExamBag : RankedBag<Exam>
    {
        public ExamBag() : base (new ScoreComparer())
        { }

        public ExamBag (SerializationInfo info, StreamingContext context) : base (info, context)
        { }
    }

    [Serializable]
    public class BadExamBag : RankedBag<Exam>, IDeserializationCallback
    {
        public BadExamBag() : base (new ScoreComparer())
        { }

        public BadExamBag (SerializationInfo info, StreamingContext context) : base (info, context)
        { }

        void IDeserializationCallback.OnDeserialization (Object sender)
        {
            // This double call is for coverage purposes only.
            OnDeserialization (sender);
            OnDeserialization (sender);
        }
    }


    public partial class TestRb : TestBtree
    {
        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRbz_ArgumentNull()
        {
            var bag = new ExamBag();
            ((ISerializable) bag).GetObjectData (null, new StreamingContext());
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRbz_NullCB()
        {
            var bag = new ExamBag ((SerializationInfo) null, new StreamingContext());
            ((IDeserializationCallback) bag).OnDeserialization (null);
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRbz_BadCount()
        {
            string fileName = @"Targets\BagBadCount.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
            { var bag = (ExamBag) formatter.Deserialize (fs); }
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRbz_MissingItems()
        {
            string fileName = @"Targets\BagMissingItems.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
            { var bag = (ExamBag) formatter.Deserialize (fs); }
        }


        [TestMethod]
        public void UnitRbz_Serialization()
        {
            string fileName = "BagOfExams.bin";
            var bag1 = new ExamBag();
            bag1.Add (new Exam (5, "Floyd"));

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, bag1); }

            RankedBag<Exam> bag2 = null;
            using (var fs = new FileStream (fileName, FileMode.Open))
            { bag2 = (ExamBag) formatter.Deserialize (fs); }

            Assert.AreEqual (1, bag2.Count);
        }


        [TestMethod]
        public void UnitRbz_BadSerialization()
        {
            string fileName = "BagOfBadExams.bin";
            var bag1 = new BadExamBag();
            bag1.Add (new Exam (1, "Agness"));

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, bag1); }

            BadExamBag bag2 = null;
            using (var fs = new FileStream (fileName, FileMode.Open))
            { bag2 = (BadExamBag) formatter.Deserialize (fs); }

            Assert.AreEqual (1, bag2.Count);
        }
    }
#endif
}
