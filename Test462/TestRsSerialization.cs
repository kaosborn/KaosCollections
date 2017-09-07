//
// Library: KaosCollections
// File:    TestRsSerializaton.cs
//

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
# if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    [Serializable]
    public class StudentComparer : System.Collections.Generic.Comparer<Student>
    {
        public override int Compare (Student x, Student y)
        { return x==null ? (y==null ? 0 : -1) : (y==null ? 1 : String.Compare (x.Name, y.Name)); }
    }

    [Serializable]
    public class Student : ISerializable
    {
        public string Name { get; private set; }

        public Student (string name)
        { this.Name = name;  }

        protected Student (SerializationInfo info, StreamingContext context)
        { this.Name = (string) info.GetValue ("Name", typeof (string)); }

        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        { info.AddValue ("Name", Name, typeof (string)); }
    }

    [Serializable]
#if TEST_BCL
    public class StudentSet : SortedSet<Student>
#else
    public class StudentSet : RankedSet<Student>
#endif
    {
        public StudentSet() : base (new StudentComparer())
        { }

        public StudentSet (SerializationInfo info, StreamingContext context) : base (info, context)
        { }
    }


    public partial class TestBtree
    {
        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_Bin_ArgumentNull()
        {
            var set = new StudentSet();
            ((ISerializable) set).GetObjectData (null, new StreamingContext());
        }

        [TestMethod]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRs_BinNullCB_Serialization()
        {
            var set = new StudentSet ((SerializationInfo) null, new StreamingContext());
            ((IDeserializationCallback) set).OnDeserialization (null);
        }

        [TestMethod]
        public void UnitRs_Serialization()
        {
            string fileName = "SetOfStudents.bin";
            var set1 = new StudentSet();
            set1.Add (new Student ("Floyd"));
            set1.Add (new Student ("Irene"));

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, set1); }

            var set2 = new StudentSet();
            using (var fs = new FileStream (fileName, FileMode.Open))
            { set2 = (StudentSet) formatter.Deserialize (fs); }

            Assert.AreEqual (2, set2.Count);
        }
    }
}
