#region License

// 	Copyright 2013-2014 Matthew Ducker
// 	
// 	Licensed under the Apache License, Version 2.0 (the "License");
// 	you may not use this file except in compliance with the License.
// 	
// 	You may obtain a copy of the License at
// 		
// 		http://www.apache.org/licenses/LICENSE-2.0
// 	
// 	Unless required by applicable law or agreed to in writing, software
// 	distributed under the License is distributed on an "AS IS" BASIS,
// 	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 	See the License for the specific language governing permissions and 
// 	limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Obscur.Core.Cryptography.Information.EllipticCurve;
using Obscur.Core.Cryptography.Support.Math.EllipticCurve.Custom.SEC;

namespace Obscur.Core.Cryptography.Information
{
    /// <summary>
    ///     Storage for information on named elliptic curves.
    /// </summary>
    public static class EcInformationStore
    {
        internal static readonly ImmutableDictionary<string, EcCurveInformation> CurveDictionary;

        #region Information initialiser

        static EcInformationStore()
        {
            CurveDictionary = ImmutableDictionary.CreateRange(new[] {
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp192k1.ToString(), new CustomFpEcCurveInformation(
                        () => new SecP192K1Curve(), CustomFpEcCurveInformation.CreateEndomorphismParameters(
                            "BB85691939B869C1D087F601554B96B80CB4F55B35F433C2",
                            "3D84F26C12238D7B4F3D516613C1759033B1A5800175D0B1",
                            new[] {
                                "71169BE7330B3038EDB025F1",
                                "-B3FB3400DEC5C4ADCEB8655C"
                            },
                            new[] {
                                "12511CFE811D0F4E6BC688B4D",
                                "71169BE7330B3038EDB025F1"
                            },
                            "71169BE7330B3038EDB025F1D0F9",
                            "B3FB3400DEC5C4ADCEB8655D4C94",
                            208)) {
                                Name = Sec2EllipticCurve.Secp192k1.ToString(),
                                DisplayName = "secp192k1",
                                BitLength = 192,
                                G = "04" + "DB4FF10EC057E9AE26B07D0280B7F4341DA5D1B1EAE06C7D" 
                                         + "9B2F2F6D9C5628A7844163D015BE86344082AA88D95E2F9D",
                                Seed = null
                            }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp192r1.ToString(), new CustomFpEcCurveInformation(
                        () => new SecP192R1Curve()) {
                            Name = Sec2EllipticCurve.Secp192r1.ToString(),
                            DisplayName = "secp192r1",
                            BitLength = 192,
                            G = "04" + "188DA80EB03090F67CBF20EB43A18800F4FF0AFD82FF1012" 
                                     + "07192B95FFC8DA78631011ED6B24CDD573F977A11E794811",
                            Seed = "3045AE6FC8422F64ED579528D38120EAE12196D5"
                        }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp224k1.ToString(), new CustomFpEcCurveInformation(
                        () => new SecP224K1Curve(), CustomFpEcCurveInformation.CreateEndomorphismParameters(
                            "FE0E87005B4E83761908C5131D552A850B3F58B749C37CF5B84D6768",
                            "60DCD2104C4CBC0BE6EEEFC2BDD610739EC34E317F9B33046C9E4788",
                            new[] {
                                "6B8CF07D4CA75C88957D9D670591",
                                "-B8ADF1378A6EB73409FA6C9C637D"
                            },
                            new[] {
                                "1243AE1B4D71613BC9F780A03690E",
                                "6B8CF07D4CA75C88957D9D670591"
                            },
                            "6B8CF07D4CA75C88957D9D67059037A4",
                            "B8ADF1378A6EB73409FA6C9C637BA7F5",
                            240)) {
                                Name = Sec2EllipticCurve.Secp224k1.ToString(),
                                DisplayName = "secp224k1",
                                BitLength = 224,
                                G = "04" + "A1455B334DF099DF30FC28A169A467E9E47075A90F7E650EB6B7A45C" 
                                         + "7E089FED7FBA344282CAFBD6F7E319F7C0B0BD59E2CA4BDB556D61A5",
                                Seed = null
                            }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp224r1.ToString(), new CustomFpEcCurveInformation(
                        () => new SecP224R1Curve()) {
                            Name = Sec2EllipticCurve.Secp224r1.ToString(),
                            DisplayName = "secp224r1",
                            BitLength = 224,
                            G = "04" + "B70E0CBD6BB4BF7F321390B94A03C1D356C21122343280D6115C1D21" 
                                     + "BD376388B5F723FB4C22DFE6CD4375A05A07476444D5819985007E34",
                            Seed = "BD71344799D5C7FCDC45B59FA3B9AB8F6A948BC5"
                        }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp256k1.ToString(), new CustomFpEcCurveInformation(
                        () => new SecP256K1Curve(), CustomFpEcCurveInformation.CreateEndomorphismParameters(
                            "7AE96A2B657C07106E64479EAC3434E99CF0497512F58995C1396C28719501EE",
                            "5363AD4CC05C30E0A5261C028812645A122E22EA20816678DF02967C1B23BD72",
                            new[] {
                                "3086D221A7D46BCDE86C90E49284EB15",
                                "-E4437ED6010E88286F547FA90ABFE4C3"
                            },
                            new[] {
                                "114CA50F7A8E2F3F657C1108D9D44CFD8",
                                "3086D221A7D46BCDE86C90E49284EB15"
                            },
                            "3086D221A7D46BCDE86C90E49284EB153DAB",
                            "E4437ED6010E88286F547FA90ABFE4C42212",
                            272)) {
                                Name = Sec2EllipticCurve.Secp256k1.ToString(),
                                DisplayName = "secp256k1",
                                BitLength = 256,
                                G = "04" + "79BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798" 
                                         + "483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8",
                                Seed = null
                            }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp256r1.ToString(), new CustomFpEcCurveInformation(
                        () => new SecP256R1Curve()) {
                            Name = Sec2EllipticCurve.Secp256r1.ToString(),
                            DisplayName = "secp256r1",
                            BitLength = 256,
                            G = "04" + "6B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296" 
                                     + "4FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5",
                            Seed = "C49D360886E704936A6678E1139D26B7819F7E90"
                        }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp384r1.ToString(), new CustomFpEcCurveInformation(
                        () => new SecP384R1Curve()) {
                            Name = Sec2EllipticCurve.Secp384r1.ToString(),
                            DisplayName = "secp384r1",
                            BitLength = 384,
                            G = "04" + "AA87CA22BE8B05378EB1C71EF320AD746E1D3B628BA79B9859F741E082542A385502F25DBF55296C3A545E3872760AB7" 
                                     + "3617DE4A96262C6F5D9E98BF9292DC29F8F41DBD289A147CE9DA3113B5F0B8C00A60B1CE1D7E819D7A431D7C90EA0E5F",
                            Seed = "A335926AA319A27A1D00896A6773A4827ACDAC73"
                        }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp521r1.ToString(), new CustomFpEcCurveInformation(
                        () => new SecP521R1Curve()) {
                            Name = Sec2EllipticCurve.Secp521r1.ToString(),
                            DisplayName = "secp521r1",
                            BitLength = 521,
                            G = "04" + "00C6858E06B70404E9CD9E3ECB662395B4429C648139053FB521F828AF606B4D3DBAA14B5E77EFE75928FE1DC127A2FFA8DE3348B3C1856A429BF97E7E31C2E5BD66" 
                                     + "011839296A789A3BC0045C8A5FB42C7D1BD998F54449579B446817AFBD17273E662C97EE72995EF42640C550B9013FAD0761353C7086A272C24088BE94769FD16650",
                            Seed = "D09E8800291CB85396CC6717393284AAA0DA64BA"
                        }),
                /* BEGIN BRAINPOOL CONSORTIUM CURVES */
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP160r1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP160r1.ToString(),
                        DisplayName = "brainpoolP160r1",
                        BitLength = 160,
                        Q = "E95E4A5F737059DC60DFC7AD95B3D8139515620F",
                        A = "340E7BE2A280EB74E2BE61BADA745D97E8F7C300",
                        B = "1E589A8595423412134FAA2DBDEC95C8D8675E58",
                        G = "04" + "BED5AF16EA3F6A4F62938C4631EB5AF7BDBCDBC3" 
                                 + "1667CB477A1A8EC338F94741669C976316DA6321",
                        N = "E95E4A5F737059DC60DF5991D45029409E60FC09",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP160t1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP160t1.ToString(),
                        DisplayName = "brainpoolP160t1",
                        BitLength = 160,
                        Q = "E95E4A5F737059DC60DFC7AD95B3D8139515620F",
                        A = "E95E4A5F737059DC60DFC7AD95B3D8139515620C",
                        B = "7A556B6DAE535B7B51ED2C4D7DAA7A0B5C55F380",
                        G = "04" + "B199B13B9B34EFC1397E64BAEB05ACC265FF2378" 
                                 + "ADD6718B7C7C1961F0991B842443772152C9E0AD",
                        N = "E95E4A5F737059DC60DF5991D45029409E60FC09",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP192r1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP192r1.ToString(),
                        DisplayName = "brainpoolP192r1",
                        BitLength = 192,
                        Q = "C302F41D932A36CDA7A3463093D18DB78FCE476DE1A86297",
                        A = "6A91174076B1E0E19C39C031FE8685C1CAE040E5C69A28EF",
                        B = "469A28EF7C28CCA3DC721D044F4496BCCA7EF4146FBF25C9",
                        G = "04" + "C0A0647EAAB6A48753B033C56CB0F0900A2F5C4853375FD6" 
                                 + "14B690866ABD5BB88B5F4828C1490002E6773FA2FA299B8F",
                        N = "C302F41D932A36CDA7A3462F9E9E916B5BE8F1029AC4ACC1",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP192t1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP192t1.ToString(),
                        DisplayName = "brainpoolP192t1",
                        BitLength = 192,
                        Q = "C302F41D932A36CDA7A3463093D18DB78FCE476DE1A86297",
                        A = "C302F41D932A36CDA7A3463093D18DB78FCE476DE1A86294",
                        B = "13D56FFAEC78681E68F9DEB43B35BEC2FB68542E27897B79",
                        G = "04" + "3AE9E58C82F63C30282E1FE7BBF43FA72C446AF6F4618129" 
                                 + "097E2C5667C2223A902AB5CA449D0084B7E5B3DE7CCC01C9",
                        N = "C302F41D932A36CDA7A3462F9E9E916B5BE8F1029AC4ACC1",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP224r1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP224r1.ToString(),
                        DisplayName = "brainpoolP224r1",
                        BitLength = 224,
                        Q = "D7C134AA264366862A18302575D1D787B09F075797DA89F57EC8C0FF",
                        A = "68A5E62CA9CE6C1C299803A6C1530B514E182AD8B0042A59CAD29F43",
                        B = "2580F63CCFE44138870713B1A92369E33E2135D266DBB372386C400B",
                        G = "04" + "0D9029AD2C7E5CF4340823B2A87DC68C9E4CE3174C1E6EFDEE12C07D" 
                                 + "58AA56F772C0726F24C6B89E4ECDAC24354B9E99CAA3F6D3761402CD",
                        N = "D7C134AA264366862A18302575D0FB98D116BC4B6DDEBCA3A5A7939F",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP224t1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP224t1.ToString(),
                        DisplayName = "brainpoolP224t1",
                        BitLength = 224,
                        Q = "D7C134AA264366862A18302575D1D787B09F075797DA89F57EC8C0FF",
                        A = "D7C134AA264366862A18302575D1D787B09F075797DA89F57EC8C0FC",
                        B = "4B337D934104CD7BEF271BF60CED1ED20DA14C08B3BB64F18A60888D",
                        G = "04" + "6AB1E344CE25FF3896424E7FFE14762ECB49F8928AC0C76029B4D580" 
                                 + "0374E9F5143E568CD23F3F4D7C0D4B1E41C8CC0D1C6ABD5F1A46DB4C",
                        N = "D7C134AA264366862A18302575D0FB98D116BC4B6DDEBCA3A5A7939F",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP256r1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP256r1.ToString(),
                        DisplayName = "brainpoolP256r1",
                        BitLength = 256,
                        Q = "A9FB57DBA1EEA9BC3E660A909D838D726E3BF623D52620282013481D1F6E5377",
                        A = "7D5A0975FC2C3057EEF67530417AFFE7FB8055C126DC5C6CE94A4B44F330B5D9",
                        B = "26DC5C6CE94A4B44F330B5D9BBD77CBF958416295CF7E1CE6BCCDC18FF8C07B6",
                        G = "04" + "8BD2AEB9CB7E57CB2C4B482FFC81B7AFB9DE27E1E3BD23C23A4453BD9ACE3262" 
                                 + "547EF835C3DAC4FD97F8461A14611DC9C27745132DED8E545C1D54C72F046997",
                        N = "A9FB57DBA1EEA9BC3E660A909D838D718C397AA3B561A6F7901E0E82974856A7",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP256t1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP256t1.ToString(),
                        DisplayName = "brainpoolP256t1",
                        BitLength = 256,
                        Q = "A9FB57DBA1EEA9BC3E660A909D838D726E3BF623D52620282013481D1F6E5377",
                        A = "A9FB57DBA1EEA9BC3E660A909D838D726E3BF623D52620282013481D1F6E5374",
                        B = "662C61C430D84EA4FE66A7733D0B76B7BF93EBC4AF2F49256AE58101FEE92B04",
                        G = "04" + "A3E8EB3CC1CFE7B7732213B23A656149AFA142C47AAFBC2B79A191562E1305F4" 
                                 + "2D996C823439C56D7F7B22E14644417E69BCB6DE39D027001DABE8F35B25C9BE",
                        N = "A9FB57DBA1EEA9BC3E660A909D838D718C397AA3B561A6F7901E0E82974856A7",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP320r1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP320r1.ToString(),
                        DisplayName = "brainpoolP320r1",
                        BitLength = 320,
                        Q = "D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27",
                        A = "3EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB4",
                        B = "520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A6",
                        G = "04" + "43BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E20611" 
                                 + "14FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1",
                        N = "D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C59311",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP320t1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP320t1.ToString(),
                        DisplayName = "brainpoolP320t1",
                        BitLength = 320,
                        Q = "D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27",
                        A = "D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E24",
                        B = "A7F561E038EB1ED560B3D147DB782013064C19F27ED27C6780AAF77FB8A547CEB5B4FEF422340353",
                        G = "04" + "925BE9FB01AFC6FB4D3E7D4990010F813408AB106C4F09CB7EE07868CC136FFF3357F624A21BED52" 
                                 + "63BA3A7A27483EBF6671DBEF7ABB30EBEE084E58A0B077AD42A5A0989D1EE71B1B9BC0455FB0D2C3",
                        N = "D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C59311",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP384r1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP384r1.ToString(),
                        DisplayName = "brainpoolP384r1",
                        BitLength = 384,
                        Q =
                            "8CB91E82A3386D280F5D6F7E50E641DF152F7109ED5456B412B1DA197FB71123ACD3A729901D1A71874700133107EC53",
                        A = "7BC382C63D8C150C3C72080ACE05AFA0C2BEA28E4FB22787139165EFBA91F90F8AA5814A503AD4EB04A8C7DD22CE2826",
                        B = "04A8C7DD22CE28268B39B55416F0447C2FB77DE107DCD2A62E880EA53EEB62D57CB4390295DBC9943AB78696FA504C11", // added leading zero
                        G = "04" + "1D1C64F068CF45FFA2A63A81B7C13F6B8847A3E77EF14FE3DB7FCAFE0CBD10E8E826E03436D646AAEF87B2E247D4AF1E" 
                                 + "8ABE1D7520F9C2A45CB1EB8E95CFD55262B70B29FEEC5864E19C054FF99129280E4646217791811142820341263C5315",
                        N = "8CB91E82A3386D280F5D6F7E50E641DF152F7109ED5456B31F166E6CAC0425A7CF3AB6AF6B7FC3103B883202E9046565",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP384t1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP384t1.ToString(),
                        DisplayName = "brainpoolP384t1",
                        BitLength = 384,
                        Q = "8CB91E82A3386D280F5D6F7E50E641DF152F7109ED5456B412B1DA197FB71123ACD3A729901D1A71874700133107EC53",
                        A = "8CB91E82A3386D280F5D6F7E50E641DF152F7109ED5456B412B1DA197FB71123ACD3A729901D1A71874700133107EC50",
                        B = "7F519EADA7BDA81BD826DBA647910F8C4B9346ED8CCDC64E4B1ABD11756DCE1D2074AA263B88805CED70355A33B471EE",
                        G = "04" + "18DE98B02DB9A306F2AFCD7235F72A819B80AB12EBD653172476FECD462AABFFC4FF191B946A5F54D8D0AA2F418808CC" 
                                 + "25AB056962D30651A114AFD2755AD336747F93475B7A1FCA3B88F2B6A208CCFE469408584DC2B2912675BF5B9E582928",
                        N = "8CB91E82A3386D280F5D6F7E50E641DF152F7109ED5456B31F166E6CAC0425A7CF3AB6AF6B7FC3103B883202E9046565",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP512r1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP512r1.ToString(),
                        DisplayName = "brainpoolP512r1",
                        BitLength = 512,
                        Q = "AADD9DB8DBE9C48B3FD4E6AE33C9FC07CB308DB3B3C9D20ED6639CCA703308717D4D9B009BC66842AECDA12AE6A380E62881FF2F2D82C68528AA6056583A48F3",
                        A = "7830A3318B603B89E2327145AC234CC594CBDD8D3DF91610A83441CAEA9863BC2DED5D5AA8253AA10A2EF1C98B9AC8B57F1117A72BF2C7B9E7C1AC4D77FC94CA",
                        B = "3DF91610A83441CAEA9863BC2DED5D5AA8253AA10A2EF1C98B9AC8B57F1117A72BF2C7B9E7C1AC4D77FC94CADC083E67984050B75EBAE5DD2809BD638016F723",
                        G = "04 " + "81AEE4BDD82ED9645A21322E9C4C6A9385ED9F70B5D916C1B43B62EEF4D0098EFF3B1F78E2D0D48D50D1687B93B97D5F7C6D5047406A5E688B352209BCB9F822" 
                                  + "7DDE385D566332ECC0EABFA9CF7822FDF209F70024A57B1AA000C55B881F8111B2DCDE494A5F485E5BCA4BD88A2763AED1CA2B2FA8F0540678CD1E0F3AD80892",
                        N = "AADD9DB8DBE9C48B3FD4E6AE33C9FC07CB308DB3B3C9D20ED6639CCA70330870553E5C414CA92619418661197FAC10471DB1D381085DDADDB58796829CA90069",
                        H = "01",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    BrainpoolEllipticCurve.BrainpoolP512t1.ToString(), new FpEcCurveInformation {
                        Name = BrainpoolEllipticCurve.BrainpoolP512t1.ToString(),
                        DisplayName = "brainpoolP512t1",
                        BitLength = 512,
                        Q = "AADD9DB8DBE9C48B3FD4E6AE33C9FC07CB308DB3B3C9D20ED6639CCA703308717D4D9B009BC66842AECDA12AE6A380E62881FF2F2D82C68528AA6056583A48F3",
                        A = "AADD9DB8DBE9C48B3FD4E6AE33C9FC07CB308DB3B3C9D20ED6639CCA703308717D4D9B009BC66842AECDA12AE6A380E62881FF2F2D82C68528AA6056583A48F0",
                        B = "7CBBBCF9441CFAB76E1890E46884EAE321F70C0BCB4981527897504BEC3E36A62BCDFA2304976540F6450085F2DAE145C22553B465763689180EA2571867423E",
                        G = "04" + "640ECE5C12788717B9C1BA06CBC2A6FEBA85842458C56DDE9DB1758D39C0313D82BA51735CDB3EA499AA77A7D6943A64F7A3F25FE26F06B51BAA2696FA9035DA" 
                                 + "5B534BD595F5AF0FA2C892376C84ACE1BB4E3019B71634C01131159CAE03CEE9D9932184BEEF216BD71DF2DADF86A627306ECFF96DBB8BACE198B61E00F8B332",
                        N = "AADD9DB8DBE9C48B3FD4E6AE33C9FC07CB308DB3B3C9D20ED6639CCA70330870553E5C414CA92619418661197FAC10471DB1D381085DDADDB58796829CA90069",
                        H = "01",
                        Seed = null
                    }),
                /* START SEC2/NIST FP CURVES (NON-CUSTOM-IMPLEMENTATION) */
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp160r1.ToString(), new FpEcCurveInformation {
                        Name = Sec2EllipticCurve.Secp160r1.ToString(),
                        DisplayName = "secp160r1",
                        BitLength = 160,
                        Q = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7FFFFFFF",
                        A = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7FFFFFFC",
                        B = "1C97BEFC54BD7A8B65ACF89F81D4D4ADC565FA45",
                        G = "04" + "4A96B5688EF573284664698968C38BB913CBFC82"
                                 + "23A628553168947D59DCC912042351377AC5FB32",
                        N = "0100000000000000000001F4C8F927AED3CA752257",
                        H = "01",
                        Seed = "1053CDE42C14D696E67687561517533BF3F83345"
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Secp160r2.ToString(), new FpEcCurveInformation {
                        Name = Sec2EllipticCurve.Secp160r2.ToString(),
                        DisplayName = "secp160r2",
                        BitLength = 160,
                        Q = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFAC73",
                        A = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFAC70",
                        B = "B4E134D3FB59EB8BAB57274904664D5AF50388BA",
                        G = "04" + "52DCB034293A117E1F4FF11B30F7199D3144CE6D"
                                 + "FEAFFEF2E331F296E071FA0DF9982CFEA7D43F2E",
                        N = "0100000000000000000000351EE786A818F3A1A16B",
                        H = "01",
                        Seed = "B99B99B099B323E02709A4D696E6768756151751"
                    }),
                /* START SEC2/NIST F2M CURVES (NON-CUSTOM-IMPLEMENTATION) */
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect163r1.ToString(), new PpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect163r1.ToString(),
                        DisplayName = "sect163r1",
                        BitLength = 163,
                        M = 163,
                        K1 = 3,
                        K2 = 6,
                        K3 = 7,
                        A = "07B6882CAAEFA84F9554FF8428BD88E246D2782AE2",
                        B = "0713612DCDDCB40AAB946BDA29CA91F73AF958AFD9",
                        G = "04" + "0369979697AB43897789566789567F787A7876A654"
                                 + "00435EDB42EFAFB2989D51FEFCE3C80988F41FF883",
                        N = "03FFFFFFFFFFFFFFFFFFFF48AAB689C29CA710279B",
                        H = "02",
                        Seed = "24B7B137C8A14D696E6768756151756FD0DA2E5C"
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect163r2.ToString(), new PpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect163r2.ToString(),
                        DisplayName = "sect163r2",
                        BitLength = 163,
                        M = 163,
                        K1 = 3,
                        K2 = 6,
                        K3 = 7,
                        A = "01",
                        B = "020A601907B8C953CA1481EB10512F78744A3205FD",
                        G = "04" + "03F0EBA16286A2D57EA0991168D4994637E8343E36"
                                 + "00D51FBC6C71A0094FA2CDD545B11C5C0C797324F1",
                        N = "040000000000000000000292FE77E70C12A4234C33",
                        H = "02",
                        Seed = "85E25BFE5C86226CDB12016F7553F9D0E693A268"
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect193r1.ToString(), new TpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect193r1.ToString(),
                        DisplayName = "sect193r1",
                        BitLength = 193,
                        M = 193,
                        K = 15,
                        A = "0017858FEB7A98975169E171F77B4087DE098AC8A911DF7B01",
                        B = "00FDFB49BFE6C3A89FACADAA7A1E5BBC7CC1C2E5D831478814",
                        G = "04" + "01F481BC5F0FF84A74AD6CDF6FDEF4BF6179625372D8C0C5E1"
                                 + "0025E399F2903712CCF3EA9E3A1AD17FB0B3201B6AF7CE1B05",
                        N = "01000000000000000000000000C7F34A778F443ACC920EBA49",
                        H = "02",
                        Seed = "103FAEC74D696E676875615175777FC5B191EF30"
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect193r2.ToString(), new TpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect193r2.ToString(),
                        DisplayName = "sect193r2",
                        BitLength = 193,
                        M = 193,
                        K = 15,
                        A = "0163F35A5137C2CE3EA6ED8667190B0BC43ECD69977702709B",
                        B = "00C9BB9E8927D4D64C377E2AB2856A5B16E3EFB7F61D4316AE",
                        G = "04" + "00D9B67D192E0367C803F39E1A7E82CA14A651350AAE617E8F"
                                 + "01CE94335607C304AC29E7DEFBD9CA01F596F927224CDECF6C",
                        N = "010000000000000000000000015AAB561B005413CCD4EE99D5",
                        H = "02",
                        Seed = "10B7B4D696E676875615175137C8A16FD0DA2211"
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect233k1.ToString(), new TpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect233k1.ToString(),
                        DisplayName = "sect233k1",
                        BitLength = 233,
                        M = 233,
                        K = 74,
                        A = "0",
                        B = "01",
                        G = "04" + "017232BA853A7E731AF129F22FF4149563A419C26BF50A4C9D6EEFAD6126"
                                 + "01DB537DECE819B7F70F555A67C427A8CD9BF18AEB9B56E0C11056FAE6A3",
                        N = "8000000000000000000000000000069D5BB915BCD46EFB1AD5F173ABDF",
                        H = "04",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect233r1.ToString(), new TpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect233r1.ToString(),
                        DisplayName = "sect233r1",
                        BitLength = 233,
                        M = 233,
                        K = 74,
                        A = "01",
                        B = "0066647EDE6C332C7F8C0923BB58213B333B20E9CE4281FE115F7D8F90AD",
                        G = "04" + "00FAC9DFCBAC8313BB2139F1BB755FEF65BC391F8B36F8F8EB7371FD558B"
                                 + "01006A08A41903350678E58528BEBF8A0BEFF867A7CA36716F7E01F81052",
                        N = "01000000000000000000000000000013E974E72F8A6922031D2603CFE0D7",
                        H = "02",
                        Seed = "74D59FF07F6B413D0EA14B344B20A2DB049B50C3"
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect239k1.ToString(), new TpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect239k1.ToString(),
                        DisplayName = "sect239k1",
                        BitLength = 239,
                        M = 239,
                        K = 158,
                        A = "0",
                        B = "01",
                        G = "04" + "29A0B6A887A983E9730988A68727A8B2D126C44CC2CC7B2A6555193035DC"
                                 + "76310804F12E549BDB011C103089E73510ACB275FC312A5DC6B76553F0CA",
                        N = "2000000000000000000000000000005A79FEC67CB6E91F1C1DA800E478A5",
                        H = "04",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect283k1.ToString(), new PpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect283k1.ToString(),
                        DisplayName = "sect283k1",
                        BitLength = 283,
                        M = 283,
                        K1 = 5,
                        K2 = 7,
                        K3 = 12,
                        A = "0",
                        B = "01",
                        G = "04" + "0503213F78CA44883F1A3B8162F188E553CD265F23C1567A16876913B0C2AC2458492836"
                                 + "01CCDA380F1C9E318D90F95D07E5426FE87E45C0E8184698E45962364E34116177DD2259",
                        N = "01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE9AE2ED07577265DFF7F94451E061E163C61",
                        H = "04",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect283r1.ToString(), new PpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect283r1.ToString(),
                        DisplayName = "sect283r1",
                        BitLength = 283,
                        M = 283,
                        K1 = 5,
                        K2 = 7,
                        K3 = 12,
                        A = "01",
                        B = "027B680AC8B8596DA5A4AF8A19A0303FCA97FD7645309FA2A581485AF6263E313B79A2F5",
                        G = "04" + "05F939258DB7DD90E1934F8C70B0DFEC2EED25B8557EAC9C80E2E198F8CDBECD86B12053"
                                 + "03676854FE24141CB98FE6D4B20D02B4516FF702350EDDB0826779C813F0DF45BE8112F4",
                        N = "03FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEF90399660FC938A90165B042A7CEFADB307",
                        H = "02",
                        Seed = "77E2B07370EB0F832A6DD5B62DFC88CD06BB84BE"
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect409k1.ToString(), new TpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect409k1.ToString(),
                        DisplayName = "sect409k1",
                        BitLength = 409,
                        M = 409,
                        K = 87,
                        A = "0",
                        B = "01",
                        G = "04" + "0060F05F658F49C1AD3AB1890F7184210EFD0987E307C84C27ACCFB8F9F67CC2C460189EB5AAAA62EE222EB1B35540CFE9023746"
                                 + "01E369050B7C4E42ACBA1DACBF04299C3460782F918EA427E6325165E9EA10E3DA5F6C42E9C55215AA9CA27A5863EC48D8E0286B",
                        N = "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE5F83B2D4EA20400EC4557D5ED3E3E7CA5B4B5C83B8E01E5FCF",
                        H = "04",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect409r1.ToString(), new TpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect409r1.ToString(),
                        DisplayName = "sect409r1",
                        BitLength = 409,
                        M = 409,
                        K = 87,
                        A = "01",
                        B = "0021A5C2C8EE9FEB5C4B9A753B7B476B7FD6422EF1F3DD674761FA99D6AC27C8A9A197B272822F6CD57A55AA4F50AE317B13545F",
                        G = "04" + "015D4860D088DDB3496B0C6064756260441CDE4AF1771D4DB01FFE5B34E59703DC255A868A1180515603AEAB60794E54BB7996A7"
                                 + "0061B1CFAB6BE5F32BBFA78324ED106A7636B9C5A7BD198D0158AA4F5488D08F38514F1FDF4B4F40D2181B3681C364BA0273C706",
                        N = "010000000000000000000000000000000000000000000000000001E2AAD6A612F33307BE5FA47C3C9E052F838164CD37D9A21173",
                        H = "02",
                        Seed = "4099B5A457F9D69F79213D094C4BCD4D4262210B"
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect571k1.ToString(), new PpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect571k1.ToString(),
                        DisplayName = "sect571k1",
                        BitLength = 571,
                        M = 571,
                        K1 = 2,
                        K2 = 5,
                        K3 = 10,
                        A = "0",
                        B = "01",
                        G = "04" + "026EB7A859923FBC82189631F8103FE4AC9CA2970012D5D46024804801841CA44370958493B205E647DA304DB4CEB08CBBD1BA39494776FB988B47174DCA88C7E2945283A01C8972"
                                 + "0349DC807F4FBF374F4AEADE3BCA95314DD58CEC9F307A54FFC61EFC006D8A2C9D4979C0AC44AEA74FBEBBB9F772AEDCB620B01A7BA7AF1B320430C8591984F601CD4C143EF1C7A3",
                        N = "020000000000000000000000000000000000000000000000000000000000000000000000131850E1F19A63E4B391A8DB917F4138B630D84BE5D639381E91DEB45CFE778F637C1001",
                        H = "04",
                        Seed = null
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    Sec2EllipticCurve.Sect571r1.ToString(), new PpbF2mEcCurveInformation {
                        Name = Sec2EllipticCurve.Sect571r1.ToString(),
                        DisplayName = "sect571r1",
                        BitLength = 571,
                        M = 571,
                        K1 = 2,
                        K2 = 5,
                        K3 = 10,
                        A = "01",
                        B = "02F40E7E2221F295DE297117B7F3D62F5C6A97FFCB8CEFF1CD6BA8CE4A9A18AD84FFABBD8EFA59332BE7AD6756A66E294AFD185A78FF12AA520E4DE739BACA0C7FFEFF7F2955727A",
                        G = "04" + "0303001D34B856296C16C0D40D3CD7750A93D1D2955FA80AA5F40FC8DB7B2ABDBDE53950F4C0D293CDD711A35B67FB1499AE60038614F1394ABFA3B4C850D927E1E7769C8EEC2D19"
                                 + "037BF27342DA639B6DCCFFFEB73D69D78C6C27A6009CBBCA1980F8533921E8A684423E43BAB08A576291AF8F461BB2A8B3531D2F0485C19B16E2F1516E23DD3C1A4827AF1B8AC15B",
                        N = "03FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE661CE18FF55987308059B186823851EC7DD9CA1161DE93D5174D66E8382E9BB2FE84E47",
                        H = "02",
                        Seed = "2AA058F73A0E33AB486B0F610410C53A7F132310"
                    }),
                /* START DANIEL J. BERNSTEIN CURVES (CUSTOM, NON-COMPATIBLE WITH ECDOMAINPARAMETERS OBJECT IMPLEMENTATIONS) */
                new KeyValuePair<string, EcCurveInformation>(
                    DjbCurve.Curve25519.ToString(), new DjbEcInformation {
                        Name = DjbCurve.Curve25519.ToString(),
                        DisplayName = "curve25519",
                        BitLength = 255
                    }),
                new KeyValuePair<string, EcCurveInformation>(
                    DjbCurve.Ed25519.ToString(), new DjbEcInformation {
                        Name = DjbCurve.Ed25519.ToString(),
                        DisplayName = "ed25519",
                        BitLength = 255
                    })
            });
        }

        #endregion

        /// <summary>
        ///     Determine the curve provider name from the curve name.
        /// </summary>
        /// <param name="curveName">Name of the curve.</param>
        /// <returns>Name of the curve provider.</returns>
        public static string GetProvider(string curveName)
        {
            if (curveName.IsMemberInEnum<DjbCurve>()) {
                return "DJB";
            }
            if (curveName.IsMemberInEnum<BrainpoolEllipticCurve>()) {
                return "Brainpool";
            }
            if (curveName.IsMemberInEnum<Sec2EllipticCurve>()) {
                return "SEC";
            }

            throw new NotSupportedException();
        }

        public static EcCurveInformation GetECCurveData(BrainpoolEllipticCurve curveEnum)
        {
            return GetECCurveData(curveEnum.ToString());
        }

        public static EcCurveInformation GetECCurveData(Sec2EllipticCurve curveEnum)
        {
            return GetECCurveData(curveEnum.ToString());
        }

        /// <summary>
        ///     Get information for a named curve from the curve name.
        /// </summary>
        /// <param name="name">Name of the curve.</param>
        /// <returns>Data for the curve as a <see cref="EcCurveInformation" />.</returns>
        public static EcCurveInformation GetECCurveData(string name)
        {
            if (Athena.Cryptography.EllipticCurves.ContainsKey(name) == false) {
                throw new NotSupportedException("Named curve is unknown or unsupported.");
            }
            return Athena.Cryptography.EllipticCurves[name];
        }
    }
}
