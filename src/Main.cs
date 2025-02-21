//  Authors: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;
using System.Collections.Generic;
using System;
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

        /// <summary>
        /// Grows all cohorts at a site for a specified number of years.
        /// Litter is decomposed following the Century model.
        /// </summary>
        public static SiteCohorts Run(ActiveSite site,
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

            SiteCohorts siteCohorts = SiteVars.Cohorts[site];
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            for (int y = 0; y < years; ++y) {

                Year = y + 1;

                ClimateRegionData.AnnualClimate[ecoregion] = Climate.FutureEcoregionYearClimate[ecoregion.Index][Year - years + PlugIn.ModelCore.CurrentTime];

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
                        AvailableN.CalculateAnnualMineralNfraction(site);
                    }
                    //PlugIn.ModelCore.UI.WriteLine("SiteVars.MineralN = {0:0.00}, month = {1}.", SiteVars.MineralN[site], i);

                    Month = months[MonthCnt];

                    SiteVars.MonthlyAGNPPcarbon[site][Month] = 0.0;
                    SiteVars.MonthlyBGNPPcarbon[site][Month] = 0.0;
                    SiteVars.MonthlySoilTemp[site][Month] = 0.0;
                    SiteVars.MonthlyNEE[site][Month] = 0.0;
                    SiteVars.MonthlyResp[site][Month] = 0.0;
                    SiteVars.MonthlyDeadWoodResp[site][Month] = 0.0;
                    SiteVars.MonthlyDeadRootResp[site][Month] = 0.0;
                    SiteVars.MonthlyDeadLeafResp[site][Month] = 0.0;
                    SiteVars.MonthlyStreamN[site][Month] = 0.0;
                    SiteVars.MonthlyLAI[site][Month] = 0.0;
                    SiteVars.SourceSink[site].Carbon = 0.0;
                    SiteVars.TotalWoodBiomass[site] = ComputeWoodBiomass((ActiveSite) site);
                    //SiteVars.LAI[site] = Century.ComputeLAI((ActiveSite)site);
                                   
                    var ppt = ClimateRegionData.AnnualClimate[ecoregion].MonthlyPrecip[Month];

                    double monthlyNdeposition;
                    if  (PlugIn.Parameters.AtmosNintercept !=-1 && PlugIn.Parameters.AtmosNslope !=-1)
                        monthlyNdeposition = PlugIn.Parameters.AtmosNintercept + (PlugIn.Parameters.AtmosNslope * ppt);
                    else 
                    {
                        monthlyNdeposition = ClimateRegionData.AnnualClimate[ecoregion].MonthlyNDeposition[Month];
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
                        var pet = ClimateRegionData.AnnualClimate[ecoregion].MonthlyPET[Month];
                        var precipitation = ClimateRegionData.AnnualClimate[ecoregion].MonthlyPrecip[Month]; //rain + irract in cm;
                        var ratioPrecipPET = pet > 0.0 ? precipitation / pet : 0.0;
                        var tave = ClimateRegionData.AnnualClimate[ecoregion].MonthlyTemp[Month];   // this is air temperature when passed to CalculateAnaerobicEffect() by SoilWater, but this might need to be soil temperature instead
                        var drain = SiteVars.SoilDrain[site];
                        var beginGrowing = ClimateRegionData.AnnualClimate[ecoregion].BeginGrowingDay;
                        var endGrowing = ClimateRegionData.AnnualClimate[ecoregion].EndGrowingDay;
                        var wiltingPoint = SiteVars.SoilWiltingPoint[site];                        
                        var previousMonth = MonthCnt == 0 ? months.Last() : months[MonthCnt - 1];

                        SiteVars.DecayFactor[site] = SoilWater.CalculateDecayFactor((int)OtherData.WType, thu.MonthlySoilTemperatureDecomp[Month], thu.MonthlySoilMoistureDecomp[Month], ratioPrecipPET);
                        SiteVars.AnaerobicEffect[site] = SoilWater.CalculateAnaerobicEffect(drain, ratioPrecipPET, pet, tave);
                        SiteVars.DryDays[site] = SoilWater.CalculateDryDays(Year, Month, beginGrowing, endGrowing, wiltingPoint, thu.MonthlySoilMoistureDecomp[Month], thu.MonthlySoilMoistureDecomp[previousMonth]);

                        SiteVars.AnnualClimaticWaterDeficit[site] = SiteVars.AnnualClimaticWaterDeficit[site] + (pet - AET)*10.0; // converting it to mm 
                        SiteVars.AnnualPET[site] = SiteVars.AnnualPET[site] + (pet*10.0); // converting it to mm
                    }
                    else
                    {
                        var liveBiomass = (double)ComputeLivingBiomass(siteCohorts);
                        //SoilWater.Run(y, Month, liveBiomass, site, out baseFlow, out stormFlow, out AET);
                        SoilWater.Run(y, Month, liveBiomass, site, out baseFlow, out stormFlow);
                    }

                    //lock (_locker)
                        //PlugIn.AnnualWaterBalance += ppt - AET;

                    //
                    // **

                    // Calculate N allocation for each cohort
                    AvailableN.CalculateMonthlyMineralNallocation(site);

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

        public static int ComputeLivingBiomass(SiteCohorts cohorts)
        {
            int total = 0;
            if (cohorts != null)
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                {
                    foreach (ICohort cohort in speciesCohorts)
                    {
                        //total += (int)(cohort.WoodBiomass + cohort.LeafBiomass);
                        //total += (int)(cohort.Data.Biomass);
                        total += (int)(cohort.Data.AdditionalParameters.WoodBiomass + cohort.Data.AdditionalParameters.LeafBiomass);
                        //total += ComputeBiomass(speciesCohorts);
                    }
                }
            return total;
        }

        //---------------------------------------------------------------------

        public static int ComputeNeedleBiomass(SiteCohorts cohorts)
        {
            int total = 0;
            if (cohorts != null)
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                {
                    foreach (ICohort cohort in speciesCohorts)
                    {
                        //total += (int)(Convert.ToInt32(cohort.Data.AdditionalParameters.LeafBiomass));
                        total += (int)(cohort.Data.AdditionalParameters.LeafBiomass);
                    }
                }
            //total += ComputeBiomass(speciesCohorts);
            return total;
        }
        //---------------------------------------------------------------------

        public static double ComputeWoodBiomass(ActiveSite site)
        {
            double woodBiomass = 0;
            if (SiteVars.Cohorts[site] != null)
                foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                {
                    foreach (ICohort cohort in speciesCohorts)
                    {
                         //double woodBiomass = Convert.ToDouble(cohort.Data.AdditionalParameters.WoodBiomass);
                         woodBiomass = cohort.Data.AdditionalParameters.WoodBiomass;
                        //woodBiomass += woodB;
                    }
                }
            return woodBiomass;
        }

        //---------------------------------------------------------------------
        public static void ComputeTotalCohortCN(ActiveSite site, SiteCohorts cohorts)
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

            double leafC = cohort.Data.AdditionalParameters.LeafBiomass * 0.47;
            double woodC = cohort.Data.AdditionalParameters.WoodBiomass * 0.47;

            double fRootC = Roots.CalculateFineRoot(cohort, leafC);
            double cRootC = Roots.CalculateCoarseRoot(cohort, woodC);

            double totalC = leafC + woodC + fRootC + cRootC;

            double leafN  = leafC /  (double) PlugIn.Parameters.LeafCN[species];
            double woodN = woodC / (double) PlugIn.Parameters.WoodCN[species];
            double cRootN = cRootC / (double) PlugIn.Parameters.CoarseRootCN[species];
            double fRootN = fRootC / (double) PlugIn.Parameters.FineRootCN[species];

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
