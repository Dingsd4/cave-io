using System;
using System.Linq;
using Cave;

namespace Tests
{
    public struct TestStruct
    {
        public static TestStruct Create(int i)
        {
            var t = new TestStruct()
            {
                Arr = BitConverter.GetBytes((long)i),
                B = (byte)(i & 0xFF),
                SB = (sbyte)(-i / 10),
                US = (ushort)i,
                C = (char)i,
                I = i,
                F = (500 - i) * 0.5f,
                D = (500 - i) * 0.5d,
                Date = new DateTime(1 + Math.Abs(i % 3000), 12, 31, 23, 59, 48, Math.Abs(i % 1000), i % 2 == 1 ? DateTimeKind.Local : DateTimeKind.Utc),
                Time = TimeSpan.FromSeconds(i),
                S = (short)(i - 500),
                UI = (uint)i,
                Text = i.ToString(),
                Dec = 0.005m * (i - 500),
                Uri = new Uri("http://localhost/" + i.ToString()),
                ConStr = "http://localhost/" + i.ToString(),
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
            if (!(obj is TestStruct)) return false;
            var other = (TestStruct)obj;
            return
                this.Arr.SequenceEqual(other.Arr) &&
                Equals(this.B, other.B) &&
                Equals(this.C, other.C) &&
                Equals(this.ConStr, other.ConStr) &&
                Equals(this.D, other.D) &&
                Equals(this.Date.ToUniversalTime(), other.Date.ToUniversalTime()) &&
                Equals(this.Dec, other.Dec) &&
                Equals(this.F, other.F) &&
                Equals(this.I, other.I) &&
                Equals(this.S, other.S) &&
                Equals(this.SB, other.SB) &&
                Equals(this.Text, other.Text) &&
                Equals(this.Time, other.Time) &&
                Equals(this.UI, other.UI) &&
                Equals(this.Uri, other.Uri) &&
                Equals(this.US, other.US);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return StringExtensions.Join(new object[] { Arr, B, C, ConStr, D, Date, Dec, F, I, S, SB, Text, Time, UI, Uri, US }, ';');
        }
    }
}
