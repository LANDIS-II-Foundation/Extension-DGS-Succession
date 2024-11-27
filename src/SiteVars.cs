//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.UniversalCohorts;  
using System.Collections.Generic;
using System;

namespace Landis.Extension.Succession.DGS
{
    /// <summary>
    /// The pools of dead biomass for the landscape's sites.
    /// </summary>
    public static class SiteVars
    {
        // Time of last succession simulation:
        private static ISiteVar<int> timeOfLast;

        // Live biomass:        
        //private static ISiteVar<Landis.Library.UniversalCohorts.SiteCohorts> universalCohortsSiteVar;       

        // Dead biomass:
        //private static ISiteVar<Layer> SurfaceDeadWood;
        //private static ISiteVar<Layer> SoilDeadWood;
        
        //private static ISiteVar<Layer> SurfaceStructural;
        //private static ISiteVar<Layer> SurfaceMetabolic;
        //private static ISiteVar<Layer> SoilStructural;
        //private static ISiteVar<Layer> SoilMetabolic;
                       
        // Soil layers
        //private static ISiteVar<Layer> som1surface;
        //private static ISiteVar<SoilLayer> SoilPrimary;
        //private static ISiteVar<SoilLayer> SoilAvailable;
        //private static ISiteVar<Layer> dissolved_organic;
        //private static ISiteVar<Layer> som2;
        //private static ISiteVar<Layer> som3;
        //private static ISiteVar<int> SoilDepth;
        //private static ISiteVar<double> SoilDrain;
        //private static ISiteVar<double> SoilBaseFlowFraction;
        //private static ISiteVar<double> SoilStormFlowFraction;
        //private static ISiteVar<double> SoilFieldCapacity;
        //private static ISiteVar<double> SoilWiltingPoint;
        //private static ISiteVar<double> SoilPercentSand;
        //private static ISiteVar<double> SoilPercentClay;
        //private static ISiteVar<double> SoilBulkDensity;
        //private static ISiteVar<double> SoilParticleDensity;


        // Similar to soil layers with respect to their pools:
        private static ISiteVar<Layer> stream;
        private static ISiteVar<Layer> sourceSink;

        // Other variables:
        //private static ISiteVar<double> mineralN;
        //private static ISiteVar<double> resorbedN;
        //private static ISiteVar<double> waterMovement;  
        //private static ISiteVar<double> availableWater;
        //private static ISiteVar<double> activeLayerDepth;
        //private static ISiteVar<double> soilWaterContent;
        //private static ISiteVar<double> liquidSnowPack;  
        //private static ISiteVar<double> decayFactor;
        //private static ISiteVar<double> soilTemperature;
        //private static ISiteVar<double> anaerobicEffect;

        //// Annual accumulators for reporting purposes.
        //private static ISiteVar<double> grossMineralization;
        //private static ISiteVar<double> ag_nppC;
        //private static ISiteVar<double> bg_nppC;
        //private static ISiteVar<double> litterfallC;
        //private static ISiteVar<double> cohortLeafN;
        //private static ISiteVar<double> cohortFRootN;
        //private static ISiteVar<double> cohortLeafC;
        //private static ISiteVar<double> cohortFRootC;
        //private static ISiteVar<double> cohortWoodN;
        //private static ISiteVar<double> cohortCRootN;
        //private static ISiteVar<double> cohortWoodC;
        //private static ISiteVar<double> cohortCRootC;
        //private static ISiteVar<double[]> monthlyAGNPPC;
        //private static ISiteVar<double[]> monthlyBGNPPC;
        //private static ISiteVar<double[]> monthlysoilTemp;
        //private static ISiteVar<double[]> monthlyNEE;
        //private static ISiteVar<double[]> monthlyStreamN;
        //private static ISiteVar<double[]> monthlyLAI;
        //public static ISiteVar<double> AnnualNEE;
        //public static ISiteVar<double> FireCEfflux;
        //public static ISiteVar<double> FireNEfflux;
        //public static ISiteVar<double> Nvol;
        //private static ISiteVar<double[]> monthlyResp;
        //private static ISiteVar<double> totalNuptake;
        //private static ISiteVar<double[]> monthlymineralN;
        //private static ISiteVar<double> frassC;
        //private static ISiteVar<double> lai;
        //private static ISiteVar<double> annualPPT_AET; //Annual water budget calculation. I'm coppying LAI implementation
        //private static ISiteVar<int> dryDays;
        //private static ISiteVar<double> fineFuels;
        //private static ISiteVar<double[]> monthlymineralN;
        public static ISiteVar<double> AnnualNEE;
        public static ISiteVar<double> FireCEfflux;
        public static ISiteVar<double> FireNEfflux;
        public static ISiteVar<double> Nvol;
        public static ISiteVar<double> TotalWoodBiomass;
        public static ISiteVar<int> PrevYearMortality;
        public static ISiteVar<byte> FireSeverity;
        public static ISiteVar<double> WoodGrowthMortality;
        public static ISiteVar<double> WoodAgeMortality;
        public static ISiteVar<string> HarvestPrescriptionName;
        public static ISiteVar<int> HarvestTime;
        //public static ISiteVar<Dictionary<int, Dictionary<int, double>>> CohortResorbedNallocation;
        public static ISiteVar<double> SmolderConsumption;
        public static ISiteVar<double> FlamingConsumption;
        public static ISiteVar<double> AnnualClimaticWaterDeficit; //Annual soil moisture calculation, defined as pet - aet
        public static ISiteVar<double> AnnualPET; //Annual pet

        public static ISiteVar<TempHydroUnit> TempHydroUnit;
        public static ISiteVar<string> ForestTypeName;
        public static ISiteVar<int> TimeOfLastBurn;
        public static ISiteVar<ushort> Slope;
        public static ISiteVar<ushort> Aspect;
        public static ISiteVar<double> fineFuels;

        public static ISiteVar<double[]> MonthlySoilResp;
        public static ISiteVar<double[]> MonthlyLAI;        
        public static ISiteVar<double[]> MonthlyHeteroResp;
        public static ISiteVar<double[]> MonthlySoilWaterContent;
        public static ISiteVar<double[]> MonthlyMeanSoilWaterContent;//SF added
        public static ISiteVar<double[]> MonthlyAnaerobicEffect;//SF added 2023-4-11
        public static ISiteVar<double[]> MonthlyClimaticWaterDeficit;//SF added 2023-6-27
        public static ISiteVar<double[]> MonthlyActualEvapotranspiration;//SF added 2023-6-27
        public static ISiteVar<int> HarvestDisturbedYear;
        public static ISiteVar<int> FireDisturbedYear;
        
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
            //cohorts = PlugIn.ModelCore.Landscape.NewSiteVar<Landis.Library.UniversalCohorts.SiteCohorts>();
            cohorts = PlugIn.ModelCore.Landscape.NewSiteVar<SiteCohorts>();
            //universalCohortsSiteVar = Landis.Library.Succession.CohortSiteVar<Landis.Library.UniversalCohorts.SiteCohorts>.Wrap(cohorts);
            FineFuels = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            TimeOfLast = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            
            // Dead biomass:
            SurfaceDeadWood     = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SoilDeadWood        = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            
            SurfaceStructural   = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SurfaceMetabolic    = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SoilStructural      = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SoilMetabolic       = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();

            // Soil Layers
            //som1surface         = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            //som2                = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            //som3                = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SoilPrimary         = PlugIn.ModelCore.Landscape.NewSiteVar<SoilLayer>();
            SoilAvailable       = PlugIn.ModelCore.Landscape.NewSiteVar<SoilLayer>();
            SoilDepth           = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            SoilDrain           = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilBaseFlowFraction = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilStormFlowFraction = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilFieldCapacity = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilWiltingPoint = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilPercentSand = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilPercentClay = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilBulkDensity = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilParticleDensity = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            // Other Layers
            Stream              = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SourceSink          = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();

            // Other variables
            MonthlyLAI = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MineralN            = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            ResorbedN           = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            WaterMovement       = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AvailableWater      = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            MonthlyActiveLayerDepth = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            ActiveLayerDepth    = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            LiquidSnowPack      = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilWaterContent    = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            DecayFactor         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilTemperature     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnaerobicEffect     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            DryDays             = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            
            // Annual accumulators
            GrossMineralization = PlugIn.ModelCore.Landscape.NewSiteVar<double>();            
            AGNPPcarbon         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            BGNPPcarbon         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            LitterfallC         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            MonthlyAGNPPcarbon = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyBGNPPcarbon = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlySoilTemp     = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyNEE          = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyStreamN      = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            AnnualNEE           = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            FireCEfflux         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            FireNEfflux         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            MonthlyResp         = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyDeadWoodResp = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyDeadRootResp = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyDeadLeafResp = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();

            CohortLeafN = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortFRootN         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortLeafC         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortFRootC     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortWoodN         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortCRootN   = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortWoodC         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortCRootC = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
                        
            TotalWoodBiomass    = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            WoodGrowthMortality        = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            WoodAgeMortality = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            Nvol                = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            PrevYearMortality   = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            TotalNuptake        = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            MonthlymineralN     = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            FrassC              = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            LAI                 = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnnualPPT_AET       = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnnualClimaticWaterDeficit = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnnualPET = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SmolderConsumption = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            FlamingConsumption = PlugIn.ModelCore.Landscape.NewSiteVar<double>(); 
            HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
            //HarvestTime = PlugIn.ModelCore.GetSiteVar<int>("Harvest.TimeOfLastEvent");
            HarvestTime = PlugIn.ModelCore.Landscape.NewSiteVar<int>();

            //CohortResorbedNallocation = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, Dictionary<int, double>>>();

            //PlugIn.ModelCore.RegisterSiteVar(universalCohortsSiteVar, "Succession.UniversalCohorts");
            PlugIn.ModelCore.RegisterSiteVar(cohorts, "Succession.UniversalCohorts");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.FineFuels, "Succession.FineFuels");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.SmolderConsumption, "Succession.SmolderConsumption");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.FlamingConsumption, "Succession.FlamingConsumption");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.AnnualClimaticWaterDeficit, "Succession.CWD");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.AnnualPET, "Succession.PET");

            TempHydroUnit = PlugIn.ModelCore.Landscape.NewSiteVar<TempHydroUnit>();
            ForestTypeName = PlugIn.ModelCore.GetSiteVar<string>("Output.ForestType");
            TimeOfLastBurn = PlugIn.ModelCore.GetSiteVar<int>("Fire.TimeOfLast");

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                SurfaceDeadWood[site]       = new Layer(LayerName.Wood, LayerType.Surface);             // dead wood pool
                SoilDeadWood[site]          = new Layer(LayerName.CoarseRoot, LayerType.Soil);          // dead coarse roots pool
                
                SurfaceStructural[site]     = new Layer(LayerName.Structural, LayerType.Surface);       // dead leaf pool (structural)
                SurfaceMetabolic[site]      = new Layer(LayerName.Metabolic, LayerType.Surface);        // dead leaf pool (metabolic)
                SoilStructural[site]        = new Layer(LayerName.Structural, LayerType.Soil);          // dead fine roots pool (structural)
                SoilMetabolic[site]         = new Layer(LayerName.Metabolic, LayerType.Soil);           // dead fine roots pool (metabolic)
                SoilPrimary[site]           = new SoilLayer(SoilName.Primary); //, LayerType.Soil);     // soil pool
                SoilAvailable[site]        = new SoilLayer(SoilName.Available); //, LayerType.Soil);  
                //LayerType.Soil)[site] = new SoilLayer(SoilName.Primary); //, LayerType.Soil);
                //som1surface[site]           = new Layer(LayerName.SOM1, LayerType.Surface);
                //som2[site]                  = new Layer(LayerName.SOM2, LayerType.Soil);
                //som3[site]                  = new Layer(LayerName.SOM3, LayerType.Soil);

                Stream[site]                = new Layer(LayerName.Other, LayerType.Other);
                SourceSink[site]            = new Layer(LayerName.Other, LayerType.Other);

                MonthlyAGNPPcarbon[site]           = new double[12];
                MonthlyBGNPPcarbon[site]           = new double[12];
                MonthlySoilTemp[site]       = new double[12];
                MonthlyActiveLayerDepth[site] = new double[12];
                MonthlyNEE[site]            = new double[12];
                MonthlyStreamN[site]         = new double[12];
                MonthlyResp[site] = new double[12];
                MonthlyDeadWoodResp[site] = new double[12];
                MonthlyDeadRootResp[site] = new double[12];
                MonthlyDeadLeafResp[site] = new double[12];
                MonthlyLAI[site] = new double[12];
                //monthlymineralN[site]       = new double[12];

                //CohortResorbedNallocation[site] = new Dictionary<int, Dictionary<int, double>>();
            }
            
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes for disturbances.
        /// </summary>
        public static void InitializeDisturbances()
        {
            FireSeverity        = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");
            HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
            //HarvestTime = PlugIn.ModelCore.GetSiteVar<int>("Harvest.TimeOfLastEvent");

            //if(HarvestPrescriptionName == null)
            //    throw new System.ApplicationException("TEST Error: Harvest Prescription Names NOT Initialized.");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Biomass cohorts at each site.
        /// </summary>
        private static ISiteVar<SiteCohorts> cohorts;
        public static ISiteVar<SiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
            set
            {
                cohorts = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the actual biomass at a site.  The biomass is the total
        /// of all the site's cohorts except young ones.  The total is limited
        /// to being no more than the site's maximum biomass less the previous
        /// year's mortality at the site.
        /// </summary>
        public static double ActualSiteBiomass(ActiveSite site)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
            SiteCohorts siteCohorts = Cohorts[site];

            if(siteCohorts == null)
                return 0.0;
            
            int youngBiomass;
            int totalBiomass = Landis.Library.UniversalCohorts.Cohorts.ComputeBiomass(siteCohorts, out youngBiomass);
            double B_ACT = totalBiomass - youngBiomass;

            //int lastMortality = SiteVars.PrevYearMortality[site];
            //B_ACT = System.Math.Min(ClimateRegionData.B_MAX[ecoregion] - lastMortality, B_ACT);

            return B_ACT;
        }
        
        //---------------------------------------------------------------------
        public static void ResetAnnualValues(Site site)
        {

            // Reset these accumulators to zero:
            DryDays[site] = 0;
            CohortLeafN[site] = 0.0;
            CohortFRootN[site] = 0.0;
            CohortLeafC[site] = 0.0;
            CohortFRootC[site] = 0.0;
            CohortWoodN[site] = 0.0;
            CohortCRootN[site] = 0.0;
            CohortWoodC[site] = 0.0;
            CohortCRootC[site] = 0.0;
            GrossMineralization[site] = 0.0;
            AGNPPcarbon[site] = 0.0;
            BGNPPcarbon[site] = 0.0;
            LitterfallC[site] = 0.0;
            
            Stream[site]          = new Layer(LayerName.Other, LayerType.Other);
            SourceSink[site]      = new Layer(LayerName.Other, LayerType.Other);
            
            SurfaceDeadWood[site].NetMineralization = 0.0;
            SurfaceStructural[site].NetMineralization = 0.0;
            SurfaceMetabolic[site].NetMineralization = 0.0;
            
            SoilDeadWood[site].NetMineralization = 0.0;
            //SiteVars.SoilStructural[site].NetMineralization = 0.0;
            //SiteVars.SoilMetabolic[site].NetMineralization = 0.0;
            
            //SiteVars.SOM1surface[site].NetMineralization = 0.0;
            //SiteVars.SoilPrimary[site].NetMineralization = 0.0;
            //SiteVars.SOM2[site].NetMineralization = 0.0;
            //SiteVars.SOM3[site].NetMineralization = 0.0;
            AnnualNEE[site] = 0.0;
            Nvol[site] = 0.0;
            AnnualNEE[site] = 0.0;
            TotalNuptake[site] = 0.0;
            ResorbedN[site] = 0.0;
            FrassC[site] = 0.0;
            LAI[site] = 0.0;
            AnnualPPT_AET[site] = 0.0;
            AnnualClimaticWaterDeficit[site] = 0.0;
            AnnualPET[site] = 0.0;
            WoodGrowthMortality[site] = 0.0;
            WoodAgeMortality[site] = 0.0;
            //SiteVars.DryDays[site] = 0;

            //SiteVars.FireEfflux[site] = 0.0;


        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLast { get; private set; }
        //{
        //    get {
        //        return timeOfLast;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// The intact dead woody pools for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SurfaceDeadWood { get; private set; }
        //{
        //    get {
        //        return surfaceDeadWood;
        //    }
        //}

        //---------------------------------------------------------------------

        /// <summary>
        /// The DEAD coarse root pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SoilDeadWood { get; private set; }
        //{
        //    get {
        //        return soilDeadWood;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// The dead surface pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SurfaceStructural { get; private set; }
        //{
        //    get {
        //        return surfaceStructural;
        //    }
        //}

        //---------------------------------------------------------------------

        /// <summary>
        /// The dead surface pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SurfaceMetabolic { get; private set; }
        //{
        //    get {
        //        return surfaceMetabolic;
        //    }
        //}

        //---------------------------------------------------------------------

        /// <summary>
        /// The fine root pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SoilStructural { get; private set; }
//{
//            get {
//                return soilStructural;
//            }
//        }
        //---------------------------------------------------------------------

        /// <summary>
        /// The fine root pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SoilMetabolic { get; private set; }
        //{
        //    get {
        //        return soilMetabolic;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM1-Surface) for the landscape's sites.
        /// </summary>
        //public static ISiteVar<Layer> SOM1surface
        //{
        //    get {
        //        return som1surface;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// The Primary soil.
        /// </summary>
        public static ISiteVar<SoilLayer> SoilPrimary { get; private set; }
        //{
        //    get {
        //        return soilPrimary;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil C available for decomposition.
        /// </summary>
        public static ISiteVar<SoilLayer> SoilAvailable { get; private set; }
        //{
        //    get
        //    {
        //        return soilAvailable;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM2) for the landscape's sites.
        /// </summary>
        //public static ISiteVar<Layer> SOM2
        //{
        //    get {
        //        return som2;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM3) for the landscape's sites.
        /// </summary>
        //public static ISiteVar<Layer> SOM3
        //{
        //    get {
        //        return som3;
        //    }
        //}
        public static ISiteVar<int> SoilDepth { get; private set; }
        public static ISiteVar<double> SoilDrain { get; private set; }
        public static ISiteVar<double> SoilBaseFlowFraction { get; private set; }
        public static ISiteVar<double> SoilStormFlowFraction { get; private set; }
        public static ISiteVar<double> SoilFieldCapacity { get; private set; }
        public static ISiteVar<double> SoilWiltingPoint { get; private set; }
        public static ISiteVar<double> SoilBulkDensity { get; private set; }
        public static ISiteVar<double> SoilParticleDensity { get; private set; }
        public static ISiteVar<double> SoilPercentSand { get; private set; }
        public static ISiteVar<double> SoilPercentClay { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// Leaching to a stream - using the soil layer object is a cheat
        /// </summary>
        public static ISiteVar<Layer> Stream { get; private set; }
        //{
        //    get {
        //        return stream;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// Fine Fuels biomass
        /// </summary>
        public static ISiteVar<double> FineFuels { get;  set; }
        //{
        //    get
        //    {
        //        return fineFuels;
        //    }
        //    set
        //    {
        //        fineFuels = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// Water movement
        /// </summary>
        public static ISiteVar<double> WaterMovement { get; set; }
        //{
        //    get {
        //        return waterMovement;
        //    }
        //    set {
        //        waterMovement = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// Water available to trees
        /// </summary>
        public static ISiteVar<double> AvailableWater { get; set; }
        //{
        //    get {
        //        return availableWater;
        //    }
        //    set {
        //        availableWater = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        ///Active layer depth
        /// </summary>
        public static ISiteVar<double> ActiveLayerDepth { get; set; }

        public static ISiteVar<double[]> MonthlyActiveLayerDepth { get; set; }

        /// <summary>
        /// Water content in soil
        /// </summary>
        public static ISiteVar<double> SoilWaterContent { get; set; }
        //{
        //    get {
        //        return soilWaterContent;
        //    }
        //    set {
        //        soilWaterContent = value;
        //    }
        //}


        /// <summary>
        /// Liquid Snowpack
        /// </summary>
        public static ISiteVar<double> LiquidSnowPack { get; set; }
        //{
        //    get
        //    {
        //        return liquidSnowPack;
        //    }
        //    set
        //    {
        //        liquidSnowPack = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// Available mineral Nitrogen
        /// </summary>
        public static ISiteVar<double> MineralN { get; set; }
        //{
        //    get {
        //        return mineralN;
        //    }
        //    set {
        //        mineralN = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// The amount of N resorbed before leaf fall
        /// </summary>
        public static ISiteVar<double> ResorbedN { get; set; }
        //{
        //    get
        //    {
        //        return resorbedN;
        //    }
        //    set
        //    {
        //        resorbedN = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// A generic decay factor determined by soil water and soil temperature.
        /// </summary>
        public static ISiteVar<double> DecayFactor { get; set; }
        //{
        //    get {
        //        return decayFactor;
        //    }
        //    set {
        //        decayFactor = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// Soil temperature (C)
        /// </summary>
        public static ISiteVar<double>SoilTemperature { get; set; }
        //{
        //    get {
        //        return soilTemperature;
        //    }
        //    set {
        //        soilTemperature = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// A generic decay factor determined by soil water and soil temperature.
        /// </summary>
        public static ISiteVar<double> AnaerobicEffect { get; set; }
        //{
        //    get {
        //        return anaerobicEffect;
        //    }
        //    set {
        //        anaerobicEffect = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// Soil moisture at the time of reproduction
        /// </summary>
        public static ISiteVar<int> DryDays { get; set; }
        //{
        //    get
        //    {
        //        return dryDays;
        //    }
        //    set
        //    {
        //        dryDays = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Leaf Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortLeafN { get; set; }
        //{
        //    get {
        //        return cohortLeafN;
        //    }
        //    set {
        //        cohortLeafN = value;
        //    }
        //}
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Fine Root Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortFRootN { get; set; }
//{
//            get
//            {
//                return cohortFRootN;
//            }
//            set
//            {
//                cohortFRootN = value;
//            }
//        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Carbon in the Leaves
        /// </summary>
        public static ISiteVar<double> CohortLeafC { get; set; }
//{
//            get {
//                return cohortLeafC;
//            }
//            set {
//                cohortLeafC = value;
//            }
//        }

        /// <summary>
        /// A summary of all Carbon in the Fine Roots
        /// </summary>
        public static ISiteVar<double> CohortFRootC { get; set; }
//{
//            get
//            {
//                return cohortFRootC;
//            }
//            set
//            {
//                cohortFRootC = value;
//            }
//        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Aboveground Wood Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortWoodN { get; set; }
//{
//            get {
//                return cohortWoodN;
//            }
//            set {
//                cohortWoodN = value;
//            }
//        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Coarse Root Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortCRootN { get; set; }
//{
//            get
//            {
//                return cohortCRootN;
//            }
//            set
//            {
//                cohortCRootN = value;
//            }
//        }



        /// <summary>
        /// A summary of all Aboveground Wood Carbon in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortWoodC { get; set; }
//{
//            get {
//                return cohortWoodC;
//            }
//            set {
//                cohortWoodC = value;
//            }
//        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of all Carbon in the Coarse Roots
        /// </summary>
        public static ISiteVar<double> CohortCRootC { get; set; }
//{
//            get
//            {
//                return cohortCRootC;
//            }
//            set
//            {
//                cohortCRootC = value;
//            }
//        }

        //-------------------------

        /// <summary>
        /// A summary of Gross Mineraliztion.
        /// </summary>
        public static ISiteVar<double> GrossMineralization { get; set; }
//{
//            get
//            {
//                return grossMineralization;
//            }
//            set
//            {
//                grossMineralization = value;
//            }
//        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Aboveground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double> AGNPPcarbon { get; private set; }
//{
//            get {
//                return ag_nppC;
//            }
//        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Belowground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double> BGNPPcarbon { get; private set; }
//{
//            get {
//                return bg_nppC;
//            }
//        }

        
        
        
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Litter fall (g C/m2).
        /// </summary>
        public static ISiteVar<double> LitterfallC { get; private set; }
//{
//            get {
//                return litterfallC;
//            }
//        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Aboveground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double[]> MonthlyAGNPPcarbon { get; set; }
//{
//            get {
//                return monthlyAGNPPC;
//            }
//            set {
//                monthlyAGNPPC = value;
//            }
//        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Belowground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double[]> MonthlyBGNPPcarbon { get; set; }
//{
//            get {
//                return monthlyBGNPPC;
//            }
//            set {
//                monthlyBGNPPC = value;
//            }
//        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Monthly Soil temperature (C)
        /// </summary>
        public static ISiteVar<double[]> MonthlySoilTemp { get; set; }
//{
//            get
//            {
//                return monthlysoilTemp;
//            }
//            set
//            {
//                monthlysoilTemp = value;
//            }
//        }
        //---------------------------------------------------------------------//---------------------------------------------------------------------
        /// <summary>
        /// A summary of heterotrophic respiration, i.e. CO2 loss from decomposition (g C/m2)
        /// </summary>
        public static ISiteVar<double[]> MonthlyResp { get; set; }
        //{
        //    get {
        //        return monthlyResp;
        //    }
        //    set {
        //        monthlyResp = value;
        //    }
        //}

        public static ISiteVar<double[]> MonthlyDeadWoodResp { get; set; }
        public static ISiteVar<double[]> MonthlyDeadRootResp { get; set; }
        public static ISiteVar<double[]> MonthlyDeadLeafResp { get; set; }
        public static ISiteVar<double[]> MonthlymineralN { get; set; }


        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Net Ecosystem Exchange (g C/m2), from a flux tower's perspective,
        /// whereby positive values indicate terrestrial C loss, negative values indicate C gain.
        /// Replace SourceSink?
        /// </summary>
        public static ISiteVar<double[]> MonthlyNEE { get; set; }
        //{
        //    get {
        //        return monthlyNEE;
        //    }
        //    set {
        //        monthlyNEE = value;
        //    }
        //}
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of N leaching
        /// </summary>
        public static ISiteVar<double[]> MonthlyStreamN { get; set; }
//{
//            get
//            {
//                return monthlyStreamN;
//            }
//            set
//            {
//                monthlyStreamN = value;
//            }
//        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of Monthly LAI
        /// </summary>
        //public static ISiteVar<double[]> MonthlyLAI { get; set; }
//{
//            get
//            {
//                return monthlyLAI;
//            }
//            set
//            {
//                monthlyLAI = value;
//            }
//        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Water loss
        /// </summary>
        public static ISiteVar<Layer> SourceSink { get; set; }
//{
//            get {
//                return sourceSink;
//            }
//            set {
//                sourceSink = value;
//            }
//        }
        //---------------------------------------------------------------------
               /// <summary>
        /// A summary of N uptake (g N/m2)
        /// </summary>
        public static ISiteVar<double> TotalNuptake { get; set; }
//{
//            get
//            {
//                return totalNuptake;
//            }
//            set 
//            {
//                totalNuptake = value;
//            }
                
            
//        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of frass deposition (g C/m2)
        /// </summary>
        public static ISiteVar<double> FrassC { get; set; }
//{
//            get
//            {
//                return frassC;
//            }
//            set
//            {
//                frassC = value;
//            }


//        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of LAI (m2/m2)
        /// </summary>
        public static ISiteVar<double> LAI { get; set; }
//{
//            get
//            {
//                return lai;
//            }
//            set
//            {
//                lai = value;
//            }


//        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Annual Water Budget (ppt - AET)
        /// </summary>
        public static ISiteVar<double> AnnualPPT_AET { get; set; }
//{
//            get
//            {
//                return annualPPT_AET;
//            }
//            set
//            {
//                annualPPT_AET = value;
//            }


//        }
        /// <summary>
        /// A summary of Annual CWD (PET - AET)
        /// </summary>
        //public static ISiteVar<double> AnnualClimaticWaterDeficit
        //{
        //    get
        //    {
        //        return annualClimaticWaterDeficit;
        //    }
        //    set
        //    {
        //        annualClimaticWaterDeficit = value;
        //    }


        //}
    }

}
 
