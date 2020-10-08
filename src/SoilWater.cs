//  Author: Robert Scheller, Melissa Lucash

using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.Climate;
using System.Linq;

namespace Landis.Extension.Succession.DGS
{

    public enum WaterType { Linear, Ratio }


    public class SoilWater
    {
        private static double Precipitation;
        private static double H2Oinputs;
        private static double tave;
        private static double tmax;
        private static double tmin;
        private static double pet;
        private static int daysInMonth;
        private static int beginGrowing;
        private static int endGrowing;

        //public static void Run(int year, int month, double liveBiomass, Site site, out double baseFlow, out double stormFlow, out double AET)
        public static void Run(int year, int month, double liveBiomass, Site site, out double baseFlow, out double stormFlow)
        {

            
            baseFlow = 0.0;
            stormFlow = 0.0;
            double availableWater = 0.0;
            double priorWaterAvail = 0.0;

            ////...Calculate external inputs
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            H2Oinputs = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPrecip[month]; //rain + irract in cm;
            Precipitation = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPrecip[month]; //rain + irract in cm;
            
            tave = ClimateRegionData.AnnualWeather[ecoregion].MonthlyTemp[month];
            
            tmax = ClimateRegionData.AnnualWeather[ecoregion].MonthlyMaxTemp[month];
            tmin = ClimateRegionData.AnnualWeather[ecoregion].MonthlyMinTemp[month];
            pet = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPET[month];
            daysInMonth = AnnualClimate.DaysInMonth(month, year);
            beginGrowing = ClimateRegionData.AnnualWeather[ecoregion].BeginGrowing;
            endGrowing = ClimateRegionData.AnnualWeather[ecoregion].EndGrowing;

            double wiltingPoint = SiteVars.SoilWiltingPoint[site];            
            availableWater = SiteVars.AvailableWater[site];
            double stormFlowFraction = SiteVars.SoilStormFlowFraction[site];
            double baseFlowFraction = SiteVars.SoilBaseFlowFraction[site];
            double drain = SiteVars.SoilDrain[site];


            
            // Compute the ratio of precipitation to PET
            double ratioPrecipPET = 0.0;
            if (pet > 0.0) ratioPrecipPET = availableWater / pet;  //assumes that the ratio is the amount of incoming precip divided by PET.

            SiteVars.DecayFactor[site] = CalculateDecayFactor((int)OtherData.WType, SiteVars.SoilTemperature[site], availableWater, ratioPrecipPET);
            SiteVars.AnaerobicEffect[site] = CalculateAnaerobicEffect(drain, ratioPrecipPET, pet, tave);            
            if (month == 0)
                SiteVars.DryDays[site] = 0;
            else
                SiteVars.DryDays[site] += CalculateDryDays(month, beginGrowing, endGrowing, wiltingPoint, availableWater, priorWaterAvail);

            return;
        }

        private static int CalculateDryDays(int month, int beginGrowing, int endGrowing, double wiltingPoint, double waterAvail, double priorWaterAvail)
        {
            //PlugIn.ModelCore.UI.WriteLine("Month={0}, begin={1}, end={2}.", month, beginGrowing, endGrowing);
            int[] julianMidMonth = { 15, 45, 74, 105, 135, 166, 196, 227, 258, 288, 319, 349 };
            int dryDays = 0;
            int julianDay = julianMidMonth[month];
            int oldJulianDay = julianMidMonth[month - 1];
            double dryDayInterp = 0.0;
            //PlugIn.ModelCore.UI.WriteLine("Month={0}, begin={1}, end={2}, wiltPt={3:0.0}, waterAvail={4:0.0}, priorWater={5:0.0}.", month, beginGrowing, endGrowing, wiltingPoint, waterAvail, priorWaterAvail);

            //Increment number of dry days, truncate at end of growing season
            if ((julianDay > beginGrowing) && (oldJulianDay < endGrowing))
            {
                if ((priorWaterAvail >= wiltingPoint) && (waterAvail >= wiltingPoint))
                {
                    dryDayInterp += 0.0;  // NONE below wilting point
                }
                else if ((priorWaterAvail > wiltingPoint) && (waterAvail < wiltingPoint))
                {
                    dryDayInterp = daysInMonth * (wiltingPoint - waterAvail) /
                                    (priorWaterAvail - waterAvail);
                    if ((oldJulianDay < beginGrowing) && (julianDay > beginGrowing))
                        if ((julianDay - beginGrowing) < dryDayInterp)
                            dryDayInterp = julianDay - beginGrowing;

                    if ((oldJulianDay < endGrowing) && (julianDay > endGrowing))
                        dryDayInterp = endGrowing - julianDay + dryDayInterp;

                    if (dryDayInterp < 0.0)
                        dryDayInterp = 0.0;

                }
                else if ((priorWaterAvail < wiltingPoint) && (waterAvail > wiltingPoint))
                {
                    dryDayInterp = daysInMonth * (wiltingPoint - priorWaterAvail) /
                                    (waterAvail - priorWaterAvail);

                    if ((oldJulianDay < beginGrowing) && (julianDay > beginGrowing))
                        dryDayInterp = oldJulianDay + dryDayInterp - beginGrowing;

                    if (dryDayInterp < 0.0)
                        dryDayInterp = 0.0;

                    if ((oldJulianDay < endGrowing) && (julianDay > endGrowing))
                        if ((endGrowing - oldJulianDay) < dryDayInterp)
                            dryDayInterp = endGrowing - oldJulianDay;
                }
                else // ALL below wilting point
                {
                    dryDayInterp = daysInMonth;

                    if ((oldJulianDay < beginGrowing) && (julianDay > beginGrowing))
                        dryDayInterp = julianDay - beginGrowing;

                    if ((oldJulianDay < endGrowing) && (julianDay > endGrowing))
                        dryDayInterp = endGrowing - oldJulianDay;
                }

                dryDays += (int)dryDayInterp;
            }
            return dryDays;
        }

        //---------------------------------------------------------------------------

        public static double CalculateDecayFactor(int idef, double soilTemp, double rwc, double ratioPrecipPET)
        {
            // Decomposition factor relfecting the effects of soil temperature and moisture on decomposition
            // Originally revised from prelim.f of CENTURY
            // Irrigation is zero for natural forests
            double decayFactor = 0.0;   //represents defac in the original program defac.f
            double W_Decomp = 0.0;      //Water effect on decomposition

            //...where
            //      soilTemp;        //Soil temperature
            //      T_Decomp;     //Effect of soil temperature on decomposition
            //      W_Decomp;     //Effect of soil moisture on decompostion
            //      rwcf[10];     //Initial relative water content for 10 soil layers
            //      avh2o;        //Water available to plants for growth in soil profile
            //      precipitation;       //Precipitation of current month
            //      irract;       //Actual amount of irrigation per month (cm H2O/month)
            //      pet;          //Monthly potential evapotranspiration in centimeters (cm)

            //Option selection for wfunc depending on idef
            //      idef = 0;     // for linear option
            //      idef = 1;     // for ratio option


            if (idef == 0)
            {
                if (rwc > 13.0)
                    W_Decomp = 1.0;
                else
                    W_Decomp = 1.0 / (1.0 + 4.0 * System.Math.Exp(-6.0 * rwc));
            }
            else if (idef == 1)
            {
                if (ratioPrecipPET > 9)
                    W_Decomp = 1.0;
                else
                    W_Decomp = 1.0 / (1.0 + 30.0 * System.Math.Exp(-8.5 * ratioPrecipPET));
            }

            double tempModifier = T_Decomp(soilTemp);

            decayFactor = tempModifier * W_Decomp;

            //defac must >= 0.0
            if (decayFactor < 0.0) decayFactor = 0.0;

            //if (soilTemp < 0 && decayFactor > 0.01)
            //{
            //    PlugIn.ModelCore.UI.WriteLine("Yr={0},Mo={1}, PET={2:0.00}, MinT={3:0.0}, MaxT={4:0.0}, AveT={5:0.0}, H20={6:0.0}.", Century.Year, month, pet, tmin, tmax, tave, H2Oinputs);
            //    PlugIn.ModelCore.UI.WriteLine("Yr={0},Mo={1}, DecayFactor={2:0.00}, tempFactor={3:0.00}, waterFactor={4:0.00}, ratioPrecipPET={5:0.000}, soilT={6:0.0}.", Century.Year, month, decayFactor, tempModifier, W_Decomp, ratioPrecipPET, soilTemp);
            //}

            return decayFactor;   //Combination of water and temperature effects on decomposition
        }

        //---------------------------------------------------------------------------
        private static double T_Decomp(double soilTemp)
        {
            //Originally from tcalc.f
            //This function computes the effect of temperature on decomposition.
            //It is an exponential function.  Older versions of Century used a density function.
            //Created 10/95 - rm


            double Teff0 = OtherData.TemperatureEffectIntercept;
            double Teff1 = OtherData.TemperatureEffectSlope;
            double Teff2 = OtherData.TemperatureEffectExponent;

            double r = Teff0 + (Teff1 * System.Math.Exp(Teff2 * soilTemp));

            return r;
        }
        //---------------------------------------------------------------------------
        public static double CalculateAnaerobicEffect(double drain, double ratioPrecipPET, double pet, double tave)
        {

            //Originally from anerob.f of Century

            //...This function calculates the impact of soil anerobic conditions
            //     on decomposition.  It returns a multiplier 'anerob' whose value
            //     is 0-1.

            //...Declaration explanations:
            //     aneref[1] - ratio RAIN/PET with maximum impact
            //     aneref[2] - ratio RAIN/PET with minimum impact
            //     aneref[3] - minimum impact
            //     drain     - percentage of excess water lost by drainage
            //     newrat    - local var calculated new (RAIN+IRRACT+AVH2O[3])/PET ratio
            //     pet       - potential evapotranspiration
            //     rprpet    - actual (RAIN+IRRACT+AVH2O[3])/PET ratio

            double aneref1 = OtherData.RatioPrecipPETMaximum;  //This value is 1.5
            double aneref2 = OtherData.RatioPrecipPETMinimum;   //This value is 3.0
            double aneref3 = OtherData.AnerobicEffectMinimum;   //This value is 0.3

            double anerob = 1.0;

            //...Determine if RAIN/PET ratio is GREATER than the ratio with
            //     maximum impact.

            if ((ratioPrecipPET > aneref1) && (tave > 2.0))
            {
                double xh2o = (ratioPrecipPET - aneref1) * pet * (1.0 - drain);

                if (xh2o > 0)
                {
                    double newrat = aneref1 + (xh2o / pet);
                    double slope = (1.0 - aneref3) / (aneref1 - aneref2);
                    anerob = 1.0 + slope * (newrat - aneref1);
                    //PlugIn.ModelCore.UI.WriteLine("If higher threshold. newrat={0:0.0}, slope={1:0.00}, anerob={2:0.00}", newrat, slope, anerob);      
                }

                if (anerob < aneref3)
                    anerob = aneref3;
                //PlugIn.ModelCore.UI.WriteLine("Lower than threshold. Anaerobic={0}", anerob);      
            }
            //PlugIn.ModelCore.UI.WriteLine("ratioPrecipPET={0:0.0}, tave={1:0.00}, pet={2:0.00}, AnaerobicFactor={3:0.00}, Drainage={4:0.00}", ratioPrecipPET, tave, pet, anerob, drain);         
            //PlugIn.ModelCore.UI.WriteLine("Anaerobic Effect = {0:0.00}.", anerob);
            return anerob;
        }
        //---------------------------------------------------------------------------
        // ML: No longer used with GIPL and SHAW
        //private static double CalculateSoilTemp(double tmin, double tmax, double liveBiomass, double litterBiomass, int month)
        //{
        //    // ----------- Calculate Soil Temperature -----------
        //    double bio = liveBiomass + (OtherData.EffectLitterSoilT * litterBiomass);
        //    bio = Math.Min(bio, 600.0);

        //    //...Maximum temperature
        //    double maxSoilTemp = tmax + (25.4 / (1.0 + 18.0 * Math.Exp(-0.20 * tmax))) * (Math.Exp(OtherData.EffectBiomassMaxSurfT * bio) - 0.13);

        //    //...Minimum temperature
        //    double minSoilTemp = tmin + OtherData.EffectBiomassMinSurfT * bio - 1.78;

        //    //...Average surface temperature
        //    //...Note: soil temperature used to calculate potential production does not
        //    //         take into account the effect of snow (AKM)
        //    //double soilTemp = (maxSoilTemp + minSoilTemp) / 2.0;  // original code.  This generates very high soil temps >= to air temp.


        //    double soilTemp = (tmax + tmin) / 4.0;  //ML added for testing purposes. Remove later
        //    PlugIn.ModelCore.UI.WriteLine("Month={0}, Soil Temperature = {1}, Tmax = {2}, Tmin = {3}.", month + 1, soilTemp, tmax, tmin);

        //    return soilTemp;
        //}
        //--------------------------------------------------------------------------
        // This waterMove section was modified here to use the available water computed by SHAW. -JM/ML
        public static void Leach(Site site, double baseFlow, double stormFlow)
        {

            //  double minlch, double frlech[3], double stream[8], double basef, double stormf)
            //Originally from leach.f of CENTURY model
            //...This routine computes the leaching of inorganic nitrogen (potential for use with phosphorus, and sulfur)
            //...Written 2/92 -rm. Revised on 12/11 by ML
            // ML left out leaching intensity factor.  Cap on MAX leaching (MINLECH/OMLECH3) is poorly defined in CENTURY manual. Added a NO3frac factor to account 
            //for the fact that only NO3 (not NH4) is leached from soils.  

            //...Called From:   SIMSOM

            //...amtlea:    amount leached
            //...linten:    leaching intensity
            //...strm:      storm flow
            //...base:      base flow

            //Outputs:
            //minerl and stream are recomputed
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
            double fieldCapacity = SiteVars.SoilFieldCapacity[site];

            var thu = SiteVars.TempHydroUnit[site];
            double availableWater = thu.MonthlySpeciesRecords[Main.Month].Values.Max(x => x.AvailableWater);
            
            //double waterMove = SiteVars.WaterMovement[site];
            double waterMove = availableWater / fieldCapacity;

            double amtNLeached = 0.0;

            //PlugIn.ModelCore.UI.WriteLine("WaterMove={0:0}, ", waterMove);         

            //...waterMove > 0. indicates a saturated water flow out of layer lyr
            if (waterMove > 1.0 && SiteVars.MineralN[site] > 0.0)
            {
                double textureEffect = OtherData.MineralLeachIntercept + OtherData.MineralLeachSlope * SiteVars.SoilPercentSand[site];//ClimateRegionData.PercentSand[ecoregion];
                //double leachIntensity = (1.0 - (OtherData.OMLeachWater - waterMove) / OtherData.OMLeachWater);
                //amtNLeached = textureEffect * SiteVars.MineralN[site] * OtherData.NfracLeachWater * OtherData.NO3frac;
                amtNLeached = textureEffect * SiteVars.MineralN[site] * OtherData.NO3frac;

                //PlugIn.ModelCore.UI.WriteLine("amtNLeach={0:0.0}, textureEffect={1:0.0}, waterMove={2:0.0}, MineralN={3:0.00}", amtNLeached, textureEffect, waterMove, SiteVars.MineralN[site]);      
            }

            double totalNleached = (baseFlow * amtNLeached) + (stormFlow * amtNLeached);

            SiteVars.MineralN[site] -= totalNleached;
            //PlugIn.ModelCore.UI.WriteLine("AfterSoilWaterLeaching. totalNLeach={0:0.0}, MineralN={1:0.00}", totalNleached, SiteVars.MineralN[site]);         

            SiteVars.Stream[site].Nitrogen += totalNleached;
            SiteVars.MonthlyStreamN[site][Main.Month] += totalNleached;
            //PlugIn.ModelCore.UI.WriteLine("AfterSoilWaterLeaching. totalNLeach={0:0.0}, MineralN={1:0.00}", totalNleached, SiteVars.MineralN[site]);        

            return;
        }

    }
}

