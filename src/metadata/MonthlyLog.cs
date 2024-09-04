using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.DGS
{
    public class MonthlyLog
    {
        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Unit = FieldUnits.Month, Desc = "Simulation Month")]
        public int Month { set; get; }

        [DataFieldAttribute(Desc = "Climate Region Name")]
        public string ClimateRegionName { set; get; }

        [DataFieldAttribute(Desc = "Climate Region Index")]
        public int ClimateRegionIndex { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Sites")]
        public int NumSites { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "Precipitation", Format = "0.00")]
        public double Ppt {get; set;}

        [DataFieldAttribute(Unit = FieldUnits.DegreeC, Desc = "Air Temperature", Format = "0.00")]
        public double Airtemp { get; set; }

        //[DataFieldAttribute(Unit = FieldUnits.DegreeC, Desc = "Soil Temperature", Format = "0.00")]
        //public double SoilTemp { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Aboveground NPP C", Format = "0.00")]
        public double avgNPPtc { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Wood Respiration", Format = "0.0000")]
        public double AvgDeadWoodResp { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Root Respiration", Format = "0.0000")]
        public double AvgDeadRootResp { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Leaf Respiration", Format = "0.0000")]
        public double AvgDeadLeafResp { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Heterotrophic Respiration", Format = "0.0000")]
        public double avgResp { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Net Ecosystem Exchange", Format = "0.00")]
        public double avgNEE { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "N Deposition", Format = "0.0000")]
        public double Ndep { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "N Leaching", Format = "0.0000")]
        public double StreamN { get; set; }
    }
}
