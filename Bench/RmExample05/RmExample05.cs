using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Kaos.Collections;

namespace ExampleApp
{
    [Serializable]
    public class PlayerComparer : Comparer<Player>
    {
        public override int Compare (Player x, Player y)
        {
            int cp = String.Compare (x.Clan, y.Clan);
            return cp != 0 ? cp : String.Compare (x.Name, y.Name);
        }
    }

    [Serializable]
    public class PlayerMap : RankedMap<Player,int>
    {
        public string Game { get; set; }

        public PlayerMap() : base (new PlayerComparer())
        { }

        public PlayerMap (SerializationInfo info, StreamingContext context) : base (info, context)
        {
            this.Game = info.GetString ("Game");
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData (info, context);
            info.AddValue ("Game", Game, typeof (string));
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
            this.Clan = (string) info.GetValue ("Clan", typeof (string));
            this.Name = (string) info.GetValue ("Name", typeof (string));
        }

        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("Clan", Clan, typeof (string));
            info.AddValue ("Name", Name, typeof (string));
        }

        public override string ToString() => Clan + "." + Name;
    }

    
    class RmExample05
    {
        static void Main()
        {
            string fileName = "MapScores.bin";
            var map1 = new PlayerMap() { Game = "Day of Defeat" };
            map1.Add (new Player ("GG", "Floyd"), 11);
            map1.Add (new Player (null, "Player"), 22);
            map1.Add (new Player ("A1", "Betty"), 44);
            map1.Add (new Player ("GG", "Edwin"), 66);
            map1.Add (new Player (null, "Player"), 77);

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, map1); }

            Console.WriteLine ($"Wrote {map1.Count} key/value pairs.\n");

            PlayerMap map2 = null;
            using (var fs = new FileStream (fileName, FileMode.Open))
            { map2 = (PlayerMap) formatter.Deserialize (fs); }

            Console.WriteLine ("Read back:");
            Console.WriteLine ($"Game = {map2.Game}");
            Console.WriteLine ("Players:");
            foreach (var kv in map2)
                Console.WriteLine ($"  {kv}");
        }

        /* Output:

        Wrote 5 key/value pairs.

        Read back:
        Game = Day of Defeat
        Players:
          [.Player, 22]
          [.Player, 77]
          [A1.Betty, 44]
          [GG.Edwin, 66]
          [GG.Floyd, 11]

        */
    }
}
