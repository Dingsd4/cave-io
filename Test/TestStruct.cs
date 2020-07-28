using System;
using System.Linq;
using Cave;

namespace Tests
{
    public struct TestStruct
    {
        public static TestStruct Create(int i)
        {
            var t = new TestStruct
            {
                Arr = BitConverter.GetBytes((long) i),
                B = (byte) (i & 0xFF),
                SB = (sbyte) (-i / 10),
                US = (ushort) i,
                C = (char) i,
                I = i,
                F = (500 - i) * 0.5f,
                D = (500 - i) * 0.5d,
                Date = new DateTime(1 + Math.Abs(i % 3000), 12, 31, 23, 59, 48, Math.Abs(i % 1000), (i % 2) == 1 ? DateTimeKind.Local : DateTimeKind.Utc),
                Time = TimeSpan.FromSeconds(i),
                S = (short) (i - 500),
                UI = (uint) i,
                Text = i.ToString(),
                Dec = 0.005m * (i - 500),
                Uri = new Uri("http://localhost/" + i),
                ConStr = "http://localhost/" + i
            };
            return t;
        }

        public long ID;
        public byte B;
        public sbyte SB;
        public char C;
        public short S;
        public ushort US;
        public int I;
        public uint UI;
        public byte[] Arr;
        public string Text;
        public TimeSpan Time;
        public DateTime Date;
        public double D;
        public float F;
        public decimal Dec;
        public Uri Uri;
        public ConnectionString ConStr;

        public override bool Equals(object obj)
        {
            if (!(obj is TestStruct))
            {
                return false;
            }

            var other = (TestStruct) obj;
            return
                Arr.SequenceEqual(other.Arr) &&
                Equals(B, other.B) &&
                Equals(C, other.C) &&
                Equals(ConStr, other.ConStr) &&
                Equals(D, other.D) &&
                Equals(Date.ToUniversalTime(), other.Date.ToUniversalTime()) &&
                Equals(Dec, other.Dec) &&
                Equals(F, other.F) &&
                Equals(I, other.I) &&
                Equals(S, other.S) &&
                Equals(SB, other.SB) &&
                Equals(Text, other.Text) &&
                Equals(Time, other.Time) &&
                Equals(UI, other.UI) &&
                Equals(Uri, other.Uri) &&
                Equals(US, other.US);
        }

        public override int GetHashCode() => ID.GetHashCode();

        public override string ToString() { return new object[] { Arr, B, C, ConStr, D, Date, Dec, F, I, S, SB, Text, Time, UI, Uri, US }.Join(';'); }
    }
}
