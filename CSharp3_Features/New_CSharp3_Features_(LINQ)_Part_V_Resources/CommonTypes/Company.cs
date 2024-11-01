using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonTypes
{
    /// <summary>
    /// A simple class Company that represents a company enlisted in the NASDAQ (National
    /// Association of Securities Dealers Automated Quotations).
    /// </summary>
    public class Company
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="publicNasdaq"></param>
        public Company(string name, string publicNasdaq)
        {
            Name = name;
            PublicNasdaq = publicNasdaq;
        }

        /// <summary>
        /// The public name of the company.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// The public id of the company at the NASDAQ directory.
        /// </summary>
        public string PublicNasdaq { get; set; }


        public static readonly IEnumerable<Company> Companies = new[] 
            {
                new Company("Activision Blizzard, Inc",	"ATVI"),
                #region more ...
                new Company("Adobe Systems Incorporated",	"ADBE"),
                new Company("Altera Corporation",	"ALTR"),
                new Company("Amazon.com, Inc.",	"AMZN"),
                new Company("Amgen Inc.",	"AMGN"),
                new Company("Apollo Group, Inc.",	"APOL"),
                new Company("Apple Inc.",	"AAPL"),
                new Company("Applied Materials, Inc.",	"AMAT"),
                new Company("Autodesk, Inc.",	"ADSK"),
                new Company("Automatic Data Processing, Inc.",	"ADP"),
                new Company("Avid",	"AVID"),
                new Company("Baidu, Inc.",	"BIDU"),
                new Company("Bed Bath & Beyond Inc.	","BBBY"),
                new Company("Biogen Idec Inc",	"BIIB"),
                new Company("BMC Software, Inc.",	"BMC"),
                new Company("Broadcom Corporation",	"BRCM"),
                new Company("C.H. Robinson Worldwide, Inc.",	"CHRW"),
                new Company("CA Inc.",	"CA"),
                new Company("Celgene Corporation",	"CELG"),
                new Company("Cephalon, Inc.","CEPH"),
                new Company("Cerner Corporation",	"CERN"),
                new Company("Check Point Software Technologies Ltd.",	"CHKP"),
                new Company("Cintas Corporation",	"CTAS"),
                new Company("Cisco Systems, Inc.",	"CSCO"),
                new Company("Citrix Systems, Inc.",	"CTXS"),
                new Company("Cognizant Technology Solutions Corporation",	"CTSH"),
                new Company("Comcast Corporation",	"CMCSA"),
                new Company("Costco Wholesale Corporation",	"COST"),
                new Company("Dell Inc.",	"DELL"),
                new Company("DENTSPLY International Inc.",	"XRAY"),
                new Company("DIRECTV",	"DTV"),
                new Company("DISH Network Corporation",	"DISH"),
                new Company("eBay Inc.",	"EBAY"),
                new Company("Electronic Arts Inc.",	"ERTS"),
                new Company("Expedia, Inc.",	"EXPE"),
                new Company("Expeditors International of Washington, Inc.",	"EXPD"),
                new Company("Express Scripts, Inc.",	"ESRX"),
                new Company("Fastenal Company",	"FAST"),
                new Company("First Solar, Inc.",	"FSLR"),
                new Company("Fiserv, Inc.",	"FISV"),
                new Company("Flextronics International Ltd.",	"FLEX"),
                new Company("FLIR Systems, Inc.",	"FLIR"),
                new Company("Foster Wheeler AG.",	"FWLT"),
                new Company("Garmin Ltd.",	"GRMN"),
                new Company("Genzyme Corporation",	"GENZ"),
                new Company("Gilead Sciences, Inc.",	"GILD"),
                new Company("Google Inc.",	"GOOG"),
                new Company("Henry Schein, Inc.",	"HSIC"),
                new Company("Hologic, Inc.",	"HOLX"),
                new Company("Illumina, Inc.",	"ILMN"),
                new Company("Infosys Technologies Limited",	"INFY"),
                new Company("Intel Corporation",	"INTC"),
                new Company("Intuit Inc.",	"INTU"),
                new Company("Intuitive Surgical, Inc.",	"ISRG"),
                new Company("J.B. Hunt Transport Services, Inc.",	"JBHT"),
                new Company("Joy Global Inc.",	"JOYG"),
                new Company("KLA-Tencor Corporation","KLAC"),
                new Company("Lam Research Corporation",	"LRCX"),
                new Company("Liberty Media Corporation",	"LINTA"),
                new Company("Life Technologies Corporation",	"LIFE"),
                new Company("Linear Technology Corporation",	"LLTC"),
                new Company("Logitech International S.A.",	"LOGI"),
                new Company("Marvell Technology Group, Ltd.",	"MRVL"),
                new Company("Mattel, Inc.",	"MAT"),
                new Company("Maxim Integrated Products, Inc.",	"MXIM"),
                new Company("Microchip Technology Incorporated",	"MCHP"),
                new Company("Microsoft Corporation",	"MSFT"),
                new Company("Millicom International Cellular S.A.",	"MICC"),
                new Company("Mylan Inc.",	"MYL"),
                new Company("NetApp, Inc.",	"NTAP"),
                new Company("News Corporation",	"NWSA"),
                new Company("NII Holdings, Inc.",	"NIHD"),
                new Company("NVIDIA Corporation",	"NVDA"),
                new Company("O'Reilly Automotive, Inc.",	"ORLY"),
                new Company("Oracle Corporation",	"ORCL"),
                new Company("PACCAR Inc.",	"PCAR"),
                new Company("Patterson Companies Inc.",	"PDCO"),
                new Company("Paychex, Inc.",	"PAYX"),
                new Company("priceline.com Incorporated",	"PCLN"),
                new Company("Qiagen N.V.",	"QGEN"),
                new Company("QUALCOMM Incorporated","QCOM"),
                new Company("Research in Motion Limited",	"RIMM"),
                new Company("Ross Stores, Inc.",	"ROST"),
                new Company("SanDisk Corporation",	"SNDK"),
                new Company("Seagate Technology.",	"STX"),
                new Company("Sears Holdings Corporation",	"SHLD"),
                new Company("Sigma-Aldrich Corporation",	"SIAL"),
                new Company("Staples, Inc.",	"SPLS"),
                new Company("Starbucks Corporation",	"SBUX"),
                new Company("Stericycle, Inc.",	"SRCL"),
                new Company("Symantec Corporation",	"SYMC"),
                new Company("Teva Pharmaceutical Industries Limited",	"TEVA"),
                new Company("Urban Outfitters, Inc.",	"URBN"),
                new Company("VeriSign, Inc.",	"VRSN"),
                new Company("Vertex Pharmaceuticals Incorporated",	"VRTX"),
                new Company("Virgin Media Inc.",	"VMED"),
                new Company("Vodafone Group Plc",	"VOD"),
                new Company("Warner Chilcott plc",	"WCRX"),
                new Company("Wynn Resorts, Limited",	"WYNN"),
                new Company("Xilinx, Inc.",	"XLNX"),
                #endregion
                new Company("Yahoo! Inc.",	"YHOO")
            };
    }
}
