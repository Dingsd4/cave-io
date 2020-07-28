#pragma warning disable CA1707

namespace Cave.IO
{
    /// <summary>Provides supported string encodings.</summary>
    public enum StringEncoding
    {
        /// <summary>Character set not defined.</summary>
        Undefined = 0,

        #region internally handled fast encodings

        /// <summary>7 Bit per Character.</summary>
        ASCII = 1,

        /// <summary>8 Bit per Character Unicode.</summary>
        UTF8 = 2,

        /// <summary>Little Endian 16 Bit per Character Unicode.</summary>
        UTF16 = 3,

        /// <summary>Little Endian 32 Bit per Character Unicode.</summary>
        UTF32 = 4,

        #endregion

        /// <summary>Arabisch (ASMO 708).</summary>
        /// <remarks>Codepage: 708, Windows Codepage: 1256</remarks>
        ASMO_708 = 708,

        /// <summary>Chinesisch traditionell (Big5).</summary>
        /// <remarks>Codepage: 950, Windows Codepage: 950</remarks>
        BIG5 = 950,

        /// <summary>IBM EBCDIC (Kyrillisch, Serbisch-Bulgarisch).</summary>
        /// <remarks>Codepage: 21025, Windows Codepage: 1251</remarks>
        CP1025 = 21025,

        /// <summary>Kyrillisch (DOS).</summary>
        /// <remarks>Codepage: 866, Windows Codepage: 1251</remarks>
        CP866 = 866,

        /// <summary>IBM EBCDIC (Griechisch, modern).</summary>
        /// <remarks>Codepage: 875, Windows Codepage: 1253</remarks>
        CP875 = 875,

        /// <summary>Arabisch (DOS).</summary>
        /// <remarks>Codepage: 720, Windows Codepage: 1256</remarks>
        DOS_720 = 720,

        /// <summary>Hebräisch (DOS).</summary>
        /// <remarks>Codepage: 862, Windows Codepage: 1255</remarks>
        DOS_862 = 862,

        /// <summary>Chinesisch vereinfacht (EUC).</summary>
        /// <remarks>Codepage: 51936, Windows Codepage: 936</remarks>
        EUC_CN = 51936,

        /// <summary>Japanisch (EUC).</summary>
        /// <remarks>Codepage: 51932, Windows Codepage: 932</remarks>
        EUC_JP = 51932,

        /// <summary>Koreanisch (EUC).</summary>
        /// <remarks>Codepage: 51949, Windows Codepage: 949</remarks>
        EUC_KR = 51949,

        /// <summary>Chinesisch vereinfacht (GB18030).</summary>
        /// <remarks>Codepage: 54936, Windows Codepage: 936</remarks>
        GB18030 = 54936,

        /// <summary>Chinesisch vereinfacht (GB2312).</summary>
        /// <remarks>Codepage: 936, Windows Codepage: 936</remarks>
        GB2312 = 936,

        /// <summary>Chinesisch vereinfacht (HZ).</summary>
        /// <remarks>Codepage: 52936, Windows Codepage: 936</remarks>
        HZ_GB_2312 = 52936,

        /// <summary>IBM EBCDIC (Thailändisch).</summary>
        /// <remarks>Codepage: 20838, Windows Codepage: 874</remarks>
        IBM_THAI = 20838,

        /// <summary>OEM Multilingual Lateinisch 1.</summary>
        /// <remarks>Codepage: 858, Windows Codepage: 1252</remarks>
        IBM00858 = 858,

        /// <summary>IBM Lateinisch-1.</summary>
        /// <remarks>Codepage: 20924, Windows Codepage: 1252</remarks>
        IBM00924 = 20924,

        /// <summary>IBM Lateinisch-1.</summary>
        /// <remarks>Codepage: 1047, Windows Codepage: 1252</remarks>
        IBM01047 = 1047,

        /// <summary>IBM EBCDIC (USA-Kanada-Europäisch).</summary>
        /// <remarks>Codepage: 1140, Windows Codepage: 1252</remarks>
        IBM01140 = 1140,

        /// <summary>IBM EBCDIC (Deutschland-Europäisch).</summary>
        /// <remarks>Codepage: 1141, Windows Codepage: 1252</remarks>
        IBM01141 = 1141,

        /// <summary>IBM EBCDIC (Dänemark-Norwegen-Europäisch).</summary>
        /// <remarks>Codepage: 1142, Windows Codepage: 1252</remarks>
        IBM01142 = 1142,

        /// <summary>IBM EBCDIC (Finnland-Schweden-Europäisch).</summary>
        /// <remarks>Codepage: 1143, Windows Codepage: 1252</remarks>
        IBM01143 = 1143,

        /// <summary>IBM EBCDIC (Italien-Europäisch).</summary>
        /// <remarks>Codepage: 1144, Windows Codepage: 1252</remarks>
        IBM01144 = 1144,

        /// <summary>IBM EBCDIC (Spanisch-Europäisch).</summary>
        /// <remarks>Codepage: 1145, Windows Codepage: 1252</remarks>
        IBM01145 = 1145,

        /// <summary>IBM EBCDIC (Großbritannien-Europäisch).</summary>
        /// <remarks>Codepage: 1146, Windows Codepage: 1252</remarks>
        IBM01146 = 1146,

        /// <summary>IBM EBCDIC (Frankreich-Europäisch).</summary>
        /// <remarks>Codepage: 1147, Windows Codepage: 1252</remarks>
        IBM01147 = 1147,

        /// <summary>IBM EBCDIC (International-Europäisch).</summary>
        /// <remarks>Codepage: 1148, Windows Codepage: 1252</remarks>
        IBM01148 = 1148,

        /// <summary>IBM EBCDIC (Isländisch-Europäisch).</summary>
        /// <remarks>Codepage: 1149, Windows Codepage: 1252</remarks>
        IBM01149 = 1149,

        /// <summary>IBM EBCDIC (USA-Kanada).</summary>
        /// <remarks>Codepage: 37, Windows Codepage: 1252</remarks>
        IBM037 = 37,

        /// <summary>IBM EBCDIC (Türkisch, Lateinisch-5).</summary>
        /// <remarks>Codepage: 1026, Windows Codepage: 1254</remarks>
        IBM1026 = 1026,

        /// <summary>IBM EBCDIC (Deutschland).</summary>
        /// <remarks>Codepage: 20273, Windows Codepage: 1252</remarks>
        IBM273 = 20273,

        /// <summary>IBM EBCDIC (Dänemark-Norwegen).</summary>
        /// <remarks>Codepage: 20277, Windows Codepage: 1252</remarks>
        IBM277 = 20277,

        /// <summary>IBM EBCDIC (Finnland-Schweden).</summary>
        /// <remarks>Codepage: 20278, Windows Codepage: 1252</remarks>
        IBM278 = 20278,

        /// <summary>IBM EBCDIC (Italien).</summary>
        /// <remarks>Codepage: 20280, Windows Codepage: 1252</remarks>
        IBM280 = 20280,

        /// <summary>IBM EBCDIC (Spanien).</summary>
        /// <remarks>Codepage: 20284, Windows Codepage: 1252</remarks>
        IBM284 = 20284,

        /// <summary>IBM EBCDIC (UK).</summary>
        /// <remarks>Codepage: 20285, Windows Codepage: 1252</remarks>
        IBM285 = 20285,

        /// <summary>IBM EBCDIC (Japanisch Katakana).</summary>
        /// <remarks>Codepage: 20290, Windows Codepage: 932</remarks>
        IBM290 = 20290,

        /// <summary>IBM EBCDIC (Frankreich).</summary>
        /// <remarks>Codepage: 20297, Windows Codepage: 1252</remarks>
        IBM297 = 20297,

        /// <summary>IBM EBCDIC (Arabisch).</summary>
        /// <remarks>Codepage: 20420, Windows Codepage: 1256</remarks>
        IBM420 = 20420,

        /// <summary>IBM EBCDIC (Griechisch).</summary>
        /// <remarks>Codepage: 20423, Windows Codepage: 1253</remarks>
        IBM423 = 20423,

        /// <summary>IBM EBCDIC (Hebräisch).</summary>
        /// <remarks>Codepage: 20424, Windows Codepage: 1255</remarks>
        IBM424 = 20424,

        /// <summary>OEM USA.</summary>
        /// <remarks>Codepage: 437, Windows Codepage: 1252</remarks>
        IBM437 = 437,

        /// <summary>IBM EBCDIC (International).</summary>
        /// <remarks>Codepage: 500, Windows Codepage: 1252</remarks>
        IBM500 = 500,

        /// <summary>Griechisch (DOS).</summary>
        /// <remarks>Codepage: 737, Windows Codepage: 1253</remarks>
        IBM737 = 737,

        /// <summary>Baltisch (DOS).</summary>
        /// <remarks>Codepage: 775, Windows Codepage: 1257</remarks>
        IBM775 = 775,

        /// <summary>Westeuropäisch (DOS).</summary>
        /// <remarks>Codepage: 850, Windows Codepage: 1252</remarks>
        IBM850 = 850,

        /// <summary>Mitteleuropäisch (DOS).</summary>
        /// <remarks>Codepage: 852, Windows Codepage: 1250</remarks>
        IBM852 = 852,

        /// <summary>OEM Kyrillisch.</summary>
        /// <remarks>Codepage: 855, Windows Codepage: 1252</remarks>
        IBM855 = 855,

        /// <summary>Türkisch (DOS).</summary>
        /// <remarks>Codepage: 857, Windows Codepage: 1254</remarks>
        IBM857 = 857,

        /// <summary>Portugiesisch (DOS).</summary>
        /// <remarks>Codepage: 860, Windows Codepage: 1252</remarks>
        IBM860 = 860,

        /// <summary>Isländisch (DOS).</summary>
        /// <remarks>Codepage: 861, Windows Codepage: 1252</remarks>
        IBM861 = 861,

        /// <summary>Französisch, Kanada (DOS).</summary>
        /// <remarks>Codepage: 863, Windows Codepage: 1252</remarks>
        IBM863 = 863,

        /// <summary>Arabisch (864).</summary>
        /// <remarks>Codepage: 864, Windows Codepage: 1256</remarks>
        IBM864 = 864,

        /// <summary>Nordisch (DOS).</summary>
        /// <remarks>Codepage: 865, Windows Codepage: 1252</remarks>
        IBM865 = 865,

        /// <summary>Griechisch, modern (DOS).</summary>
        /// <remarks>Codepage: 869, Windows Codepage: 1253</remarks>
        IBM869 = 869,

        /// <summary>IBM EBCDIC (Multilingual Lateinisch-2).</summary>
        /// <remarks>Codepage: 870, Windows Codepage: 1250</remarks>
        IBM870 = 870,

        /// <summary>IBM EBCDIC (Isländisch).</summary>
        /// <remarks>Codepage: 20871, Windows Codepage: 1252</remarks>
        IBM871 = 20871,

        /// <summary>IBM EBCDIC (Kyrillisch, Russisch).</summary>
        /// <remarks>Codepage: 20880, Windows Codepage: 1251</remarks>
        IBM880 = 20880,

        /// <summary>IBM EBCDIC (Türkisch).</summary>
        /// <remarks>Codepage: 20905, Windows Codepage: 1254</remarks>
        IBM905 = 20905,

        /// <summary>Japanisch (JIS, 1 Byte Kana erlaubt - SO/SI).</summary>
        /// <remarks>Codepage: 50222, Windows Codepage: 932</remarks>
        ISO_2022_JP = 50222,

        /// <summary>Koreanisch (ISO).</summary>
        /// <remarks>Codepage: 50225, Windows Codepage: 949</remarks>
        ISO_2022_KR = 50225,

        /// <summary>Westeuropäisch (ISO).</summary>
        /// <remarks>Codepage: 28591, Windows Codepage: 1252</remarks>
        ISO_8859_1 = 28591,

        /// <summary>Estnisch (ISO).</summary>
        /// <remarks>Codepage: 28603, Windows Codepage: 1257</remarks>
        ISO_8859_13 = 28603,

        /// <summary>Lateinisch 9 (ISO).</summary>
        /// <remarks>Codepage: 28605, Windows Codepage: 1252</remarks>
        ISO_8859_15 = 28605,

        /// <summary>Mitteleuropäisch (ISO).</summary>
        /// <remarks>Codepage: 28592, Windows Codepage: 1250</remarks>
        ISO_8859_2 = 28592,

        /// <summary>Lateinisch 3 (ISO).</summary>
        /// <remarks>Codepage: 28593, Windows Codepage: 1254</remarks>
        ISO_8859_3 = 28593,

        /// <summary>Baltisch (ISO).</summary>
        /// <remarks>Codepage: 28594, Windows Codepage: 1257</remarks>
        ISO_8859_4 = 28594,

        /// <summary>Kyrillisch (ISO).</summary>
        /// <remarks>Codepage: 28595, Windows Codepage: 1251</remarks>
        ISO_8859_5 = 28595,

        /// <summary>Arabisch (ISO).</summary>
        /// <remarks>Codepage: 28596, Windows Codepage: 1256</remarks>
        ISO_8859_6 = 28596,

        /// <summary>Griechisch (ISO).</summary>
        /// <remarks>Codepage: 28597, Windows Codepage: 1253</remarks>
        ISO_8859_7 = 28597,

        /// <summary>Hebräisch (ISO-Visual).</summary>
        /// <remarks>Codepage: 28598, Windows Codepage: 1255</remarks>
        ISO_8859_8 = 28598,

        /// <summary>Hebräisch (ISO-Logical).</summary>
        /// <remarks>Codepage: 38598, Windows Codepage: 1255</remarks>
        ISO_8859_8_I = 38598,

        /// <summary>Türkisch (ISO).</summary>
        /// <remarks>Codepage: 28599, Windows Codepage: 1254</remarks>
        ISO_8859_9 = 28599,

        /// <summary>Koreanisch (Johab).</summary>
        /// <remarks>Codepage: 1361, Windows Codepage: 949</remarks>
        JOHAB = 1361,

        /// <summary>Kyrillisch (KOI8-R).</summary>
        /// <remarks>Codepage: 20866, Windows Codepage: 1251</remarks>
        KOI8_R = 20866,

        /// <summary>Kyrillisch (KOI8-U).</summary>
        /// <remarks>Codepage: 21866, Windows Codepage: 1251</remarks>
        KOI8_U = 21866,

        /// <summary>Koreanisch.</summary>
        /// <remarks>Codepage: 949, Windows Codepage: 949</remarks>
        KS_C_5601_1987 = 949,

        /// <summary>Westeuropäisch (Mac).</summary>
        /// <remarks>Codepage: 10000, Windows Codepage: 1252</remarks>
        MACINTOSH = 10000,

        /// <summary>US-ASCII.</summary>
        /// <remarks>Codepage: 20127, Windows Codepage: 1252</remarks>
        US_ASCII = 20127,

        /// <summary>Unicode.</summary>
        /// <remarks>Codepage: 1200, Windows Codepage: 1200</remarks>
        UTF_16 = 1200,

        /// <summary>Unicode (Big-Endian).</summary>
        /// <remarks>Codepage: 1201, Windows Codepage: 1200</remarks>
        UTF_16BE = 1201,

        /// <summary>Unicode (UTF-32).</summary>
        /// <remarks>Codepage: 12000, Windows Codepage: 1200</remarks>
        UTF_32 = 12000,

        /// <summary>Unicode (UTF-32 Big-Endian).</summary>
        /// <remarks>Codepage: 12001, Windows Codepage: 1200</remarks>
        UTF_32BE = 12001,

        /// <summary>Unicode (UTF-7).</summary>
        /// <remarks>Codepage: 65000, Windows Codepage: 1200</remarks>
        UTF_7 = 65000,

        /// <summary>Unicode (UTF-8).</summary>
        /// <remarks>Codepage: 65001, Windows Codepage: 1200</remarks>
        UTF_8 = 65001,

        /// <summary>Hebräisch (Windows).</summary>
        /// <remarks>Codepage: 1255, Windows Codepage: 1255</remarks>
        WINDOWS_1255 = 1255,

        /// <summary>Arabisch (Windows).</summary>
        /// <remarks>Codepage: 1256, Windows Codepage: 1256</remarks>
        WINDOWS_1256 = 1256,

        /// <summary>Baltisch (Windows).</summary>
        /// <remarks>Codepage: 1257, Windows Codepage: 1257</remarks>
        WINDOWS_1257 = 1257,

        /// <summary>Vietnamesisch (Windows).</summary>
        /// <remarks>Codepage: 1258, Windows Codepage: 1258</remarks>
        WINDOWS_1258 = 1258,

        /// <summary>Thailändisch (Windows).</summary>
        /// <remarks>Codepage: 874, Windows Codepage: 874</remarks>
        WINDOWS_874 = 874,

        /// <summary>Chinesisch traditionell (CNS).</summary>
        /// <remarks>Codepage: 20000, Windows Codepage: 950</remarks>
        X_CHINESE_CNS = 20000,

        /// <summary>Chinesisch traditionell (Eten).</summary>
        /// <remarks>Codepage: 20002, Windows Codepage: 950</remarks>
        X_CHINESE_ETEN = 20002,

        /// <summary>TCA Taiwan.</summary>
        /// <remarks>Codepage: 20001, Windows Codepage: 950</remarks>
        X_CP20001 = 20001,

        /// <summary>IBM5550 Taiwan.</summary>
        /// <remarks>Codepage: 20003, Windows Codepage: 950</remarks>
        X_CP20003 = 20003,

        /// <summary>TeleText Taiwan.</summary>
        /// <remarks>Codepage: 20004, Windows Codepage: 950</remarks>
        X_CP20004 = 20004,

        /// <summary>Wang Taiwan.</summary>
        /// <remarks>Codepage: 20005, Windows Codepage: 950</remarks>
        X_CP20005 = 20005,

        /// <summary>T.61.</summary>
        /// <remarks>Codepage: 20261, Windows Codepage: 1252</remarks>
        X_CP20261 = 20261,

        /// <summary>ISO-6937.</summary>
        /// <remarks>Codepage: 20269, Windows Codepage: 1252</remarks>
        X_CP20269 = 20269,

        /// <summary>GB2312-80 Chinesisch (vereinfacht).</summary>
        /// <remarks>Codepage: 20936, Windows Codepage: 936</remarks>
        X_CP20936 = 20936,

        /// <summary>Koreanisch Wansung.</summary>
        /// <remarks>Codepage: 20949, Windows Codepage: 949</remarks>
        X_CP20949 = 20949,

        /// <summary>Chinesisch vereinfacht (ISO-2022).</summary>
        /// <remarks>Codepage: 50227, Windows Codepage: 936</remarks>
        X_CP50227 = 50227,

        /// <summary>IBM EBCDIC (Koreanisch, erweitert).</summary>
        /// <remarks>Codepage: 20833, Windows Codepage: 949</remarks>
        X_EBCDIC_KOREANEXTENDED = 20833,

        /// <summary>Europa.</summary>
        /// <remarks>Codepage: 29001, Windows Codepage: 1252</remarks>
        X_EUROPA = 29001,

        /// <summary>Westeuropäisch (IA5).</summary>
        /// <remarks>Codepage: 20105, Windows Codepage: 1252</remarks>
        X_IA5 = 20105,

        /// <summary>Deutsch (IA5).</summary>
        /// <remarks>Codepage: 20106, Windows Codepage: 1252</remarks>
        X_IA5_GERMAN = 20106,

        /// <summary>Norwegisch (IA5).</summary>
        /// <remarks>Codepage: 20108, Windows Codepage: 1252</remarks>
        X_IA5_NORWEGIAN = 20108,

        /// <summary>Schwedisch (IA5).</summary>
        /// <remarks>Codepage: 20107, Windows Codepage: 1252</remarks>
        X_IA5_SWEDISH = 20107,

        /// <summary>ISCII Assamesisch.</summary>
        /// <remarks>Codepage: 57006, Windows Codepage: 57006</remarks>
        X_ISCII_AS = 57006,

        /// <summary>ISCII Bangla.</summary>
        /// <remarks>Codepage: 57003, Windows Codepage: 57003</remarks>
        X_ISCII_BE = 57003,

        /// <summary>ISCII Devanagari.</summary>
        /// <remarks>Codepage: 57002, Windows Codepage: 57002</remarks>
        X_ISCII_DE = 57002,

        /// <summary>ISCII Gujarati.</summary>
        /// <remarks>Codepage: 57010, Windows Codepage: 57010</remarks>
        X_ISCII_GU = 57010,

        /// <summary>ISCII Kannada.</summary>
        /// <remarks>Codepage: 57008, Windows Codepage: 57008</remarks>
        X_ISCII_KA = 57008,

        /// <summary>ISCII Malayalam.</summary>
        /// <remarks>Codepage: 57009, Windows Codepage: 57009</remarks>
        X_ISCII_MA = 57009,

        /// <summary>ISCII Odia.</summary>
        /// <remarks>Codepage: 57007, Windows Codepage: 57007</remarks>
        X_ISCII_OR = 57007,

        /// <summary>ISCII Punjabi.</summary>
        /// <remarks>Codepage: 57011, Windows Codepage: 57011</remarks>
        X_ISCII_PA = 57011,

        /// <summary>ISCII Tamil.</summary>
        /// <remarks>Codepage: 57004, Windows Codepage: 57004</remarks>
        X_ISCII_TA = 57004,

        /// <summary>ISCII Telugu.</summary>
        /// <remarks>Codepage: 57005, Windows Codepage: 57005</remarks>
        X_ISCII_TE = 57005,

        /// <summary>Arabisch (Mac).</summary>
        /// <remarks>Codepage: 10004, Windows Codepage: 1256</remarks>
        X_MAC_ARABIC = 10004,

        /// <summary>Mitteleuropäisch (Mac).</summary>
        /// <remarks>Codepage: 10029, Windows Codepage: 1250</remarks>
        X_MAC_CE = 10029,

        /// <summary>Chinesisch vereinfacht (Mac).</summary>
        /// <remarks>Codepage: 10008, Windows Codepage: 936</remarks>
        X_MAC_CHINESESIMP = 10008,

        /// <summary>Chinesisch traditionell (Mac).</summary>
        /// <remarks>Codepage: 10002, Windows Codepage: 950</remarks>
        X_MAC_CHINESETRAD = 10002,

        /// <summary>Kroatisch (Mac).</summary>
        /// <remarks>Codepage: 10082, Windows Codepage: 1250</remarks>
        X_MAC_CROATIAN = 10082,

        /// <summary>Kyrillisch (Mac).</summary>
        /// <remarks>Codepage: 10007, Windows Codepage: 1251</remarks>
        X_MAC_CYRILLIC = 10007,

        /// <summary>Griechisch (Mac).</summary>
        /// <remarks>Codepage: 10006, Windows Codepage: 1253</remarks>
        X_MAC_GREEK = 10006,

        /// <summary>Hebräisch (Mac).</summary>
        /// <remarks>Codepage: 10005, Windows Codepage: 1255</remarks>
        X_MAC_HEBREW = 10005,

        /// <summary>Isländisch (Mac).</summary>
        /// <remarks>Codepage: 10079, Windows Codepage: 1252</remarks>
        X_MAC_ICELANDIC = 10079,

        /// <summary>Japanisch (Mac).</summary>
        /// <remarks>Codepage: 10001, Windows Codepage: 932</remarks>
        X_MAC_JAPANESE = 10001,

        /// <summary>Koreanisch (Mac).</summary>
        /// <remarks>Codepage: 10003, Windows Codepage: 949</remarks>
        X_MAC_KOREAN = 10003,

        /// <summary>Rumänisch (Mac).</summary>
        /// <remarks>Codepage: 10010, Windows Codepage: 1250</remarks>
        X_MAC_ROMANIAN = 10010,

        /// <summary>Thailändisch (Mac).</summary>
        /// <remarks>Codepage: 10021, Windows Codepage: 874</remarks>
        X_MAC_THAI = 10021,

        /// <summary>Türkisch (Mac).</summary>
        /// <remarks>Codepage: 10081, Windows Codepage: 1254</remarks>
        X_MAC_TURKISH = 10081,

        /// <summary>Ukrainisch (Mac).</summary>
        /// <remarks>Codepage: 10017, Windows Codepage: 1251</remarks>
        X_MAC_UKRAINIAN = 10017
    }
}

#pragma warning restore CA1707
