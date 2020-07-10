//  Authors: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.LeafBiomassCohorts;
using System.Collections.Generic;
using Landis.Library.Climate;
using System.Linq;
using System.Diagnostics;

namespace Landis.Extension.Succession.DGS
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public class Main
    {
        public static int Year;
        public static int Month;
        public static int MonthCnt;

        //public static int[] SiteMonth;
        //public static int[] SiteMonthCnt;

        //public static int siteCounter;
        //public static Stopwatch timer1;
        //public static Stopwatch soilLayerTimer;

        //private static object _locker = new object();

        /// <summary>
        /// Grows all cohorts at a site for a specified number of years.
        /// Litter is decomposed following the Century model.
        /// </summary>
        public static ISiteCohorts Run(ActiveSite site,
                                       int         years,
                                       bool        isSuccessionTimeStep)
        {
            //++siteCounter;

            //if (siteCounter % 100000 == 0)
            //{
            //    PlugIn.ModelCore.UI.WriteLine($"Main.Run SiteCounter: {siteCounter}");
            //    PlugIn.ModelCore.UI.WriteLine($"Main.Run Total ElapsedTime: {timer1.Elapsed}");
            //    PlugIn.ModelCore.UI.WriteLine($"Main.Run SoilLayer ElapsedTime: {soilLayerTimer.Elapsed}");

            //    timer1.Reset();
            //    soilLayerTimer.Reset();
            //}

            //timer1.Start();

            ISiteCohorts siteCohorts = SiteVars.Cohorts[site];
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            for (int y = 0; y < years; ++y) {

                Year = y + 1;

                if (Climate.Future_MonthlyData.ContainsKey(PlugIn.FutureClimateBaseYear + y + PlugIn.ModelCore.CurrentTime - years))
                    ClimateRegionData.AnnualWeather[ecoregion] = Climate.Future_MonthlyData[PlugIn.FutureClimateBaseYear + y - years + PlugIn.ModelCore.CurrentTime][ecoregion.Index];

                //PlugIn.ModelCore.UI.WriteLine("PlugIn_FutureClimateBaseYear={0}, y={1}, ModelCore_CurrentTime={2}, CenturyTimeStep = {3}, SimulatedYear = {4}.", PlugIn.FutureClimateBaseYear, y, PlugIn.ModelCore.CurrentTime, years, (PlugIn.FutureClimateBaseYear + y - years + PlugIn.ModelCore.CurrentTime));

                SiteVars.ResetAnnualValues(site);

                // Next, Grow and Decompose each month
                int[] months = new int[12]{6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5};

                if(OtherData.CalibrateMode)
                    //months = new int[12]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}; This output will not match normal mode due to differences in initialization
                    months = new int[12] { 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 };

                PlugIn.AnnualWaterBalance = 0;

                for (MonthCnt = 0; MonthCnt < 12; MonthCnt++)
                {
                    //var mon = months[MonthCnt];

                    //SiteMonthCnt[site.DataIndex] = MonthCnt;
                    //SiteMonth[site.DataIndex] = Month;

                    // Calculate mineral N fractions based on coarse root biomass.  Only need to do once per year.
                    if (MonthCnt == 0)
                    {
                        AvailableN.CalculateMineralNfraction(site);
                    }
                    //PlugIn.ModelCore.UI.WriteLine("SiteVars.MineralN = {0:0.00}, month = {1}.", SiteVars.MineralN[site], i);

                    Month = months[MonthCnt];

                    SiteVars.MonthlyAGNPPcarbon[site][Month] = 0.0;
                    SiteVars.MonthlyBGNPPcarbon[site][Month] = 0.0;
                    SiteVars.MonthlySoilTemp[site][Month] = 0.0;
                    SiteVars.MonthlyNEE[site][Month] = 0.0;
                    SiteVars.MonthlyResp[site][Month] = 0.0;
                    SiteVars.MonthlyStreamN[site][Month] = 0.0;
                    SiteVars.MonthlyLAI[site][Month] = 0.0;
                    SiteVars.SourceSink[site].Carbon = 0.0;
                    SiteVars.TotalWoodBiomass[site] = Main.ComputeWoodBiomass((ActiveSite) site);
                    //SiteVars.LAI[site] = Century.ComputeLAI((ActiveSite)site);
                                   
                    double ppt = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPrecip[Month];

                    double monthlyNdeposition;
                    if  (PlugIn.Parameters.AtmosNintercept !=-1 && PlugIn.Parameters.AtmosNslope !=-1)
                        monthlyNdeposition = PlugIn.Parameters.AtmosNintercept + (PlugIn.Parameters.AtmosNslope * ppt);
                    else 
                    {
                        monthlyNdeposition = ClimateRegionData.AnnualWeather[ecoregion].MonthlyNDeposition[Month];
                    }

                    if (monthlyNdeposition < 0)
                        throw new System.ApplicationException("Error: Nitrogen deposition less than zero.");

                    ClimateRegionData.MonthlyNDeposition[ecoregion][Month] = monthlyNdeposition;
                    ClimateRegionData.AnnualNDeposition[ecoregion] += monthlyNdeposition;
                    //lock (_locker)
                        SiteVars.MineralN[site] += monthlyNdeposition;
                    //PlugIn.ModelCore.UI.WriteLine("Ndeposition={0},MineralN={1:0.00}.", monthlyNdeposition, SiteVars.MineralN[site]);

                    //SiteVars.DecayFactor[site] = 1;

                    double baseFlow, stormFlow, AET;
                    if (PlugIn.ShawGiplEnabled)
                    {
                        //var thu = PlugIn.TempHydroUnit;
                        var thu = SiteVars.TempHydroUnit[site];

                        //var thu = PlugIn.TempHydroUnits[PlugIn.ModelCore.Ecoregion[site]];

                        baseFlow = thu.MonthlyShawDammResults[Month].MonthDeepPercolationInCm;
                        stormFlow = thu.MonthlyShawDammResults[Month].MonthRunoffInCm;
                        AET = thu.MonthlyShawDammResults[Month].MonthEvapotranspirationInCm;

                        // calculate decay factor and anaerobic effect
                        var pet = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPET[Month];
                        var precipitation = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPrecip[Month]; //rain + irract in cm;
                        var ratioPrecipPET = pet > 0.0 ? precipitation / pet : 0.0;
                        var tave = ClimateRegionData.AnnualWeather[ecoregion].MonthlyTemp[Month];   // this is air temperature when passed to CalculateAnaerobicEffect() by SoilWater, but this might need to be soil temperature instead
                        var drain = SiteVars.SoilDrain[site];

                        SiteVars.DecayFactor[site] = SoilWater.CalculateDecayFactor((int)OtherData.WType, thu.MonthlySoilTemperatureDecomp[Month], thu.MonthlySoilMoistureDecomp[Month], ratioPrecipPET);
                        SiteVars.AnaerobicEffect[site] = SoilWater.CalculateAnaerobicEffect(drain, ratioPrecipPET, pet, tave);
                    }
                    else
                    {
                        var liveBiomass = (double)ComputeLivingBiomass(siteCohorts);
                        SoilWater.Run(y, Month, liveBiomass, site, out baseFlow, out stormFlow, out AET);
                    }

                    //lock (_locker)
                        PlugIn.AnnualWaterBalance += ppt - AET;

                    //
                    // **

                    // Calculate N allocation for each cohort
                    AvailableN.SetMineralNallocation(site);

                    if (MonthCnt == 11)
                        siteCohorts.Grow(site, (y == years && isSuccessionTimeStep), true);
                    else
                        siteCohorts.Grow(site, (y == years && isSuccessionTimeStep), false);

                    WoodLayer.Decompose(site);
                    LitterLayer.Decompose(site);
                    //soilLayerTimer.Start();
                    SoilLayer.Decompose(y, Month, site);
                    //soilLayerTimer.Stop();

                    // Volatilization loss as a function of the mineral N which remains after uptake by plants.  
                    // ML added a correction factor for wetlands since their denitrification rate is double that of wetlands
                    // based on a review paper by Seitziner 2006.

                    double volatilize = (SiteVars.MineralN[site] * PlugIn.Parameters.DenitrificationRate); 

                    //PlugIn.ModelCore.UI.WriteLine("BeforeVol.  MineralN={0:0.00}.", SiteVars.MineralN[site]);

                    SiteVars.MineralN[site] -= volatilize;
                    SiteVars.SourceSink[site].Nitrogen += volatilize;
                    SiteVars.Nvol[site] += volatilize;

                    SoilWater.Leach(site, baseFlow, stormFlow);

                    SiteVars.MonthlyNEE[site][Month] -= SiteVars.MonthlyAGNPPcarbon[site][Month];
                    SiteVars.MonthlyNEE[site][Month] -= SiteVars.MonthlyBGNPPcarbon[site][Month];
                    SiteVars.MonthlySoilTemp[site][Month] -= SiteVars.MonthlyAGNPPcarbon[site][Month];
                    SiteVars.MonthlyNEE[site][Month] += SiteVars.SourceSink[site].Carbon;
                    SiteVars.FineFuels[site] = (SiteVars.SurfaceStructural[site].Carbon + SiteVars.SurfaceMetabolic[site].Carbon) * 2.0;
                    //SiteVars.FineFuels[site] = (System.Math.Min(1.0, (double) (PlugIn.ModelCore.CurrentTime - SiteVars.HarvestTime[site]) * 0.1));
                }
            }

            ComputeTotalCohortCN(site, siteCohorts);

            //timer1.Stop();

            return siteCohorts;
        }

        //---------------------------------------------------------------------

        public static int ComputeLivingBiomass(ISiteCohorts cohorts)
        {
            int total = 0;
            if (cohorts != null)
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                    foreach (ICohort cohort in speciesCohorts)
                        total += (int) (cohort.WoodBiomass + cohort.LeafBiomass);
                    //total += ComputeBiomass(speciesCohorts);
            return total;
        }

        //---------------------------------------------------------------------

        public static int ComputeNeedleBiomass(ISiteCohorts cohorts)
        {
            int total = 0;
            if (cohorts != null)
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                    foreach (ICohort cohort in speciesCohorts)
                        total += (int)(cohort.LeafBiomass);
            //total += ComputeBiomass(speciesCohorts);
            return total;
        }
        //---------------------------------------------------------------------

        public static double ComputeWoodBiomass(ActiveSite site)
        {
            double woodBiomass = 0;
            if (SiteVars.Cohorts[site] != null)
                foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                    foreach (ICohort cohort in speciesCohorts)
                        woodBiomass += cohort.WoodBiomass;
            return woodBiomass;
        }

        //---------------------------------------------------------------------
        public static void ComputeTotalCohortCN(ActiveSite site, ISiteCohorts cohorts)
        {
            SiteVars.CohortLeafC[site] = 0;
            SiteVars.CohortFRootC[site] = 0;
            SiteVars.CohortLeafN[site] = 0;
            SiteVars.CohortFRootN[site] = 0;
            SiteVars.CohortWoodC[site] = 0;
            SiteVars.CohortCRootC[site] = 0;
            SiteVars.CohortWoodN[site] = 0;
            SiteVars.CohortCRootN[site] = 0;

            if (cohorts != null)
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                    foreach (ICohort cohort in speciesCohorts)
                        CalculateCohortCN(site, cohort);
            return;
        }

        /// <summary>
        /// Summarize cohort C&N for output.
        /// </summary>
        private static void CalculateCohortCN(ActiveSite site, ICohort cohort)
        {
            ISpecies species = cohort.Species;

            double leafC = cohort.LeafBiomass * 0.47;
            double woodC = cohort.WoodBiomass * 0.47;

            double fRootC = Roots.CalculateFineRoot(cohort, leafC);
            double cRootC = Roots.CalculateCoarseRoot(cohort, woodC);

            double totalC = leafC + woodC + fRootC + cRootC;

            double leafN  = leafC /  (double) SpeciesData.LeafCN[species];
            double woodN = woodC / (double) SpeciesData.WoodCN[species];
            double cRootN = cRootC / (double) SpeciesData.CoarseRootCN[species];
            double fRootN = fRootC / (double) SpeciesData.FineRootCN[species];

            //double totalN = woodN + cRootN + leafN + fRootN;

            //PlugIn.ModelCore.UI.WriteLine("month={0}, species={1}, leafB={2:0.0}, leafC={3:0.00}, leafN={4:0.0}, woodB={5:0.0}, woodC={6:0.000}, woodN={7:0.0}", Month, cohort.Species.Name, cohort.LeafBiomass, leafC, leafN, cohort.WoodBiomass, woodC, woodN);

            SiteVars.CohortLeafC[site] += leafC;
            SiteVars.CohortFRootC[site] += fRootC;
            SiteVars.CohortLeafN[site] += leafN;
            SiteVars.CohortFRootN[site] += fRootN;
            SiteVars.CohortWoodC[site] += woodC;
            SiteVars.CohortCRootC[site] += cRootC;
            SiteVars.CohortWoodN[site] += woodN ;
            SiteVars.CohortCRootN[site] += cRootN;

            return;

        }


    }
}
