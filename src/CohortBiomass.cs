 //  Authors: Robert Scheller, Melissa Lucash

using Landis.Utilities;
using Landis.Core;
using Landis.SpatialModeling;
using System.Collections.Generic;
using Landis.Library;
using Landis.Library.UniversalCohorts;
using System;
using System.Dynamic;
using System.Linq;


namespace Landis.Extension.Succession.DGS
{
    /// <summary>
    /// Calculations for an individual cohort's biomass.
    /// </summary>
    public class CohortBiomass
        : Landis.Library.UniversalCohorts.ICalculator
    {

        /// <summary>
        /// The single instance of the biomass calculations that is used by
        /// the plug-in.
        /// </summary>
        public static CohortBiomass Calculator;

        //  Ecoregion where the cohort's site is located
        private IEcoregion ecoregion;
        private double defoliation;
        private double defoliatedLeafBiomass;

        //---------------------------------------------------------------------

        public CohortBiomass()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the change in a cohort's biomass due to Annual Net Primary
        /// Productivity (ANPP), age-related mortality (M_AGE), and development-
        /// related mortality (M_BIO).
        /// </summary>
        public double ComputeChange(ICohort cohort, ActiveSite site, out double ANPP, out ExpandoObject otherParams)
        {
            dynamic tempObject = new ExpandoObject();
            tempObject.WoodBiomass = 0;
            tempObject.LeafBiomass = 0;

            ecoregion = PlugIn.ModelCore.Ecoregion[site];

            // First call to the Calibrate Log:
            //if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
                //Outputs.CalibrateLog.Write("{0},{1},{2},{3},{4},{5:0.0},{6:0.0},", PlugIn.ModelCore.CurrentTime, Main.Month + 1, ecoregion.Index, cohort.Species.Name, cohort.Data.Age, cohort.Data.AdditionalParameters.WoodBiomass, cohort.Data.AdditionalParameters.LeafBiomass);

            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.year = PlugIn.ModelCore.CurrentTime;
                CalibrateLog.month = Main.Month + 1;
                CalibrateLog.climateRegionIndex = ecoregion.Index;
                CalibrateLog.speciesName = cohort.Species.Name;
                CalibrateLog.cohortAge = cohort.Data.Age;
                CalibrateLog.cohortWoodB = cohort.Data.AdditionalParameters.WoodBiomass;
                CalibrateLog.cohortLeafB = cohort.Data.AdditionalParameters.LeafBiomass;
            }


            double siteBiomass = Main.ComputeLivingBiomass(SiteVars.Cohorts[site]);

            if(siteBiomass < 0)
                throw new ApplicationException("Error: Site biomass < 0");

            // ****** Mortality *******
            // Age-related mortality includes woody and standing leaf biomass.
            double[] mortalityAge = ComputeAgeMortality(cohort, site);

            // ****** Growth *******
            double[] actualANPP = ComputeActualANPP(cohort, site, siteBiomass, mortalityAge);
            ANPP = actualANPP[0] + actualANPP[1];

            //  Growth-related mortality
            double[] mortalityGrowth = ComputeGrowthMortality(cohort, site, siteBiomass, actualANPP);

            double[] totalMortality = new double[2]{Math.Min(cohort.Data.AdditionalParameters.WoodBiomass, mortalityAge[0] + mortalityGrowth[0]), Math.Min(cohort.Data.AdditionalParameters.LeafBiomass, mortalityAge[1] + mortalityGrowth[1])};
            double nonDisturbanceLeafFall = totalMortality[1];

            
            double scorch = 0.0;
            defoliatedLeafBiomass = 0.0;

            if (Main.Month == 6)  //July = 6
            {
                if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                    scorch = FireEffects.CrownScorching(cohort, SiteVars.FireSeverity[site]);

                if (scorch > 0.0)  // NEED TO DOUBLE CHECK WHAT CROWN SCORCHING RETURNS
                    totalMortality[1] = Math.Min(cohort.Data.AdditionalParameters.LeafBiomass, scorch + totalMortality[1]);

                // Defoliation (index) ranges from 1.0 (total) to none (0.0).
                if (PlugIn.ModelCore.CurrentTime > 0) //Skip this during initialization
                {
                    int cohortBiomass = (int)(cohort.Data.AdditionalParameters.LeafBiomass + cohort.Data.AdditionalParameters.WoodBiomass);
                    defoliation = Landis.Library.UniversalCohorts.CohortDefoliation.Compute(site, cohort, cohortBiomass, (int)siteBiomass);
                    
                }

                if (defoliation > 1.0)
                    defoliation = 1.0;

                if (defoliation > 0.0)
                {
                    defoliatedLeafBiomass = (cohort.Data.AdditionalParameters.LeafBiomass) * defoliation;
                   if (totalMortality[1] + defoliatedLeafBiomass - cohort.Data.AdditionalParameters.LeafBiomass > 0.001)
                        defoliatedLeafBiomass = cohort.Data.AdditionalParameters.LeafBiomass - totalMortality[1];
                    //PlugIn.ModelCore.UI.WriteLine("Defoliation.Month={0:0.0}, LeafBiomass={1:0.00}, DefoliatedLeafBiomass={2:0.00}, TotalLeafMort={2:0.00}", Main.Month, cohort.LeafBiomass, defoliatedLeafBiomass , mortalityAge[1]);

                    ForestFloor.AddFrassLitter(defoliatedLeafBiomass, cohort.Species, site);

                }
            }
            else
            {
                defoliation = 0.0;
                defoliatedLeafBiomass = 0.0;
            }

            // RMS 03/2016: Additional mortality as reaching capacity limit:  SAVE FOR NEXT RELEASE
            //double maxBiomass = SpeciesData.B_MAX_Spp[cohort.Species][ecoregion];
            //double limitCapacity = Math.Min(1.0, Math.Exp(siteBiomass / maxBiomass * 5.0) / Math.Exp(5.0));  // 1.0 = total limit; 0.0 = No limit
            //totalMortality[0] += (actualANPP[0] * limitCapacity); // totalMortality not to exceed ANPP allocation


            if (totalMortality[0] <= 0.0 || cohort.Data.AdditionalParameters.WoodBiomass <= 0.0)
                totalMortality[0] = 0.0;

            if (totalMortality[1] <= 0.0 || cohort.Data.AdditionalParameters.LeafBiomass <= 0.0)
                totalMortality[1] = 0.0;


            if ((totalMortality[0]) > cohort.Data.AdditionalParameters.WoodBiomass)
            {
                PlugIn.ModelCore.UI.WriteLine("Warning: WOOD Mortality exceeds cohort wood biomass. M={0:0.0}, B={1:0.0}", (totalMortality[0]), cohort.Data.AdditionalParameters.WoodBiomass);
                PlugIn.ModelCore.UI.WriteLine("Warning: If M>B, then list mortality. Mage={0:0.0}, Mgrow={1:0.0},", mortalityAge[0], mortalityGrowth[0]);
                throw new ApplicationException("Error: WOOD Mortality exceeds cohort biomass");

            }
            if ((totalMortality[1] + defoliatedLeafBiomass - cohort.Data.AdditionalParameters.LeafBiomass) > 0.01)
            {
                PlugIn.ModelCore.UI.WriteLine("Warning: LEAF Mortality exceeds cohort biomass. Mortality={0:0.000}, Leafbiomass={1:0.000}", (totalMortality[1] + defoliatedLeafBiomass), cohort.Data.AdditionalParameters.LeafBiomass);
                PlugIn.ModelCore.UI.WriteLine("Warning: If M>B, then list mortality. Mage={0:0.00}, Mgrow={1:0.00}, Mdefo={2:0.000},", mortalityAge[1], mortalityGrowth[1], defoliatedLeafBiomass);
                throw new ApplicationException("Error: LEAF Mortality exceeds cohort biomass");

            }
            double deltaWood = (double)(actualANPP[0] - totalMortality[0]);
            double deltaLeaf = (double)(actualANPP[1] - totalMortality[1] - defoliatedLeafBiomass);

            //ANPP = (int)actualANPP[0] + (int)actualANPP[1];

            tempObject.WoodBiomass = deltaWood;
            tempObject.LeafBiomass = deltaLeaf;

            otherParams = tempObject;

            double deltaAGB = deltaWood + deltaLeaf;

            //if((totalMortality[1] + defoliatedLeafBiomass) > cohort.LeafBiomass)
            //PlugIn.ModelCore.UI.WriteLine("Mortality Calcs. WoodMortality={0:0.000}, leafMortality={1:0.000}, DefoliatedLeafBiomass={2:0.000}", totalMortality[0], totalMortality[1], defoliatedLeafBiomass);

            UpdateDeadBiomass(cohort, site, totalMortality);

            CalculateNPPcarbon(site, cohort, actualANPP);

            AvailableN.AdjustAvailableN(cohort, site, actualANPP);

            if (OtherData.CalibrateMode && PlugIn.ModelCore.CurrentTime > 0)
            {
                CalibrateLog.deltaLeaf = deltaLeaf;
                CalibrateLog.deltaWood = deltaWood;
                CalibrateLog.WriteLogFile();
            }

            return  deltaAGB;
        }


        //---------------------------------------------------------------------

        private double[] ComputeActualANPP(ICohort    cohort,
                                         ActiveSite site,
                                         double    siteBiomass,
                                         double[]   mortalityAge)
        {
            dynamic additionalParameters = cohort.Data.AdditionalParameters; 
            double leafFractionNPP  = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].FractionANPPtoLeaf;
            double maxBiomass       = SpeciesData.Max_Biomass[cohort.Species];//.B_MAX_Spp[cohort.Species][ecoregion];
            double sitelai          = SiteVars.LAI[site];
            double maxNPP           = SpeciesData.Max_ANPP[cohort.Species];//.ANPP_MAX_Spp[cohort.Species][ecoregion];

            double limitT, soilTemperature, limitH20, availableWater;

            if (PlugIn.ShawGiplEnabled)
            {
                //var thu = PlugIn.TempHydroUnit;
                var thu = SiteVars.TempHydroUnit[site];

                var rec = thu.MonthlySpeciesRecords[Main.Month][cohort.Species];
                soilTemperature = rec.SoilTemperature;
                limitT = rec.TemperatureLimit;
                                
                availableWater = rec.AvailableWater;
                limitH20 = rec.WaterLimit;
                
            }
            else
            {
                soilTemperature = SiteVars.SoilTemperature[site];
                limitT = TemperatureLimitEquation(soilTemperature, cohort.Species);

                availableWater = SiteVars.AvailableWater[site];
                limitH20 = WaterLimitEquation(availableWater, cohort.Species);
            }

            //double limitT   = calculateTemp_Limit(site, cohort.Species, out var soilTemperature);

            //double limitH20 = calculateWater_Limit(site, ecoregion, cohort.Species, out var availableWater);

            
            double limitLAI = calculateLAI_Limit(cohort, site);

            // RMS 03/2016: Testing alternative more similar to how Biomass Succession operates
            //double limitCapacity = 1.0 - Math.Min(1.0, Math.Exp(siteBiomass / maxBiomass * 5.0) / Math.Exp(5.0));

            double competition_limit = calculateCompetition_Limit(cohort, site);

            double potentialNPP = maxNPP * limitLAI * limitH20 * limitT * competition_limit;
            
            double limitN = calculateN_Limit(site, cohort, potentialNPP, leafFractionNPP);
            

            potentialNPP *= limitN;

            //  Age mortality is discounted from ANPP to prevent the over-
            //  estimation of growth.  ANPP cannot be negative.
            double actualANPP = Math.Max(0.0, potentialNPP - mortalityAge[0] - mortalityAge[1]);

            // Growth can be reduced by another extension via this method.
            // To date, no extension has been written to utilize this hook.
            double growthReduction = CohortGrowthReduction.Compute(cohort, site);

            if (growthReduction > 0.0)
            {
                actualANPP *= (1.0 - growthReduction);
            }

            double leafNPP  = actualANPP * leafFractionNPP;
            double woodNPP  = actualANPP * (1.0 - leafFractionNPP);
                        
            if (Double.IsNaN(leafNPP) || Double.IsNaN(woodNPP))
            {
                throw new ApplicationException($"Wood or leaf NPP is NaN Year={PlugIn.ModelCore.CurrentTime} Month={Main.Month} woodNPP={woodNPP} leafNPP= {leafNPP} for site {site}");
                PlugIn.ModelCore.UI.WriteLine("  EITHER WOOD or LEAF NPP = NaN!  Will set to zero.");
                //PlugIn.ModelCore.UI.WriteLine("  Yr={0},Mo={1}, SpeciesName={2}, CohortAge={3}.   GROWTH LIMITS: LAI={4:0.00}, H20={5:0.00}, N={6:0.00}, T={7:0.00}, Capacity={8:0.0}.", PlugIn.ModelCore.CurrentTime, Main.Month + 1, cohort.Species.Name, cohort.Age, limitLAI, limitH20, limitN, limitT, limitCapacity);
                PlugIn.ModelCore.UI.WriteLine("  Yr={0},Mo={1}.     Other Information: MaxB={2}, Bsite={3}, Bcohort={4:0.0}.", PlugIn.ModelCore.CurrentTime, Main.Month, maxBiomass, (int)siteBiomass, (cohort.Data.AdditionalParameters.WoodBiomass + cohort.Data.AdditionalParameters.LeafBiomass));
                PlugIn.ModelCore.UI.WriteLine("  Yr={0},Mo={1}.     WoodNPP={2:0.00}, LeafNPP={3:0.00}.", PlugIn.ModelCore.CurrentTime, Main.Month + 1, woodNPP, leafNPP);
                if (Double.IsNaN(leafNPP))
                    leafNPP = 0.0;
                if (Double.IsNaN(woodNPP))
                    woodNPP = 0.0;

            }

            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.limitLAI = limitLAI;
                CalibrateLog.limitH20 = limitH20;
                CalibrateLog.limitT = limitT;
                CalibrateLog.limitN = limitN;
                CalibrateLog.limitLAIcompetition = competition_limit; // Chihiro, 2021.03.26: added                
                CalibrateLog.soilTemp = soilTemperature;
                CalibrateLog.availableWater = availableWater;
                CalibrateLog.maxNPP = maxNPP;
                CalibrateLog.maxB = SpeciesData.Max_Biomass[cohort.Species];
                CalibrateLog.siteB = siteBiomass;
                CalibrateLog.cohortB = (additionalParameters.WoodBiomass + additionalParameters.LeafBiomass);
                CalibrateLog.actualWoodNPP = woodNPP;
                CalibrateLog.actualLeafNPP = leafNPP;
            }
                        
            return new double[2]{woodNPP, leafNPP};

        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes M_AGE_ij: the mortality caused by the aging of the cohort.
        /// See equation 6 in Scheller and Mladenoff, 2004.
        /// </summary>
        private double[] ComputeAgeMortality(ICohort cohort, ActiveSite site)
        {
            dynamic additionalParameters = cohort.Data.AdditionalParameters; 
            double monthAdjust = 1.0 / 12.0;
            double totalBiomass = (double) (additionalParameters.WoodBiomass + additionalParameters.LeafBiomass);
            double max_age      = (double) cohort.Species.Longevity;
            double d            = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].LongevityMortalityShape;

            double M_AGE_wood =     additionalParameters.WoodBiomass *  monthAdjust *
                                    Math.Exp((double) cohort.Data.Age / max_age * d) / Math.Exp(d);

            double M_AGE_leaf =     additionalParameters.LeafBiomass *  monthAdjust *
                                    Math.Exp((double) cohort.Data.Age / max_age * d) / Math.Exp(d);

            M_AGE_wood = Math.Min(M_AGE_wood, cohort.Data.AdditionalParameters.WoodBiomass);
            M_AGE_leaf = Math.Min(M_AGE_leaf, cohort.Data.AdditionalParameters.LeafBiomass);

            double[] M_AGE = new double[2]{M_AGE_wood, M_AGE_leaf};

            SiteVars.WoodAgeMortality[site] += (M_AGE_wood);

            if(M_AGE_wood < 0.0 || M_AGE_leaf < 0.0)
            {
                PlugIn.ModelCore.UI.WriteLine("Mwood={0}, Mleaf={1}.", M_AGE_wood, M_AGE_leaf);
                throw new ApplicationException("Error: Woody or Leaf Age Mortality is < 0");
            }

            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.mortalityAGEleaf = M_AGE_leaf;
                CalibrateLog.mortalityAGEwood = M_AGE_wood;
            }


            return M_AGE;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Monthly mortality as a function of standing leaf and wood biomass.
        /// </summary>
        private double[] ComputeGrowthMortality(ICohort cohort, ActiveSite site, double siteBiomass, double[] AGNPP)
        {
            dynamic additionalParameters = cohort.Data.AdditionalParameters; 
            double maxBiomass = SpeciesData.Max_Biomass[cohort.Species];
            double NPPwood = (double)AGNPP[0];
            
            double M_wood_fixed = additionalParameters.WoodBiomass * FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].MonthlyWoodMortality;
            double M_leaf = 0.0;

            double relativeBiomass = siteBiomass / maxBiomass;
            double M_constant = 5.0;  //This constant controls the rate of change of mortality with NPP

            //Function which calculates an adjustment factor for mortality that ranges from 0 to 1 and exponentially increases with relative biomass.
            double M_wood_NPP = Math.Max(0.0, (Math.Exp(M_constant * relativeBiomass) - 1.0) / (Math.Exp(M_constant) - 1.0));
            M_wood_NPP = Math.Min(M_wood_NPP, 1.0);

            //This function calculates mortality as a function of NPP 
            //M_wood = NPPwood * M_wood_relative;
            double M_wood = (NPPwood * M_wood_NPP) + M_wood_fixed;

            // Leaves and Needles dropped.
            if (SpeciesData.LeafLongevity[cohort.Species] > 1.0) 
            {
                M_leaf = additionalParameters.LeafBiomass / (double) SpeciesData.LeafLongevity[cohort.Species] / 12.0;  //Needle deposit spread across the year.
               
            }
            else
            {
                if(Main.Month +1 == FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].FoliageDropMonth)
                {
                    M_leaf = additionalParameters.LeafBiomass / 2.0;  //spread across 2 months
                    
                }
                if (Main.Month +2 > FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].FoliageDropMonth)
                {
                    M_leaf = additionalParameters.LeafBiomass;  //drop the remainder
                }
            }

            double[] M_BIO = new double[2]{M_wood, M_leaf};

            if(M_wood < 0.0 || M_leaf < 0.0)
            {
                PlugIn.ModelCore.UI.WriteLine("Mwood={0}, Mleaf={1}.", M_wood, M_leaf);
                throw new ApplicationException("Error: Wood or Leaf Growth Mortality is < 0");
            }

            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.mortalityBIOwood = M_wood;
                CalibrateLog.mortalityBIOleaf = M_leaf;
            }

            SiteVars.WoodGrowthMortality[site] += (M_wood);

            return M_BIO;

        }


        //---------------------------------------------------------------------

        private void UpdateDeadBiomass(ICohort cohort, ActiveSite site, double[] totalMortality)
        {


            double mortality_wood    = (double) totalMortality[0];
            double mortality_nonwood = (double)totalMortality[1];

            //  Add mortality to dead biomass pools.
            //  Coarse root mortality is assumed proportional to aboveground woody mortality
            //    mass is assumed 25% of aboveground wood (White et al. 2000, Niklas & Enquist 2002)
            if(mortality_wood > 0.0)
            {
                ForestFloor.AddWoodLitter(mortality_wood, cohort.Species, site);
                Roots.AddCoarseRootLitter(mortality_wood, cohort, cohort.Species, site);
            }

            if(mortality_nonwood > 0.0)
            {
                AvailableN.AddResorbedN(cohort, totalMortality[1], site); //ignoring input from scorching, which is rare, but not resorbed.             
                ForestFloor.AddResorbedFoliageLitter(mortality_nonwood, cohort.Species, site);
                Roots.AddFineRootLitter(mortality_nonwood, cohort, cohort.Species, site);
            }

            return;

        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the initial biomass for a cohort at a site.
        /// </summary>
        public static float[] InitialBiomass(ISpecies species, ISiteCohorts siteCohorts,
                                            ActiveSite site)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            double leafFrac = FunctionalType.Table[SpeciesData.FuncType[species]].FractionANPPtoLeaf;

            double B_ACT = SiteVars.ActualSiteBiomass(site);
            double B_MAX = SpeciesData.Max_Biomass[species]; // B_MAX_Spp[species][ecoregion];

            //  Initial biomass exponentially declines in response to
            //  competition.
            double initialBiomass = 0.002 * B_MAX * Math.Exp(-1.6 * B_ACT / B_MAX);

            initialBiomass = Math.Max(initialBiomass, 5.0);

            double initialLeafB = initialBiomass * leafFrac;
            double initialWoodB = initialBiomass - initialLeafB;
            double[] initialB = new double[2] { initialWoodB, initialLeafB };




            float[] initialWoodLeafBiomass = new float[2] { (float)initialB[0], (float)initialB[1] };

            return initialWoodLeafBiomass;
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// Summarize NPP
        /// </summary>
        private static void CalculateNPPcarbon(ActiveSite site, ICohort cohort, double[] AGNPP)
        {
            double NPPwood = (double) AGNPP[0] * 0.47;
            double NPPleaf = (double) AGNPP[1] * 0.47;

            double NPPcoarseRoot = Roots.CalculateCoarseRoot(cohort, NPPwood);
            double NPPfineRoot = Roots.CalculateFineRoot(cohort, NPPleaf);

            if (Double.IsNaN(NPPwood) || Double.IsNaN(NPPleaf) || Double.IsNaN(NPPcoarseRoot) || Double.IsNaN(NPPfineRoot))
            {
                PlugIn.ModelCore.UI.WriteLine("  EITHER WOOD or LEAF NPP or COARSE ROOT or FINE ROOT = NaN!  Will set to zero.");
                PlugIn.ModelCore.UI.WriteLine("  Yr={0},Mo={1}.     WoodNPP={0}, LeafNPP={1}, CRootNPP={2}, FRootNPP={3}.", NPPwood, NPPleaf, NPPcoarseRoot, NPPfineRoot);
                if (Double.IsNaN(NPPleaf))
                    NPPleaf = 0.0;
                if (Double.IsNaN(NPPwood))
                    NPPwood = 0.0;
                if (Double.IsNaN(NPPcoarseRoot))
                    NPPcoarseRoot = 0.0;
                if (Double.IsNaN(NPPfineRoot))
                    NPPfineRoot = 0.0;
            }


            SiteVars.AGNPPcarbon[site] += NPPwood + NPPleaf;
            SiteVars.BGNPPcarbon[site] += NPPcoarseRoot + NPPfineRoot;
            SiteVars.MonthlyAGNPPcarbon[site][Main.Month] += NPPwood + NPPleaf;
            SiteVars.MonthlyBGNPPcarbon[site][Main.Month] += NPPcoarseRoot + NPPfineRoot;            

        }

        //--------------------------------------------------------------------------
        //N limit is actual demand divided by maximum uptake.

        //old method, not called
        //private double calculateN_Limit(ActiveSite site, ICohort cohort, double NPP, double leafFractionNPP)
        //{

        //    //Get Cohort Mineral and Resorbed N allocation.
        //    double mineralNallocation = AvailableN.GetMineralNallocation(cohort, site);  // NECN only does it for a cohort, not site.
        //    double resorbedNallocation = AvailableN.GetResorbedNallocation(cohort, site);

        //    //double LeafNPP = Math.Max(NPP * leafFractionNPP, 0.002 * cohort.WoodBiomass);  This allowed for Ndemand in winter when there was no leaf NPP
        //    double LeafNPP = (NPP * leafFractionNPP);
            
        //    double WoodNPP = NPP * (1.0 - leafFractionNPP); 
         
        //    double limitN = 0.0;
        //    if (SpeciesData.NFixer[cohort.Species])
        //        limitN = 1.0;  // No limit for N-fixing shrubs
        //    else
        //    {
        //        // Divide allocation N by N demand here:
        //        //PlugIn.ModelCore.UI.WriteLine("  WoodNPP={0:0.00}, LeafNPP={1:0.00}, FineRootNPP={2:0.00}, CoarseRootNPP={3:0.00}.", WoodNPP, LeafNPP);
        //       double Ndemand = (AvailableN.CalculateCohortNDemand(cohort.Species, site, cohort, new double[] { WoodNPP, LeafNPP})); 

        //        if (Ndemand > 0.0)
        //        {
        //            limitN = Math.Min(1.0, (mineralNallocation + resorbedNallocation) / Ndemand);                   

        //        }
        //        else
        //            limitN = 1.0; // No demand means that it is a new or very small cohort.  Will allow it to grow anyways.                
        //    }


        //    if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
        //    {
        //        CalibrateLog.mineralNalloc = mineralNallocation;
        //        CalibrateLog.resorbedNalloc = resorbedNavailable;
        //    }

        //    return Math.Max(limitN, 0.0); 
           
        //}
        //new Calculate N limit from NECN v6.0
        private double calculateN_Limit(ActiveSite site, ICohort cohort, double NPP, double leafFractionNPP)
        {

            //Get Cohort Mineral and Resorbed N allocation.
            double mineralNallocation = cohort.Data.AdditionalParameters.MineralNallocation;
            double resorbedNavailable = 0.0;
            try
            {
                resorbedNavailable = cohort.Data.AdditionalParameters.Nresorption;
            }
            catch
            {
                cohort.Data.AdditionalParameters.Nresorption = 0.0;
            }

            double LeafNPP = (NPP * leafFractionNPP);

            double WoodNPP = NPP * (1.0 - leafFractionNPP);

            double limitN = 0.0;
            if (SpeciesData.NFixer[cohort.Species])
                limitN = 1.0;  // No limit for N-fixing shrubs
            else
            {
                // Divide allocation N by N demand here:
                //PlugIn.ModelCore.UI.WriteLine("  WoodNPP={0:0.00}, LeafNPP={1:0.00}, FineRootNPP={2:0.00}, CoarseRootNPP={3:0.00}.", WoodNPP, LeafNPP);
                double Ndemand = (AvailableN.CalculateCohortNDemand(site, cohort, new double[] { WoodNPP, LeafNPP }));

                if (Ndemand > 0.0)
                {
                    limitN = Math.Min(1.0, (mineralNallocation + resorbedNavailable) / Ndemand);
                    //PlugIn.ModelCore.UI.WriteLine("mineralN={0}, resorbedN={1}, Ndemand={2}", mineralNallocation, resorbedNallocation, Ndemand);

                }
                else
                    limitN = 1.0; // No demand means that it is a new or very small cohort.  Will allow it to grow anyways.                
            }


            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.mineralNalloc = mineralNallocation;
                CalibrateLog.resorbedNalloc = resorbedNavailable;
            }

            return Math.Max(limitN, 0.0);
        }
        //--------------------------------------------------------------------------
        // Originally from lacalc.f of CENTURY model

        private static double calculateLAI_Limit(ICohort cohort, ActiveSite site)
        {

            //...Calculate true LAI using leaf biomass and a biomass-to-LAI
            //     conversion parameter which is the slope of a regression
            //     line derived from LAI vs Foliar Mass for Slash Pine.

            //...Calculate theoretical LAI as a function of large wood mass.
            //     There is no strong consensus on the true nature of the relationship
            //     between LAI and stemwood mass.  Many sutdies have cited as "general"
            //      an increase of LAI up to a maximum, then a decrease to a plateau value
            //     (e.g. Switzer et al. 1968, Gholz and Fisher 1982).  However, this
            //     response is not general, and seems to mostly be a feature of young
            //     pine plantations.  Northern hardwoods have shown a monotonic increase
            //     to a plateau  (e.g. Switzer et al. 1968).  Pacific Northwest conifers
            //     have shown a steady increase in LAI with no plateau evident (e.g.
            //     Gholz 1982).  In this version, we use a simple saturation fucntion in
            //     which LAI increases linearly against large wood mass initially, then
            //     approaches a plateau value.  The plateau value can be set very large to
            //     give a response of steadily increasing LAI with stemwood.

            //     References:
            //             1)  Switzer, G.L., L.E. Nelson and W.H. Smith 1968.
            //                 The mineral cycle in forest stands.  'Forest
            //                 Fertilization:  Theory and Practice'.  pp 1-9
            //                 Tenn. Valley Auth., Muscle Shoals, AL.
            //
            //             2)  Gholz, H.L., and F.R. Fisher 1982.  Organic matter
            //                 production and distribution in slash pine (Pinus
            //                 elliotii) plantations.  Ecology 63(6):  1827-1839.
            //
            //             3)  Gholz, H.L.  1982.  Environmental limits on aboveground
            //                 net primary production and biomass in vegetation zones of
            //                 the Pacific Northwest.  Ecology 63:469-481.

            //...Local variables
            double leafC = (double)cohort.Data.AdditionalParameters.LeafBiomass * 0.47;
            double woodC = (double)cohort.Data.AdditionalParameters.WoodBiomass * 0.47;

            double lai = 0.0;
            double laitop = -0.47;  // This is the value given for all biomes in the tree.100 file.           
            double btolai = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].BiomassToLAI;
            double klai   = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].KLAI;
            double maxlai = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].MaxLAI;

            //double rlai = (Math.Max(0.0, 1.0 - Math.Exp(btolai * leafC)));
            double rlai = Math.Pow ((Math.Sin((Main.Month/12.0) * Math.PI + btolai)), 3.0);

            if (SpeciesData.LeafLongevity[cohort.Species] > 1.0)
            {
                rlai = 1.0;
            }

            double tlai = (maxlai * woodC)/(klai + woodC);

            //if (rlai < tlai) lai = (rlai + tlai) / 2.0;
            lai = tlai * rlai;
            //else lai = tlai;

            // This will allow us to set MAXLAI to zero such that LAI is completely dependent upon
            // foliar carbon, which may be necessary for simulating defoliation events.
            if(tlai <= 0.0) lai = rlai;

            if (Main.Month == 6 && lai > SiteVars.LAI[site])
                SiteVars.LAI[site] = lai; //Tracking LAI.

            // The minimum LAI to calculate effect is 0.1.
            if (lai < 0.1) lai = 0.1;            

            //SiteVars.MonthlyLAI[site][Main.Month] = lai;

            double LAI_limit = Math.Max(0.0, 1.0 - Math.Exp(laitop * lai));

            //This allows LAI to go to zero for deciduous trees.

            if (SpeciesData.LeafLongevity[cohort.Species] <= 1.0 &&
                (Main.Month > FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].FoliageDropMonth || Main.Month < 3))
            {
                lai = 0.0;
                LAI_limit = 0.0;
            }

            SiteVars.MonthlyLAI[site][Main.Month] = lai;

            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.rlai = rlai;
                CalibrateLog.tlai = tlai;                
                CalibrateLog.LAI = SiteVars.MonthlyLAI[site][Main.Month]; // Chihiro, 2021.03.26: added
            }

            return LAI_limit;

        }

        private static double calculateCompetition_Limit(ICohort cohort, ActiveSite site)
        {
            //double k = -0.25;  // This is the value given for all temperature ecosystems. I started with -0.1, latest code with -0.05, -0.25 works nicely for BS and alder
            // making the competition limit a functional group parameter
            double k = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].K *-1.0; 
            double monthly_LAI = SiteVars.MonthlyLAI[site][Main.Month];
            double competition_limit = Math.Max(0.0, Math.Exp(k * monthly_LAI));
            
            return competition_limit;

        }

        //---------------------------------------------------------------------------
        
        public static double CalculateWater_Limit(ActiveSite site, IEcoregion ecoregion, ISpecies species, out double availableWater)
        {
            if (PlugIn.ShawGiplEnabled)
            {
                var hasAdventRoots = SpeciesData.AdventRoots[species];
                var rootingdepth = SpeciesData.RootingDepth[species] / 100.0;   // convert rooting depth to meters
                                                                                //var thu = PlugIn.TempHydroUnits[PlugIn.ModelCore.Ecoregion[site]];
                //var thu = PlugIn.TempHydroUnit;
                var thu = SiteVars.TempHydroUnit[site];

                // integrate Shaw's soil moisture profile (at Shaw depths) to get the AvailableWater.
                //  start the average at either the top of the profile (if the species has adventitious roots), 
                //  or at the bottom of the adventitious layer (if the species does not have adventitious roots).
                //  end the average at the rooting depth for the species.

                var startingDepth = hasAdventRoots ? 0.0 : SpeciesData.AdventitiousLayerDepth;

                availableWater = AverageOrIntegrateOverProfile(false, thu.MonthlyShawDammResults[Main.Month].MonthSoilLiquidWaterProfile, thu.ShawDepths, thu.ShawDepthIncrements, startingDepth, rootingdepth);
            }
            else
            {
                availableWater = SiteVars.AvailableWater[site];
            }
            return WaterLimitEquation(availableWater, species);
        }

        //---------------------------------------------------------------------------

        public static double WaterLimitEquation(double availableWater, ISpecies species)
        //public static double WaterLimitEquation(double availableWater, ICohort cohort)
        {
            //var vertex = FunctionalType.Table[SpeciesData.FuncType[species]].MoistureCurve1;
            //var xIntercept = FunctionalType.Table[SpeciesData.FuncType[species]].MoistureCurve2;
            //var yIntercept = FunctionalType.Table[SpeciesData.FuncType[species]].MoistureCurve3;

            //var waterLimit = vertex * Math.Pow(availableWater - xIntercept, 2) + yIntercept;
            //if (waterLimit > 1.0) waterLimit = 1.0;
            //if (waterLimit < 0.01) waterLimit = 0.01;

            //return waterLimit;
            var A1 = FunctionalType.Table[SpeciesData.FuncType[species]].MoistureCurve1;
            var A2 = FunctionalType.Table[SpeciesData.FuncType[species]].MoistureCurve2;
            var A3 = FunctionalType.Table[SpeciesData.FuncType[species]].MoistureCurve3;
            var A4 = FunctionalType.Table[SpeciesData.FuncType[species]].MoistureCurve4;

            var frac = (A2 - availableWater) / (A2 - A1);
            var waterLimit = 0.0;
            if (frac > 0.0)
                waterLimit = Math.Exp(A3 / A4 * (1.0 - Math.Pow(frac, A4))) * Math.Pow(frac, A3);

            return waterLimit;

        }



        //-----------
       
        public double Calculate_SoilTemp(ActiveSite site, ISpecies species, out double soilTemperature)
        //public double Calculate_SoilTemp(ActiveSite site, ICohort cohort, out double soilTemperature)
        {
            //Originally from gpdf.f of CENTURY model
            //It calculates the limitation of soil temperature on aboveground forest potential production.
            //It is a function and only called by potcrp.f and potfor.f.

            //A1 is temperature. A2~A5 are paramters from tree.100

            //...This routine is functionally equivalent to the routine of the
            //     same name, described in the publication:

            //       Some Graphs and their Functional Forms
            //       Technical Report No. 153
            //       William Parton and George Innis (1972)
            //       Natural Resource Ecology Lab.
            //       Colorado State University
            //       Fort collins, Colorado  80523
            // https://mountainscholar.org/bitstream/handle/10217/16102/IBP153.pdf?sequence=1&isAllowed=y

            if (PlugIn.ShawGiplEnabled)
            {
                var hasAdventRoots = SpeciesData.AdventRoots[species];
                var rootingdepth = SpeciesData.RootingDepth[species] / 100.0;   // convert rooting depth to meters
                //var thu = PlugIn.TempHydroUnit;
                var thu = SiteVars.TempHydroUnit[site];

                //var thu = PlugIn.TempHydroUnits[PlugIn.ModelCore.Ecoregion[site]];

                // average Gipl's soil temperature profile (at Shaw depths) to get the A1 temperature.
                //  start the average at either the top of the profile (if the species has adventitious roots), 
                //  or at the bottom of the adventitious layer (if the species does not have adventitious roots).
                //  end the average at the rooting depth for the species.

                var startingDepth = hasAdventRoots ? 0.0 : SpeciesData.AdventitiousLayerDepth;

                soilTemperature = AverageOrIntegrateOverProfile(true, thu.MonthlyGiplDammResults[Main.Month].AverageSoilTemperatureProfileAtShawDepths, thu.ShawDepths, thu.ShawDepthIncrements, startingDepth, rootingdepth);
            }
            else
            {
                soilTemperature = SiteVars.SoilTemperature[site];
            }
            return TemperatureLimitEquation(soilTemperature, species);
        }

        public static double TemperatureLimitEquation(double soilTemperature, ISpecies species)
        //public static double TemperatureLimitEquation(double soilTemperature, ICohort cohort)
        {
            //Originally from gpdf.f of CENTURY model
            //It calculates the limitation of soil temperature on aboveground forest potential production.
            //It is a function and only called by potcrp.f and potfor.f.

            //A1-A4 are paramters from tree.100

            //...This routine is functionally equivalent to the routine of the
            //     same name, described in the publication:

            //       Some Graphs and their Functional Forms
            //       Technical Report No. 153
            //       William Parton and George Innis (1972)
            //       Natural Resource Ecology Lab.
            //       Colorado State University
            //       Fort collins, Colorado  80523
            // https://mountainscholar.org/bitstream/handle/10217/16102/IBP153.pdf?sequence=1&isAllowed=y

            var A1 = FunctionalType.Table[SpeciesData.FuncType[species]].TempCurve1;
            var A2 = FunctionalType.Table[SpeciesData.FuncType[species]].TempCurve2;
            var A3 = FunctionalType.Table[SpeciesData.FuncType[species]].TempCurve3;
            var A4 = FunctionalType.Table[SpeciesData.FuncType[species]].TempCurve4;

            var frac = (A2 - soilTemperature) / (A2 - A1);
            var temp_limit = 0.0;
            if (frac > 0.0)
                temp_limit = Math.Exp(A3 / A4 * (1.0 - Math.Pow(frac, A4))) * Math.Pow(frac, A3);

            //PlugIn.ModelCore.UI.WriteLine("  TEMPERATURE Limits:  Soil Temp={0:0.00}, Temp Limit={1:0.00000}. [PPDF1={2:0.0},PPDF2={3:0.0},PPDF3={4:0.0},PPDF4={5:0.0}]", soilTemperature, U1, A1, A2,A3,A4);

            return temp_limit;
        }

        private static double AverageOrIntegrateOverProfile(bool makeAverage, List<double> profile, List<double> depths, List<double> depthIncrements, double startingDepth, double endingDepth)
        {
            // assumes the depths start at 0.0.

            var weight = 0.0;
            var sum = 0.0;

            var i0 = 0;
            if (startingDepth > 0.0)
            {
                // find the first depth point that exceeds startingDepth
                i0 = depths.FindIndex(x => x > startingDepth);
                if (i0 < 0)
                    throw new ApplicationException($"Error: CohortBiomass.AverageOverProfile(): starting depth {startingDepth} not within the profile range.");

                // add the profile value at i0 - 1, weighted by the depth between the starting depth and the depth at i0.
                sum += profile[i0 - 1] * (depths[i0] - startingDepth);
                weight += depths[i0] - startingDepth;
            }

            // find the last depth point before ending depth
            var i1 = depths.FindIndex(x => x > endingDepth);
            i1 = i1 < 0 ? depths.Count - 1 : i1 - 1;    // if endingDepth is below the last depth, use the last depth point and assume the profile extends to the ending depth

            // add the profile at i1, weighted by the depth between the depth at i1 and the ending depth.
            sum += profile[i1] * (endingDepth - depths[i1]);
            weight += endingDepth - depths[i1];

            // now add all the points from i0 to i1 - 1, weighted by their depth increments
            for (var i = i0; i < i1; ++i)
            {
                sum += profile[i] * depthIncrements[i];
                weight += depthIncrements[i];
            }

            return makeAverage ? sum / weight : sum;
        }

        public Percentage ComputeNonWoodyPercentage(ICohort cohort, ActiveSite site)
        {
            throw new NotImplementedException();
        }
    }
}
