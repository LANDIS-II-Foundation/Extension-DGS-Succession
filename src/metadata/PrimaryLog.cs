using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.DGS
{
    public class PrimaryLog
    {
            //log.WriteLine("");

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Desc = "Name of Climate Region")]
        public string ClimateRegionName { set; get; }

        [DataFieldAttribute(Desc = "Climate Region Index")]
        public int ClimateRegionIndex { set; get; }

                [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Sites")]
        public int NumSites { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Net Ecosystem Exchange C", Format = "0.0")]
        public double NEEC {get; set;}

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Total Soil Organic Carbon", Format = "0.0")]
        public double SOMTC { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Aboveground Biomass", Format = "0.0")]
        public double AGB { get; set; }
        
        //log.Write("AG_NPPC, BG_NPPC, LitterfallC, AgeMortality, ");
        [DataFieldAttribute(Unit = FieldUnits.g_C_m2_yr1, Desc = "Aboveground NPP C", Format = "0.0")]
        public double AG_NPPC { get; set; }
        
        [DataFieldAttribute(Unit = FieldUnits.g_C_m2_yr1, Desc = "Below ground NPP C", Format = "0.0")]
        public double BG_NPPC { get; set; }
        
        [DataFieldAttribute(Unit = FieldUnits.g_C_m2_yr1, Desc = "Litterfall C", Format = "0.0")]
        public double Litterfall { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2_yr1, Desc = "Age Mortality Biomass", Format = "0.0")]
        public double AgeMortality { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2_yr1, Desc = "Growth Mortality Biomass", Format = "0.0")]
        public double GrowthMortality { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Mineral N", Format = "0.00")]
        public double MineralN { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Gross Mineralization", Format = "0.0")]
        public double GrossMineralization { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Total N", Format = "0.0")]
        public double TotalN { get; set; }
        
        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Live Leaf C", Format = "0.0")]
        public double C_LiveLeaf { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Live Fine Root C", Format = "0.0")]
        public double C_LiveFRoot { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Live Wood C", Format = "0.0")]
        public double C_LiveWood { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Live Coarse Root C", Format = "0.0")]
        public double C_LiveCRoot { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Wood C", Format = "0.0")]
        public double C_DeadWood { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Coarse Root C", Format = "0.0")]
        public double C_DeadCRoot { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Leaf Structural C", Format = "0.0")]
        public double C_DeadLeaf_Struc { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Leaf Metabolic C", Format = "0.0")]
        public double C_DeadLeaf_Meta { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Fine Root Structural C", Format = "0.0")]
        public double C_DeadFRoot_Struc { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Fine RootMetabolic C", Format = "0.0")]
        public double C_DeadFRoot_Meta { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Available SOC", Format = "0.0")]
        public double C_SoilAvailable { get; set; }
        
        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Leaf N", Format = "0.0")]
        public double N_Leaf { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Fine Root N", Format = "0.0")]
        public double N_FRoot { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Wood N", Format = "0.0")]
        public double N_Wood { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Coarse Root N", Format = "0.0")]
        public double N_CRoot { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Dead Wood N", Format = "0.0")]
        public double N_DeadWood { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Dead Coarse Root N", Format = "0.0")]
        public double N_DeadCRoot { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Dead Leaf Structural N", Format = "0.0")]
        public double N_DeadLeaf_Struc { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Dead Leaf Metabolic N", Format = "0.0")]
        public double N_DeadLeaf_Meta { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Dead Fine Root Structural N", Format = "0.0")]
        public double N_DeadFRoot_Struc { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Dead Fine Root Metabolic N", Format = "0.0")]
        public double N_DeadFRoot_Meta { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Stream C", Format = "0.00")]
        public double StreamC { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Stream N", Format = "0.00")]
        public double StreamN { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Fire C Efflux", Format = "0.0")]
        public double FireCEfflux { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Fire N Efflux", Format = "0.0")]
        public double FireNEfflux { get; set; }
        
        //log.Write("Nuptake, Nresorbed, TotalSoilN, Nvol, avgfrassC,");
        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "N Uptake", Format = "0.0")]
        public double Nuptake { get; set; }
        
        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "N Resorbed", Format = "0.0")]
        public double Nresorbed { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Total Soil N", Format = "0.0")]
        public double TotalSoilN { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "N Volatilized", Format = "0.00")]
        public double Nvol { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Frass C", Format = "0.0")]
        public double FrassC { get; set; }

    }
}
