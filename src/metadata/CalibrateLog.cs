using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.DGS
{
    public class CalibrateLog
    {

        public static int year, month, climateRegionIndex, cohortAge;
        public static double cohortWoodB, cohortLeafB;
        public static string speciesName;
        public static double mortalityAGEwood, mortalityAGEleaf;
        public static double availableWater;
        public static double rlai, tlai, LAI;
        public static double mineralNalloc, resorbedNalloc;
        public static double limitLAI, limitH20, limitT, limitN, limitLAIcompetition;
        public static double maxNPP, maxB, siteB, cohortB, soilTemp;
        public static double actualWoodNPP, actualLeafNPP;
        public static double deltaWood, deltaLeaf;
        public static double mortalityBIOwood, mortalityBIOleaf;        
        public static double resorbedNused, mineralNused, demand_N;

        public static void WriteLogFile()
        {
            Outputs.calibrateLog.Clear();            
            CalibrateLog clog = new CalibrateLog();

            clog.Year = year;
            clog.Month = month;
            clog.ClimateRegionIndex = climateRegionIndex;
            clog.CohortAge = cohortAge;
            clog.CohortWoodBiomass = cohortWoodB;
            clog.CohortLeafBiomass = cohortLeafB;
            clog.SpeciesName = speciesName;
            clog.MortalityAGEwoodBiomass = mortalityAGEwood;
            clog.MortalityAGEleafBiomass = mortalityAGEleaf;
            clog.MortalityTHINwoodBiomass = mortalityBIOwood;
            clog.MortalityTHINleafBiomass = mortalityBIOleaf;            
            clog.Rlai = rlai;
            clog.Tlai = tlai;                         
            clog.ActualLAI = LAI; // Chihiro, 2021.03.26: added
            clog.GrowthLimitLAIcompetition = limitLAIcompetition; // Chihiro, 2021.03.26: added            
            clog.MineralNalloc = mineralNalloc;
            clog.ResorbedNalloc = resorbedNalloc;
            clog.GrowthLimitLAI = limitLAI;
            clog.GrowthLimitT = limitH20;
            clog.GrowthLimitSoilWater = limitT;
            clog.GrowthLimitN = limitN;
            clog.SoilTemperature = soilTemp;
            clog.AvailableWater = availableWater;                                 
            clog.MaximumANPP = maxNPP;
            clog.CohortMaximumBiomass = maxB;
            clog.TotalSiteBiomass = siteB;
            clog.CohortBiomass = cohortB;           
            clog.ActualWoodANPP = actualWoodNPP;
            clog.ActualLeafANPP = actualLeafNPP;
            clog.ResorbedNconsumed = resorbedNused;
            clog.MineralNconsumed = mineralNused;
            clog.TotalNDemand = demand_N;
            clog.DeltaWood = deltaWood;
            clog.DeltaLeaf = deltaLeaf;
            


            Outputs.calibrateLog.AddObject(clog);
            Outputs.calibrateLog.WriteToFile();

        }


        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Year {set; get;}

        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.Month, Desc = "Simulation Month")]
        public int Month { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Index", Desc = "Climate Region Index")]
        public int ClimateRegionIndex { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Desc = "Species Name")]
        public string SpeciesName { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit= FieldUnits.Year, Desc = "CohortAge")]
        public int CohortAge { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Cohort Wood Biomass", Format = "0.0")]
        public double CohortWoodBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Cohort Leaf Biomass", Format = "0.0")]
        public double CohortLeafBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Cohort Total Biomass", Format = "0.0")]
        public double CohortBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Mortality Age Wood Biomass", Format = "0.000")]
        public double MortalityAGEwoodBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Mortality Age Leaf Biomass", Format = "0.00000")]
        public double MortalityAGEleafBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Mortality Thinning Wood Biomass", Format = "0.000")]
        public double MortalityTHINwoodBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Mortality Thinning Leaf Biomass", Format = "0.00000")]
        public double MortalityTHINleafBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "rLAI", Format = "0.00")]
        public double Rlai { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "tLAI", Format = "0.00")]
        public double Tlai { set; get; }
        // ********************************************************************
        // Chihiro, 2021.03.26: modified from LAI
        [DataFieldAttribute(Unit = "m2_m-2", Desc = "Actual LAI adjusted by seasonality", Format = "0.00")]
        public double ActualLAI { set; get; }
        // ********************************************************************    
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit LAI", Format = "0.00")]
        public double GrowthLimitLAI { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit Soil Water", Format = "0.00")]
        public double GrowthLimitSoilWater { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit Temperature", Format = "0.00")]
        public double GrowthLimitT { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit Nitrogen", Format = "0.00")]
        public double GrowthLimitN { set; get; }
        // ********************************************************************
        // Chihiro, 2021.03.26: added
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit LAI competition", Format = "0.00")]
        public double GrowthLimitLAIcompetition { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Maximum ANPP")]
        public double MaximumANPP { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Actual Wood ANPP", Format = "0.000")]
        public double ActualWoodANPP { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Actual Leaf ANPP", Format = "0.0000")]
        public double ActualLeafANPP { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Change in Wood Biomass", Format = "0.000")]
        public double DeltaWood { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Change in Leaf Biomass", Format = "0.0000")]
        public double DeltaLeaf { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Cohort Maximum Biomass")]
        public double CohortMaximumBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Total Site Biomass")]
        public double TotalSiteBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "Available Water", Format = "0.00")]
        public double AvailableWater { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.DegreeC, Desc = "Soil Temperature", Format = "0.00")]
        public double SoilTemperature { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Mineral N Allocation", Format = "0.000")]
        public double MineralNalloc { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Resorbed N Allocation", Format = "0.000")]
        public double ResorbedNalloc { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Mineral N Consumed", Format = "0.000")]
        public double MineralNconsumed { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Resorbed N Consumed", Format = "0.000")]
        public double ResorbedNconsumed { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Total N Demand", Format = "0.000")]
        public double TotalNDemand { set; get; }

    }
}
