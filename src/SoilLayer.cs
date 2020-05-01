//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using System;
using Landis.Library.Climate;

namespace Landis.Extension.Succession.DGS
{
    public enum SoilName { Primary, Secondary, Tertiary, Other };
    /// <summary>
    /// </summary>
    public class SoilLayer
    {

        private SoilName name;
        //private LayerType type;
        private double o_carbon;
        private double o_nitrogen;
        private double do_carbon;
        private double do_nitrogen;
        private double microbial_carbon;
        private double microbial_nitrogen;
        private double enzymatic_concentration;
        private double monthlyCinputs;
        //private double decayValue;
        //private double fractionLignin;
        //private double netMineralization;
        //private double grossMineralization;


        //---------------------------------------------------------------------
        public SoilLayer(SoilName name)//, LayerType type)
        {
            this.name = name;
            //this.type = type;
            this.o_carbon = 0.0;
            this.o_nitrogen = 0.0;
            this.do_carbon = 0.0;
            this.do_nitrogen = 0.0;
            this.microbial_carbon = 0.0;
            this.microbial_nitrogen = 0.0;
            this.enzymatic_concentration = 0.0;
            this.monthlyCinputs = 0.0;

        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Layer Name
        /// </summary>
        public SoilName Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Soil Organic Carbon
        /// </summary>
        public double Carbon
        {
            get
            {
                return o_carbon;
            }
            set
            {
                o_carbon = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Soil Organic Nitrogen
        /// </summary>
        public double Nitrogen
        {
            get
            {
                return o_nitrogen;
            }
            set
            {
                o_nitrogen = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Dissolved Organic Carbon
        /// </summary>
        public double DOC
        {
            get
            {
                return do_carbon;
            }
            set
            {
                do_carbon = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Dissolved Organic Nitrogen
        /// </summary>
        public double DON
        {
            get
            {
                return do_nitrogen;
            }
            set
            {
                do_nitrogen = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Microbial Carbon
        /// </summary>
        public double MicrobialCarbon
        {
            get
            {
                return microbial_carbon;
            }
            set
            {
                microbial_carbon = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Microbial Nitrogen
        /// </summary>
        public double MicrobialNitrogen
        {
            get
            {
                return microbial_nitrogen;
            }
            set
            {
                microbial_nitrogen = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Enzymatic Concentration
        /// </summary>
        public double EnzymaticConcentration
        {
            get
            {
                return enzymatic_concentration;
            }
            set
            {
                enzymatic_concentration = value;
            }
        }

        /// Inputs to litter pool= roots, wood, litterfall.
        /// </summary>
        public double MonthlyCarbonInputs
        {
            get
            {
                return monthlyCinputs;
            }
            set
            {
                monthlyCinputs = value;
            }
        }


        // used fitted values from Abramoff et al. or values from Vogel when applicable. NOTE:  p[n] indicates the field from the prior csv input file.
        private static double ea_dep = 66.2675487981142;        //p[1]  #activation energy of SOM depolymerization
        private static double ea_upt = 67.8225452963818;        //p[2]  #activation energy of DOC uptake
        private static double a_dep = 116986794418;         //p[3]   #pre-exponential constant for SOM depolymerization
        private static double a_upt = 115130563903;         //p[4]  #pre-exponential constant for uptake
        //private static double frac = 0.000378981;          //p[5]  #fraction of unprotected SOM, Magill et al. 2000
        private static double frac = 0.0004048;          //p[5]  # Vogel fraction of unprotected SOM
                                                           // p[6] 0.000466501;  NOT USED
                                                           // p[7] 0.000486085;  NOT USED
                                                           //private static double cn_litter = 50.56466;     //p[8] #C:N of litter (was: cnl) Used in line 324
        private static double cn_litter = 24.59;     //p[8] #C:N of litter (was: cnl) Used in line 324
        //public static double CN_DOCN = 29.60162;         //p[9] #C:N of soil (was: cns)  // Rob: only used for initializing DON
        //private static double cn_microbial = 9.803885526;  //p[10] #C:N of microbial biomass (was: cnm)
        private static double cn_microbial = 17.783;  //p[10] #C:N of microbial biomass, Vogel
        private static double cn_enzymes = 2.86940;    //p[11] #C:N of enzymes (was: cne)
        private static double km_dep = 0.00249;        //p[12] #half-saturation constant for SOM depolymerization
        private static double km_upt = 0.33217;
                    //p[13] #half-saturation constant for DOC uptake
        private static double r_ecloss = 0.00095780;       //p[14] #enzyme turnover rate
        private static double r_death = 0.0001437;       //p[15] #microbial turnover rate
        private static double c_use_efficiency = 0.357329;           //p[16] #carbon use efficiency (was: cue)
        private static double p_enz_SOC = 0.522748;      //p[17] #proportion of enzyme pool acting on SOC (was: a)
        private static double pconst = 0.551334;
                    //p[18] #proportion of assimilated C allocated to enzyme production
        private static double qconst = 0.50446;
                    //p[19] #proportion of assimilated N allocated to enzyme production
        private static double mic_to_som = 0.551334;    //p[20] #fraction of dead microbial biomass allocated to SOM
        private static double km_o2 = 0.128;         //p[21] #Michaelis constant for O2
        private static double dgas = 1.676;          //p[22] #diffusion coefficient for O2 in air
        private static double dliq = 3.3386;          //p[23] #diffusion coefficient for unprotected SOM and DOM in liquid
        private static double o2airfrac = 0.205165;     //p[24] #volume fraction of O2 in air
        //private double bulk_density = 0.75743956;         //p[25] #bulk density (was: bd)
        //private double particle_density = 2.50156948;     //p[26] #particle density (was: pd)
        private static double soilMoistureA = 1.95;  //p[27]
        //private static double soilMoistureA = 0.0001;  //p[27]
        private static double soilMoistureB = 10.0;  //p[28]                     
        private static double saturation = 0.492526227563312;     //p[29] #saturation level (was: sat)
        public static double r = 0.008314;                 // gas constant

        public static void Decompose(int Year, int Month, ActiveSite site)
        {
            double month_to_hr = 24.0 * (double)AnnualClimate.DaysInMonth(Month, Year);
            double g_to_mg = 1000;           
            double m2_to_cm2 = 10000;            
            double depth_to_volume = 10.0; //We are simulating just the top 10cm.
            var soilTemperature = 0.0;
            var availableWater = 0.0;

            double SOC = SiteVars.SoilPrimary[site].Carbon;
            if (SOC > 0.0000001)
            {
                if (PlugIn.ShawGiplEnabled)
                {
                    //var thu = PlugIn.TempHydroUnit;
                    var thu = SiteVars.TempHydroUnit[site];

                    soilTemperature = thu.MonthlySoilTemperatureDecomp[Month];
                    availableWater = thu.MonthlySoilMoistureDecomp[Month];
                }
                else
                {
                    soilTemperature = SiteVars.SoilTemperature[site];
                    availableWater = SiteVars.AvailableWater[site];

                }
            }
            // used for testing to keep everything constant for testing with DAMM R code.
            //availableWater = 0.15; 
            //soilTemperature = 10.0;  

            //availableWater = 0.30; //Email from Dmitry with SM data from Dalton gives BNZ a SM of 0.40 3/26/20 
           //if (Month==6)
           //{
           //    SiteVars.SoilPrimary[site].Carbon = 652.5;
             //    SiteVars.SoilPrimary[site].Nitrogen = 21.917;
             //    SiteVars.SoilPrimary[site].DOC = 0.020000000;
             //    SiteVars.SoilPrimary[site].DON = 0.011;
             //}           

            // convert Landis units to Rose's units, * g_to_mg) / (m2_to_cm2)          
            SOC = (SiteVars.SoilPrimary[site].Carbon * g_to_mg) / (m2_to_cm2 * depth_to_volume);
                //SOC = 65.25; //(Rose);
                double SON = (SiteVars.SoilPrimary[site].Nitrogen * g_to_mg) / (m2_to_cm2 * depth_to_volume);
            //double SON = 2.1917; //(Rose);
            //double bulk_density = SiteVars.SoilBulkDensity[site];   //double bulk_density = 0.0377 (Jason);
            //double bulk_density = 0.156152207886358;   //original testing
            double bulk_density = 0.0377; //Jason
                //double particle_density = 1.11176644135636; //original testing.
                double particle_density = SiteVars.SoilParticleDensity[site]; //double particle_density = 0.8877 (Jason);               
                double DOC = (SiteVars.SoilPrimary[site].DOC * g_to_mg) / (m2_to_cm2 * depth_to_volume);
                //double DOC = 0.0020000000;
                double DON = (SiteVars.SoilPrimary[site].DON * g_to_mg) / (m2_to_cm2 * depth_to_volume);
                //double DON = 0.0011;
                double microbial_C = SiteVars.SoilPrimary[site].MicrobialCarbon;  //double microbial_C = 1.9703 (Rose);               
                double microbial_N = SiteVars.SoilPrimary[site].MicrobialNitrogen;  //double microbial_N = 0.197 (Rose);
                double enzymatic_concentration = SiteVars.SoilPrimary[site].EnzymaticConcentration;  //double enzymatic_concentration = 0.0339 (Rose);   
            double gross_mineralizaton = SiteVars.GrossMineralization[site] * g_to_mg / (m2_to_cm2 * depth_to_volume);
            double mineralN = SiteVars.MineralN[site] * g_to_mg / (m2_to_cm2 * depth_to_volume);
            // Splitting C inputs into 50% litter and 50% DOC at the onset.
            double LitterCinput = (SiteVars.SoilPrimary[site].MonthlyCarbonInputs * (0.5) * g_to_mg) / (m2_to_cm2 * month_to_hr * depth_to_volume);  // Using 70/30 division.
            double DOCinput= (SiteVars.SoilPrimary[site].MonthlyCarbonInputs * (0.5) * g_to_mg) / (m2_to_cm2 * month_to_hr * depth_to_volume);
            // Splitting C inputs into 50% litter and 50% DOC at the onset.
            //double LitterCinput = 0.00057;  //Jason's value at an hourly rate.
            //double DOCinput = 0.00057;  //Jason's value at an hourly rate.

            //PlugIn.ModelCore.UI.WriteLine("Inputs in Rose's units. LitterCinput={0:0.000000000000}, DON={1:0.0000000}, microbial_C={2:0.00}, microbial_N={3:0.000000}, enzymatic_concentration={4:0.000000}, DOC={5:0.0000000},", LitterCinput, DON, microbial_C, microbial_N, enzymatic_concentration, DOC);

            double porosity = 1 - bulk_density / particle_density;
            double co2loss= SiteVars.MonthlyResp[site][Main.Month];
            //calculate porosity                
            var cout_old = 0.0;

            for (int hours = 0; hours < month_to_hr; hours++)
            {
                double soilm = -soilMoistureA + soilMoistureB * availableWater;                                //calculate soil moisture scalar        
                //double soilm = availableWater;                                                                //calculate soil moisture        
                soilm = (soilm > saturation) ? saturation : soilm;                                          //set upper bound on soil moisture (saturation)
                soilm = (soilm < 0.1) ? 0.1 : soilm;                                                        //set lower bound on soil moisture               
                double o2 = dgas * o2airfrac * Math.Pow((porosity - soilm), (4.0 / 3.0));                   //calculate oxygen concentration
                double sol_soc = dliq * Math.Pow(soilm, 3.0) * frac * SOC;
                double sol_son = dliq * Math.Pow(soilm, 3.0) * frac * SON;                                    //calculate unprotected SON
                double vmax_dep = a_dep * Math.Exp(-ea_dep / (r * (soilTemperature + 273.0)));                          //calculate maximum depolymerization rate         
                double vmax_upt = a_upt * Math.Exp(-ea_upt / (r * (soilTemperature + 273.0)));                          //calculate maximum depolymerization rate
                double upt_c = microbial_C * vmax_upt * DOC / (km_upt + DOC) * o2 / (km_o2 + o2);           //calculate DOC uptake
                double c_mineralization = upt_c * (1.0 - c_use_efficiency);                                   //calculate initial C mineralization               
                double upt_n = microbial_N * vmax_upt * DON / (km_upt + DON) * o2 / (km_o2 + o2);           //calculate DON uptake
                double death_c = r_death * Math.Pow(microbial_C, 2.0);                                        //calculate density-dependent microbial C turnover
                double death_n = r_death * Math.Pow(microbial_N, 2.0);                                        //calculate density-dependent microbial N turnover, upt_n, death_c, death_n);

                double enz_c = pconst * c_use_efficiency * upt_c;                                           //calculate potential enzyme C production
                double enz_n = qconst * upt_n;                                                              //calculate potential enzyme N production
                double eprod = (enz_c / cn_enzymes >= enz_n) ? enz_n : (enz_c / cn_enzymes);                //calculate actual enzyme based on Liebig's Law
                double growth_c = (1.0 - pconst) * (upt_c * c_use_efficiency) + enz_c - cn_enzymes * eprod;   //calculate potential microbial biomass C growth
                double growth_n = (1.0 - qconst) * upt_n + enz_n - eprod;                                     //calculate potential microbial biomass N growth
                double growth = (growth_c / cn_microbial >= growth_n) ? growth_n : (growth_c / cn_microbial); //calculate actual microbial biomass growth based on Liebig's Law of the minimum (Schimel & Weintraub 2003 SBB)

                double overflow = growth_c - cn_microbial * growth;                         //calculate overflow metabolism of C
                double nmin = growth_n - growth;                                            //calculate N mineralization

                double dmic_c = cn_microbial * growth - death_c;                            //calculate change in microbial C pool
                double dmic_n = growth - death_n;                                           //calculate change in microbial N pool

                double eloss = r_ecloss * enzymatic_concentration;                          //calculate enzyme turnover
                double dec = eprod - eloss;                                                 //calculate change in enzyme pool

                double decom_c = vmax_dep * p_enz_SOC * enzymatic_concentration * sol_soc / (km_dep + sol_soc + enzymatic_concentration);     //calculate depolymerization of SOC using ECA kinetics (Tang 2015 GMD)
                double decom_n = vmax_dep * (1 - p_enz_SOC) * enzymatic_concentration * sol_son / (km_dep + sol_son + enzymatic_concentration); //calculate depolymerization of SON using ECA kinetics 

                double dsoc = LitterCinput + death_c * mic_to_som - decom_c;                    //calculate change in SOC pool
                double dson = (LitterCinput / cn_litter) + death_n * mic_to_som - decom_n;                    //calculate change in SON pool              
                double ddoc = DOCinput + decom_c + death_c * (1.0 - mic_to_som) + cn_enzymes * eloss - upt_c; //calculate change in DOC pool                
                double ddon = (DOCinput / SOC / SON) + decom_n + death_n * (1.0 - mic_to_som) + eloss - upt_n; //calculate change in DON pool

                // convert Rose's results to Landis units, * m2_to_cm2 / (g_to_mg)
                SOC += dsoc;
                SON += dson;
                DOC += ddoc;
                DON += ddon;
                microbial_C += dmic_c;
                microbial_N += dmic_n;
                enzymatic_concentration += dec;
                mineralN += nmin;
                co2loss += c_mineralization + overflow;
                //if (hours>0)                    //Loop ensuring that soil resp is a difference
                //    co2loss +=c_mineralization + overflow - cout_old;
                //cout_old = c_mineralization + overflow; 
                gross_mineralizaton += nmin;

            }
                                 
            // adjust pools, to LANDIS units
            SiteVars.SoilPrimary[site].Carbon = SOC * m2_to_cm2 * depth_to_volume / (g_to_mg);
            SiteVars.SoilPrimary[site].Nitrogen = SON * m2_to_cm2 * depth_to_volume / (g_to_mg);
            SiteVars.SoilPrimary[site].DOC = DOC * m2_to_cm2 * depth_to_volume / (g_to_mg);
            SiteVars.SoilPrimary[site].DON = DON * m2_to_cm2 * depth_to_volume / (g_to_mg);
            SiteVars.SoilPrimary[site].MicrobialCarbon = microbial_C;
            SiteVars.SoilPrimary[site].MicrobialNitrogen = microbial_N;
            SiteVars.SoilPrimary[site].EnzymaticConcentration = enzymatic_concentration;
            SiteVars.MineralN[site] = mineralN * m2_to_cm2 * depth_to_volume / (g_to_mg);

            // adjust rates, to LANDIS units
            SiteVars.MonthlyResp[site][Main.Month] = co2loss * m2_to_cm2 * depth_to_volume / (g_to_mg);            
            SiteVars.GrossMineralization[site] = gross_mineralizaton * m2_to_cm2 * depth_to_volume / (g_to_mg);

            // make sure the pools aren't allowed to go below zero
            if (SiteVars.SoilPrimary[site].Carbon < 0.0)
                SiteVars.SoilPrimary[site].Carbon = 0.0;

            if (SiteVars.SoilPrimary[site].Nitrogen < 0.0)
                SiteVars.SoilPrimary[site].Nitrogen = 0.0;

            if (SiteVars.SoilPrimary[site].DOC < 0.0)
                SiteVars.SoilPrimary[site].DOC = 0.0;

            if (SiteVars.SoilPrimary[site].DON < 0.0)
                SiteVars.SoilPrimary[site].DON = 0.0;

            if (SiteVars.SoilPrimary[site].MicrobialCarbon < 0.0)
                SiteVars.SoilPrimary[site].MicrobialCarbon = 0.0;

            if (SiteVars.SoilPrimary[site].MicrobialNitrogen < 0.0)
                SiteVars.SoilPrimary[site].MicrobialNitrogen = 0.0;

            if (SiteVars.SoilPrimary[site].EnzymaticConcentration < 0.0)
                SiteVars.SoilPrimary[site].EnzymaticConcentration = 0.0;


            //PlugIn.ModelCore.UI.WriteLine("Outputs. SOC={0:0.0}, DON={1:0.00}, DOC={2:0.00}, DON={3:0.00}", SiteVars.SoilPrimary[site].Carbon, SiteVars.SoilPrimary[site].Nitrogen, SiteVars.SoilPrimary[site].DOC, SiteVars.SoilPrimary[site].DON);
            
            //double dcout = cmin + overflow;                                             //calculate C efflux
            //double co2loss = (c_mineralization + overflow) * month_to_hr * m2_to_cm2 / (g_to_mg );                //calculate C efflux
            //double mineralNFlow = (nmin * month_to_hr * m2_to_cm2 )/ (g_to_mg);                //N mineralization

            //Add lost CO2 to C sink/source and to monthly heterotrophic respiration
            //SiteVars.MonthlyResp[site][Main.Month] += co2loss;
           
            //SiteVars.SourceSink[site].Carbon = Math.Round((SiteVars.SourceSink[site].Carbon + co2loss));
            //SiteVars.MineralN[site] += mineralNFlow;
            //SiteVars.GrossMineralization[site] += mineralNFlow;

            //SiteVars.SoilPrimary[site].Respiration(c_loss, site);
            SiteVars.SoilPrimary[site].MonthlyCarbonInputs = 0.0;  // Reset to zero each timestep.

            double cLeached = 0.0;  // Carbon leached to a stream

            //if (SiteVars.WaterMovement[site] > 0.0)  //Volume of water moving-ML.  Used to be an index of water movement that indicates saturation (amov)
            //{

            //    double leachTextureEffect = OtherData.OMLeachIntercept + OtherData.OMLeachSlope * SiteVars.SoilPercentSand[site];

            //    // this may need to be revisited for DGS. Water movement in SHAW?  ML
            //    double indexWaterMovement = SiteVars.WaterMovement[site] / (SiteVars.SoilDepth[site] * SiteVars.SoilFieldCapacity[site]);

            //    cLeached = co2loss * leachTextureEffect * indexWaterMovement;

            //    //Partition and schedule C flows 
            //    if (cLeached > SiteVars.SoilPrimary[site].Carbon)
            //        cLeached = SiteVars.SoilPrimary[site].Carbon;

            //    //round these to avoid unexpected behavior
            //    SiteVars.SoilPrimary[site].Carbon = Math.Round((SiteVars.SoilPrimary[site].Carbon - cLeached));
            //    SiteVars.Stream[site].Carbon = Math.Round((SiteVars.Stream[site].Carbon + cLeached));

            //    // Compute and schedule N flows and update mineralization accumulators
            //    // Need to use the ratio for som1 for organic leaching
            //    double ratioCN_SoilPrimary = SiteVars.SoilPrimary[site].Carbon / SiteVars.SoilPrimary[site].Nitrogen;
            //    double orgflow = cLeached / ratioCN_SoilPrimary;

            //    SiteVars.SoilPrimary[site].Nitrogen -= orgflow;
            //    SiteVars.Stream[site].Nitrogen += orgflow;

            //    SiteVars.MonthlyStreamN[site][Main.Month] += orgflow;
            //}

        }

        //}

        public void Respiration(double co2loss, ActiveSite site)
        {
            // Compute flows associated with microbial respiration.

            // Input:
            //  co2loss = CO2 loss associated with decomposition           

            //c...Mineralization associated with respiration is proportional to the N fraction.
            double mineralNFlow = co2loss * this.Nitrogen / this.Carbon;  //ML: need to have it be the average hours per month.

            if (mineralNFlow > this.Nitrogen)
            {
                //if((mineralNFlow - this.Nitrogen) > 0.01)
                //{
                //    PlugIn.ModelCore.UI.WriteLine("RESPIRATION for layer {0} {1}:  Mineral N flow exceeds layer Nitrogen.", this.Name, this.Type);
                //    PlugIn.ModelCore.UI.WriteLine("  MineralNFlow={0:0.000}, this.Nitrogen ={0:0.000}", mineralNFlow, this.Nitrogen);
                //    PlugIn.ModelCore.UI.WriteLine("  CO2 loss={0:0.000}, this.Carbon={0:0.000}", co2loss, this.Carbon);
                //    PlugIn.ModelCore.UI.WriteLine("  Site R/C: {0}/{1}.", site.Location.Row, site.Location.Column);
                //}
                mineralNFlow = this.Nitrogen;
                co2loss = this.Carbon;
            }

            if (co2loss > this.Carbon)
                co2loss = this.Carbon;

            //round these to avoid unexpected behavior
            // this.Carbon = Math.Round((this.Carbon - co2loss)); This is double-counting of dscoc (above)?
            SiteVars.SourceSink[site].Carbon = Math.Round((SiteVars.SourceSink[site].Carbon + co2loss));

            //Add lost CO2 to monthly heterotrophic respiration
            SiteVars.MonthlyResp[site][Main.Month] += co2loss;

            this.Nitrogen -= mineralNFlow;
            SiteVars.MineralN[site] += mineralNFlow;

            //PlugIn.ModelCore.UI.WriteLine("     Source:  this.Name={0}, this.Type={1}", this.Name, this.Type);
            //PlugIn.ModelCore.UI.WriteLine("  Respiration.mineralN= {0:0.000}, co2loss={1:00}", mineralNFlow, co2loss);

            //c...Update gross mineralization
            // this.GrossMineralization += mineralNFlow;
            if (mineralNFlow > 0)
                SiteVars.GrossMineralization[site] += mineralNFlow;

            //c...Update net mineralization
            //this.NetMineralization += mineralNFlow;

            return;
        }
    }
}

